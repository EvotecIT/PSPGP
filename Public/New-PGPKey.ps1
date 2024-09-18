function New-PGPKey {
    [cmdletBinding(DefaultParameterSetName = 'ClearText')]
    param(
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'StrengthCredential')]
        [parameter(Mandatory, ParameterSetName = 'ClearText')]
        [parameter(Mandatory, ParameterSetName = 'Credential')]
        [string] $FilePathPublic,
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'StrengthCredential')]
        [parameter(Mandatory, ParameterSetName = 'ClearText')]
        [parameter(Mandatory, ParameterSetName = 'Credential')]
        [string] $FilePathPrivate,
        [parameter(ParameterSetName = 'Strength')]
        [parameter(ParameterSetName = 'ClearText')]
        [string] $UserName,
        [parameter(ParameterSetName = 'Strength')]
        [parameter(ParameterSetName = 'ClearText')]
        [string] $Password,
        [parameter(Mandatory, ParameterSetName = 'StrengthCredential')]
        [parameter(Mandatory, ParameterSetName = 'Credential')]
        [pscredential] $Credential,
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'StrengthCredential')]
        [int] $Strength,
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'StrengthCredential')]
        [int] $Certainty,
        [parameter(ParameterSetName = 'Strength')]
        [parameter(ParameterSetName = 'StrengthCredential')]
        [switch] $EmitVersion,

        [alias('HashAlgorithmTag')][Org.BouncyCastle.Bcpg.HashAlgorithmTag] $HashAlgorithm,
        [Org.BouncyCastle.Bcpg.CompressionAlgorithmTag] $CompressionAlgorithm,
        [PgpCore.Enums.PGPFileType] $FileType,
        [Int32] $PgpSignatureType,
        [Org.BouncyCastle.Bcpg.PublicKeyAlgorithmTag] $PublicKeyAlgorithm,
        [Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag] $SymmetricKeyAlgorithm
    )
    try {
        $PGP = [PgpCore.PGP]::new()
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "New-PGPKey - Creating keys genarated erorr: $($_.Exception.Message)"
            return
        }
    }
    if ($Credential) {
        $UserName = $Credential.UserName
        $Password = $Credential.GetNetworkCredential().Password
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

    $ResolvedPublicKey = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($FilePathPublic)
    $ResolvedPrivateKey = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($FilePathPrivate)

    try {
        if ($Strength) {
            $PGP.GenerateKey($ResolvedPublicKey, $ResolvedPrivateKey, $UserName, $Password, $Strength, $Certainty, $EmitVersion.IsPresent)
        } else {
            $PGP.GenerateKey($ResolvedPublicKey, $ResolvedPrivateKey, $UserName, $Password)
        }
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "New-PGPKey - Creating keys genarated erorr: $($_.Exception.Message)"
            return
        }
    }

    #void GenerateKey(string publicKeyFilePath, string privateKeyFilePath, string username, string password, int strength, int certainty, bool emitVersion)
    #void GenerateKey(System.IO.Stream publicKeyStream, System.IO.Stream privateKeyStream, string username, string password, int strength, int certainty, bool armor, bool emitVersion)
}