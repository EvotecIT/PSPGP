Describe 'PGP Tests' {
    # prepare things
    $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
    $KeyPublic = [io.path]::Combine($KeysDirectory, 'PublicPGP.asc')
    $KeyPrivate = [io.path]::Combine($KeysDirectory, 'PrivatePGP.asc')

    $KeyPublic1 = [io.path]::Combine($KeysDirectory, 'PublicPGP1.asc')
    $KeyPrivate1 = [io.path]::Combine($KeysDirectory, 'PrivatePGP1.asc')
    [string] $Script:ProtectedString = ''
    [string] $Script:ProtectedString = ''

    BeforeAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        New-Item -Path $KeysDirectory -Force -ItemType Directory
    }
    It ' Running New-PGPKey with Username and password should create public and private keys' -TestCases @{ KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate; KeyPublic1 = $KeyPublic1; KeyPrivate1 = $KeyPrivate1 } {
        New-PGPKey -FilePathPublic $KeyPublic -FilePathPrivate $KeyPrivate -UserName 'przemyslaw.klys' -Password 'ZielonaMila9!'
        Test-Path -LiteralPath $KeyPublic | Should -Be $true
        Test-Path -LiteralPath $KeyPrivate | Should -Be $true

        New-PGPKey -FilePathPublic $KeyPublic1 -FilePathPrivate $KeyPrivate1 -UserName 'przemyslaw.klys1' -Password 'ZielonaMila9!1'
        Test-Path -LiteralPath $KeyPublic1 | Should -Be $true
        Test-Path -LiteralPath $KeyPrivate1 | Should -Be $true
    }
    It ' Test script encryption' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        $Script:ProtectedString = Protect-PGP -FilePathPublic $KeyPublic -String "This is string to encrypt" -ErrorAction Stop
    }

    It ' Test script encryption (multiple keys)' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPublic1 = $KeyPublic1 } {
        $Script:ProtectedStringMultiple = Protect-PGP -FilePathPublic $KeyPublic, $KeyPublic1 -String "This is string to encrypt with multiple keys" -ErrorAction Stop
    }

    It ' Decrypt string' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPublic = $KeyPublic; KeyPrivate = $KeyPrivate } {
        $String = Unprotect-PGP -FilePathPrivate $KeyPrivate -Password 'ZielonaMila9!' -String $Script:ProtectedString
        $String | Should -Be "This is string to encrypt"
    }

    It ' Decrypt string (multiple keys)' -TestCases @{ ProtectedString = $ProtectedString; KeysDirectory = $KeysDirectory; KeyPrivate = $KeyPrivate; KeyPrivate1 = $KeyPrivate1 } {
        $String1 = Unprotect-PGP -FilePathPrivate $KeyPrivate1 -Password 'ZielonaMila9!1' -String $Script:ProtectedStringMultiple
        $String1 | Should -Be "This is string to encrypt with multiple keys"

        $String = Unprotect-PGP -FilePathPrivate $KeyPrivate -Password 'ZielonaMila9!' -String $Script:ProtectedStringMultiple
        $String | Should -Be "This is string to encrypt with multiple keys"


    }
    # clean everything
    AfterAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        Remove-Item -Path $KeysDirectory -Recurse -Force
    }
}