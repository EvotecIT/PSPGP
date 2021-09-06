Describe 'PGP Tests' {
    # prepare things
    $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
    $KeyPublic = [io.path]::Combine($KeysDirectory, 'PublicPGP.asc')
    $KeyPrivate = [io.path]::Combine($KeysDirectory, 'PrivatePGP.asc')

    BeforeAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        New-Item -Path $KeysDirectory -Force -ItemType Directory
    }
    It 'Running New-PGPKey with Username and password should create public and private keys' -TestCases @{ KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        New-PGPKey -FilePathPublic $KeyPublic -FilePathPrivate $KeyPrivate -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
        Test-Path -LiteralPath $KeyPublic | Should -Be $true
        Test-Path -LiteralPath $KeyPrivate | Should -Be $true
    }

    # clean everything
    AfterAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        Remove-Item -Path $KeysDirectory -Recurse -Force
    }
}