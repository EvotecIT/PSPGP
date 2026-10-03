# PSPGP - PowerShell Module

<p align="center">
  <a href="https://github.com/EvotecIT/PSPGP/actions/workflows/test-dotnet.yml"><img src="https://github.com/EvotecIT/PSPGP/actions/workflows/test-dotnet.yml/badge.svg?branch=v2-speedygonzales" alt="Test .NET"></a>
  <a href="https://github.com/EvotecIT/PSPGP/actions/workflows/test-powershell.yml"><img src="https://github.com/EvotecIT/PSPGP/actions/workflows/test-powershell.yml/badge.svg?branch=v2-speedygonzales" alt="Test PowerShell"></a>
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

PSPGP uses PgpCore 8.0. New operations therefore default to AES-256 encryption, SHA-256 hashing, ZIP compression, and 3072-bit keys with a certainty of 24. Existing encrypted data remains readable; set the algorithm parameters explicitly only when an older integration requires legacy output.

CI for this branch runs in GitHub Actions via the `.github/workflows/test-dotnet.yml` and `.github/workflows/test-powershell.yml` workflows.

## Module surface

Exported cmdlets:

- `Get-PGPInspect`
- `Get-PGPKey`
- `Get-PGPKeyInfo`
- `New-PGPKey`
- `Protect-PGP`
- `Test-PGP`
- `Unprotect-PGP`

No script functions or aliases are exported.

Current capabilities:

- Encrypt and decrypt files, folders, and strings with public/private keys
- Encrypt and decrypt strings, files, and folders symmetrically with a passphrase
- Create signed, detached-signature, and clear-signed content and verify it with one or more public keys
- Write verified clear content from signed files to an output path
- Inspect signed, encrypted, and armored message metadata with `Get-PGPInspect`, including encrypted recipient key IDs when available
- Download public keys from a key server and inspect local key IDs, fingerprints, size, creation and expiration dates, and key state
- Generate new key pairs and optionally upload the public key

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

PSPGP supports Windows PowerShell 5.1 and PowerShell 7+.

On Windows PowerShell 5.1, the module requires **.NET Framework 4.7.2** or newer. On PowerShell 7+, the module works cross-platform on Windows, Linux, and macOS.

PgpCore 8 also provides typed errors for invalid key material, wrong passphrases, missing keys, unsupported input, and integrity failures. PSPGP maps these to PowerShell error categories such as `InvalidData`, `AuthenticationError`, `ObjectNotFound`, and `SecurityError`, making `try`/`catch` and `$Error` inspection more useful.

## Using

### Create new PGP Public/Private Keys

```powershell
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'

New-PGPKey -FilePathPublic $PSScriptRoot\Keys\ExpiringPublic.asc -FilePathPrivate $PSScriptRoot\Keys\ExpiringPrivate.asc -UserName 'user@example.com' -Password 'secret' -Strength 4096 -Certainty 24 -KeyExpirationInSeconds 31536000 -PreferredHashAlgorithm Sha256,Sha512 -PreferredSymmetricKeyAlgorithm Aes256
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
$Result = Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc,$PSScriptRoot\Keys\PublicPGP2.asc -String $Signature
$Result.ClearText
```

### Detached signatures

```powershell
$Signature = Protect-PGP -SignOnly -Detached -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Detached signed text'
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String 'Detached signed text' -Signature $Signature

Protect-PGP -SignOnly -Detached -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Test\Test1.txt.sig
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $PSScriptRoot\Test\Test1.txt -SignaturePath $PSScriptRoot\Test\Test1.txt.sig
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

`Get-PGPInspect` returns metadata for signed content, armored messages, and encrypted messages that can be inspected without decrypting the payload. For encrypted content it also reports recipient key IDs when the packet exposes them.

### Sign string

```powershell
$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed text'
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $Signature
```

### Decrypt and verify signed-and-encrypted content

```powershell
$Protected = Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'ZielonaMila9!' -String 'Signed and encrypted text'
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -Password 'ZielonaMila9!' -String $Protected -Verify
```

File encryption, decryption, signing, and verified extraction use PgpCore staging. Existing destinations are replaced after the operation succeeds. Decryption checks message integrity when the message carries an integrity packet; `-Verify` also requires a valid signature. Invalid output is discarded.

### Download and inspect public keys

```powershell
Get-PGPKey -KeyServer 'https://keys.openpgp.org' -Search 'user@example.com'
$KeyInfo = Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP1.asc
$KeyInfo | Select-Object KeyId, Fingerprint, Algorithm, BitStrength, CreationTime, Expiration, IsEncryptionKey, IsRevoked
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'user@example.com' -Password 'secret' -UploadKeyServer 'https://keys.openpgp.org'
```

### Runnable examples

See the `Examples` folder for runnable scripts:

- `Examples\Example-GeneratePGP.ps1`
- `Examples\Example-EncryptString.ps1`
- `Examples\Example-EncryptStringMultipleKeys.ps1`
- `Examples\Example-EncryptStringNoPassword.ps1`
- `Examples\Example-DecryptString.ps1`
- `Examples\Example-EncryptFolder.ps1`
- `Examples\Example-EncryptFolderMultipleKeys.ps1`
- `Examples\Example-DecryptFolder.ps1`
- `Examples\Example-DecryptFile.ps1`
- `Examples\Example-SignString.ps1`
- `Examples\Example-VerifySignature.ps1`
- `Examples\Example-DetachedSignature.ps1`
- `Examples\Example-ClearSignString.ps1`
- `Examples\Example-SymmetricString.ps1`
- `Examples\Example-InspectSignedString.ps1`
- `Examples\Example-GetPGPKeyInfo.ps1`

## Safe workflows and compatibility

Folder operations preserve relative directories. An output subtree inside the input is excluded from subsequent runs. Output-name collisions are rejected before writing; symbolic links and reparse points are skipped. Decryption removes only a terminal `.pgp`, `.gpg`, or `.asc` suffix. Other filenames receive `.decrypted`.

File parameters accept `FileInfo` pipeline records. `-PassThru` returns successful output files, and file-writing commands honor `-WhatIf` and `-Confirm`.

```powershell
$encrypted = Get-Item './report.txt' |
    Protect-PGP -FilePathPublic './recipient.asc' -PassThru
$encrypted | Unprotect-PGP -FilePathPrivate './recipient-private.asc' -PassThru
```

Omit `-Password` or `-SignPassword` for an unprotected private key. `New-PGPKey` defaults to 3072 bits and requires `-Force` to replace existing key files. `-Strength` and `-Certainty` can be set independently. `Protect-PGP -PublicKeyAlgorithm` is rejected because an existing key determines its algorithm; choose the algorithm with `New-PGPKey`. String output is armored text. Use file input for binary output.

`Test-PGP` returns a result object. Test its `Status` property: PowerShell treats an object as true even when its status is false.

```powershell
$result = Test-PGP -FilePathPublic './signer.asc' -FilePath './report.txt.sig'
if (-not $result.Status) { throw $result.Error }
```

Encrypted input requires `Unprotect-PGP -Verify`; `Test-PGP` verifies signed or clear-signed input and detached signatures. Verification authenticates the signature against the supplied certificate. Obtain the signer's complete fingerprint through a trusted channel. Verification can use expired or revoked certificates to check older signatures; it does not establish whether a signature was made before expiration or revocation.

`Get-PGPKeyInfo` reports every primary certificate, its authenticated user IDs, and a `Subkeys` array. `-IncludeSubkeys` also emits subkeys as separate records. Use `CanSign` and `CanEncrypt` for authenticated capabilities, and `IsUsableForSigning` / `IsUsableForEncryption` for current use. `IsEncryptionKey` describes the underlying algorithm.

Key servers require HTTPS. Downloads reject redirects, malformed certificates, secret-key packets, and responses larger than 8 MiB. Requests time out after 60 seconds and can be stopped with the PowerShell pipeline. `-ExpectedFingerprint` requires a single matching certificate:

```powershell
# Set this from an independently trusted channel.
Get-PGPKey -KeyServer 'https://keys.openpgp.org' -Search $expectedFingerprint `
    -ExpectedFingerprint $expectedFingerprint -OutFilePath './recipient.asc'
```

Errors use PowerShell error records and honor `-ErrorAction`, `-ErrorVariable`, and `$ErrorActionPreference`.

## Building and testing

The build uses PSPublishModule 3.0.153 or later. Its default mode builds artifacts without installing or publishing them. Signing defaults to enabled; use an explicitly unsigned build for local validation:

```powershell
./Build/Build-Module.ps1 -ConfigurationGateMode Build -SignModule $false
./PSPGP.Tests.ps1 -ModulePath './Artefacts/Unpacked/PSPGP/PSPGP.psd1'
```

Tests use Pester 5.7.1 and exercise the packaged module on Windows PowerShell 5.1 and PowerShell 7. CI also runs that same artifact on Linux and macOS. The package contains .NET Framework 4.7.2 and .NET Standard 2.0 binaries. PowerShell Core uses the shared loader's assembly isolation to avoid conflicts with other modules' PGP dependencies.

The source bootstrapper loads development binaries only when `PSPGP_DEVELOPMENT=true`. Set `PSPGP_DEVELOPMENT_CONFIGURATION` to choose `Debug` or `Release`. Command help is generated from C# XML documentation; regenerate it through the build's `Documentation` mode.
