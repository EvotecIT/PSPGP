Describe 'PGP key-server safety' {
    BeforeAll {
        if (-not (Get-Module PSPGP)) { Import-Module "$PSScriptRoot/../PSPGP.psd1" -Force -ErrorAction Stop }
    }

    It 'Rejects insecure transport before writing a destination' {
        $destination = Join-Path $TestDrive 'public.asc'
        [IO.File]::WriteAllText($destination, 'preserve')
        { Get-PGPKey -KeyServer 'http://127.0.0.1:1' -Search 'user@example.test' -OutFilePath $destination -ErrorAction Stop } |
            Should -Throw '*HTTPS*'
        [IO.File]::ReadAllText($destination) | Should -Be 'preserve'
    }

    It 'Does not download or create files under WhatIf' {
        $destination = Join-Path $TestDrive 'whatif.asc'
        Get-PGPKey -KeyServer 'https://127.0.0.1:1' -Search 'user@example.test' -OutFilePath $destination -WhatIf -ErrorAction Stop
        Test-Path $destination | Should -BeFalse
    }

    It 'Cancels a stalled HTTPS request when PowerShell stops the pipeline' {
        $listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
        $listener.Start()
        $client = $null
        $pipeline = [PowerShell]::Create()
        try {
            $accept = $listener.AcceptTcpClientAsync()
            $url = 'https://127.0.0.1:' + $listener.LocalEndpoint.Port
            $modulePath = (Get-Module PSPGP).Path
            $null = $pipeline.AddScript('param($modulePath, $url) Import-Module $modulePath -Force; Get-PGPKey -KeyServer $url -Search test -ErrorAction Stop').AddArgument($modulePath).AddArgument($url)
            $running = $pipeline.BeginInvoke()
            $accept.Wait(10000) | Should -BeTrue
            $client = $accept.Result
            $stop = $pipeline.BeginStop($null, $null)
            $stop.AsyncWaitHandle.WaitOne(5000) | Should -BeTrue
            $pipeline.EndStop($stop)
            $pipeline.InvocationStateInfo.State.ToString() | Should -Be 'Stopped'
            $running.IsCompleted | Should -BeTrue
        } finally {
            if ($client) { $client.Dispose() }
            $listener.Stop()
            $pipeline.Dispose()
        }
    }
}
