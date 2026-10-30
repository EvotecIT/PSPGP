@{
    AliasesToExport        = @()
    Author                 = 'Przemyslaw Klys'
    CmdletsToExport        = @('Get-PGPInspect', 'Get-PGPKey', 'Get-PGPKeyInfo', 'New-PGPKey', 'Protect-PGP', 'Test-PGP', 'Unprotect-PGP')
    CompanyName            = 'Evotec'
    CompatiblePSEditions   = @('Desktop', 'Core')
    Copyright              = '(c) 2011 - 2026 Przemyslaw Klys @ Evotec. All rights reserved.'
    Description            = 'PGP encryption, decryption, signing, verification, and key management for PowerShell.'
    DotNetFrameworkVersion = '4.7.2'
    FunctionsToExport      = @()
    GUID                   = 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45'
    ModuleVersion          = '1.0.2'
    PowerShellVersion      = '5.1'
    PrivateData            = @{
        PSData = @{
            ExternalModuleDependencies = @()
            IconUri                    = 'https://evotec.xyz/wp-content/uploads/2021/08/PSPGP.png'
            LicenseUri                 = 'https://github.com/EvotecIT/PSPGP/blob/v2-speedygonzales/License'
            ProjectUri                 = 'https://github.com/EvotecIT/PSPGP'
            RequireLicenseAcceptance   = $false
            Tags                       = @('pgp', 'gpg', 'encrypt', 'decrypt', 'windows', 'macos', 'linux')
        }
    }
    RequiredModules        = @()
    RootModule             = 'PSPGP.psm1'
}
