Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$ClearSigned = Protect-PGP -ClearSign -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'This is clear-signed text'
$ClearSigned

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ClearSigned -ClearSigned
