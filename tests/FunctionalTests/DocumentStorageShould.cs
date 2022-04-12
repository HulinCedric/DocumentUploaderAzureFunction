using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DocumentUploader.Setup;
using FluentAssertions;
using Xunit;

namespace DocumentUploader.FunctionalTests;

[Collection(nameof(AzureFunctionTestCollection))]
public class DocumentStorageShould
{
    private readonly AzureFunctionFixture fixture;

    public DocumentStorageShould(AzureFunctionFixture fixture)
        => this.fixture = fixture;

    [Fact]
    public async Task Return_200_with_inline_content_disposition_and_content_type()
    {
        // Arrange
        var documentUrl = await SetupAndGiveDocumentUrl();

        // Act
        var httpResponse = await fixture.Client.GetAsync(documentUrl);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var contentDisposition = httpResponse.Content.Headers.ContentDisposition;
        contentDisposition.Should().NotBeNull();
        contentDisposition!.DispositionType.Should().Be(DispositionTypeNames.Inline);

        var contentType = httpResponse.Content.Headers.ContentType;
        contentType.Should().NotBeNull();
        contentType!.MediaType.Should().Be(MediaTypeNames.Image.Jpeg);
    }

    private async Task<Uri> SetupAndGiveDocumentUrl()
    {
        const string command = @"
{
  ""contentType"": ""image/jpeg"",
  ""fileName"": ""invoice.jpg"",
  ""fileCategory"": ""invoice"",
  ""base64FileContent"": ""/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////wgALCAABAAEBAREA/8QAFBABAAAAAAAAAAAAAAAAAAAAAP/aAAgBAQABPxA=""
}";
        var requestBody = new StringContent(command, Encoding.UTF8, "application/json");

        var httpResponse = await fixture.Client.PostAsync("api/UploadDocument", requestBody);

        var responseContent = await httpResponse.Content.ReadAsAsync<UploadDocumentResponse>();

        return responseContent.DocumentUrl;
    }
}