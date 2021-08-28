function New-PGPKey {
    [cmdletBinding(DefaultParameterSetName = 'Credential')]
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
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'ClearText')]
        [string] $UserName,
        [parameter(Mandatory, ParameterSetName = 'Strength')]
        [parameter(Mandatory, ParameterSetName = 'ClearText')]
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
        [switch] $EmitVersion
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
    try {
        if ($Strength) {
            $PGP.GenerateKey($FilePathPublic, $FilePathPrivate, $UserName, $Password, $Strength, $Certainty, $EmitVersion.IsPresent)
        } else {
            $PGP.GenerateKey($FilePathPublic, $FilePathPrivate, $UserName, $Password)
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