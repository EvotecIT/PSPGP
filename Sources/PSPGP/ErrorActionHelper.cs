using System;
using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// Utility methods for handling <c>-ErrorAction</c> related logic
/// across cmdlets.
/// </summary>
internal static class ErrorActionHelper {
    /// <summary>
    /// Determines whether the specified cmdlet should treat
    /// errors as terminating based on the <c>-ErrorAction</c>
    /// parameter or the global preference.
    /// </summary>
    /// <param name="cmdlet">Cmdlet being executed.</param>
    /// <returns><c>true</c> if error action is <see cref="ActionPreference.Stop"/>.</returns>
    internal static bool IsStop(PSCmdlet cmdlet) {
        if (cmdlet.MyInvocation.BoundParameters.TryGetValue("ErrorAction", out object value)) {
            if (value is ActionPreference bound) {
                return bound == ActionPreference.Stop;
            }
            if (Enum.TryParse<ActionPreference>(value.ToString(), true, out var parsed)) {
                return parsed == ActionPreference.Stop;
            }
        }

        object pref = cmdlet.GetVariableValue("ErrorActionPreference");
        if (pref is ActionPreference prefEnum) {
            return prefEnum == ActionPreference.Stop;
        }
        if (pref != null && Enum.TryParse<ActionPreference>(pref.ToString(), true, out var parsedPref)) {
            return parsedPref == ActionPreference.Stop;
        }

        return false;
    }

    /// <summary>
    /// Writes an error record or warning depending on the
    /// cmdlet's error action preference.
    /// </summary>
    /// <param name="cmdlet">Cmdlet writing the message.</param>
    /// <param name="exception">Exception to report.</param>
    /// <param name="id">Error identifier.</param>
    /// <param name="category">Error category.</param>
    /// <param name="targetObject">Object associated with the error.</param>
    /// <param name="warningMessage">Message to display when not stopping.</param>
    internal static void WriteErrorOrWarning(
        PSCmdlet cmdlet,
        Exception exception,
        string id,
        ErrorCategory category,
        object targetObject,
        string warningMessage) {
        if (IsStop(cmdlet)) {
            var record = new ErrorRecord(exception, id, category, targetObject);
            cmdlet.WriteError(record);
        } else {
            cmdlet.WriteWarning(warningMessage);
        }
    }
}
