---
title: "Generate a PGP key pair"
description: "Use PSPGP to create public and private PGP key files."
layout: docs
meta.project_base_slug: "pspgp"
meta.project_name: "PSPGP"
meta.project_section: "examples"
meta.project_hub_path: "/projects/pspgp/"
meta.project_link_examples: "/projects/pspgp/examples/"
---

This pattern is useful when a workflow needs dedicated PGP keys for encrypted file exchange.

It is adapted from the source example at `Examples/Example-GeneratePGP.ps1`.

## When to use this pattern

- You need a project-specific PGP key pair.
- The public key will be shared with another party.
- The private key and password will be stored securely outside the script.

## Example

```powershell
Import-Module PSPGP

$keyFolder = Join-Path $PSScriptRoot 'Keys'
New-Item -ItemType Directory -Force -Path $keyFolder | Out-Null

New-PGPKey `
    -HashAlgorithm Sha512 `
    -FilePathPublic "$keyFolder\PublicPGP.asc" `
    -FilePathPrivate "$keyFolder\PrivatePGP.asc" `
    -UserName 'automation@example.com' `
    -Password '<store-this-securely>'
```

## What this demonstrates

- creating a public/private key pair
- choosing a hash algorithm
- keeping file paths explicit for later automation steps

## Source

- [Example-GeneratePGP.ps1](https://github.com/EvotecIT/PSPGP/blob/v2-speedygonzales/Examples/Example-GeneratePGP.ps1)
