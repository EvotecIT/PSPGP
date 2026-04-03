Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP1.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded

Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP1.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -FolderPath '~\Downloads\Cloudflare' -OutputFolderPath '~\Downloads\Cloudflare'
