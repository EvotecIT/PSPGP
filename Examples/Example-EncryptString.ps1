Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String 'This is string to encrypt' -HashAlgorithm Sha256 -CompressionAlgorithm Zip
