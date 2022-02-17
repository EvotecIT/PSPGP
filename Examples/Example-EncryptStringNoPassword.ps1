Import-Module .\PSPGP.psd1 -Force

# Using public key to encrypt a file
$EncryptedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP3.asc -String "This is string to encrypt"

# Using private key to decrypt a file without any password
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP3.asc -String $EncryptedString