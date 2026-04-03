Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$SignedString = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'This is signed text'

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $SignedString
# Returns a VerificationResult object with Status $true when the signature is valid

$ClearSigned = Protect-PGP -ClearSign -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'This is clear-signed text'
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ClearSigned -ClearSigned
