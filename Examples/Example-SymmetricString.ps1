Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$EncryptedString = Protect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String 'This is string to encrypt'
$EncryptedString

Unprotect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String $EncryptedString
