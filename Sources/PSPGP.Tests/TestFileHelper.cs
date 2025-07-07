using System;
using System.IO;

namespace PSPGP.Tests;

/// <summary>
/// Helper class for managing test files and directories
/// </summary>
public class TestFileHelper : IDisposable {
    private readonly string _tempDirectory;

    /// <summary>
    /// Gets the temporary directory path for this test session
    /// </summary>
    public string TempDirectory => _tempDirectory;

    /// <summary>
    /// Initializes a new instance of the TestFileHelper class
    /// </summary>
    public TestFileHelper() {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "PSPGP_Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    /// <summary>
    /// Creates a temporary file with the specified content
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="content">The file content</param>
    /// <returns>The full path to the created file</returns>
    public string CreateTempFile(string filename, string content = "Test content") {
        var filePath = Path.Combine(_tempDirectory, filename);
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, content);
        return filePath;
    }

    /// <summary>
    /// Creates a temporary directory
    /// </summary>
    /// <param name="directoryName">The directory name</param>
    /// <returns>The full path to the created directory</returns>
    public string CreateTempDirectory(string directoryName) {
        var dirPath = Path.Combine(_tempDirectory, directoryName);
        Directory.CreateDirectory(dirPath);
        return dirPath;
    }

    /// <summary>
    /// Gets the full path for a file in the temp directory
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <returns>The full path</returns>
    public string GetTempFilePath(string filename) {
        return Path.Combine(_tempDirectory, filename);
    }

    /// <summary>
    /// Checks if a file exists in the temp directory
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <returns>True if the file exists</returns>
    public bool FileExists(string filename) {
        return File.Exists(Path.Combine(_tempDirectory, filename));
    }

    /// <summary>
    /// Reads the content of a file in the temp directory
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <returns>The file content</returns>
    public string ReadFile(string filename) {
        return File.ReadAllText(Path.Combine(_tempDirectory, filename));
    }

    /// <summary>
    /// Deletes any temporary files and directories created for the test.
    /// </summary>
    public void Dispose() {
        try {
            if (Directory.Exists(_tempDirectory)) {
                Directory.Delete(_tempDirectory, true);
            }
        } catch {
            // Ignore cleanup errors
        }
    }
}