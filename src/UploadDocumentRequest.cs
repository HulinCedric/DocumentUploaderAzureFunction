using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentValidation;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace DocumentUploader;

[PublicAPI]
public class UploadDocumentRequest
{
    public UploadDocumentRequest(
        string fileName,
        string fileCategory,
        string contentType,
        string base64FileContent)
    {
        Base64FileContent = base64FileContent;
        ContentType = contentType;
        FileCategory = fileCategory;
        FileName = fileName;
    }

    public string Base64FileContent { get; }

    // TODO Replace Guid by hash
    [JsonIgnore]
    public string BlobName
        => $"{FileCategory}/{Guid.NewGuid()}.{GetFileExtension()}";

    public string ContentType { get; }

    private string FileCategory { get; }

    private string FileName { get; }

    private string GetFileExtension()
        => Path.GetExtension(FileName);

    private string GetFileNameWithoutExtension()
        => Path.GetFileNameWithoutExtension(FileName);

    public byte[] GetFileContent()
        => Convert.FromBase64String(Base64FileContent);

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


        // var service = blobContainerClient.GetParentBlobServiceClient();
        //
        // BlobServiceProperties prop = service.GetProperties();
        // BlobContainerProperties test=     blobContainerClient.GetProperties();
        //
        //
        // prop.DefaultServiceVersion = "2021-04-10";


        // service.SetProperties(prop);

        var blobClient = blobContainerClient.GetBlobClient(BlobName);

        await blobClient.UploadAsync(
            BinaryData.FromBytes(GetFileContent()),
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = ContentType,
                    ContentDisposition = "attachment; filename=\"" + FileName + "\""
                }
            });


        return blobClient;
    }

    private class UploadDocumentRequestValidator : AbstractValidator<UploadDocumentRequest>
    {
        public UploadDocumentRequestValidator()
        {
            RuleFor(command => command.FileName)
                .NotEmpty()
                .MustBeValueObject(DocumentUploader.FileName.Create)
                .When(x => x.FileName is not null);

            RuleFor(command => command.ContentType).NotEmpty();
            RuleFor(command => command.FileCategory).NotEmpty();
            RuleFor(command => command.Base64FileContent).NotEmpty();

            RuleFor(command => command)
                .Must(
                    command =>
                    {
                        try
                        {
                            command.GetFileContent();
                        }
                        catch
                        {
                            return false;
                        }

                        return true;
                    })
                .WithName(nameof(Base64FileContent))
                .WithMessage("'Base64 File Content' must be a valid base 64 string");
        }
    }
}