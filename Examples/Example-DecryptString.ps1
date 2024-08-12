Import-Module .\PSPGP.psd1 -Force

$ProtectedString = Protect-PGP -FilePathPublic "$PSScriptRoot\Keys\PublicPGP1.asc" -String "This is string to encrypt"

Unprotect-PGP -FilePathPrivate "$PSScriptRoot\Keys\PrivatePGP1.asc" -Password 'ZielonaMila9!' -String $ProtectedString