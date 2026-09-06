param(
    [string]$Method,
    [string]$LogName = 'unity',
    [switch]$Graphics,
    [switch]$Test
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$editor = 'C:\Program Files\Unity 6000.3.23f1\Editor\Unity.exe'
# Conservative guard: never contend with an interactive or another batch editor.
if (Get-Process Unity -ErrorAction SilentlyContinue) {
    throw 'Close Unity before running this command. No existing editor was stopped.'
}
$artifacts = Join-Path $root 'Artifacts'
New-Item -ItemType Directory -Force $artifacts | Out-Null
$arguments = @('-batchmode', '-projectPath', ('"{0}"' -f (Join-Path $root 'Game')), '-logFile', ('"{0}"' -f (Join-Path $artifacts ($LogName + '.log'))))
if (-not $Graphics) { $arguments += '-nographics' }
if ($Test) {
    $arguments += @('-runTests', '-testPlatform', 'EditMode', '-testResults', ('"{0}"' -f (Join-Path $artifacts 'editmode-results.xml')))
} else {
    $arguments += '-quit'
    if ($Method) { $arguments += @('-executeMethod', $Method) }
}
$process = Start-Process -FilePath $editor -ArgumentList $arguments -PassThru -WindowStyle Hidden
# Wait for the editor itself, not long-lived licensing/package-service descendants.
$process.WaitForExit()
@{ method = $Method; exitCode = $process.ExitCode; log = $LogName; utc = [DateTime]::UtcNow.ToString('o') } | ConvertTo-Json | Set-Content (Join-Path $artifacts ($LogName + '-exit.json'))
if ($process.ExitCode -ne 0) { throw "Unity exited with $($process.ExitCode). See Artifacts/$LogName.log." }
