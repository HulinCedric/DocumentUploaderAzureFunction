# Document uploader Azure Function

Serverless function to store documents to Azure Blob Storage and return the corresponding public url to download purpose.

## Requirements

- .NET 6.0 SDK
- Azure Functions Core Tools
- Azurite

## Functional testing

Functional test require that Azurite or Azure storage emulator is running.
The targeted storage can be updated in appsettings.json file.
Default settings target common windows setup.

