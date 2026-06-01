# TrustCortex Architecture

TrustCortex implements a governed RAG lifecycle. Governance is applied across
multiple runtime stages; it is not a single pre-retrieval gate.

## Runtime Flow

User Question
  |
  v
Input Safety / Prompt Validation
  |
  v
Enterprise Retrieval
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
Answer Generation
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response

## Component Responsibilities

- React UI / Swagger sends the user question to the ASP.NET Core API.
- The API validates the prompt before retrieval to block unsafe or policy-bypass
  attempts.
- Enterprise retrieval uses Azure AI Search or Mock Search to return candidate
  documents.
- Policy and governance filtering runs after retrieval, using document metadata
  such as sensitivity level and allowed roles.
- Only approved context is passed to answer generation.
- Response validation checks the generated answer before it is returned.
- Audit logging records the decision trail after answer generation and
  validation.

## V3 Answer Generation Architecture

V3 introduces a provider-based answer generation layer.

- Azure AI Search is the retrieval layer. It returns candidate enterprise
  documents for the user's question.
- TrustCortex governance sits between retrieval and answer generation. It
  filters retrieved documents using policy metadata and produces approved
  context.
- Foundry/OpenAI is the reasoning layer. It should only receive the user's
  question plus approved context that already passed TrustCortex governance.
- MockAnswerService remains the default answer provider for cost safety.

TrustCortex should not use Azure OpenAI "On Your Data" directly in V3 because
that would let the model service perform retrieval internally. V3 keeps
retrieval in the Application flow so document-level governance can run before
any content is sent to the answer generation provider.

Provider selection is configuration-driven:

- AnswerProvider = Mock uses MockAnswerService.
- AnswerProvider = AzureFoundry uses AzureFoundryAnswerService.

## V4 Runtime Readiness Architecture

V4 prepares TrustCortex for final demo execution using explicit provider modes.

Runtime modes:

- Local Safe Mode: SearchProvider = Mock, AnswerProvider = Mock.
- Azure Retrieval Mode: SearchProvider = Azure, AnswerProvider = Mock.
- Full Azure AI Mode: SearchProvider = Azure, AnswerProvider = AzureFoundry.

Design responsibilities:

- Azure AI Search is responsible for retrieval only.
- Azure Foundry / Azure OpenAI is responsible for answer generation only.
- TrustCortex owns governance orchestration between retrieval and answer
  generation.
- Blocked documents must never be sent to the answer generation provider.
- Mock provider defaults must remain available for cost-controlled local
  execution.
- Runtime diagnostics should expose selected providers without exposing
  Endpoint, ApiKey, DeploymentName, or other secrets.

Final demo flow:

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

## V1 Scope

V1 uses:
- local policy metadata
- mock search service
- mock answer generation
- Azure AI Search Free tier
- Azure Content Safety resource
