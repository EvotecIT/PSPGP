using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace PSPGP.Tests;

/// <summary>
/// Unit tests for TestFileHelper utility class
/// </summary>
public class TestFileHelperTests : IDisposable {
    private readonly TestFileHelper _fileHelper;

    public TestFileHelperTests() {
        _fileHelper = new TestFileHelper();
    }

    [Fact]
    public void TempDirectory_ShouldExist() {
        // Act & Assert
        Directory.Exists(_fileHelper.TempDirectory).Should().BeTrue("Temp directory should be created");
    }

    [Fact]
    public void CreateTempFile_ShouldCreateFileWithContent() {
        // Arrange
        var filename = "test.txt";
        var content = "Test content";

        // Act
        var filePath = _fileHelper.CreateTempFile(filename, content);

        // Assert
        File.Exists(filePath).Should().BeTrue("File should be created");
        File.ReadAllText(filePath).Should().Be(content, "File should contain specified content");
        filePath.Should().Be(_fileHelper.GetTempFilePath(filename), "Path should match expected location");
    }

    [Fact]
    public void CreateTempDirectory_ShouldCreateDirectory() {
        // Arrange
        var dirName = "testdir";

        // Act
        var dirPath = _fileHelper.CreateTempDirectory(dirName);

        // Assert
        Directory.Exists(dirPath).Should().BeTrue("Directory should be created");
        dirPath.Should().Be(Path.Combine(_fileHelper.TempDirectory, dirName), "Path should match expected location");
    }

    [Fact]
    public void FileExists_ShouldReturnCorrectStatus() {
        // Arrange
        var filename = "exists.txt";
        _fileHelper.CreateTempFile(filename, "content");

        // Act & Assert
        _fileHelper.FileExists(filename).Should().BeTrue("Should return true for existing file");
        _fileHelper.FileExists("nonexistent.txt").Should().BeFalse("Should return false for non-existing file");
    }

    [Fact]
    public void ReadFile_ShouldReturnContent() {
        // Arrange
        var filename = "read.txt";
        var content = "Content to read";
        _fileHelper.CreateTempFile(filename, content);

        // Act
        var readContent = _fileHelper.ReadFile(filename);

        // Assert
        readContent.Should().Be(content, "Should return the file content");
    }

    [Fact]
    public void GetTempFilePath_ShouldReturnCorrectPath() {
        // Arrange
        var filename = "path.txt";

        // Act
        var path = _fileHelper.GetTempFilePath(filename);

        // Assert
        path.Should().Be(Path.Combine(_fileHelper.TempDirectory, filename), "Should return correct path");
    }

    public void Dispose() {
        _fileHelper?.Dispose();
    }
}