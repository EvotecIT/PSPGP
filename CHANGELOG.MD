### ChangeLog

#### 0.1.9 - 2022.03.01
  - Fixes [Full Folder Decryption file path having incorrect property name](https://github.com/EvotecIT/PSPGP/issues/7) in `Write-Warning`

#### 0.1.8 - 2022.03.01
  - Fixes [Protect-PGP does not create an encrypted file by specifying the path with -OutFilePath](https://github.com/EvotecIT/PSPGP/issues/6)

#### 0.1.7 - 2022.02.17
  - Adds [ability to use multiple public keys to encrypt](https://github.com/EvotecIT/PSPGP/issues/2)
  - Fixes [Unprotect-PGP does not resolve path #3](https://github.com/EvotecIT/PSPGP/issues/3)
    - `Unprotect-PGP`/`Protect-PGP`/`Test-PGP` now resolve paths
  - `New-PGPKey`, `Unprotect-PGP` now [works without password](https://github.com/EvotecIT/PSPGP/issues/4)
#### 0.1.6 - 2021.09.06
  - Fixes to publishing
#### 0.1.5 - 2021.08.28
  - Improved error handling
#### 0.1.4 - 2021.08.28
  - Updated library to higher version
#### 0.1.3 - 2021.05.09
  - First release