# PSPGP - PowerShell Module

<p align="center">
  <a href="https://dev.azure.com/evotecpl/PSPGP/_build/results?buildId=latest"><img src="https://dev.azure.com/evotecpl/PSPGP/_apis/build/status/EvotecIT.PSPGP"></a>
  <a href="https://www.powershellgallery.com/packages/PSPGP"><img src="https://img.shields.io/powershellgallery/v/PSPGP.svg?style=flat-square"></a>
  <a href="https://www.powershellgallery.com/packages/PSPGP"><img src="https://img.shields.io/powershellgallery/v/PSPGP.svg?label=powershell%20gallery%20preview&colorB=yellow&style=flat-square&include_prereleases"></a>
  <a href="https://github.com/EvotecIT/PSPGP"><img src="https://img.shields.io/github/license/EvotecIT/PSPGP.svg?style=flat-square"></a>
</p>

<p align="center">
  <a href="https://www.powershellgallery.com/packages/PSPGP"><img src="https://img.shields.io/powershellgallery/p/PSPGP.svg?style=flat-square"></a>
  <a href="https://github.com/EvotecIT/PSPGP"><img src="https://img.shields.io/github/languages/top/evotecit/PSPGP.svg?style=flat-square"></a>
  <a href="https://github.com/EvotecIT/PSPGP"><img src="https://img.shields.io/github/languages/code-size/evotecit/PSPGP.svg?style=flat-square"></a>
  <a href="https://www.powershellgallery.com/packages/PSPGP"><img src="https://img.shields.io/powershellgallery/dt/PSPGP.svg?style=flat-square"></a>
</p>

<p align="center">
  <a href="https://twitter.com/PrzemyslawKlys"><img src="https://img.shields.io/twitter/follow/PrzemyslawKlys.svg?label=Twitter%20%40PrzemyslawKlys&style=flat-square"></a>
  <a href="https://evotec.xyz/hub"><img src="https://img.shields.io/badge/Blog-evotec.xyz-2A6496.svg?style=flat-square"></a>
  <a href="https://www.linkedin.com/in/pklys"><img src="https://img.shields.io/badge/LinkedIn-pklys-0077B5.svg?logo=LinkedIn&style=flat-square"></a>
  <a href="https://evo.yt/discord"><img alt="Discord" src="https://img.shields.io/discord/508328927853281280?style=flat-square&label=discord%20chat"></a>
</p>

**PSPGP** is a PowerShell module that provides PGP functionality in PowerShell. It allows encrypting and decrypting files/folders and strings using PGP.
**PSPGP** uses the following .NET library:

- [PgpCore](https://github.com/mattosaurus/PgpCore) - licensed MIT

Exported cmdlets:

- `Get-PGPInspect`
- `Get-PGPKey`
- `Get-PGPKeyInfo`
- `New-PGPKey`
- `Protect-PGP`
- `Test-PGP`
- `Unprotect-PGP`

The checked-in sample assets under `Examples\Keys` use the `PublicPGP1.asc` / `PrivatePGP1.asc` naming pattern.

## To install

```powershell
Install-Module -Name PSPGP -AllowClobber -Force
```

Force and AllowClobber aren't necessary, but they do skip errors in case some appear.

## And to update

```powershell
Update-Module -Name PSPGP
```

That's it. Whenever there's a new version, you run the command, and you can enjoy it. Remember that you may need to close, reopen PowerShell session if you have already used module before updating it.

**The essential thing** is if something works for you on production, keep using it till you test the new version on a test computer. I do changes that may not be big, but big enough that auto-update may break your code. For example, a small rename to a parameter, and your code stops working! Be responsible!

## IMPORTANT

This module works correctly on Windows/Linux and MacOS, but since it uses **.NET STANDARD 2.0** library it requires minimum of **.NET Framework 4.7.2** installed on a Windows machine when using PowerShell 5.1. Please make sure to keep your .NET Framework up to date on **Windows Client/Servers**.

## Using

### Create new PGP Public/Private Keys

```powershell
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
```

### Encrypt Folder

```powershell
Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded
```

### Decrypt Folder

```powershell
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'ZielonaMila9!' -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded
```

Decrypting can also be done using multiple private keys:

```powershell
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc,$PSScriptRoot\Keys\PrivatePGP2.asc -Password 'ZielonaMila9!' -String $ProtectedString
```

### Encrypt / Decrypt String

```powershell
$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String "This is string to encrypt"
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'ZielonaMila9!' -String $ProtectedString
```

### Encrypt / Decrypt String Symmetrically

```powershell
$ProtectedString = Protect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String 'This is string to encrypt'
Unprotect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String $ProtectedString
```

### Verify signature

```powershell
$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed text'

# Verify using one or more public keys
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc,$PSScriptRoot\Keys\PublicPGP2.asc -String $Signature
```

### Clear sign and verify clear-signed content

```powershell
$ClearSigned = Protect-PGP -ClearSign -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed text'
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ClearSigned -ClearSigned
```

### Inspect message metadata

```powershell
$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed text'
Get-PGPInspect -String $Signature
```

### Sign string

```powershell
$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed text'
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $Signature
```

### Encrypt, sign, decrypt and verify

```powershell
$ProtectedString = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed and encrypted'
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -Password 'ZielonaMila9!' -String $ProtectedString -Verify
```

### Download and inspect public keys

```powershell
Get-PGPKey -KeyServer 'https://keys.openpgp.org' -Search 'user@example.com'
Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP1.asc
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'user@example.com' -Password 'secret' -UploadKeyServer 'https://keys.openpgp.org'
```

### Runnable examples

See the `Examples` folder for runnable scripts covering:

- basic encrypt/decrypt flows
- symmetric encryption/decryption
- detached signatures
- clear-sign and verify
- decrypt and verify
- message inspection
- key inspection
