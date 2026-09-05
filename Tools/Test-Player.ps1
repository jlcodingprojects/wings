param([switch]$Visible)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$player = Join-Path $root 'Builds/Checkpoint1C/Wings.exe'
$artifacts = Join-Path $root 'Artifacts'
$arguments = @('-screen-fullscreen', '0', '-screen-width', '1440', '-screen-height', '900', '-force-d3d11', '-wingsSmoke', ('"{0}"' -f $artifacts), '-logFile', ('"{0}"' -f (Join-Path $artifacts 'player-smoke.log')))
# Visible is an explicit user opt-in. Hidden runs may not render and will fail image validation.
$windowStyle = if ($Visible) { 'Normal' } else { 'Hidden' }
$process = Start-Process -FilePath $player -ArgumentList $arguments -PassThru -WindowStyle $windowStyle
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Player check failed ($($process.ExitCode)); see Artifacts/player-smoke.json and player-smoke.log." }
$result = Get-Content (Join-Path $artifacts 'player-smoke.json') -Raw | ConvertFrom-Json
if ($result.result -ne 'passed') { throw 'Player reported a failed check.' }
$result
