---
title: "Encrypt and decrypt a string"
description: "Use PSPGP to protect a string and decrypt it with the matching private key."
layout: docs
meta.project_base_slug: "pspgp"
meta.project_name: "PSPGP"
meta.project_section: "examples"
meta.project_hub_path: "/projects/pspgp/"
meta.project_link_examples: "/projects/pspgp/examples/"
---

This pattern is useful when a script has to pass a protected payload between two controlled steps.

It is adapted from the source examples at `Examples/Example-EncryptString.ps1` and `Examples/Example-DecryptString.ps1`.

## When to use this pattern

- You need to encrypt text with a public key.
- The matching private key is available only to the decrypting side.
- You want a small smoke test before encrypting files or folders.

## Example

```powershell
Import-Module PSPGP

$publicKey = "$PSScriptRoot\Keys\PublicPGP.asc"
$privateKey = "$PSScriptRoot\Keys\PrivatePGP.asc"

$protectedString = Protect-PGP -FilePathPublic $publicKey -String 'This is a string to encrypt'

Unprotect-PGP `
    -FilePathPrivate $privateKey `
    -Password '<private-key-password>' `
    -String $protectedString
```

## What this demonstrates

- encrypting with the public key
- decrypting with the private key
- keeping key paths explicit in the workflow

## Source

- [Example-EncryptString.ps1](https://github.com/EvotecIT/PSPGP/blob/v2-speedygonzales/Examples/Example-EncryptString.ps1)
- [Example-DecryptString.ps1](https://github.com/EvotecIT/PSPGP/blob/v2-speedygonzales/Examples/Example-DecryptString.ps1)
