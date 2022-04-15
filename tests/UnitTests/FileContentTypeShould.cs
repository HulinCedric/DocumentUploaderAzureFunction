using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace DocumentUploader.UnitTests;

public class FileContentTypeShould
{
    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("application/json")]
    [InlineData("application/pdf")]
    [InlineData("valid/with_one_slash")]
    [InlineData("application/unknown")]
    [InlineData("unknown/unknown")]
    public void Be_valid(string input)
    {
        // When
        var creationResult = FileContentType.Create(input);

        // Then
        creationResult.Should().BeSuccess();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid_without_slash")]
    [InlineData("invalid/with/two_slash")]
    public void Be_invalid(string input)
    {
        // When
        var creationResult = FileContentType.Create(input);

        // Then
        creationResult.Should().BeFailure();
    }
}