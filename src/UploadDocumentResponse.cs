using System;
using JetBrains.Annotations;

namespace DocumentUploader;

[PublicAPI]
public class UploadDocumentResponse
{
    public UploadDocumentResponse(Uri documentUrl)
        => DocumentUrl = documentUrl;

    public Uri DocumentUrl { get; }
}