Import-Module .\PSPGP.psd1 -Force

$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "This is string to encrypt"

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String $ProtectedString
# Expected to produce no output when signature is valid

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FolderPath $PSScriptRoot\Encoded
# Returns objects with Status $true for valid signatures
