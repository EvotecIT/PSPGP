Import-Module .\PSPGP.psd1 -Force

$String = Protect-PGP -FilePathPublic @("$PSScriptRoot\Keys\PublicPGP1.asc", "$PSScriptRoot\Keys\PublicPGP2.asc") -String "This is string to encrypt"
$String


Unprotect-PGP -String $String -FilePathPrivate "$PSScriptRoot\Keys\PrivatePGP1.asc" -Password 'ZielonaMila9!'

Unprotect-PGP -String $String -FilePathPrivate "$PSScriptRoot\Keys\PrivatePGP2.asc" -Password 'ZielonaMila10!'