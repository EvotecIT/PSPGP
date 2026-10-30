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

    // Refuse case-only aliases even on case-sensitive volumes. The destination may
    // live on a different volume, and probing it would create files during WhatIf.
    internal static StringComparer PathComparer => StringComparer.OrdinalIgnoreCase;

    internal static Item[] Plan(string inputFolder, string outputFolder, Func<string, string> outputName) {
        string root = Path.GetFullPath(inputFolder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        string output = outputFolder == null ? null : Path.GetFullPath(outputFolder);
        string excluded = output == null || !IsWithin(output, root) || PathComparer.Equals(root.TrimEnd(Path.DirectorySeparatorChar), output.TrimEnd(Path.DirectorySeparatorChar))
            ? null : output.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var items = new List<Item>();
        foreach (string file in EnumerateFiles(root, excluded)) {
            string relative = file.Substring(root.Length);
            string destination = output == null ? outputName(file) : Path.Combine(output, outputName(relative));
            items.Add(new Item { Input = file, Output = Path.GetFullPath(destination) });
        }
        var destinations = new HashSet<string>(PathComparer);
        var inputs = new HashSet<string>(items.Select(item => item.Input), PathComparer);
        foreach (Item item in items) {
            if (!destinations.Add(item.Output)) throw new IOException($"Multiple inputs map to '{item.Output}'. Choose a different output folder or rename the inputs.");
            if (inputs.Contains(item.Output)) throw new IOException($"Output '{item.Output}' would overwrite an input in this operation.");
        }
        foreach (Item item in items) {
            for (string parent = Path.GetDirectoryName(item.Output); !string.IsNullOrEmpty(parent); parent = Path.GetDirectoryName(parent)) {
                if (destinations.Contains(parent))
                    throw new IOException($"Output '{parent}' would need to be both a file and a directory. Choose a different output folder or rename the inputs.");
            }
        }
        return items.OrderBy(item => item.Input, PathComparer).ToArray();
    }

    /// <summary>Enumerates folder inputs without following child directory or file reparse points.</summary>
    internal static IEnumerable<string> EnumerateFiles(string inputFolder, string excluded = null) {
        var pending = new Stack<string>();
        pending.Push(Path.GetFullPath(inputFolder));
        while (pending.Count > 0) {
            string directory = pending.Pop();
            if (excluded != null && IsWithin(directory, excluded)) {
                if (!(directory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
                    .StartsWith(excluded, StringComparison.Ordinal))
                    throw new IOException($"Source directory '{directory}' differs only by letter case from the output subtree. Use its exact spelling or choose a different output folder.");
                continue;
            }
            foreach (string child in Directory.GetDirectories(directory)) {
                if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) == 0) pending.Push(child);
            }
            foreach (string file in Directory.GetFiles(directory)) {
                if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) != 0) continue;
                yield return file;
            }
        }
    }

    private static bool IsWithin(string path, string root) => (path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
        .StartsWith(root, StringComparison.OrdinalIgnoreCase);

    internal static string RemoveEncryptedSuffix(string path) {
        foreach (string extension in new[] { ".pgp", ".gpg", ".asc" }) {
            if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase) && Path.GetFileName(path).Length > extension.Length)
                return path.Substring(0, path.Length - extension.Length);
        }
        return path + ".decrypted";
    }

    internal static string RemoveSignedSuffix(string path) => path.EndsWith(".sig", StringComparison.OrdinalIgnoreCase) && Path.GetFileName(path).Length > 4
        ? path.Substring(0, path.Length - 4) : RemoveEncryptedSuffix(path);

    internal static void EnsureOutputDirectory(string input, string output) {
        if (PathComparer.Equals(Path.GetFullPath(input), Path.GetFullPath(output)))
            throw new IOException("Input and output must be different files.");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
    }
}
