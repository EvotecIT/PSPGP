Import-Module .\PSPGP.psd1 -Force

Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "This is string to encrypt"