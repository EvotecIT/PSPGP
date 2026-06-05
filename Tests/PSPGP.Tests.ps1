Describe 'PGP Tests' {
    # prepare things
    $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
    $KeyPublic = [io.path]::Combine($KeysDirectory, 'PublicPGP.asc')
    $KeyPrivate = [io.path]::Combine($KeysDirectory, 'PrivatePGP.asc')

    $KeyPublic1 = [io.path]::Combine($KeysDirectory, 'PublicPGP1.asc')
    $KeyPrivate1 = [io.path]::Combine($KeysDirectory, 'PrivatePGP1.asc')
    $KeyPublicBom = [io.path]::Combine($KeysDirectory, 'PublicPGP-Bom.asc')
    $KeyPrivateBom = [io.path]::Combine($KeysDirectory, 'PrivatePGP-Bom.asc')
    [string] $Script:ProtectedString = ''

    BeforeAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        New-Item -Path $KeysDirectory -Force -ItemType Directory
        # Ensure the module is loaded in test context
        if (-not (Get-Module PSPGP)) {
            Import-Module $PSScriptRoot\..\PSPGP.psd1 -Force
        }
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

    It ' Decrypt string using multiple keys at once' -TestCases @{ ProtectedStringMultiple = $ProtectedStringMultiple; KeyPrivate = $KeyPrivate; KeyPrivate1 = $KeyPrivate1 } {
        $String = Unprotect-PGP -FilePathPrivate $KeyPrivate, $KeyPrivate1 -Password 'ZielonaMila9!' -String $Script:ProtectedStringMultiple
        $String | Should -Be "This is string to encrypt with multiple keys"
    }

    It ' Sign and verify string' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic } {
        $signed = Protect-PGP -SignOnly -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Signed Text'
        $result = Test-PGP -FilePathPublic $KeyPublic -String $signed
        $result.Status | Should -Be $true
    }

    It ' Clear sign and verify string' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic } {
        $clearSigned = Protect-PGP -ClearSign -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Clear Signed Text'
        $result = Test-PGP -FilePathPublic $KeyPublic -String $clearSigned -ClearSigned
        $result.Status | Should -Be $true
    }

    It ' Clear sign verification fails with the wrong public key' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic1 } {
        $clearSigned = Protect-PGP -ClearSign -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Clear Signed Text'
        $result = Test-PGP -FilePathPublic $KeyPublic -String $clearSigned -ClearSigned
        $result.Status | Should -Be $false
        $result.Signer | Should -BeNullOrEmpty
    }

    It ' Clear sign and verify file' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeysDirectory = $KeysDirectory } {
        $sourceFile = [io.path]::Combine($KeysDirectory, 'clear-sign-input.txt')
        $signedFile = [io.path]::Combine($KeysDirectory, 'clear-sign-input.txt.asc')
        Set-Content -Path $sourceFile -Value 'Clear signed file content' -NoNewline
        Protect-PGP -ClearSign -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -FilePath $sourceFile -OutFilePath $signedFile -ErrorAction Stop
        $result = Test-PGP -FilePathPublic $KeyPublic -FilePath $signedFile -ClearSigned
        $result.Status | Should -Be $true
    }

    It ' Encrypt and decrypt string symmetrically' {
        $protected = Protect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String 'Symmetric Text'
        $plain = Unprotect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String $protected
        $plain | Should -Be 'Symmetric Text'
    }

    It ' Encrypt, sign, decrypt and verify string' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic } {
        $protected = Protect-PGP -FilePathPublic $KeyPublic -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Decrypt and verify text'
        $plain = Unprotect-PGP -FilePathPrivate $KeyPrivate -FilePathPublic $KeyPublic -Password 'ZielonaMila9!' -String $protected -Verify -ErrorAction Stop
        $plain | Should -Be 'Decrypt and verify text'
    }

    It ' Encrypt, sign, decrypt and verify string rejects the wrong public key' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeyPublic1 = $KeyPublic1 } {
        $protected = Protect-PGP -FilePathPublic $KeyPublic -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Decrypt and verify text'
        { Unprotect-PGP -FilePathPrivate $KeyPrivate -FilePathPublic $KeyPublic1 -Password 'ZielonaMila9!' -String $protected -Verify -ErrorAction Stop } | Should -Throw -ExpectedMessage '*Failed to verify file*'
    }

    It ' Decrypts normally when Verify is explicitly false' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeyPublic1 = $KeyPublic1 } {
        $protected = Protect-PGP -FilePathPublic $KeyPublic -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Explicit false verify text'
        $plain = Unprotect-PGP -FilePathPrivate $KeyPrivate -FilePathPublic $KeyPublic1 -Password 'ZielonaMila9!' -String $protected -Verify:$false -ErrorAction Stop
        $plain | Should -Be 'Explicit false verify text'
    }

    It ' Encrypt, sign, decrypt and verify file' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeysDirectory = $KeysDirectory } {
        $sourceFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-input.txt')
        $protectedFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-input.txt.pgp')
        $outputFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-output.txt')
        Set-Content -Path $sourceFile -Value 'Decrypt and verify file content' -NoNewline
        Protect-PGP -FilePathPublic $KeyPublic -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -FilePath $sourceFile -OutFilePath $protectedFile -ErrorAction Stop
        Unprotect-PGP -FilePathPrivate $KeyPrivate -FilePathPublic $KeyPublic -Password 'ZielonaMila9!' -FilePath $protectedFile -OutFilePath $outputFile -Verify -ErrorAction Stop
        Get-Content -LiteralPath $outputFile -Raw | Should -Be 'Decrypt and verify file content'
    }

    It ' Encrypt, sign, decrypt and verify file rejects the wrong public key without writing output' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeyPublic1 = $KeyPublic1; KeysDirectory = $KeysDirectory } {
        $sourceFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-wrong-key-input.txt')
        $protectedFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-wrong-key-input.txt.pgp')
        $outputFile = [io.path]::Combine($KeysDirectory, 'decrypt-verify-wrong-key-output.txt')
        Set-Content -Path $sourceFile -Value 'Decrypt and verify wrong key file content' -NoNewline
        Protect-PGP -FilePathPublic $KeyPublic -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -FilePath $sourceFile -OutFilePath $protectedFile -ErrorAction Stop
        { Unprotect-PGP -FilePathPrivate $KeyPrivate -FilePathPublic $KeyPublic1 -Password 'ZielonaMila9!' -FilePath $protectedFile -OutFilePath $outputFile -Verify -ErrorAction Stop } | Should -Throw -ExpectedMessage '*Failed to verify file*'
        Test-Path -LiteralPath $outputFile | Should -Be $false
    }

    It ' Inspect signed string' -TestCases @{ KeyPrivate = $KeyPrivate } {
        $signed = Protect-PGP -SignOnly -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -String 'Inspect me'
        $inspection = Get-PGPInspect -String $signed
        $inspection.IsSigned | Should -Be $true
        $inspection.IsArmored | Should -Be $true
    }

    It ' Inspect folder continues when one file cannot be inspected' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPublic = $KeyPublic; KeysDirectory = $KeysDirectory } {
        $inspectFolder = [io.path]::Combine($KeysDirectory, 'inspect-folder')
        $sourceFile = [io.path]::Combine($KeysDirectory, 'inspect-input.txt')
        $signedFile = [io.path]::Combine($inspectFolder, 'signed.asc')
        $encryptedFile = [io.path]::Combine($inspectFolder, 'encrypted.pgp')
        New-Item -ItemType Directory -Path $inspectFolder -Force | Out-Null
        Set-Content -Path $sourceFile -Value 'Inspect folder content' -NoNewline
        Protect-PGP -SignOnly -SignKey $KeyPrivate -SignPassword 'ZielonaMila9!' -FilePath $sourceFile -OutFilePath $signedFile -ErrorAction Stop
        Protect-PGP -FilePathPublic $KeyPublic -FilePath $sourceFile -OutFilePath $encryptedFile -ErrorAction Stop

        $inspectErrors = @()
        $results = Get-PGPInspect -FolderPath $inspectFolder -ErrorAction Continue -ErrorVariable +inspectErrors

        $results.Count | Should -Be 1
        [IO.Path]::GetFullPath($results[0].SourcePath) | Should -Be ([IO.Path]::GetFullPath($signedFile))
        $results[0].IsSigned | Should -Be $true
        $inspectErrors.Count | Should -Be 1
        $inspectErrors[0].Exception.Message | Should -Match 'Signed content inspection works'
    }

    It ' Test-PGP can flag encrypted content when ThrowIfEncrypted is used' -TestCases @{ KeyPublic = $KeyPublic } {
        $protected = Protect-PGP -FilePathPublic $KeyPublic -String 'Encrypted but unsigned'
        $result = Test-PGP -FilePathPublic $KeyPublic -String $protected -ThrowIfEncrypted
        $result.Status | Should -Be $false
        $result.Error | Should -Not -BeNullOrEmpty
    }

    It ' Encrypt file when public key is UTF-8 BOM encoded' -TestCases @{ KeyPublic = $KeyPublic; KeyPublicBom = $KeyPublicBom; KeysDirectory = $KeysDirectory } {
        [System.IO.File]::WriteAllText($KeyPublicBom, [System.IO.File]::ReadAllText($KeyPublic), [System.Text.UTF8Encoding]::new($true))
        $sourceFile = [io.path]::Combine($KeysDirectory, 'bom-input.txt')
        $encryptedFile = [io.path]::Combine($KeysDirectory, 'bom-input.txt.pgp')
        Set-Content -Path $sourceFile -Value 'BOM tolerant encryption test' -NoNewline

        { Protect-PGP -FilePathPublic $KeyPublicBom -FilePath $sourceFile -OutFilePath $encryptedFile -ErrorAction Stop } | Should -Not -Throw
        Test-Path -LiteralPath $encryptedFile | Should -Be $true
    }

    It ' Decrypt string when private key is UTF-8 BOM encoded' -TestCases @{ KeyPrivate = $KeyPrivate; KeyPrivateBom = $KeyPrivateBom; ProtectedString = $ProtectedString } {
        [System.IO.File]::WriteAllText($KeyPrivateBom, [System.IO.File]::ReadAllText($KeyPrivate), [System.Text.UTF8Encoding]::new($true))
        $String = Unprotect-PGP -FilePathPrivate $KeyPrivateBom -Password 'ZielonaMila9!' -String $Script:ProtectedString
        $String | Should -Be "This is string to encrypt"
    }

    Context 'Error action preference' {
        $Missing = [io.path]::Combine($env:TEMP, 'missing.asc')

        $cmdlets = @(
            @{ Name = 'Get-PGPKeyInfo'; Params = @{ FilePath = $Missing } },
            @{ Name = 'Protect-PGP'; Params = @{ FilePathPublic = $Missing; String = 'text' } },
            @{ Name = 'Test-PGP'; Params = @{ FilePathPublic = $Missing; String = 'text' } },
            @{ Name = 'Unprotect-PGP'; Params = @{ FilePathPrivate = $Missing; Password = 'pass'; String = 'text' } }
        )

        foreach ($cmdlet in $cmdlets) {
            $current = $cmdlet

            It "$($current.Name) throws when -ErrorAction Stop" -TestCases @{ CommandName = $current.Name; Params = $current.Params } {
                param($CommandName, $Params)
                {
                    $paramString = ($Params.GetEnumerator() | ForEach-Object { "-$($_.Key) '$($_.Value)'" }) -join ' '
                    Invoke-Expression "$CommandName $paramString -ErrorAction Stop"
                } | Should -Throw
            }

            It "$($current.Name) throws when `$ErrorActionPreference is Stop" -TestCases @{ CommandName = $current.Name; Params = $current.Params } {
                param($CommandName, $Params)
                {
                    $old = $ErrorActionPreference
                    try {
                        $ErrorActionPreference = 'Stop'
                        $paramString = ($Params.GetEnumerator() | ForEach-Object { "-$($_.Key) '$($_.Value)'" }) -join ' '
                        Invoke-Expression "$CommandName $paramString"
                    } finally {
                        $ErrorActionPreference = $old
                    }
                } | Should -Throw
            }

            It "$($current.Name) warns when ErrorActionPreference is Continue" -TestCases @{ CommandName = $current.Name; Params = $current.Params } {
                param($CommandName, $Params)
                {
                    $old = $ErrorActionPreference
                    try {
                        $ErrorActionPreference = 'Continue'
                        $paramString = ($Params.GetEnumerator() | ForEach-Object { "-$($_.Key) '$($_.Value)'" }) -join ' '
                        Invoke-Expression "$CommandName $paramString -WarningAction SilentlyContinue"
                    }
                    finally {
                        $ErrorActionPreference = $old
                    }
                } | Should -Not -Throw
            }
        }
    }
    # clean everything
    AfterAll {
        $KeysDirectory = [io.path]::Combine($env:TEMP, 'Keys')
        Remove-Item -Path $KeysDirectory -Recurse -Force
    }
}
