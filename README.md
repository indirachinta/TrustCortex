# TrustCortex

Policy-aware governed enterprise AI platform on Azure.

## Problem

Enterprises want AI over internal knowledge, but ungoverned AI creates risks:

- sensitive data leakage
- hallucinated policy answers
- prompt injection
- unauthorized retrieval
- lack of auditability

## Solution

TrustCortex applies governance across the AI lifecycle:

- input safety
- enterprise retrieval
- document-level policy filtering
- approved-context-only answer generation
- response validation
- audit logging

## Architecture

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

## Provider Modes

### Local Safe Mode

SearchProvider = Mock

AnswerProvider = Mock

### Azure Retrieval Mode

SearchProvider = Azure

AnswerProvider = Mock

### Full Azure AI Mode

SearchProvider = Azure

AnswerProvider = AzureFoundry

## Azure Services

Used:
- Azure AI Search

Foundry-ready:
- Azure Foundry / Azure OpenAI model deployment

Planned governance extension:
- Microsoft Purview for classification metadata
- Azure AI Content Safety for advanced prompt safety
- App Insights for audit telemetry

## Cost Safety

Mock mode is the default.

Azure services are enabled only through configuration.

Secrets are stored using user-secrets or secure configuration, not
appsettings.json.

## Runtime Diagnostics

GET /api/admin/runtime-status

Shows selected providers and configuration readiness without exposing secrets.

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

```powershell
dotnet user-secrets set "SearchProvider" "Azure"
dotnet user-secrets set "AnswerProvider" "AzureFoundry"
dotnet user-secrets set "AzureSearch:Endpoint" "<endpoint>"
dotnet user-secrets set "AzureSearch:AdminKey" "<key>"
dotnet user-secrets set "AzureFoundry:Endpoint" "<endpoint>"
dotnet user-secrets set "AzureFoundry:ApiKey" "<key>"
dotnet user-secrets set "AzureFoundry:DeploymentName" "<deployment>"
```

## Demo

See DEMO.md.

## Documentation

- [Architecture](specs/02-architecture.md)
- [Governance Policy](specs/03-governance-policy.md)
- [V2 Azure AI Search Integration](specs/05-v2-azure-search.md)
- [V2 Governed RAG Flow Correction](specs/06-v2-correction-governed-rag-flow.md)
- [V3 Azure Foundry Answer Generation Layer](specs/07-v3-foundry-answer-generation.md)
- [V4 Final Runtime Readiness](specs/08-v4-final-runtime-readiness.md)
- [V5 Real Azure Execution](specs/09-v5-real-azure-execution.md)
