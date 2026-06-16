Update README.md only.

Add a V5 section:

## V5 - Real Azure Execution

TrustCortex can run in full Azure AI mode:

SearchProvider = Azure
AnswerProvider = AzureFoundry

In this mode:
- Azure AI Search retrieves candidate enterprise documents.
- TrustCortex applies role and sensitivity policy filtering.
- Only approved context is sent to AzureFoundry.
- AzureFoundry generates a grounded answer.
- TrustCortex validates and audits the response.

## Why not Azure OpenAI On Your Data?

TrustCortex intentionally controls retrieval and filtering before answer generation.
Using On Your Data directly would move retrieval inside the model call and reduce visibility into governance filtering.

## Configuration

Use user-secrets:

dotnet user-secrets set "SearchProvider" "Azure"
dotnet user-secrets set "AnswerProvider" "AzureFoundry"
dotnet user-secrets set "AzureSearch:Endpoint" "<endpoint>"
dotnet user-secrets set "AzureSearch:AdminKey" "<key>"
dotnet user-secrets set "AzureFoundry:Endpoint" "<endpoint>"
dotnet user-secrets set "AzureFoundry:ApiKey" "<key>"
dotnet user-secrets set "AzureFoundry:DeploymentName" "<deployment>"

## Demo

See DEMO.md.