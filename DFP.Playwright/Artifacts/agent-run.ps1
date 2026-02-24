param(
    [Parameter(Mandatory=$true)]
    [string]$PageKey,
    [Parameter(Mandatory=$true)]
    [string]$BaseUrl,
    [Parameter(Mandatory=$false)]
    [string]$ArtifactsRoot
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptRoot "..")

if ([string]::IsNullOrWhiteSpace($ArtifactsRoot)) {
    $ArtifactsRoot = Join-Path $projectRoot "Artifacts"
}

$watcherScript = Join-Path $ArtifactsRoot "codegen-watch.ps1"
$codegenScript = Join-Path $ArtifactsRoot "codegen-start.ps1"
$agentScript = Join-Path $ArtifactsRoot "agent-generate.ps1"

if (!(Test-Path $watcherScript)) { throw "Missing: $watcherScript" }
if (!(Test-Path $codegenScript)) { throw "Missing: $codegenScript" }
if (!(Test-Path $agentScript)) { throw "Missing: $agentScript" }

# Start watcher in background
$job = Start-Job -ScriptBlock {
    param($scriptPath)
    if (Get-Command "pwsh" -ErrorAction SilentlyContinue) {
        pwsh -File $scriptPath
    } else {
        powershell -ExecutionPolicy Bypass -File $scriptPath
    }
} -ArgumentList $watcherScript

# Launch codegen (interactive)
if (Get-Command "pwsh" -ErrorAction SilentlyContinue) {
    pwsh -File $codegenScript -PageKey $PageKey -BaseUrl $BaseUrl
} else {
    powershell -ExecutionPolicy Bypass -File $codegenScript -PageKey $PageKey -BaseUrl $BaseUrl
}

# After codegen exits, run generator
if (Get-Command "pwsh" -ErrorAction SilentlyContinue) {
    pwsh -File $agentScript -PageKey $PageKey
} else {
    powershell -ExecutionPolicy Bypass -File $agentScript -PageKey $PageKey
}

# Stop watcher job
Stop-Job $job | Out-Null
Remove-Job $job | Out-Null

Write-Host "Done: selectors updated and pages/steps generated."
