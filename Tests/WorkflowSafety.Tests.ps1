Describe 'PGP workflow safety' {
    BeforeAll {
        if (-not (Get-Module PSPGP)) { Import-Module "$PSScriptRoot/../PSPGP.psd1" -Force -ErrorAction Stop }
        $PublicKey = Join-Path $TestDrive 'public.asc'
        $PrivateKey = Join-Path $TestDrive 'private.asc'
        New-PGPKey -FilePathPublic $PublicKey -FilePathPrivate $PrivateKey -Strength 1024 -ErrorAction Stop
    }

    It 'Uses unprotected private keys when the password is omitted' {
        $encrypted = Protect-PGP -FilePathPublic $PublicKey -String 'no password' -ErrorAction Stop
        Unprotect-PGP -FilePathPrivate $PrivateKey -String $encrypted -ErrorAction Stop | Should -Be 'no password'
        $signed = Protect-PGP -SignOnly -SignKey $PrivateKey -String 'signed' -ErrorAction Stop
        (Test-PGP -FilePathPublic $PublicKey -String $signed -ErrorAction Stop).Status | Should -BeTrue
    }

    It 'Returns successful output files through PassThru and derives pipeline decryption paths' {
        $source = Join-Path $TestDrive 'pipeline.txt'
        [IO.File]::WriteAllText($source, 'pipeline')
        $encrypted = Get-Item $source | Protect-PGP -FilePathPublic $PublicKey -PassThru -ErrorAction Stop
        $encrypted | Should -BeOfType ([IO.FileInfo])
        $encrypted.Exists | Should -BeTrue
        Remove-Item $source
        $decrypted = $encrypted | Unprotect-PGP -FilePathPrivate $PrivateKey -PassThru -ErrorAction Stop
        $decrypted.FullName | Should -Be $source
        [IO.File]::ReadAllText($decrypted.FullName) | Should -Be 'pipeline'
    }

    It 'Rejects configuration that cannot affect the selected operation' {
        { Protect-PGP -FilePathPublic $PublicKey -String text -PublicKeyAlgorithm RsaGeneral -ErrorAction Stop } | Should -Throw '*existing key*'
        { Protect-PGP -FilePathPublic $PublicKey -String text -Armor:$false -ErrorAction Stop } | Should -Throw '*armored text*'
    }

    It 'Resolves relative signing keys from the PowerShell location' {
        Push-Location $TestDrive
        try {
            $signed = Protect-PGP -SignOnly -SignKey './private.asc' -String 'relative key' -ErrorAction Stop
            (Test-PGP -FilePathPublic './public.asc' -String $signed -ErrorAction Stop).Status | Should -BeTrue
        } finally { Pop-Location }
    }

    It 'Preserves directory structure when filenames repeat' {
        $inputPath = Join-Path $TestDrive 'tree'
        $encrypted = Join-Path $TestDrive 'encrypted'
        $decrypted = Join-Path $TestDrive 'decrypted'
        New-Item -ItemType Directory -Path (Join-Path $inputPath 'a'), (Join-Path $inputPath 'b') | Out-Null
        [IO.File]::WriteAllText((Join-Path $inputPath 'a/same.txt'), 'first')
        [IO.File]::WriteAllText((Join-Path $inputPath 'b/same.txt'), 'second')
        Protect-PGP -FilePathPublic $PublicKey -FolderPath $inputPath -OutputFolderPath $encrypted -ErrorAction Stop
        Unprotect-PGP -FilePathPrivate $PrivateKey -FolderPath $encrypted -OutputFolderPath $decrypted -ErrorAction Stop
        [IO.File]::ReadAllText((Join-Path $decrypted 'a/same.txt')) | Should -Be 'first'
        [IO.File]::ReadAllText((Join-Path $decrypted 'b/same.txt')) | Should -Be 'second'
    }

    It 'Rejects suffix collisions before producing any output' {
        $inputPath = Join-Path $TestDrive 'collisions'
        $output = Join-Path $TestDrive 'collision-output'
        New-Item -ItemType Directory -Path $inputPath | Out-Null
        [IO.File]::WriteAllText((Join-Path $inputPath 'same.pgp'), 'first')
        [IO.File]::WriteAllText((Join-Path $inputPath 'same.gpg'), 'second')
        { Unprotect-PGP -FilePathPrivate $PrivateKey -FolderPath $inputPath -OutputFolderPath $output -ErrorAction Stop } | Should -Throw '*Multiple inputs*'
        Test-Path $output | Should -BeFalse
    }

    It 'Skips an existing output subtree during repeated folder protection' {
        $inputPath = Join-Path $TestDrive 'nested-tree'
        $output = Join-Path $inputPath 'output'
        New-Item -ItemType Directory -Path $inputPath, $output | Out-Null
        [IO.File]::WriteAllText((Join-Path $inputPath 'plain.txt'), 'content')
        Protect-PGP -FilePathPublic $PublicKey -FolderPath $inputPath -OutputFolderPath $output -ErrorAction Stop
        Protect-PGP -FilePathPublic $PublicKey -FolderPath $inputPath -OutputFolderPath $output -ErrorAction Stop
        @(Get-ChildItem $output -File -Recurse).Count | Should -Be 1
    }

    It 'Does not create output folders or key files during WhatIf' {
        $directory = Join-Path $TestDrive 'whatif'
        New-PGPKey -FilePathPublic (Join-Path $directory 'public.asc') -FilePathPrivate (Join-Path $directory 'private.asc') -WhatIf -ErrorAction Stop
        Test-Path $directory | Should -BeFalse
        $inputPath = Join-Path $TestDrive 'whatif-input.txt'
        [IO.File]::WriteAllText($inputPath, 'content')
        Protect-PGP -FilePathPublic $PublicKey -FilePath $inputPath -OutFilePath (Join-Path $directory 'output.pgp') -WhatIf -ErrorAction Stop
        Test-Path $directory | Should -BeFalse
    }

    It 'Preserves existing key files unless replacement is explicit' {
        $original = [IO.File]::ReadAllText($PrivateKey)
        { New-PGPKey -FilePathPublic $PublicKey -FilePathPrivate $PrivateKey -ErrorAction Stop } | Should -Throw '*-Force*'
        [IO.File]::ReadAllText($PrivateKey) | Should -Be $original
    }

    It 'Records missing-key errors under SilentlyContinue without warning' {
        $operationErrors = @()
        $operationWarnings = @()
        Protect-PGP -FilePathPublic (Join-Path $TestDrive 'missing.asc') -String 'text' -ErrorAction SilentlyContinue -ErrorVariable operationErrors -WarningVariable operationWarnings
        $operationErrors.Count | Should -BeGreaterThan 0
        $operationWarnings.Count | Should -Be 0
    }

    It 'Accepts FileInfo pipeline records' {
        $inputPath = Join-Path $TestDrive 'pipeline.txt'
        [IO.File]::WriteAllText($inputPath, 'pipeline content')
        Get-Item $inputPath | Protect-PGP -FilePathPublic $PublicKey -ErrorAction Stop
        Test-Path ($inputPath + '.pgp') | Should -BeTrue
    }

    It 'Shows subkey capabilities from authenticated certificate metadata' {
        $primary = Get-PGPKeyInfo -FilePath $PublicKey -ErrorAction Stop
        $primary.CanSign | Should -BeTrue
        $primary.CanEncrypt | Should -BeFalse
        $primary.Subkeys.Count | Should -Be 1
        $primary.Subkeys[0].IsUsableForEncryption | Should -BeTrue
        $primary.Subkeys[0].CanSign | Should -BeFalse
        @(Get-PGPKeyInfo -FilePath $PublicKey -IncludeSubkeys -ErrorAction Stop).Count | Should -Be 2
    }

    It 'Rejects encrypted unsigned input and gives invalid signatures an actionable result' {
        $encrypted = Protect-PGP -FilePathPublic $PublicKey -String 'unsigned'
        $result = Test-PGP -FilePathPublic $PublicKey -String $encrypted
        $result.Status | Should -BeFalse
        $result.Error | Should -Not -BeNullOrEmpty
        $signature = Protect-PGP -SignOnly -Detached -SignKey $PrivateKey -String 'original'
        $result = Test-PGP -FilePathPublic $PublicKey -String 'changed' -Signature $signature
        $result.Status | Should -BeFalse
        $result.Error | Should -Not -BeNullOrEmpty
    }
}
