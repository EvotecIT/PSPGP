using System;
using System.Management.Automation;

namespace PSPGP;

/// <summary>Writes normal PowerShell error records, leaving common-parameter policy to the engine.</summary>
internal static class CmdletError {
    internal static void Write(PSCmdlet cmdlet, Exception exception, string id, ErrorCategory category, object target) =>
        cmdlet.WriteError(new ErrorRecord(exception, id, category, target));
}
