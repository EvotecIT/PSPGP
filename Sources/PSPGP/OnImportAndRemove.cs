using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

/// <summary>
/// Handles module import and removal events to resolve assemblies when running in PowerShell.
/// </summary>
public class OnModuleImportAndRemove : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
    /// <summary>
    /// Invoked when the module is imported.
    /// </summary>
    public void OnImport() {
        if (IsNetFramework()) {
            AppDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
        }
    }

    /// <summary>
    /// Invoked when the module is removed.
    /// </summary>
    /// <param name="module">Module being removed.</param>
    public void OnRemove(PSModuleInfo module) {
        if (IsNetFramework()) {
            AppDomain.CurrentDomain.AssemblyResolve -= MyResolveEventHandler;
        }
    }

    /// <summary>
    /// Resolves assemblies for the module when running under .NET Framework.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">Arguments describing the assembly to resolve.</param>
    /// <returns>The resolved <see cref="Assembly"/> instance, or <c>null</c> if none could be located.</returns>
    private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args) {
        //This code is used to resolve the assemblies
        //Console.WriteLine($"Resolving {args.Name}");
        var directoryPath = Path.GetDirectoryName(typeof(OnModuleImportAndRemove).Assembly.Location);
        var filesInDirectory = Directory.GetFiles(directoryPath);

        foreach (var file in filesInDirectory) {
            var fileName = Path.GetFileName(file);
            var assemblyName = Path.GetFileNameWithoutExtension(file);

            if (args.Name.StartsWith(assemblyName)) {
                //Console.WriteLine($"Loading {args.Name} assembly {fileName}");
                return Assembly.LoadFile(file);
            }
        }
        return null;
    }

    /// <summary>
    /// Determines whether the current runtime is .NET Framework.
    /// </summary>
    /// <returns><c>true</c> when running on .NET Framework.</returns>
    private bool IsNetFramework() {
        // Get the version of the CLR
        Version clrVersion = System.Environment.Version;
        // Check if the CLR version is 4.x.x.x
        return clrVersion.Major == 4;
    }

    /// <summary>
    /// Determines whether the current runtime is .NET Core.
    /// </summary>
    /// <returns><c>true</c> when running on .NET Core.</returns>
    private bool IsNetCore() {
        return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current runtime is .NET 5 or higher.
    /// </summary>
    /// <returns><c>true</c> when running on .NET 5 or higher.</returns>
    private bool IsNet5OrHigher() {
        return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET 5", StringComparison.OrdinalIgnoreCase) ||
               System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET 6", StringComparison.OrdinalIgnoreCase) ||
               System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET 7", StringComparison.OrdinalIgnoreCase) ||
               System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET 8", StringComparison.OrdinalIgnoreCase);
    }
}

