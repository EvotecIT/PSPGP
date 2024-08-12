Import-Module .\PSPGP.psd1 -Force

Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded

Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -FolderPath '~\Downloads\Cloudflare' -OutputFolderPath '~\Downloads\Cloudflare'