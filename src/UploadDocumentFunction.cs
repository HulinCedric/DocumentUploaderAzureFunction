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
            log.LogInformation("Start uploading document");

            var canUploadDocumentErrors = request.CanUploadDocument();
            if (canUploadDocumentErrors.Any())
                return new BadRequestObjectResult(canUploadDocumentErrors);

            var blobDocument = await request.UploadDocument(blobContainerClient);

            log.LogInformation("End uploading document");

            return new OkObjectResult(new UploadDocumentResponse(blobDocument.Uri));
        }
    }
}