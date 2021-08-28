function Test-PGP {
    [cmdletBinding(DefaultParameterSetName = 'File')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'Folder')]
        [Parameter(Mandatory, ParameterSetName = 'File')]
        [Parameter(Mandatory, ParameterSetName = 'String')]
        [string] $FilePathPublic,

        [Parameter(Mandatory, ParameterSetName = 'Folder')][string] $FolderPath,
        [Parameter(ParameterSetName = 'Folder')][string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'File')][string] $FilePath,
        [Parameter(ParameterSetName = 'File')][string] $OutFilePath,

        [Parameter(Mandatory, ParameterSetName = 'String')][string] $String
    )
    if (Test-Path -LiteralPath $FilePathPublic) {
        $PublicKey = [System.IO.FileInfo]::new($FilePathPublic)
    } else {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Protect-PGP - Public key doesn't exists $($FilePathPublic): $($_.Exception.Message)"
            return
        }
    }
    try {
        $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKey)
        $PGP = [PgpCore.PGP]::new($EncryptionKeys)
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Test-PGP - Can't test files because: $($_.Exception.Message)"
            return
        }
    }
    if ($FolderPath) {
        foreach ($File in Get-ChildItem -LiteralPath $FolderPath -Recurse:$Recursive) {
            try {
                $Output = $PGP.VerifyFile($File.FullName)
                $ErrorMessage = ''
            } catch {
                $Output = $false
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
                    $ErrorMessage = $($_.Exception.Message)
                }
            }
            [PSCustomObject] @{
                FilePath = $File.FullName
                Status   = $Output
                Error    = $ErrorMessage
            }
        }
    } elseif ($FilePath) {
        try {
            $Output = $PGP.VerifyFile($FilePath)
        } catch {
            $Output = $false
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
                $ErrorMessage = $($_.Exception.Message)
            }
        }
        [PSCustomObject] @{
            FilePath = $FilePath
            Status   = $Output
            Error    = $ErrorMessage
        }
    } elseif ($String) {
        try {
            $PGP.VerifyArmoredString($String)
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
            }
        }
    }

}