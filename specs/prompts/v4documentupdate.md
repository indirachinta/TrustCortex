Update documentation/specs only. Do not modify C# code yet.

We are starting V4 of TrustCortex.

Current V3 state:
- Azure AI Search provider exists.
- Mock Search provider exists.
- Mock Answer provider exists.
- AzureFoundryAnswerService exists behind IAnswerService.
- AnswerProvider defaults to Mock.
- Governed RAG flow is corrected.

V4 goal:
Make TrustCortex demo-ready as a governed enterprise AI platform with Azure AI Search retrieval, approved-context-only answer generation, and Foundry-ready model integration.

Create a new spec:

specs/08-v4-final-runtime-readiness.md

Content should include:

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
 ↓
Input Safety / Prompt Validation
 ↓
Azure AI Search or Mock Retrieval
 ↓
Retrieved Documents
 ↓
Policy + Governance Filtering
 ↓
Approved Context
 ↓
Mock or AzureFoundry Answer Generation
 ↓
Response Validation
 ↓
Audit Logging
 ↓
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
- Runtime diagnostics endpoint exposes selected providers without exposing secrets.
- AzureFoundryAnswerService validates configuration safely.
- Mock mode continues to work without Azure cost.
- Full Azure mode is configuration-driven.

Also update:
- specs/02-architecture.md
- specs/07-v3-foundry-answer-generation.md
- README.md

Do not change code in this pass.