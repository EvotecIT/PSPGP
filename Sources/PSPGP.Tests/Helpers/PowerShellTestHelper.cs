using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSPGP.Tests.Helpers;

/// <summary>
/// Helper class for testing PowerShell cmdlets
/// </summary>
public class PowerShellTestHelper : IDisposable {
    private readonly PowerShell _powerShell;
    private readonly Runspace _runspace;

    /// <summary>
    /// Initializes a new instance of the PowerShellTestHelper class
    /// </summary>
    public PowerShellTestHelper() {
        var initialSessionState = InitialSessionState.CreateDefault();

        try {
            // Import the PSPGP module assembly
            var assembly = typeof(PSPGP.Cmdlets.CmdletNewPGPKey).Assembly;
            if (!string.IsNullOrEmpty(assembly.Location)) {
                initialSessionState.ImportPSModule(new string[] { assembly.Location });
            } else {
                // Fallback: manually add cmdlets if assembly location is not available
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("New-PGPKey", typeof(PSPGP.Cmdlets.CmdletNewPGPKey), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Protect-PGP", typeof(PSPGP.Cmdlets.CmdletProtectPGP), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Unprotect-PGP", typeof(PSPGP.Cmdlets.CmdletUnprotectPGP), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Test-PGP", typeof(PSPGP.Cmdlets.CmdletTestPGP), null));
            }
        } catch (Exception ex) {
            // Fallback: manually add cmdlets if module import fails
            try {
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("New-PGPKey", typeof(PSPGP.Cmdlets.CmdletNewPGPKey), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Protect-PGP", typeof(PSPGP.Cmdlets.CmdletProtectPGP), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Unprotect-PGP", typeof(PSPGP.Cmdlets.CmdletUnprotectPGP), null));
                initialSessionState.Commands.Add(new SessionStateCmdletEntry("Test-PGP", typeof(PSPGP.Cmdlets.CmdletTestPGP), null));
            } catch {
                throw new InvalidOperationException($"Failed to initialize PowerShell test environment: {ex.Message}", ex);
            }
        }

        _runspace = RunspaceFactory.CreateRunspace(initialSessionState);
        _runspace.Open();

        _powerShell = PowerShell.Create();
        _powerShell.Runspace = _runspace;
    }

    /// <summary>
    /// Execute a PowerShell command and return the results
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <returns>Collection of PSObject results</returns>
    public Collection<PSObject> InvokeCommand(string command) {
        _powerShell.Commands.Clear();
        _powerShell.AddScript(command);
        return _powerShell.Invoke();
    }

    /// <summary>
    /// Execute a PowerShell command with parameters
    /// </summary>
    /// <param name="cmdletName">The cmdlet name</param>
    /// <param name="parameters">Parameters to pass to the cmdlet</param>
    /// <returns>Collection of PSObject results</returns>
    public Collection<PSObject> InvokeCmdlet(string cmdletName, params object[] parameters) {
        _powerShell.Commands.Clear();
        _powerShell.AddCommand(cmdletName);

        for (int i = 0; i < parameters.Length; i += 2) {
            if (i + 1 < parameters.Length) {
                _powerShell.AddParameter(parameters[i].ToString(), parameters[i + 1]);
            }
        }

        return _powerShell.Invoke();
    }

    /// <summary>
    /// Get any errors from the last command execution
    /// </summary>
    /// <returns>Collection of error records</returns>
    public Collection<ErrorRecord> GetErrors() {
        return _powerShell.Streams.Error.ReadAll();
    }

    /// <summary>
    /// Clear all streams
    /// </summary>
    public void ClearStreams() {
        _powerShell.Streams.ClearStreams();
    }

    public void Dispose() {
        _powerShell?.Dispose();
        _runspace?.Dispose();
    }
}