function Protect-PGP {
    [cmdletBinding(DefaultParameterSetName = 'File')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'Folder')]
        [Parameter(Mandatory, ParameterSetName = 'File')]
        [Parameter(Mandatory, ParameterSetName = 'String')]
        [string[]] $FilePathPublic,

        [Parameter(Mandatory, ParameterSetName = 'Folder')][string] $FolderPath,
        [Parameter(ParameterSetName = 'Folder')][string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'File')][string] $FilePath,
        [Parameter(ParameterSetName = 'File')][string] $OutFilePath,

        [Parameter(Mandatory, ParameterSetName = 'String')][string] $String,

        [System.IO.FileInfo] $SignKey,
        [string] $SignPassword,
        [alias('HashAlgorithmTag')][Org.BouncyCastle.Bcpg.HashAlgorithmTag] $HashAlgorithm,
        [Org.BouncyCastle.Bcpg.CompressionAlgorithmTag] $CompressionAlgorithm,
        [PgpCore.Enums.PGPFileType] $FileType,
        [Int32] $PgpSignatureType,
        [Org.BouncyCastle.Bcpg.PublicKeyAlgorithmTag] $PublicKeyAlgorithm,
        [Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag] $SymmetricKeyAlgorithm

    )
    $PublicKeys = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
    foreach ($FilePathPubc in $FilePathPublic) {
        $ResolvedPublicKey = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($FilePathPubc)
        if (Test-Path -LiteralPath $ResolvedPublicKey) {
            $PublicKeys.Add([System.IO.FileInfo]::new($ResolvedPublicKey))
        } else {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Public key doesn't exists $($ResolvedPublicKey): $($_.Exception.Message)"
                return
            }
        }
    }
    try {
        if ($SignKey) {
            $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKeys, $SignKey, $SignPassword)
        } else {
            $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKeys)
        }
        $PGP = [PgpCore.PGP]::new($EncryptionKeys)
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Protect-PGP - Can't encrypt files because: $($_.Exception.Message)"
            return
        }
    }

    if ($PSBoundParameters.ContainsKey('HashAlgorithm')) {
        $PGP.HashAlgorithmTag = $HashAlgorithm
    }
    if ($PSBoundParameters.ContainsKey('CompressionAlgorithm')) {
        $PGP.CompressionAlgorithm = $CompressionAlgorithm
    }
    if ($PSBoundParameters.ContainsKey('FileType')) {
        $PGP.FileType = $FileType
    }
    if ($PSBoundParameters.ContainsKey('PgpSignatureType')) {
        $PGP.PgpSignatureType = $PgpSignatureType
    }
    if ($PSBoundParameters.ContainsKey('PublicKeyAlgorithm')) {
        $PGP.PublicKeyAlgorithm = $PublicKeyAlgorithm
    }
    if ($PSBoundParameters.ContainsKey('SymmetricKeyAlgorithm')) {
        $PGP.SymmetricKeyAlgorithm = $SymmetricKeyAlgorithm
    }

    if ($FolderPath) {
        $ResolvedFolderPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($FolderPath)
        foreach ($File in Get-ChildItem -LiteralPath $ResolvedFolderPath -Recurse:$Recursive) {
            try {
                if ($OutputFolderPath) {
                    $ResolvedOutputFolder = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputFolderPath)
                    $OutputFile = [io.Path]::Combine($ResolvedOutputFolder, "$($File.Name).pgp")
                    if ($SignKey) {
                        $PGP.EncryptFileAndSign($File.FullName, $Outputfile)
                    } else {
                        $PGP.EncryptFile($File.FullName, $OutputFile)
                    }
                } else {
                    if ($SignKey) {
                        $PGP.EncryptFileAndSign($File.FullName, "$($File.FullName).pgp")
                    } else {
                        $PGP.EncryptFile($File.FullName, "$($File.FullName).pgp")
                    }
                }
            } catch {
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FullName): $($_.Exception.Message)"
                    return
                }
            }
        }
    } elseif ($FilePath) {
        try {
            $ResolvedFilePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($FilePath)
            if ($OutFilePath) {
                $ResolvedOutFilePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutFilePath)
                if ($SignKey) {
                    $PGP.EncryptFileAndSign($ResolvedFilePath, $ResolvedOutFilePath)
                } else {
                    $PGP.EncryptFile($ResolvedFilePath, $ResolvedOutFilePath)
                }
            } else {
                if ($SignKey) {
                    $PGP.EncryptFileAndSign($ResolvedFilePath, "$($ResolvedFilePath).pgp")
                } else {
                    $PGP.EncryptFile($ResolvedFilePath, "$($ResolvedFilePath).pgp")
                }
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($FilePath): $($_.Exception.Message)"
                return
            }
        }
    } elseif ($String) {
        try {
            if ($SignKey) {
                $PGP.EncryptArmoredStringAndSign($String)
            } else {
                $PGP.EncryptArmoredString($String)
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt string: $($_.Exception.Message)"
            }
        }
    }
}
