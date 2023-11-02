using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

public class OnModuleImportAndRemove : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
    public void OnImport() {
        //#if FRAMEWORK
        AppDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
        //#endif
    }

    public void OnRemove(PSModuleInfo module) {
        //#if FRAMEWORK
        AppDomain.CurrentDomain.AssemblyResolve -= MyResolveEventHandler;
        //#endif
    }

    private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args) {
        if (args.Name.StartsWith("BouncyCastle.Cryptography,")) {
            string binPath = Path.Combine(Path.GetDirectoryName(typeof(OnModuleImportAndRemove).Assembly.Location), "BouncyCastle.Cryptography.dll");
            return Assembly.LoadFile(binPath);
        } else if (args.Name.StartsWith("PgpCore,")) {
            string binPath = Path.Combine(Path.GetDirectoryName(typeof(OnModuleImportAndRemove).Assembly.Location), "PgpCore.dll");
            return Assembly.LoadFile(binPath);
        }
        return null;
    }
}