Clear-Host
Import-Module 'C:\Users\przemyslaw.klys\OneDrive - Evotec\Support\GitHub\PSPublishModule\PSPublishModule.psd1' -Force

$Configuration = @{
    Information = @{
        ModuleName           = 'PSPGP'

        DirectoryProjects    = 'C:\Support\GitHub'
        DirectoryModulesCore = "$Env:USERPROFILE\Documents\PowerShell\Modules"
        DirectoryModules     = "$Env:USERPROFILE\Documents\WindowsPowerShell\Modules"

        FunctionsToExport    = 'Public'
        AliasesToExport      = 'Public'

        LibrariesCore        = 'Lib\Standard'
        LibrariesDefault     = 'Lib\Standard'

        Manifest             = @{
            # Minimum version of the Windows PowerShell engine required by this module
            PowerShellVersion    = '5.1'
            # prevent using over CORE/PS 7
            CompatiblePSEditions = @('Desktop', 'Core')
            # ID used to uniquely identify this module
            GUID                 = 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45'
            # Version number of this module.
            ModuleVersion        = '0.1.X'
            # Author of this module
            Author               = 'Przemyslaw Klys'
            # Company or vendor of this module
            CompanyName          = 'Evotec'
            # Copyright statement for this module
            Copyright            = "(c) 2011 - $((Get-Date).Year) Przemyslaw Klys @ Evotec. All rights reserved."
            # Description of the functionality provided by this module
            Description          = 'PSPGP is a PowerShell module that provides PGP functionality in PowerShell. It allows encrypting and decrypting files/folders and strings using PGP.'
            # Tags applied to this module. These help with module discovery in online galleries.
            Tags                 = @('pgp', 'gpg', 'encrypt', 'decrypt', 'windows', 'macos', 'linux')
            # A URL to the main website for this project.
            ProjectUri           = 'https://github.com/EvotecIT/PSPGP'

            IconUri              = 'https://evotec.xyz/wp-content/uploads/2021/08/PSPGP.png'

            #LicenseUri           = 'https://github.com/EvotecIT/PSWriteWord/blob/master/License'

            RequiredModules      = @(
                #@{ ModuleName = 'PSSharedGoods'; ModuleVersion = "Latest"; Guid = 'ee272aa8-baaa-4edf-9f45-b6d6f7d844fe' }
            )
        }
    }
    Options     = @{
        Merge             = @{
            Sort           = 'None'
            FormatCodePSM1 = @{
                Enabled           = $true
                RemoveComments    = $true
                FormatterSettings = @{
                    IncludeRules = @(
                        'PSPlaceOpenBrace',
                        'PSPlaceCloseBrace',
                        'PSUseConsistentWhitespace',
                        'PSUseConsistentIndentation',
                        'PSAlignAssignmentStatement',
                        'PSUseCorrectCasing'
                    )

                    Rules        = @{
                        PSPlaceOpenBrace           = @{
                            Enable             = $true
                            OnSameLine         = $true
                            NewLineAfter       = $true
                            IgnoreOneLineBlock = $true
                        }

                        PSPlaceCloseBrace          = @{
                            Enable             = $true
                            NewLineAfter       = $false
                            IgnoreOneLineBlock = $true
                            NoEmptyLineBefore  = $false
                        }

                        PSUseConsistentIndentation = @{
                            Enable              = $true
                            Kind                = 'space'
                            PipelineIndentation = 'IncreaseIndentationAfterEveryPipeline'
                            IndentationSize     = 4
                        }

                        PSUseConsistentWhitespace  = @{
                            Enable          = $true
                            CheckInnerBrace = $true
                            CheckOpenBrace  = $true
                            CheckOpenParen  = $true
                            CheckOperator   = $true
                            CheckPipe       = $true
                            CheckSeparator  = $true
                        }

                        PSAlignAssignmentStatement = @{
                            Enable         = $true
                            CheckHashtable = $true
                        }

                        PSUseCorrectCasing         = @{
                            Enable = $true
                        }
                    }
                }
            }
            FormatCodePSD1 = @{
                Enabled        = $true
                RemoveComments = $false
            }
            Integrate      = @{
                ApprovedModules = @('PSSharedGoods', 'PSWriteColor', 'Connectimo', 'PSUnifi', 'PSWebToolbox', 'PSMyPassword')
            }
        }
        Standard          = @{
            FormatCodePSM1 = @{

            }
            FormatCodePSD1 = @{
                Enabled = $true
                #RemoveComments = $true
            }
        }
        PowerShellGallery = @{
            ApiKey   = 'C:\Support\Important\PowerShellGalleryAPI.txt'
            FromFile = $true
        }
        GitHub            = @{
            ApiKey   = 'C:\Support\Important\GithubAPI.txt'
            FromFile = $true
            UserName = 'EvotecIT'
            #RepositoryName = 'PSWriteHTML'
        }
        Documentation     = @{
            Path       = 'Docs'
            PathReadme = 'Docs\Readme.md'
        }
    }
    Steps       = @{
        BuildModule        = @{  # requires Enable to be on to process all of that
            Enable              = $true
            DeleteBefore        = $true
            Merge               = $true
            MergeMissing        = $true
            LibrarySeparateFile = $true
            LibraryDotSource    = $false
            ClassesDotSource    = $false
            SignMerged          = $true
            CreateFileCatalog   = $false # not working
            Releases            = $true
            ReleasesUnpacked    = $false
            RefreshPSD1Only     = $false
        }
        BuildDocumentation = $false
        ImportModules      = @{
            Self            = $true
            RequiredModules = $false
            Verbose         = $false
        }
        PublishModule      = @{  # requires Enable to be on to process all of that
            Enabled      = $true
            Prerelease   = ''
            RequireForce = $false
            GitHub       = $true
        }
    }
}

New-PrepareModule -Configuration $Configuration