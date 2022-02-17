Import-Module .\PSPGP.psd1 -Force

Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded