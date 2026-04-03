Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP1.asc
