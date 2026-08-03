---
Module Name: PSPGP
Module Guid: edbf6d52-2d66-405e-a4d4-d4a95db8fb45
Download Help Link: https://github.com/EvotecIT/PSPGP
Help Version: 1.0.2
Locale: en-US
---
# PSPGP Module
## Description
PSPGP is a PowerShell module that provides PGP functionality in PowerShell. It allows encrypting and decrypting files/folders and strings using PGP.

## PSPGP Cmdlets
### [Get-PGPInspect](Get-PGPInspect.md)
Inspects PGP content and returns message metadata.

### [Get-PGPKey](Get-PGPKey.md)
Downloads a public key from a key server.

### [Get-PGPKeyInfo](Get-PGPKeyInfo.md)
Returns information about a PGP key such as algorithm, expiration and user IDs.

### [New-PGPKey](New-PGPKey.md)
Generates a new PGP key pair.

### [Protect-PGP](Protect-PGP.md)
Encrypts or signs files, folders or strings using one or more public keys.
Use -SignOnly or the Sign* parameter sets to create signatures
without encryption. Add -Detached to create a separate detached signature.

### [Test-PGP](Test-PGP.md)
Verifies PGP signatures for files, folders or strings.

### [Unprotect-PGP](Unprotect-PGP.md)
Removes PGP encryption from files or strings using a private key or symmetric passphrase.
