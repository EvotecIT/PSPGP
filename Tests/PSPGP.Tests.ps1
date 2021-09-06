Describe 'PGP Tests' {
    # prepare things
    $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
    $KeyPublic = [io.path]::Combine($KeysDirectory, 'PublicPGP.asc')
    $KeyPrivate = [io.path]::Combine($KeysDirectory, 'PrivatePGP.asc')
    [string] $Script:ProtectedString = ''

    BeforeAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        New-Item -Path $KeysDirectory -Force -ItemType Directory
    }
    It ' Running New-PGPKey with Username and password should create public and private keys' -TestCases @{ KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        New-PGPKey -FilePathPublic $KeyPublic -FilePathPrivate $KeyPrivate -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
        Test-Path -LiteralPath $KeyPublic | Should -Be $true
        Test-Path -LiteralPath $KeyPrivate | Should -Be $true
    }
    It ' Test script encryption' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        $Script:ProtectedString = Protect-PGP -FilePathPublic $KeyPublic -String "This is string to encrypt" -ErrorAction Stop
    }
    It ' Decrypt string' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        $String = Unprotect-PGP -FilePathPrivate $KeyPrivate -Password 'ZielonaMila9!' -String $Script:ProtectedString
        $String | Should -Be "This is string to encrypt"
    }
    # clean everything
    AfterAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        Remove-Item -Path $KeysDirectory -Recurse -Force
    }
}