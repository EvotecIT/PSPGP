Import-Module .\PSPGP.psd1 -Force

Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -Password 'ZielonaMila9!' -FilePath $PSScriptRoot\Encoded\Test1.txt.pgp -OutFilePath $PSScriptRoot\Decoded\Test4.txt