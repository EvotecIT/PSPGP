Import-Module .\PSPGP.psd1 -Force

New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'