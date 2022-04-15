using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;

namespace DocumentUploader;

[PublicAPI]
public class UploadDocumentRequest
{
    public UploadDocumentRequest(
        string? fileName,
        string? fileCategory,
        string? contentType,
        string? base64FileContent)
    {
        Base64FileContent = base64FileContent;
        ContentType = contentType;
        FileCategory = fileCategory;
        FileName = fileName;
    }

    public string? Base64FileContent { get; }

    public string? ContentType { get; }

    public string? FileCategory { get; }

    public string? FileName { get; }

    public IEnumerable<UploadDocumentError> Validate()
    {
        var validator = new UploadDocumentRequestValidator();
        var validationResult = validator.Validate(this);
        if (!validationResult.IsValid)
            return validationResult.Errors.Select(e => new UploadDocumentError(e.PropertyName, e.ErrorMessage));

        return ArraySegment<UploadDocumentError>.Empty;
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