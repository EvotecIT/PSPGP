@{
    AliasesToExport      = @()
    Author               = 'Przemyslaw Klys'
    CmdletsToExport      = @()
    CompanyName          = 'Evotec'
    CompatiblePSEditions = @('Desktop', 'Core')
    Copyright            = '(c) 2011 - 2021 Przemyslaw Klys @ Evotec. All rights reserved.'
    Description          = 'Simple project to create Microsoft Word in PowerShell without having Office installed.'
    FunctionsToExport    = @('New-PGPKey', 'Protect-PGP', 'Test-PGP', 'Unprotect-PGP')
    GUID                 = 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45'
    ModuleVersion        = '0.1.1'
    PowerShellVersion    = '5.1'
    PrivateData          = @{
        PSData = @{
            Tags       = @('word', 'docx', 'write', 'PSWord', 'office', 'windows', 'doc')
            ProjectUri = 'https://github.com/EvotecIT/PSPGP'
        }
    }
    RequiredModules      = @(@{
            ModuleVersion = '0.0.200'
            ModuleName    = 'PSSharedGoods'
            Guid          = 'ee272aa8-baaa-4edf-9f45-b6d6f7d844fe'
        })
    RootModule           = 'PSPGP.psm1'
}