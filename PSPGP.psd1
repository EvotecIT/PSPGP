@{
    AliasesToExport        = @()
    Author                 = 'Przemyslaw Klys'
    CmdletsToExport        = @()
    CompanyName            = 'Evotec'
    CompatiblePSEditions   = @('Desktop', 'Core')
    Copyright              = '(c) 2011 - 2023 Przemyslaw Klys @ Evotec. All rights reserved.'
    Description            = 'PSPGP is a PowerShell module that provides PGP functionality in PowerShell. It allows encrypting and decrypting files/folders and strings using PGP.'
    DotNetFrameworkVersion = '4.7.2'
    FunctionsToExport      = @('New-PGPKey', 'Protect-PGP', 'Test-PGP', 'Unprotect-PGP')
    GUID                   = 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45'
    ModuleVersion          = '0.1.11'
    PowerShellVersion      = '5.1'
    PrivateData            = @{
        PSData = @{
            ExternalModuleDependencies = @('Microsoft.PowerShell.Management', 'Microsoft.PowerShell.Utility')
            IconUri                    = 'https://evotec.xyz/wp-content/uploads/2021/08/PSPGP.png'
            LicenseUri                 = 'https://github.com/EvotecIT/PSPGP/blob/master/License'
            ProjectUri                 = 'https://github.com/EvotecIT/PSPGP'
            Tags                       = @('pgp', 'gpg', 'encrypt', 'decrypt', 'windows', 'macos', 'linux')
        }
    }
    RequiredModules        = @('Microsoft.PowerShell.Management', 'Microsoft.PowerShell.Utility')
    RootModule             = 'PSPGP.psm1'
}