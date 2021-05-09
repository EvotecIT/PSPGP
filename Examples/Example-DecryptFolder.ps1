Import-Module .\PSPGP.psd1 -Force

Unprotect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded