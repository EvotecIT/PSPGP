function Unprotect-PGP {
    [cmdletBinding(DefaultParameterSetName = 'FolderCredential')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'FolderCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FolderClearText')]
        [Parameter(Mandatory, ParameterSetName = 'FileCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FileClearText')]
        [Parameter(Mandatory, ParameterSetName = 'StringClearText')]
        [Parameter(Mandatory, ParameterSetName = 'StringCredential')]
        [string] $FilePathPrivate,

        [Parameter(Mandatory, ParameterSetName = 'FolderClearText')]
        [Parameter(Mandatory, ParameterSetName = 'FileClearText')]
        [Parameter(Mandatory, ParameterSetName = 'StringClearText')]
        [string] $Password,

        [Parameter(Mandatory, ParameterSetName = 'FileCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FolderCredential')]
        [Parameter(Mandatory, ParameterSetName = 'StringCredential')]
        [pscredential] $Credential,


        [Parameter(Mandatory, ParameterSetName = 'FolderCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FolderClearText')]
        [string] $FolderPath,

        [Parameter(Mandatory, ParameterSetName = 'FolderCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FolderClearText')]
        [string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'FileCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FileClearText')]
        [string] $FilePath,

        [Parameter(Mandatory, ParameterSetName = 'FileCredential')]
        [Parameter(Mandatory, ParameterSetName = 'FileClearText')]
        [string] $OutFilePath,

        [Parameter(Mandatory, ParameterSetName = 'StringClearText')]
        [Parameter(Mandatory, ParameterSetName = 'StringCredential')]
        [string] $String
    )



    if ($Credential) {
        $Password = $Credential.GetNetworkCredential().Password
    }

    if (-not (Test-Path -LiteralPath $FilePathPrivate)) {
        Write-Warning -Message "Unprotect-PGP - Remove PGP encryption failed because private key not given."
        return
    }
    $PrivateKey = Get-Content -LiteralPath $FilePathPrivate -Raw
    try {
        $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PrivateKey, $Password)

        $PGP = [PgpCore.PGP]::new($EncryptionKeys)
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Protect-PGP - Can't encrypt files because: $($_.Exception.Message)"
            return
        }
    }
    if ($FolderPath) {
        foreach ($File in Get-ChildItem -LiteralPath $FolderPath -Recurse:$Recursive) {
            try {
                if ($OutputFolderPath) {
                    $OutputFile = [io.Path]::Combine($OutputFolderPath, "$($File.Name.Replace('.pgp',''))")
                    $PGP.DecryptFile($File.FullName, $OutputFile)
                } else {
                    $PGP.DecryptFile($File.FullName, "$($File.FullName)")
                }
            } catch {
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Unprotect-PGP - Remove PGP encryption from $($File.FuleName) failed: $($_.Exception.Message)"
                    return
                }
            }
        }
    } elseif ($FilePath) {
        try {
            if ($OutFilePath) {
                $PGP.DecryptFile($FilePath, "$OutFilePath", $FilePathPrivate, $Password)
            } else {
                $PGP.DecryptFile($FilePath, "$($FilePath.Replace('.pgp',''))")
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Unprotect-PGP - Remove PGP encryption from $($File.FuleName) failed: $($_.Exception.Message)"
                return
            }
        }
    } elseif ($String) {
        try {
            $PGP.DecryptArmoredString($String)
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Unprotect-PGP - Remove PGP encryption from string failed: $($_.Exception.Message)"
                return
            }
        }
    }
}