Import-Module .\PSPGP.psd1 -Force

Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded