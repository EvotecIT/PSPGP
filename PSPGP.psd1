@{
    AliasesToExport      = @()
    Author               = 'Przemyslaw Klys'
    CmdletsToExport      = @()
    CompanyName          = 'Evotec'
    CompatiblePSEditions = @('Desktop', 'Core')
    Copyright            = '(c) 2011 - 2021 Przemyslaw Klys @ Evotec. All rights reserved.'
    Description          = 'PSPGP is a PowerShell module that provides PGP functionality in PowerShell. It allows encrypting and decrypting files/folders and strings using PGP.'
    FunctionsToExport    = @('New-PGPKey', 'Protect-PGP', 'Test-PGP', 'Unprotect-PGP')
    GUID                 = 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45'
    ModuleVersion        = '0.1.6'
    PowerShellVersion    = '5.1'
    PrivateData          = @{
        PSData = @{
            Tags       = @('pgp', 'gpg', 'encrypt', 'decrypt', 'windows', 'macos', 'linux')
            ProjectUri = 'https://github.com/EvotecIT/PSPGP'
            IconUri    = 'https://evotec.xyz/wp-content/uploads/2021/08/PSPGP.png'
        }
    }
    RootModule           = 'PSPGP.psm1'
}