using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace DocumentUploader.UnitTests;

public class Base64FileContentShould
{
    [Theory]
    [InlineData("/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////wgALCAABAAEBAREA/8QAFBABAAAAAAAAAAAAAAAAAAAAAP/aAAgBAQABPxA=")]
    public void Be_valid(string input)
    {
        // When
        var creationResult = Base64FileContent.Create(input);

        // Then
        creationResult.Should().BeSuccess();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("%")]
    public void Be_invalid(string input)
    {
        // When
        var creationResult = Base64FileContent.Create(input);

        // Then
        creationResult.Should().BeFailure();
    }

}