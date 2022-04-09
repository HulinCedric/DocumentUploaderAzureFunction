using JetBrains.Annotations;

namespace DocumentUploader;

[PublicAPI]
public class UploadDocumentError
{
    public UploadDocumentError(
        string field,
        string error)
    {
        Error = error;
        Field = field;
    }

    public string Error { get; }

    public string Field { get; }
}