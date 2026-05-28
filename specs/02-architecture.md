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

## V1 Scope

V1 uses:
- local policy metadata
- mock search service
- mock answer generation
- Azure AI Search Free tier
- Azure Content Safety resource
