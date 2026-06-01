# V4 - Final Runtime Readiness

## Goal

Prepare TrustCortex for final demo execution using cost-safe provider switching.

## Runtime Modes

### Local Safe Mode

SearchProvider = Mock

AnswerProvider = Mock

### Azure Retrieval Mode

SearchProvider = Azure

AnswerProvider = Mock

### Full Azure AI Mode

SearchProvider = Azure

AnswerProvider = AzureFoundry

## Correct Flow

User Question
  |
  v
Input Safety / Prompt Validation
  |
  v
Azure AI Search or Mock Retrieval
  |
  v
Retrieved Documents
  |
  v
Policy + Governance Filtering
  |
  v
Approved Context
  |
  v
Mock or AzureFoundry Answer Generation
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response

## V4 Design Rules

- Azure AI Search is responsible for retrieval only.
- Azure Foundry / Azure OpenAI is responsible for answer generation only.
- TrustCortex owns governance orchestration.
- Blocked documents must never be sent to the answer generation provider.
- Mock mode must remain the default to control cost.
- Missing AzureFoundry configuration must produce clear diagnostics.
- The final README must explain SearchProvider and AnswerProvider modes clearly.

## Azure Resource Scope

Already available:
- Azure AI Search

Only required for full Azure AI mode:
- Azure Foundry / Azure OpenAI model deployment

Not required for V4 completion:
- Microsoft Purview
- APIM
- Blob Storage
- Application Insights
- Azure Functions

## Acceptance Criteria

- Documentation clearly explains all provider modes.
- README contains final architecture.
- README contains setup/configuration section.
- Runtime diagnostics endpoint exposes selected providers without exposing
  secrets.
- AzureFoundryAnswerService validates configuration safely.
- Mock mode continues to work without Azure cost.
- Full Azure mode is configuration-driven.
