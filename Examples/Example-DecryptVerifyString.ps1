Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$EncryptedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'This is signed and encrypted text'
$EncryptedString

Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -Password 'ZielonaMila9!' -String $EncryptedString -Verify
