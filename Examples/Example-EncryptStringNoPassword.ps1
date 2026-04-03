Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

# Using a public key to encrypt a string
$EncryptedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP3.asc -String 'This is string to encrypt'

# Using a private key to decrypt a string without any password
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP3.asc -String $EncryptedString
