using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace DocumentUploader;

internal class Document
{
    private readonly string category;
    private readonly Base64FileContent content;
    private readonly ContentDisposition contentDisposition;
    private readonly FileContentType contentType;
    private readonly FileName name;

    public Document(
        FileName name,
        string category,
        FileContentType contentType,
        Base64FileContent content)
    {
        this.name = name;
        this.category = category.Trim().ToLowerInvariant();
        this.contentType = contentType;
        this.content = content;

        contentDisposition = new ContentDisposition(DispositionTypeNames.Inline)
        {
            FileName = name.Value
        };
    }

    public BinaryData Content
        => content.GetBinaryValue();

    public string ContentDisposition
        => contentDisposition.ToString();

    public string ContentType
        => contentType.Value;

    public IDictionary<string, string> Description
        => new Dictionary<string, string>
        {
            { "Name", name.Value },
            { "Category", category },
        };

    public string Path
        => $"{category}/{name.GetRandomizedValue()}";

    public override string ToString()
        => $"Name: {name} - Category: {category}";
}