Import-Module .\PSPGP.psd1 -Force

New-PGPKey -HashAlgorithm Sha512 -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP2.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP2.asc -Password 'ZielonaMila10!' -UserName 'przemyslaw.klys@evotec.pl'

# No password is required if you don't want one and just trust the key
#New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP3.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP3.asc -Strength 4096 -Certainty 8
New-PGPKey -HashAlgorithm Sha512 -FilePathPublic $PSScriptRoot\Keys\PublicPGP3.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP3.asc

<#
New-PGPKey -FilePathPublic '\\ad1\c$\Temp\PublicPGP1.asc' -FilePathPrivate '\\ad1\c$\Temp\PrivatePGP1.asc' -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
New-PGPKey -FilePathPublic '\\ad1\c$\Temp\PublicPGP2.asc' -FilePathPrivate '\\ad1\c$\Temp\PrivatePGP2.asc' -UserName 'przemyslaw.klys' -Password 'ZielonaMila10!'
# No password is required if you don't want one and just trust the key
New-PGPKey -FilePathPublic '\\ad1\c$\Temp\PublicPGP3.asc' -FilePathPrivate '\\ad1\c$\Temp\PrivatePGP3.asc'
#>