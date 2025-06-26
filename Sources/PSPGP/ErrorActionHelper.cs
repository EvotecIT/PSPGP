using System;
using System.Management.Automation;

namespace PSPGP;

internal static class ErrorActionHelper
{
    internal static bool IsStop(PSCmdlet cmdlet)
    {
        if (cmdlet.MyInvocation.BoundParameters.TryGetValue("ErrorAction", out object value))
        {
            if (value is ActionPreference bound)
            {
                return bound == ActionPreference.Stop;
            }
            if (Enum.TryParse<ActionPreference>(value.ToString(), true, out var parsed))
            {
                return parsed == ActionPreference.Stop;
            }
        }

        object pref = cmdlet.GetVariableValue("ErrorActionPreference");
        if (pref is ActionPreference prefEnum)
        {
            return prefEnum == ActionPreference.Stop;
        }
        if (pref != null && Enum.TryParse<ActionPreference>(pref.ToString(), true, out var parsedPref))
        {
            return parsedPref == ActionPreference.Stop;
        }

        return false;
    }

    internal static void WriteErrorOrWarning(
        PSCmdlet cmdlet,
        Exception exception,
        string id,
        ErrorCategory category,
        object targetObject,
        string warningMessage)
    {
        if (IsStop(cmdlet))
        {
            var record = new ErrorRecord(exception, id, category, targetObject);
            cmdlet.WriteError(record);
        }
        else
        {
            cmdlet.WriteWarning(warningMessage);
        }
    }
}
