using System.Management.Automation;

namespace PSPGP;
public static class PathResolver {
    public static string Resolve(PSCmdlet cmdlet, string path) {
        return cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
    }
}