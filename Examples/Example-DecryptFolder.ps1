Import-Module .\PSPGP.psd1 -Force

Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'ZielonaMila9!' -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded