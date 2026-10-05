Describe 'Unsupported PGP encryption formats' {
    BeforeAll {
        if (-not (Get-Module PSPGP)) { Import-Module "$PSScriptRoot/../PSPGP.psd1" -Force -ErrorAction Stop }
        # GnuPG public-key encryption: version 3 recipient packet followed by OCB tag 20.
        $message = [Convert]::FromBase64String('hIwDHCBL6iCIoI8BA/9Kc/oyjM3SWmRB9TzGwc0K4pW4VSU5mIT1C6sdIP75+B2n1kZNC1VWEp4f1L34sjKZnIQv3vvA++ie2QUwnKM5wMXB9G3WazeW4WmevpqV1FF2KG4xKHvWZTLhASDudyePBWevKI7FslOxhRidlVUli/r6DB+UKSCdEXt9n2pR1dRjAQkCEKjWmVrf5NwLP1sVMTmyoD8/dB5YgaZfe0AnPPRkqWvYxt2smfuNgwlgD+EYUX0dsyYyGj+byibIsjJHKIO0HwXDiqMlWRfevEuT8LAPaptvyZK8lsKg5LGbApgeCbpv')
        $encrypted = Join-Path $TestDrive 'ocb.pgp'
        $output = Join-Path $TestDrive 'existing.txt'
        $public = Join-Path $TestDrive 'public.asc'
        $private = Join-Path $TestDrive 'private.asc'
        [IO.File]::WriteAllBytes($encrypted, $message)
        New-PGPKey -FilePathPublic $public -FilePathPrivate $private -Strength 1024 -ErrorAction Stop
    }

    It 'Reports unsupported encryption before key selection and preserves the destination' {
        [IO.File]::WriteAllText($output, 'preserve existing content')
        $operationErrors = @()
        Unprotect-PGP -FilePathPrivate $private -FilePath $encrypted -OutFilePath $output -ErrorAction SilentlyContinue -ErrorVariable operationErrors
        $operationErrors.Count | Should -BeGreaterThan 0
        $operationErrors[0].Exception.Message | Should -Match 'AEAD.*packet tag 20'
        $operationErrors[0].CategoryInfo.Category | Should -Be 'NotImplemented'
        [IO.File]::ReadAllText($output) | Should -Be 'preserve existing content'
    }

    It 'Uses the same unsupported-format diagnostic for inspection' {
        { Get-PGPInspect -FilePath $encrypted -ErrorAction Stop } | Should -Throw '*AEAD*packet tag 20*'
    }
}
