using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentUploader.Setup;
using FluentAssertions;
using Xunit;

namespace DocumentUploader.FunctionalTests;

[Collection(nameof(AzureFunctionTestCollection))]
public class UploadDocumentShould
{
    private const string RequestUri = "api/UploadDocument";
    private readonly AzureFunctionFixture fixture;

    public UploadDocumentShould(AzureFunctionFixture fixture)
        => this.fixture = fixture;

    [Fact]
    public async Task Return_200_with_randomized_document_url()
    {
        // Arrange
        const string fileName = "invoice.jpg";
        const string command = $@"
{{
  ""contentType"": ""image/jpeg"",
  ""fileName"": ""{fileName}"",
  ""fileCategory"": ""invoice"",
  ""base64FileContent"": ""/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////wgALCAABAAEBAREA/8QAFBABAAAAAAAAAAAAAAAAAAAAAP/aAAgBAQABPxA=""
}}";
        var requestBody = new StringContent(command, Encoding.UTF8, "application/json");

        // Act
        var httpResponse = await fixture.Client.PostAsync(RequestUri, requestBody);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reponse = await httpResponse.Content.ReadAsAsync<UploadDocumentResponse>();
        reponse.Should().NotBeNull();
        reponse.DocumentUrl.Should().NotBeNull();
        reponse.DocumentUrl.AbsoluteUri.Should().NotContain(fileName);
    }

    [Fact]
    public async Task Return_400_when_empty_command_provided()
    {
        // Arrange
        var command = "{}";
        var requestBody = new StringContent(command, Encoding.UTF8, "application/json");

        // Act
        var httpResponse = await fixture.Client.PostAsync(RequestUri, requestBody);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await httpResponse.Content.ReadAsAsync<IReadOnlyCollection<UploadDocumentError>>();
        errors.Should().NotBeNull().And.NotBeEmpty();
        errors.Should().Contain(error => error.Field == "FileName");
    }

    [Fact]
    public async Task Return_400_when_invalid_base64_content_file_provided()
    {
        // Arrange
        var command = @"
{
  ""contentType"": ""image/jpeg"",
  ""fileName"": ""invalid.jpg"",
  ""fileCategory"": ""invoice"",
  ""base64FileContent"": ""invalid base 64 content % %""
}";
        var requestBody = new StringContent(command, Encoding.UTF8, "application/json");

        // Act
        var httpResponse = await fixture.Client.PostAsync(RequestUri, requestBody);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await httpResponse.Content.ReadAsAsync<IReadOnlyCollection<UploadDocumentError>>();
        errors.Should().NotBeNull().And.NotBeEmpty();
        errors.Should().Contain(error => error.Field == "Base64FileContent");
    }
}