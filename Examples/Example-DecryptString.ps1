Import-Module .\PSPGP.psd1 -Force

$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "This is string to encrypt"

Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -Password 'ZielonaMila9!' -String $ProtectedString