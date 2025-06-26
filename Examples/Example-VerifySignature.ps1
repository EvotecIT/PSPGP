Import-Module .\PSPGP.psd1 -Force

# encrypt using first key
$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "This is string to encrypt"

# verify using one or more public keys
Test-PGP -FilePathPublic @($PSScriptRoot\Keys\PublicPGP.asc, $PSScriptRoot\Keys\PublicPGP2.asc) -String $ProtectedString
# Expected to produce no output when signature is valid
# verify signatures in folder with multiple keys
Test-PGP -FilePathPublic @($PSScriptRoot\Keys\PublicPGP.asc, $PSScriptRoot\Keys\PublicPGP2.asc) -FolderPath $PSScriptRoot\Encoded
# Returns objects with Status $true for valid signatures
