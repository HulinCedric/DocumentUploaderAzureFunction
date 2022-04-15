using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentValidation;
using JetBrains.Annotations;

namespace DocumentUploader;

[PublicAPI]
public class UploadDocumentRequest
{
    public UploadDocumentRequest(
        string? fileName,
        string fileCategory,
        string? contentType,
        string? base64FileContent)
    {
        Base64FileContent = base64FileContent;
        ContentType = contentType;
        FileCategory = fileCategory;
        FileName = fileName;
    }

    [PublicAPI]
    public string? Base64FileContent { get; }

    [PublicAPI]
    public string? ContentType { get; }

    [PublicAPI]
    private string FileCategory { get; }

    [PublicAPI]
    private string? FileName { get; }

    public IEnumerable<UploadDocumentError> CanUploadDocument()
    {
        var validator = new UploadDocumentRequestValidator();
        var validationResult = validator.Validate(this);
        if (!validationResult.IsValid)
            return validationResult.Errors.Select(e => new UploadDocumentError(e.PropertyName, e.ErrorMessage));

        return ArraySegment<UploadDocumentError>.Empty;
    }

    public async Task<BlobClient> UploadDocument(BlobContainerClient blobContainerClient)
    {
        var errors = CanUploadDocument().ToList();
        if (errors.Any())
            throw new InvalidOperationException(string.Join(",", errors.Select(e => e.Error)));

        var fileName = DocumentUploader.FileName.Create(FileName!).Value;
        var fileContent = DocumentUploader.Base64FileContent.Create(Base64FileContent!).Value;
        var fileContentType = FileContentType.Create(ContentType!).Value;
        var blobName = $"{FileCategory}/{fileName.GetRandomizedValue()}";
        var contentDisposition = new ContentDisposition(DispositionTypeNames.Inline)
        {
            FileName = fileName.Value
        };

        var blobClient = blobContainerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            fileContent.GetBinaryValue(),
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = fileContentType.Value,
                    ContentDisposition = contentDisposition.ToString()
                },
                Metadata = new Dictionary<string, string>
                {
                    { nameof(FileName), fileName.Value }
                }
            });

        return blobClient;
    }

    private class UploadDocumentRequestValidator : AbstractValidator<UploadDocumentRequest>
    {
        public UploadDocumentRequestValidator()
        {
            RuleFor(command => command.FileName)
                .MustBeValueObject(DocumentUploader.FileName.Create)
                .When(x => x.FileName is not null)
                .NotEmpty();

            RuleFor(command => command.ContentType)
                .MustBeValueObject(FileContentType.Create)
                .When(x => x.FileName is not null)
                .NotEmpty();

            RuleFor(command => command.FileCategory).NotEmpty();

            RuleFor(command => command.Base64FileContent)
                .MustBeValueObject(DocumentUploader.Base64FileContent.Create)
                .When(x => x.Base64FileContent is not null)
                .NotEmpty();
        }
    }
}