param(
    [string] $ModulePath = "$PSScriptRoot/PSPGP.psd1"
)

$ErrorActionPreference = 'Stop'
Import-Module Pester -RequiredVersion 5.7.1 -Force
Import-Module $ModulePath -Force
$config = New-PesterConfiguration
$config.Run.Path = "$PSScriptRoot/Tests"
$config.Run.PassThru = $true
$config.Output.Verbosity = 'Detailed'
$result = Invoke-Pester -Configuration $config
if ($result.FailedCount -gt 0 -or $result.PassedCount -eq 0) {
    throw "PGP tests failed: $($result.FailedCount) failed, $($result.PassedCount) passed."
}
