Update README.md for final V4 demo positioning.

README should clearly explain:

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
Secrets are stored using user-secrets or secure configuration, not appsettings.json.

## Runtime Diagnostics

GET /api/admin/runtime-status

Shows selected providers and configuration readiness without exposing secrets.

Do not include actual keys or secrets.