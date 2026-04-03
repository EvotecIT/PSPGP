Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded
