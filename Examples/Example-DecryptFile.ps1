Import-Module .\PSPGP.psd1 -Force

Protect-PGP -FilePathPublic "$PSScriptRoot\Keys\PublicPGP1.asc" -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Encoded\Test1.txt.pgp
Unprotect-PGP -FilePathPrivate "$PSScriptRoot\Keys\PrivatePGP1.asc" -Password 'ZielonaMila9!' -FilePath $PSScriptRoot\Encoded\Test1.txt.pgp -OutFilePath $PSScriptRoot\Decoded\Test4.txt

#Protect-PGP -FilePathPublic "\\ad1\c$\Temp\PublicPGP1.asc" -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Encoded\Test1.txt.pgp
#Unprotect-PGP -FilePathPrivate '\\ad1\c$\Temp\PrivatePGP1.asc' -Password 'ZielonaMila9!' -FilePath $PSScriptRoot\Encoded\Test1.txt.pgp -OutFilePath $PSScriptRoot\Decoded\Test4.txt
#Unprotect-PGP -FilePathPrivate 'C:\Temp\PrivatePGP1.asc' -Password 'ZielonaMila9!' -FilePath $PSScriptRoot\Encoded\Test1.txt.pgp -OutFilePath $PSScriptRoot\Decoded\Test4.txt

#Protect-PGP -FilePathPublic "\\ad1\c$\Temp\PublicPGP1.asc" -FilePath "\\AD1.AD.EVOTEC.XYZ\c`$\Temp\PrivatePGP1.asc" -OutFilePath "\\AD1.AD.EVOTEC.XYZ\c`$\Temp\PrivatePGP1-Encrypted.asc.pgp"
#Unprotect-PGP -FilePathPrivate '\\ad1\c$\Temp\PrivatePGP1.asc' -Password 'ZielonaMila9!' -FilePath "\\AD1.AD.EVOTEC.XYZ\c`$\Temp\PrivatePGP1-Encrypted.asc.pgp" -OutFilePath "\\AD1.AD.EVOTEC.XYZ\c`$\Temp\PrivatePGP1-Encrypted.asc.pgp.asc"