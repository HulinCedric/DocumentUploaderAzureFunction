using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace DocumentUploader.UnitTests;

public class FileNameShould
{
    [Theory]
    [InlineData("invoice.pdf")]
    [InlineData("quote.jpg")]
    public void Be_valid(string input)
    {
        // When
        var creationResult = FileName.Create(input);

        // Then
        creationResult.Should().BeSuccess();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invoice")]
    [InlineData(".jpg")]
    [InlineData("invoice.")]
    public void Be_invalid(string input)
    {
        // When
        var creationResult = FileName.Create(input);

        // Then
        creationResult.Should().BeFailure();
    }

    [Theory]
    [InlineData("invoice.pdf", "Invoice.pdf")]
    [InlineData("invoice.pdf", "invoice.Pdf")]
    public void Be_equals(string first, string second)
    {
        // Given
        var firstFileName = FileName.Create(first).Value;
        var secondFileName = FileName.Create(second).Value;

        // When
        var equalityResult = firstFileName == secondFileName;

        // Then
        equalityResult.Should().BeTrue();
    }

    [Fact]
    public void Give_randomized_fileName()
    {
        // Given
        var fileName = FileName.Create("quote.jpg").Value;

        // When
        var randomizedFileName = fileName.GetRandomizedValue();

        // Then
        randomizedFileName.Should().NotContain("quote.jpg");
        randomizedFileName.Should().NotContain("quote");
        randomizedFileName.Should().EndWith(".jpg");
    }

    [Fact]
    public void Give_always_the_same_randomized_fileName()
    {
        // Given
        var fileName = FileName.Create("quote.jpg").Value;

        // When
        var randomizedFileName = fileName.GetRandomizedValue();

        // Then
        randomizedFileName.Should().Be("7af4107f61052c91a3a70ea7ade30587b0a51c26e9af772f80c793a1be08e31b.jpg");
    }
}