Import-Module .\PSPGP.psd1 -Force

New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP2.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP2.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila10!'