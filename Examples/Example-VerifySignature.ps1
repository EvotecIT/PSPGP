Import-Module .\PSPGP.psd1 -Force

$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "This is string to encrypt"

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String $ProtectedString

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FolderPath $PSScriptRoot\Encoded