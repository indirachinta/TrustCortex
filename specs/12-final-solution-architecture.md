# Final Solution Architecture

## Business Goal

Protect enterprise knowledge while enabling AI-powered search and reasoning.

TrustCortex demonstrates a governance-first architecture for enterprise AI. It
allows users to ask natural-language questions over internal knowledge while
ensuring that retrieved content is validated, authorized, grounded, and audited
before any answer is returned.

## Architecture Diagram

```text
User
  |
  v
TrustCortex API
  |
  v
Prompt Validation
  |
  v
Azure AI Search
  |
  v
Purview Metadata Resolution
  |
  v
Governance Policy Engine
  |
  v
Approved Context Builder
  |
  v
Azure AI Foundry
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response
```

## Component Descriptions

### User

Submits a question and role context to TrustCortex.

### TrustCortex API

Receives requests, coordinates the runtime pipeline, and returns governed
responses with citations and governance metadata.

### Prompt Validation

Blocks unsafe prompts, prompt injection attempts, and policy-bypass language
before enterprise retrieval begins.

### Azure AI Search

Retrieves candidate enterprise documents relevant to the user question.

### Purview Metadata Resolution

Resolves document governance metadata such as classification, source system,
owner department, retention policy, and last reviewed date.

### Governance Policy Engine

Evaluates the user's role against document classification metadata and decides
which retrieved documents are approved or blocked.

### Approved Context Builder

Constructs model context only from approved documents. Blocked documents are not
included in the prompt sent to the answer provider.

### Azure AI Foundry

Generates grounded answers using only the approved context supplied by
TrustCortex.

### Response Validation

Checks whether the answer is grounded in approved context and suitable to return
to the user.

### Audit Logging

Records prompt safety, retrieval counts, metadata classification, policy
decisions, response validation, and final governance outcomes.

## Security Boundaries

- User input is validated before retrieval.
- Azure AI Search retrieves candidate documents but does not authorize access.
- Governance metadata is resolved before policy evaluation.
- Policy evaluation occurs before answer generation.
- Approved context is the only content boundary crossed into Azure AI Foundry.
- Secrets, endpoints, API keys, and blocked document content are not exposed in
  responses or audit records.

## Governance Decision Points

- Prompt safety decision before retrieval.
- Metadata resolution decision for each retrieved document.
- Classification policy decision for each document.
- Approved context construction decision before model generation.
- Response grounding decision before returning the answer.
- Audit logging decision after validation.

## Azure Service Responsibilities

### Azure AI Search

Enterprise retrieval only. It returns candidate documents for governance
evaluation.

### Azure AI Foundry

Answer generation only. It receives only approved context and does not perform
hidden retrieval or authorization.

### Azure OpenAI Deployments

Provide the model deployment used by Azure AI Foundry answer generation.

### Purview-Inspired Metadata

Provides the governance metadata contract used for classification-based policy
evaluation. The implemented V6 provider is mock/sample-backed and designed to
support future Microsoft Purview integration.

## Operational Flow

1. A user submits a question and role.
2. TrustCortex validates the prompt for safety.
3. Azure AI Search retrieves candidate documents.
4. TrustCortex resolves Purview-inspired metadata for each candidate document.
5. The governance policy engine evaluates role access against classifications.
6. Approved context is built from authorized documents only.
7. Azure AI Foundry generates an answer from approved context.
8. TrustCortex validates response grounding.
9. TrustCortex records audit evidence for the full decision trail.
10. The API returns a governed response with citations and governance metadata.

## Enterprise Benefits

- Reduces risk of sensitive data exposure.
- Enforces authorization before model generation.
- Improves transparency into retrieval and policy decisions.
- Supports auditability for enterprise AI workflows.
- Separates retrieval, governance, and generation responsibilities.
- Provides a practical path from mock-safe local demos to Azure execution.

## Technical Benefits

- Provider-based architecture supports Mock, Azure Retrieval, and Full Azure AI
  modes.
- Approved-context-only prompting prevents blocked content from reaching the
  model.
- Metadata-driven policy evaluation is extensible to real Microsoft Purview
  integration.
- Runtime diagnostics can show readiness without exposing secrets.
- Unit tests validate prompt safety, policy filtering, approved-context
  handoff, runtime status safety, and audit metadata.

## Lessons Learned

- Enterprise AI governance must happen before answer generation, not after.
- Retrieval quality is not enough; retrieved documents still require policy
  evaluation.
- Model providers should not be responsible for hidden enterprise authorization.
- Approved-context-only prompting creates a clear security boundary.
- Audit logs are most useful when they capture metadata and policy decisions,
  not just final allow or deny outcomes.
- Mock providers are valuable for cost control, repeatable demos, and safe test
  coverage.
