function Protect-PGP {
    [cmdletBinding(DefaultParameterSetName = 'File')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'Folder')]
        [Parameter(Mandatory, ParameterSetName = 'File')]
        [string] $FilePathPublic,

        [Parameter(Mandatory, ParameterSetName = 'Folder')][string] $FolderPath,
        [Parameter(ParameterSetName = 'Folder')][string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'File')][string] $FilePath,
        [Parameter(ParameterSetName = 'File')][string] $OutFilePath
    )

    $PublicKey = [System.IO.FileInfo]::new($FilePathPublic)
    $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKey)

    if ($FolderPath) {
        foreach ($File in Get-ChildItem -LiteralPath $FolderPath -Recurse:$Recursive) {
            try {
                $PGP = [PgpCore.PGP]::new($EncryptionKeys)
                if ($OutputFolderPath) {
                    $OutputFile = [io.Path]::Combine($OutputFolderPath, "$($File.Name).pgp")
                    $PGP.EncryptFile($File.FullName, $OutputFile)
                } else {
                    $PGP.EncryptFile($File.FullName, "$($File.FullName).pgp")
                }
            } catch {
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
                    return
                }
            }
        }
    } elseif ($FilePath) {
        try {
            $PGP = [PgpCore.PGP]::new($EncryptionKeys)
            if ($OutFilePath) {
                $PGP.EncryptFile($FilePath, "$OutFilePath")
            } else {
                $PGP.EncryptFile($FilePath, "$($FilePath).pgp")
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
                return
            }
        }
    }

}