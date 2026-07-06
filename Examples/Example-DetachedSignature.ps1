Import-Module (Join-Path $PSScriptRoot '..\PSPGP.psd1') -Force

$Message = 'This text is signed separately from its detached signature'
$Signature = Protect-PGP -SignOnly -Detached -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String $Message
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $Message -Signature $Signature

$SourceFile = Join-Path $PSScriptRoot 'Test\Test1.txt'
$SignatureFile = Join-Path $PSScriptRoot 'Test\Test1.txt.sig'
Protect-PGP -SignOnly -Detached -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -FilePath $SourceFile -OutFilePath $SignatureFile
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $SourceFile -SignaturePath $SignatureFile
