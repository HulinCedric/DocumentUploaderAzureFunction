using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DocumentUploader
{
    public static class UploadDocumentFunction
    {
        public const string DocumentContainerName = "docs";

        [FunctionName("UploadDocument")]
        public static async Task<IActionResult> UploadDocument(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            UploadDocumentRequest request,
            [Blob(DocumentContainerName, FileAccess.ReadWrite)]
            BlobContainerClient blobContainerClient,
            ILogger log)
        {
            log.LogInformation("Upload document process starting");

            var validationErrors = request.Validate();
            if (validationErrors.Any())
            {
                log.LogWarning(
                    "Invalid upload document request: {@UploadDocumentRequestValidationErrors}",
                    validationErrors);
                return new BadRequestObjectResult(validationErrors);
            }

            var documentUploader = new DocumentUploaderService(blobContainerClient);

            var document = new Document(
                FileName.Create(request.FileName!).Value,
                request.FileCategory!,
                FileContentType.Create(request.ContentType!).Value,
                Base64FileContent.Create(request.Base64FileContent!).Value);

            log.LogInformation("Start uploading document ({DocumentDescription})", document.ToString());

            var documentUrl = await documentUploader.Upload(document);

            log.LogInformation("End uploading document ({DocumentUrl})", documentUrl);

            return new OkObjectResult(new UploadDocumentResponse(documentUrl));
        }
    }
}