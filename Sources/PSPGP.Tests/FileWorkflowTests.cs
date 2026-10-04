using System;
using System.IO;
using Xunit;

namespace PSPGP.Tests;

public class FileWorkflowTests {
    [Fact]
    public void FileDirectoryOutputCollision_IsRejectedBeforeOutputCreation() {
        using var files = new TestFileHelper();
        string input = files.CreateTempDirectory("input");
        Directory.CreateDirectory(Path.Combine(input, "a"));
        File.WriteAllText(Path.Combine(input, "a.pgp"), "first encrypted file");
        File.WriteAllText(Path.Combine(input, "a", "b.pgp"), "second encrypted file");
        string output = Path.Combine(files.TempDirectory, "output");
        Assert.Throws<IOException>(() => FileWorkflow.Plan(input, output, FileWorkflow.RemoveEncryptedSuffix));
        Assert.False(Directory.Exists(output));
    }

    [Theory]
    [InlineData(".pgp", ".pgp.decrypted")]
    [InlineData("a/.gpg", "a/.gpg.decrypted")]
    [InlineData("a/.asc", "a/.asc.decrypted")]
    [InlineData("a/.sig", "a/.sig.decrypted")]
    public void SuffixOnlyFilename_DoesNotMapToItsContainingDirectory(string input, string expected) {
        Assert.Equal(expected, FileWorkflow.RemoveSignedSuffix(input));
    }

    [Fact]
    public void CaseOnlyOutputAliases_AreRejectedBeforeOutputCreation() {
        using var files = new TestFileHelper();
        string input = files.CreateTempDirectory("input");
        File.WriteAllText(Path.Combine(input, "report.pgp"), "first");
        File.WriteAllText(Path.Combine(input, "REPORT.gpg"), "second");
        string output = Path.Combine(files.TempDirectory, "output");
        Assert.Throws<IOException>(() => FileWorkflow.Plan(input, output, FileWorkflow.RemoveEncryptedSuffix));
        Assert.False(Directory.Exists(output));
    }

    [Fact]
    public void CaseOnlyOutputSubtreeAlias_IsRejectedInsteadOfSilentlyOmittingSourceFiles() {
        using var files = new TestFileHelper();
        string input = files.CreateTempDirectory("input");
        string sourceDirectory = Directory.CreateDirectory(Path.Combine(input, "OUTPUT")).FullName;
        string source = Path.Combine(sourceDirectory, "report.txt");
        File.WriteAllText(source, "must be processed or explicitly rejected");
        string output = Path.Combine(input, "output");
        Assert.Throws<IOException>(() => FileWorkflow.Plan(input, output, name => name + ".pgp"));
        Assert.Equal("must be processed or explicitly rejected", File.ReadAllText(source));
        Assert.Empty(Directory.GetFiles(sourceDirectory, "*.pgp"));
    }

    [Fact]
    public void CaseOnlyInputAlias_IsRejectedBeforeCreatingDirectories() {
        using var files = new TestFileHelper();
        string input = files.CreateTempFile("source.txt");
        Assert.Throws<IOException>(() => FileWorkflow.EnsureOutputDirectory(input,
            Path.Combine(files.TempDirectory, "SOURCE.txt")));
        Assert.Equal("Test content", File.ReadAllText(input));
    }
}
