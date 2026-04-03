Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'This is signed text'

Get-PGPInspect -String $Signature
