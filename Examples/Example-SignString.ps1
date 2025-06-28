Import-Module ./PSPGP.psd1 -Force

$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP.asc -SignPassword 'ZielonaMila9!' -String 'This is signed text'

Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String $Signature
