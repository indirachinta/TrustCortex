Update specs only. Do not modify C# code yet.

We are starting V5 of TrustCortex.

V5 goal:
Enable real Azure execution readiness using:
- Azure AI Search for enterprise retrieval
- Azure Foundry / Azure OpenAI for grounded answer generation
- TrustCortex governance between retrieval and model generation

Create new spec:
specs/09-v5-real-azure-execution.md

Content:

# V5 - Real Azure Execution

## Goal

V5 prepares TrustCortex to run in full Azure AI mode:

SearchProvider = Azure
AnswerProvider = AzureFoundry

## Correct Runtime Flow

User Question
 ↓
Input Safety / Prompt Validation
 ↓
Azure AI Search Retrieval
 ↓
Retrieved Documents
 ↓
Policy + Governance Filtering
 ↓
Approved Context
 ↓
AzureFoundry Answer Generation
 ↓
Response Validation
 ↓
Audit Logging
 ↓
Governed Response

## Component Responsibility

### Azure AI Search
Responsible for retrieving candidate enterprise documents.

### TrustCortex Governance Layer
Responsible for input safety, role/sensitivity filtering, approved context construction, response validation, and audit logging.

### Azure Foundry / Azure OpenAI
Responsible for answer generation only.
It must receive only approved context from TrustCortex.

## Important Design Rule

Do not use Azure OpenAI "On Your Data" in V5.
TrustCortex must control retrieval and filtering before model generation.

## Required Azure Resources

Already available:
- Azure AI Search

Required for V5:
- Azure Foundry / Azure OpenAI model deployment

Not required:
- Microsoft Purview
- APIM
- Blob Storage
- Azure Functions
- Application Insights

## Configuration

SearchProvider = Azure
AnswerProvider = AzureFoundry

AzureSearch:
- Endpoint
- AdminKey
- IndexName

AzureFoundry:
- Endpoint
- ApiKey
- DeploymentName
- ApiVersion
- MaxTokens
- Temperature

## Acceptance Criteria

- Runtime status shows Azure Search configured.
- Runtime status shows AzureFoundry configured.
- Azure Search index can be initialized.
- Ask endpoint retrieves from Azure AI Search.
- Policy filtering happens before model generation.
- AzureFoundryAnswerService receives approved documents only.
- Final response includes answer, citations, and governance metadata.
- Secrets are not stored in appsettings.json.

Also update:
- specs/02-architecture.md
- specs/08-v4-final-runtime-readiness.md if present
- README.md only enough to mention V5 spec exists

Do not modify code in this pass.