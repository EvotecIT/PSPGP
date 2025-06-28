using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// Resolves PowerShell paths to absolute file system paths.
/// </summary>
public static class PathResolver {
    /// <summary>
    /// Resolves the provided PowerShell path to an absolute path using the
    /// session state of the supplied cmdlet.
    /// </summary>
    /// <param name="cmdlet">Cmdlet whose session state is used for resolution.</param>
    /// <param name="path">PowerShell path to resolve.</param>
    /// <returns>Absolute file system path.</returns>
    public static string Resolve(PSCmdlet cmdlet, string path) {
        return cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
    }
}
