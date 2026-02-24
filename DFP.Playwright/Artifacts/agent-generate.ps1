param(
    [Parameter(Mandatory=$false)]
    [string]$PageKey,
    [Parameter(Mandatory=$false)]
    [string]$FeaturesRoot,
    [Parameter(Mandatory=$false)]
    [string]$PagesOut,
    [Parameter(Mandatory=$false)]
    [string]$StepsOut,
    [Parameter(Mandatory=$false)]
    [string]$SelectorsRoot,
    [Parameter(Mandatory=$false)]
    [string]$NamespaceRoot = "DFP.Playwright"
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptRoot "..")

if ([string]::IsNullOrWhiteSpace($FeaturesRoot)) {
    $FeaturesRoot = Join-Path $projectRoot "Features"
}
if ([string]::IsNullOrWhiteSpace($PagesOut)) {
    $PagesOut = Join-Path $projectRoot "Pages\Generated"
}
if ([string]::IsNullOrWhiteSpace($StepsOut)) {
    $StepsOut = Join-Path $projectRoot "StepDefinitions\Generated"
}
if ([string]::IsNullOrWhiteSpace($SelectorsRoot)) {
    $SelectorsRoot = Join-Path $projectRoot "Artifacts\Selectors"
}

function Ensure-Dir($path) {
    if (!(Test-Path $path)) { New-Item -ItemType Directory -Force $path | Out-Null }
}

function To-PascalCase($text) {
    $t = $text -replace "[^A-Za-z0-9 ]", " "
    $parts = $t -split "\s+" | Where-Object { $_ -ne "" }
    return ($parts | ForEach-Object { $_.Substring(0,1).ToUpper() + $_.Substring(1) }) -join ""
}

function Step-MethodName($stepText) {
    return (To-PascalCase $stepText)
}

function Escape-CSharpString([string]$s) {
    if ($null -eq $s) { return $s }
    $t = $s -replace "\\", "\\\\"
    $t = $t -replace '"', '\"'
    $t = $t -replace "`r", "\r"
    $t = $t -replace "`n", "\n"
    return $t
}

function Get-ExistingMethodNames([string]$content) {
    $set = New-Object System.Collections.Generic.HashSet[string]
    if ([string]::IsNullOrWhiteSpace($content)) { return $set }
    $matches = [regex]::Matches($content, 'public\s+async\s+Task\s+([A-Za-z0-9_]+)\s*\(')
    foreach ($m in $matches) { $null = $set.Add($m.Groups[1].Value) }
    return $set
}

function Replace-SelectorsBlock([string]$content, [string]$selectorsBlock) {
    if ([string]::IsNullOrWhiteSpace($content)) { return $content }
    if ($content -match "// codegen:selectors-start") {
        return [regex]::Replace(
            $content,
            "// codegen:selectors-start([\\s\\S]*?)// codegen:selectors-end",
            $selectorsBlock
        )
    }

    # Fallback: replace existing Selectors array if present
    if ($content -match 'public static readonly string\[\] Selectors') {
        return [regex]::Replace(
            $content,
            'public static readonly string\[\] Selectors = new\[\]([\s\S]*?);',
            ($selectorsBlock -replace "// codegen:selectors-start\\s*", "" -replace "\\s*// codegen:selectors-end", "")
        )
    }

    return $content
}

function Insert-MethodsBeforeClassEnd([string]$content, [string]$methodsText) {
    if ([string]::IsNullOrWhiteSpace($methodsText)) { return $content }
    $marker = "`r`n    }`r`n}`r`n"
    $idx = $content.LastIndexOf($marker)
    if ($idx -lt 0) { return $content + $methodsText }
    return $content.Substring(0, $idx) + $methodsText + $content.Substring($idx)
}


function Read-Selectors($pageKey) {
    $path = Join-Path $SelectorsRoot ("selectors.{0}.json" -f $pageKey)
    if (!(Test-Path $path)) { return $null }
    try { return (Get-Content -Raw $path | ConvertFrom-Json) } catch { return $null }
}

function Extract-SelectorName($selector) {
    if ([string]::IsNullOrWhiteSpace($selector)) { return $null }
    $m1 = [regex]::Match($selector, "name='([^']+)'")
    if ($m1.Success) { return $m1.Groups[1].Value }
    $m2 = [regex]::Match($selector, 'name="([^"]+)"')
    if ($m2.Success) { return $m2.Groups[1].Value }
    $m3 = [regex]::Match($selector, "has-text=/\\^?([^$/]+)\\$?/")
    if ($m3.Success) { return $m3.Groups[1].Value }
    $m4 = [regex]::Match($selector, 'text="([^"]+)"')
    if ($m4.Success) { return $m4.Groups[1].Value }
    $m5 = [regex]::Match($selector, "text=/\\^?([^$/]+)\\$?/")
    if ($m5.Success) { return $m5.Groups[1].Value }
    return $null
}

function Pick-SelectorForStep($stepText, $selectors) {
    if (!$selectors) { return $null }
    $textLower = $stepText.ToLower()
    foreach ($s in $selectors) {
        $name = Extract-SelectorName $s
        if ([string]::IsNullOrWhiteSpace($name)) { continue }
        if ($name.Length -lt 3) { continue }
        if ($textLower -match "log out|logs out|logout") {
            if ($name.ToLower() -match "log out|logout") { return $s }
        }
        if ($textLower -like "*$($name.ToLower())*") {
            return $s
        }
    }
    $tokens = $textLower -split "\s+" | Where-Object { $_.Length -ge 3 }
    foreach ($s in $selectors) {
        $selLower = $s.ToLower()
        foreach ($t in $tokens) {
            if ($selLower -like "*$t*") {
                return $s
            }
        }
    }
    return $null
}

function Infer-Action($stepText) {
    $t = $stepText.ToLower()
    if ($t -match "click|select|open|press|tap|log out|logs out|logout") { return "click" }
    if ($t -match "see|visible|displayed|shown|should be|am on|on the|navigated") { return "wait" }
    return "todo"
}

Ensure-Dir $PagesOut
Ensure-Dir $StepsOut

$featureFiles = Get-ChildItem -Path $FeaturesRoot -Filter "*.feature" -File -ErrorAction SilentlyContinue
if (!$featureFiles) {
    Write-Host "No .feature files found in $FeaturesRoot"
    exit 0
}

if ($PageKey -and $PageKey.Trim().Length -gt 0) {
    $pk = $PageKey.Trim().ToLower()
    $featureFiles = $featureFiles | Where-Object {
        ([System.IO.Path]::GetFileNameWithoutExtension($_.Name)).ToLower() -eq $pk
    }
    if (!$featureFiles) {
        Write-Host "No .feature file matched PageKey '$PageKey' in $FeaturesRoot"
        exit 1
    }
}

foreach ($ff in $featureFiles) {
    $featureName = [System.IO.Path]::GetFileNameWithoutExtension($ff.Name)
    $pageKey = $featureName.ToLower()

    $selectorsObj = Read-Selectors $pageKey

    $lines = Get-Content -Path $ff.FullName

    $steps = @()
    foreach ($line in $lines) {
        $trim = $line.Trim()
        if ($trim -match "^(Given|When|Then|And|But)\s+") {
            $keyword = $Matches[1]
            $text = $trim.Substring($keyword.Length).Trim()
            $steps += [pscustomobject]@{ keyword = $keyword; text = $text }
        }
    }

    $pageClassName = "${featureName}Page"
    $stepsClassName = "${featureName}StepDefinitions"
    $pageVarName = $featureName.Substring(0,1).ToLower() + $featureName.Substring(1)
    if ($featureName.Length -ge 2 -and $featureName.Substring(0,2) -eq $featureName.Substring(0,2).ToUpper()) {
        # Preserve common acronym case: APIPage -> apiPage, DFPDashboard -> dfpDashboard
        $pageVarName = $featureName.Substring(0,2).ToLower() + $featureName.Substring(2)
    }


    # Generate Page class (incremental)
    $pageNs = "$NamespaceRoot.Pages.Generated"
    if ($PagesOut.ToLower().Contains("\\pages\\web")) {
        $pageNs = "$NamespaceRoot.Pages.Web"
    }
    $pagePath = Join-Path $PagesOut ("{0}.cs" -f $pageClassName)

    $selectorsBlock = New-Object System.Text.StringBuilder
    $null = $selectorsBlock.AppendLine("        // codegen:selectors-start")
    $null = $selectorsBlock.AppendLine("        // Selectors captured by codegen for '$pageKey'")
    $null = $selectorsBlock.AppendLine("        public static readonly string[] Selectors = new[]")
    $null = $selectorsBlock.AppendLine("        {")
    if ($selectorsObj -and $selectorsObj.selectors) {
        foreach ($s in $selectorsObj.selectors) {
            $null = $selectorsBlock.AppendLine(('            "{0}",' -f (Escape-CSharpString $s)))
        }
    }
    $null = $selectorsBlock.AppendLine("        };")
    $null = $selectorsBlock.AppendLine("        // codegen:selectors-end")
    $selectorsBlockText = $selectorsBlock.ToString()

    $pageMethodsSb = New-Object System.Text.StringBuilder
    foreach ($s in $steps) {
        $methodName = Step-MethodName $s.text
        $selector = $null
        if ($selectorsObj -and $selectorsObj.selectors) {
            $selector = Pick-SelectorForStep $s.text $selectorsObj.selectors
        }
        $action = Infer-Action $s.text
        $null = $pageMethodsSb.AppendLine("        public async Task $methodName()")
        $null = $pageMethodsSb.AppendLine("        {")
        if ($pageKey -eq "login" -and $selectorsObj -and $selectorsObj.login -and ($s.text.ToLower() -match "login page|login screen")) {
            $u = Escape-CSharpString $selectorsObj.login.username
            if ($u) {
                $null = $pageMethodsSb.AppendLine(('            await Page.WaitForSelectorAsync("{0}");' -f $u))
            } else {
                $null = $pageMethodsSb.AppendLine("            // TODO_UI: login page selector not found")
                $null = $pageMethodsSb.AppendLine("            return;")
            }
        } elseif ($pageKey -eq "login" -and $selectorsObj -and $selectorsObj.login -and ($s.text.ToLower() -match "logs in|log in|login with|sign in")) {
            $u = Escape-CSharpString $selectorsObj.login.username
            $p = Escape-CSharpString $selectorsObj.login.password
            $sub = Escape-CSharpString $selectorsObj.login.submit
            $null = $pageMethodsSb.AppendLine('            var username = Environment.GetEnvironmentVariable(Constants.DFP_USERNAME);')
            $null = $pageMethodsSb.AppendLine('            var password = Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD);')
            $null = $pageMethodsSb.AppendLine('            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))')
            $null = $pageMethodsSb.AppendLine('                throw new InvalidOperationException("DFP_USERNAME/DFP_PASSWORD are required.");')
            if ($u) {
                $null = $pageMethodsSb.AppendLine(('            await Page.Locator("{0}").FillAsync(username);' -f $u))
            }
            if ($p) {
                $null = $pageMethodsSb.AppendLine(('            await Page.Locator("{0}").FillAsync(password);' -f $p))
            }
            if ($sub) {
                $null = $pageMethodsSb.AppendLine(('            await Page.Locator("{0}").ClickAsync();' -f $sub))
            } else {
                $null = $pageMethodsSb.AppendLine('            await Page.Keyboard.PressAsync("Enter");')
            }
        } elseif ($selector -and $action -eq "click") {
            $null = $pageMethodsSb.AppendLine(('            var locator = await FindLocatorAsync(new[] {{ "{0}" }});' -f (Escape-CSharpString $selector)))
            $null = $pageMethodsSb.AppendLine("            await locator.ClickAsync();")
        } elseif ($selector -and $action -eq "wait") {
            $null = $pageMethodsSb.AppendLine(('            await FindLocatorAsync(new[] {{ "{0}" }});' -f (Escape-CSharpString $selector)))
        } else {
            $null = $pageMethodsSb.AppendLine(('            // TODO_UI: map selector for step "{0}"' -f $s.text))
            if ($selector) {
                $null = $pageMethodsSb.AppendLine(('            // TODO_UI: candidate selector: {0}' -f $selector))
            }
            $null = $pageMethodsSb.AppendLine("            return;")
        }
        $null = $pageMethodsSb.AppendLine("        }")
        $null = $pageMethodsSb.AppendLine("")
    }
    $pageMethodsText = $pageMethodsSb.ToString()

    if (Test-Path $pagePath) {
        $existingContent = Get-Content -Raw $pagePath
        $existingMethods = Get-ExistingMethodNames $existingContent
        $methodsToAppend = New-Object System.Text.StringBuilder
        foreach ($s in $steps) {
            $methodName = Step-MethodName $s.text
            if ($existingMethods.Contains($methodName)) { continue }
            # regenerate just this method
            $selector = $null
            if ($selectorsObj -and $selectorsObj.selectors) { $selector = Pick-SelectorForStep $s.text $selectorsObj.selectors }
            $action = Infer-Action $s.text
            $null = $methodsToAppend.AppendLine("        public async Task $methodName()")
            $null = $methodsToAppend.AppendLine("        {")
            if ($pageKey -eq "login" -and $selectorsObj -and $selectorsObj.login -and ($s.text.ToLower() -match "login page|login screen")) {
                $u = Escape-CSharpString $selectorsObj.login.username
                if ($u) {
                    $null = $methodsToAppend.AppendLine(('            await Page.WaitForSelectorAsync("{0}");' -f $u))
                } else {
                    $null = $methodsToAppend.AppendLine("            // TODO_UI: login page selector not found")
                    $null = $methodsToAppend.AppendLine("            return;")
                }
            } elseif ($pageKey -eq "login" -and $selectorsObj -and $selectorsObj.login -and ($s.text.ToLower() -match "logs in|log in|login with|sign in")) {
                $u = Escape-CSharpString $selectorsObj.login.username
                $p = Escape-CSharpString $selectorsObj.login.password
                $sub = Escape-CSharpString $selectorsObj.login.submit
                $null = $methodsToAppend.AppendLine('            var username = Environment.GetEnvironmentVariable(Constants.DFP_USERNAME);')
                $null = $methodsToAppend.AppendLine('            var password = Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD);')
                $null = $methodsToAppend.AppendLine('            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))')
                $null = $methodsToAppend.AppendLine('                throw new InvalidOperationException("DFP_USERNAME/DFP_PASSWORD are required.");')
                if ($u) { $null = $methodsToAppend.AppendLine(('            await Page.Locator("{0}").FillAsync(username);' -f $u)) }
                if ($p) { $null = $methodsToAppend.AppendLine(('            await Page.Locator("{0}").FillAsync(password);' -f $p)) }
                if ($sub) { $null = $methodsToAppend.AppendLine(('            await Page.Locator("{0}").ClickAsync();' -f $sub)) }
                else { $null = $methodsToAppend.AppendLine('            await Page.Keyboard.PressAsync("Enter");') }
            } elseif ($selector -and $action -eq "click") {
                $null = $methodsToAppend.AppendLine(('            var locator = await FindLocatorAsync(new[] {{ "{0}" }});' -f (Escape-CSharpString $selector)))
                $null = $methodsToAppend.AppendLine("            await locator.ClickAsync();")
            } elseif ($selector -and $action -eq "wait") {
                $null = $methodsToAppend.AppendLine(('            await FindLocatorAsync(new[] {{ "{0}" }});' -f (Escape-CSharpString $selector)))
            } else {
                $null = $methodsToAppend.AppendLine(('            // TODO_UI: map selector for step "{0}"' -f $s.text))
                if ($selector) { $null = $methodsToAppend.AppendLine(('            // TODO_UI: candidate selector: {0}' -f $selector)) }
                $null = $methodsToAppend.AppendLine("            return;")
            }
            $null = $methodsToAppend.AppendLine("        }")
            $null = $methodsToAppend.AppendLine("")
        }

        $updated = Replace-SelectorsBlock $existingContent $selectorsBlockText
        $updated = Insert-MethodsBeforeClassEnd $updated $methodsToAppend.ToString()
        Set-Content -Path $pagePath -Value $updated
    } else {
        $pageSb = New-Object System.Text.StringBuilder
        $null = $pageSb.AppendLine("// <auto-generated />")
        $null = $pageSb.AppendLine("using Microsoft.Playwright;")
        $null = $pageSb.AppendLine("using System;")
        $null = $pageSb.AppendLine("using System.Threading.Tasks;")
        $null = $pageSb.AppendLine("using DFP.Playwright.Pages.Web.BasePages;")
        $null = $pageSb.AppendLine("using DFP.Playwright.Helpers;")
        $null = $pageSb.AppendLine("")
        $null = $pageSb.AppendLine("namespace $pageNs")
        $null = $pageSb.AppendLine("{")
        $null = $pageSb.AppendLine("    public sealed class $pageClassName : BasePage")
        $null = $pageSb.AppendLine("    {")
        $null = $pageSb.AppendLine("        public $pageClassName(IPage page) : base(page)")
        $null = $pageSb.AppendLine("        {")
        $null = $pageSb.AppendLine("        }")
        $null = $pageSb.AppendLine("")
        $null = $pageSb.AppendLine($selectorsBlockText.TrimEnd())
        $null = $pageSb.AppendLine("")
        $null = $pageSb.AppendLine($pageMethodsText.TrimEnd())
        $null = $pageSb.AppendLine("    }")
        $null = $pageSb.AppendLine("}")
        $pageSb.ToString() | Set-Content -Path $pagePath
    }

    # Generate StepDefinitions class (incremental)
    $stepsNs = "$NamespaceRoot.StepDefinitions.Generated"
    $stepsPath = Join-Path $StepsOut ("{0}.cs" -f $stepsClassName)

    if (Test-Path $stepsPath) {
        $existingSteps = Get-Content -Raw $stepsPath
        # Normalize TestContext to fully-qualified to avoid ambiguity
        $existingSteps = $existingSteps.Replace("using DFP.Playwright.Support;`r`n", "")
        $existingSteps = $existingSteps.Replace("using DFP.Playwright.Support;`n", "")
        $existingSteps = $existingSteps.Replace("private readonly TestContext ", "private readonly DFP.Playwright.Support.TestContext ")
        $existingSteps = $existingSteps.Replace("public $stepsClassName(TestContext ", "public $stepsClassName(DFP.Playwright.Support.TestContext ")

        $existingMethods = Get-ExistingMethodNames $existingSteps
        $methodsToAppend = New-Object System.Text.StringBuilder
        foreach ($s in $steps) {
            $methodName = Step-MethodName $s.text
            if ($existingMethods.Contains($methodName)) { continue }
            $keyword = $s.keyword
            $null = $methodsToAppend.AppendLine(('        [{0}("{1}")]' -f $keyword, (Escape-CSharpString $s.text)))
            $null = $methodsToAppend.AppendLine("        public async Task $methodName()")
            $null = $methodsToAppend.AppendLine("        {")
            $null = $methodsToAppend.AppendLine("            await _$pageVarName.$methodName();")
            $null = $methodsToAppend.AppendLine("        }")
            $null = $methodsToAppend.AppendLine("")
        }
        $updatedSteps = Insert-MethodsBeforeClassEnd $existingSteps $methodsToAppend.ToString()
        Set-Content -Path $stepsPath -Value $updatedSteps
    } else {
        $stepsSb = New-Object System.Text.StringBuilder
        $null = $stepsSb.AppendLine("// <auto-generated />")
        $null = $stepsSb.AppendLine("using Reqnroll;")
        $null = $stepsSb.AppendLine("using System.Threading.Tasks;")
        $null = $stepsSb.AppendLine("using $pageNs;")
        $null = $stepsSb.AppendLine("")
        $null = $stepsSb.AppendLine("namespace $stepsNs")
        $null = $stepsSb.AppendLine("{")
        $null = $stepsSb.AppendLine("    [Binding]")
        $null = $stepsSb.AppendLine("    public sealed class $stepsClassName")
        $null = $stepsSb.AppendLine("    {")
        $null = $stepsSb.AppendLine("        private readonly $NamespaceRoot.Support.TestContext _tc;")
        $null = $stepsSb.AppendLine("        private readonly $pageClassName _$pageVarName;")
        $null = $stepsSb.AppendLine("")
        $null = $stepsSb.AppendLine("        public $stepsClassName($NamespaceRoot.Support.TestContext tc, $pageClassName $pageVarName)")
        $null = $stepsSb.AppendLine("        {")
        $null = $stepsSb.AppendLine("            _tc = tc;")
        $null = $stepsSb.AppendLine("            _$pageVarName = $pageVarName;")
        $null = $stepsSb.AppendLine("        }")
        $null = $stepsSb.AppendLine("")
        foreach ($s in $steps) {
            $methodName = Step-MethodName $s.text
            $keyword = $s.keyword
            $null = $stepsSb.AppendLine(('        [{0}("{1}")]' -f $keyword, (Escape-CSharpString $s.text)))
            $null = $stepsSb.AppendLine("        public async Task $methodName()")
            $null = $stepsSb.AppendLine("        {")
            $null = $stepsSb.AppendLine("            await _$pageVarName.$methodName();")
            $null = $stepsSb.AppendLine("        }")
            $null = $stepsSb.AppendLine("")
        }
        $null = $stepsSb.AppendLine("    }")
        $null = $stepsSb.AppendLine("}")
        $stepsSb.ToString() | Set-Content -Path $stepsPath
    }

    Write-Host "Generated: $pagePath"
    Write-Host "Generated: $stepsPath"
}
