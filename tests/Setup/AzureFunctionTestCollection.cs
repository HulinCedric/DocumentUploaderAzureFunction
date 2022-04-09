using Xunit;

namespace DocumentUploader.Setup;

[CollectionDefinition(nameof(AzureFunctionTestCollection))]
public class AzureFunctionTestCollection : ICollectionFixture<AzureFunctionFixture>
{
}