# TrustCortex

TrustCortex is a governed RAG prototype for enterprise question answering. It
combines prompt safety, enterprise retrieval, document-level policy filtering,
answer generation, response validation, and audit logging.

## Governed RAG Lifecycle

Governance is not a single pre-retrieval step. TrustCortex applies controls at
the points where they have the correct context.

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

## Runtime Responsibilities

- Input safety happens before retrieval and blocks unsafe prompts or attempts to
  bypass policy.
- Azure AI Search or Mock Search retrieves candidate enterprise documents.
- Policy and governance filtering happens after retrieval using document
  metadata such as sensitivity level and allowed roles.
- Only approved context is sent to answer generation.
- Response validation happens after answer generation.
- Audit logging records the governed decision trail before the final response is
  returned.

## V3 - Foundry-Ready Answer Generation

V3 introduces a provider-based answer generation layer.

AnswerProvider modes:
- Mock
- AzureFoundry

The default is Mock for cost-controlled local execution.

TrustCortex does not send all retrieved documents to the model. It sends only
approved context after role and sensitivity filtering.

Architecture:
- Azure AI Search retrieves candidate documents.
- TrustCortex policy engine filters documents.
- Azure Foundry / Azure OpenAI generates answers only from approved context.

TrustCortex does not use Azure OpenAI "On Your Data" directly in V3. Retrieval
must stay inside the TrustCortex flow so the Application layer can apply
document-level policy and governance before answer generation.

Sample config:

```json
"AnswerProvider": "Mock",
"AzureFoundry": {
  "Endpoint": "",
  "ApiKey": "",
  "DeploymentName": "",
  "MaxTokens": 600,
  "Temperature": 0.2
}
```

Real Azure Foundry/OpenAI resource creation is planned for Day 4 to control
cost.

## Documentation

- [Architecture](specs/02-architecture.md)
- [Governance Policy](specs/03-governance-policy.md)
- [V2 Azure AI Search Integration](specs/05-v2-azure-search.md)
- [V2 Governed RAG Flow Correction](specs/06-v2-correction-governed-rag-flow.md)
- [V3 Azure Foundry Answer Generation Layer](specs/07-v3-foundry-answer-generation.md)
