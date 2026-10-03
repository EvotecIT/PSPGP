param(
    [Alias('RunMode')]
    [ValidateSet('Manifest', 'Documentation', 'Build', 'Publish')]
    [string] $ConfigurationGateMode = 'Build',
    [bool] $SignModule = $true
)

Import-Module PSPublishModule -MinimumVersion 3.0.153 -Force -ErrorAction Stop

Build-Module -ModuleName 'PSPGP' -SkipInstall -ErrorAction Stop {
    New-ConfigurationGate -Mode $ConfigurationGateMode
    New-ConfigurationManifest -PowerShellVersion '5.1' -CompatiblePSEditions 'Desktop', 'Core' `
        -GUID 'edbf6d52-2d66-405e-a4d4-d4a95db8fb45' -ModuleVersion '1.0.X' `
        -Author 'Przemyslaw Klys' -CompanyName 'Evotec' `
        -Copyright "(c) 2011 - $((Get-Date).Year) Przemyslaw Klys @ Evotec. All rights reserved." `
        -Description 'PGP encryption, decryption, signing, verification, and key management for PowerShell.' `
        -Tags 'pgp', 'gpg', 'encrypt', 'decrypt', 'windows', 'macos', 'linux' `
        -ProjectUri 'https://github.com/EvotecIT/PSPGP' `
        -LicenseUri 'https://github.com/EvotecIT/PSPGP/blob/v2-speedygonzales/License' `
        -IconUri 'https://evotec.xyz/wp-content/uploads/2021/08/PSPGP.png' -DotNetFrameworkVersion '4.7.2'

    New-ConfigurationFormat -ApplyTo 'DefaultPSD1', 'OnMergePSD1' -PSD1Style 'Minimal'
    New-ConfigurationDocumentation -Enable -PathReadme 'Docs/Readme.md' -Path 'Docs' -SyncExternalHelpToProjectRoot
    New-ConfigurationBuild -Enable -SignModule:$SignModule `
        -CertificateThumbprint '92e95fb58effa6a4a75e77a33cdd6bfe6dd30f1a' `
        -NETProjectPath "$PSScriptRoot/../Sources/PSPGP" -NETProjectName 'PSPGP' `
        -NETBinaryModule 'PSPGP.dll' -NETConfiguration 'Release' -NETFramework 'netstandard2.0', 'net472' `
        -NETBinaryModuleDocumentation -NETHandleAssemblyWithSameName -NETAssemblyLoadContext `
        -NETDevelopmentBinaries -NETDevelopmentBinariesMode Environment `
        -NETDevelopmentBinariesEnvironmentVariable 'PSPGP_DEVELOPMENT' `
        -NETDevelopmentBinariesPath 'Sources/PSPGP/bin' -NETDevelopmentSourceBootstrapperMode ReplaceSingleFile `
        -DeleteTargetModuleBeforeBuild

    New-ConfigurationArtefact -Type Unpacked -Enable -Path "$PSScriptRoot/../Artefacts/Unpacked" -CopyFilesRelative
    New-ConfigurationArtefact -Type Packed -Enable -Path "$PSScriptRoot/../Artefacts/Packed" `
        -ArtefactName '<ModuleName>.v<ModuleVersion>.zip'
} -ExitCode
