# Read-only validation of downloaded packages. Does not execute installers.
$ErrorActionPreference = 'Stop'
$cache = Join-Path (Split-Path $PSScriptRoot -Parent) 'Downloads'
$release = Get-Content (Join-Path $PSScriptRoot 'unity-release.json') -Raw | ConvertFrom-Json
$unity = $release.downloads | Where-Object { $_.platform -eq 'WINDOWS' -and $_.architecture -eq 'X86_64' } | Select-Object -First 1
$blenderFile = 'blender-4.5.13-windows-x64.msi'
$checksumLine = Get-Content (Join-Path $PSScriptRoot 'blender-4.5.13.sha256') | Where-Object { $_.EndsWith('  ' + $blenderFile) }
if (@($checksumLine).Count -ne 1) { throw 'Expected exactly one Blender x64 MSI checksum.' }
$blenderSha256 = ($checksumLine -split '\s+')[0]
$expected = @(
    @{ File = 'UnitySetup64-6000.3.23f1.exe'; Bytes = [long]$unity.downloadSize.value; Algorithm = 'MD5'; Hash = [BitConverter]::ToString([Convert]::FromBase64String(($unity.integrity -replace '^md5-', ''))).Replace('-', ''); Url = $unity.url },
    @{ File = $blenderFile; Bytes = 359968768L; Algorithm = 'SHA256'; Hash = $blenderSha256; Url = 'https://download.blender.org/release/Blender4.5/' + $blenderFile }
)
$results = foreach ($item in $expected) {
    $path = Join-Path $cache $item.File
    $file = Get-Item -LiteralPath $path
    if ($file.Length -ne $item.Bytes) { throw "Incorrect byte count for $($item.File)" }
    $hash = (Get-FileHash -LiteralPath $path -Algorithm $item.Algorithm).Hash
    if ($hash -ne $item.Hash) { throw "Publisher checksum mismatch for $($item.File)" }
    $signature = Get-AuthenticodeSignature -LiteralPath $path
    [pscustomobject]@{
        File = $item.File
        Url = $item.Url
        Bytes = $file.Length
        PublisherChecksumAlgorithm = $item.Algorithm
        PublisherChecksum = $hash
        PublisherChecksumMatched = $true
        SHA256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
        SignatureStatus = [string]$signature.Status
        Signer = $signature.SignerCertificate.Subject
        CheckedAtUtc = [DateTime]::UtcNow.ToString('o')
        InstallerExecuted = $false
    }
}
$results | ConvertTo-Json -Depth 4
if ($results.Where({ $_.SignatureStatus -ne 'Valid' }).Count -gt 0) {
    throw 'Publisher checksums match, but at least one signature requires review.'
}
