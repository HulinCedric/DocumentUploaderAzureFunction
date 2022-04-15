using System;
using System.Collections.Generic;
using System.Net.Mime;
using CSharpFunctionalExtensions;

namespace DocumentUploader;

public class FileContentType : ValueObject
{
    private readonly ContentType contentType;

    private FileContentType(ContentType contentType)
        => this.contentType = contentType;

    public string Value
        => contentType.ToString();

    public static Result<FileContentType> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<FileContentType>("Value must not be empty");

        var contentType = CreateContentType(input);
        if (contentType.IsFailure)
            return Result.Failure<FileContentType>("Value must be a valid content type");

        return new FileContentType(contentType.Value);
    }

    private static Result<ContentType> CreateContentType(string input)
    {
        try
        {
            return new ContentType(input);
        }
        catch (Exception e)
        {
            return Result.Failure<ContentType>(e.Message);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return contentType;
    }
}