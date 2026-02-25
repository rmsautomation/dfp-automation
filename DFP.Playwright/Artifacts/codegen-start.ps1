param(
    [Parameter(Mandatory=$true)]
    [string]$PageKey,
    [Parameter(Mandatory=$false)]
    [string]$BaseUrl,
    [Parameter(Mandatory=$false)]
    [string]$ArtifactsRoot,
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug","Release")]
    [string]$Configuration = "Debug"
)

if ([string]::IsNullOrWhiteSpace($PageKey)) {
    throw "PageKey is required. Example: -PageKey login"
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptRoot "..")
$isWin = ($env:OS -eq "Windows_NT") -or ($PSVersionTable.PSEdition -eq "Desktop")

if ([string]::IsNullOrWhiteSpace($ArtifactsRoot)) {
    $ArtifactsRoot = Join-Path $projectRoot "Artifacts"
}

$envFile = Join-Path $projectRoot "Config\.env.local"
if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
    if (Test-Path $envFile) {
        $lines = Get-Content $envFile | Where-Object { $_ -and -not $_.TrimStart().StartsWith("#") }
        foreach ($line in $lines) {
            if ($line -match "^\s*BASE_URL\s*=\s*(.+)\s*$") {
                $BaseUrl = $Matches[1].Trim()
                break
            }
        }
    }
}

if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
    $BaseUrl = [Environment]::GetEnvironmentVariable("BASE_URL")
}

if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
    throw "BaseUrl not found. Provide -BaseUrl or set BASE_URL in Config/.env.local or env."
}

# PowerShell 5.1 compatible trim of trailing separators
$ArtifactsRoot = $ArtifactsRoot.TrimEnd('\','/')
if (!(Test-Path $ArtifactsRoot)) { New-Item -ItemType Directory -Force $ArtifactsRoot | Out-Null }

$jsonlPath = Join-Path $ArtifactsRoot ("codegen.{0}.jsonl" -f $PageKey)

$binRoot = Join-Path $projectRoot ("bin\{0}\net9.0" -f $Configuration)
if ($isWin) {
    $playwrightScript = Join-Path $binRoot "playwright.ps1"
} else {
    $playwrightScript = Join-Path $binRoot "playwright.sh"
}
if (!(Test-Path $playwrightScript)) {
    Write-Host "Playwright script not found. Building project ($Configuration)..."
    & "dotnet" "build" $projectRoot "-c" $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed. ExitCode=$LASTEXITCODE"
    }
    if (!(Test-Path $playwrightScript)) {
        throw "Playwright script not found after build: $playwrightScript"
    }
}

Write-Host "Running: $playwrightScript codegen --target=jsonl --output $jsonlPath $BaseUrl"
if ($isWin) {
    & $playwrightScript "codegen" "--target=jsonl" "--output" $jsonlPath $BaseUrl
} else {
    & "bash" $playwrightScript "codegen" "--target=jsonl" "--output" $jsonlPath $BaseUrl
}
