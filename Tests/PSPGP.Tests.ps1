Describe 'PGP Tests' {
    # prepare things
    $KeysDirectory = "$env:TEMP\Keys"
    New-Item -Path $KeysDirectory -Force

    It 'Running New-PGPKey with Username and password should create public and private keys' {
        New-PGPKey -FilePathPublic $KeysDirectory\PublicPGP.asc -FilePathPrivate $KeysDirectory\PrivatePGP.asc -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
        Test-Path -LiteralPath $KeysDirectory\PublicPGP.asc | Should -Be $true
        Test-Path -LiteralPath $KeysDirectory\PrivatePGP.asc | Should -Be $true
    }

    # clean everything
    Remove-Item -Path $KeysDirectory -Recurse -Force
}