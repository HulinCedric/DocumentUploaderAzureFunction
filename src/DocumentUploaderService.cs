using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DocumentUploader;

internal class DocumentUploaderService
{
    private readonly BlobContainerClient container;

    public DocumentUploaderService(BlobContainerClient container)
        => this.container = container;

    public async Task<Uri> Upload(Document document)
    {
        var blob = container.GetBlobClient(document.Path);

        await blob.UploadAsync(
            document.Content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = document.ContentType,
                    ContentDisposition = document.ContentDisposition
                },
                Metadata = document.Description
            });

        return blob.Uri;
    }
}