Import-Module .\PSPGP.psd1 -Force

New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP2.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP2.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila10!'
# No password is required if you don't want one and just trust the key
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP3.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP3.asc