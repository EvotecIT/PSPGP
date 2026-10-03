using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSPGP;

/// <summary>Plans PowerShell folder operations before any output is created.</summary>
internal static class FileWorkflow {
    internal sealed class Item {
        internal string Input { get; set; }
        internal string Output { get; set; }
    }

    internal static StringComparer PathComparer => Path.DirectorySeparatorChar == '\\'
        ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    internal static Item[] Plan(string inputFolder, string outputFolder, Func<string, string> outputName) {
        string root = Path.GetFullPath(inputFolder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        string output = outputFolder == null ? null : Path.GetFullPath(outputFolder);
        string excluded = output == null || !IsWithin(output, root) || PathComparer.Equals(root.TrimEnd(Path.DirectorySeparatorChar), output.TrimEnd(Path.DirectorySeparatorChar))
            ? null : output.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var items = new List<Item>();
        var pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count > 0) {
            string directory = pending.Pop();
            if (excluded != null && IsWithin(directory, excluded)) continue;
            foreach (string child in Directory.GetDirectories(directory)) {
                if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) == 0) pending.Push(child);
            }
            foreach (string file in Directory.GetFiles(directory)) {
                if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) != 0) continue;
                string relative = file.Substring(root.Length);
                string destination = output == null ? outputName(file) : Path.Combine(output, outputName(relative));
                items.Add(new Item { Input = file, Output = destination });
            }
        }
        var destinations = new HashSet<string>(PathComparer);
        var inputs = new HashSet<string>(items.Select(item => item.Input), PathComparer);
        foreach (Item item in items) {
            if (!destinations.Add(item.Output)) throw new IOException($"Multiple inputs map to '{item.Output}'. Choose a different output folder or rename the inputs.");
            if (inputs.Contains(item.Output)) throw new IOException($"Output '{item.Output}' would overwrite an input in this operation.");
        }
        return items.OrderBy(item => item.Input, PathComparer).ToArray();
    }

    private static bool IsWithin(string path, string root) => (path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
        .StartsWith(root, Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    internal static string RemoveEncryptedSuffix(string path) {
        foreach (string extension in new[] { ".pgp", ".gpg", ".asc" }) {
            if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) return path.Substring(0, path.Length - extension.Length);
        }
        return path + ".decrypted";
    }

    internal static string RemoveSignedSuffix(string path) => path.EndsWith(".sig", StringComparison.OrdinalIgnoreCase)
        ? path.Substring(0, path.Length - 4) : RemoveEncryptedSuffix(path);

    internal static void EnsureOutputDirectory(string input, string output) {
        if (PathComparer.Equals(Path.GetFullPath(input), Path.GetFullPath(output)))
            throw new IOException("Input and output must be different files.");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
    }
}
