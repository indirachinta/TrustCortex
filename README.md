# TrustCortex

TrustCortex is a governance-first enterprise AI platform for secure retrieval
augmented generation on Azure. It demonstrates how organizations can use Azure
AI Search, Azure AI Foundry, Azure OpenAI deployments, and Purview-inspired
metadata governance to generate grounded answers without bypassing enterprise
access policy.

# Business Problem

Enterprises want AI over internal knowledge, but ungoverned enterprise AI
introduces serious risk:

- sensitive data exposure
- unauthorized retrieval
- hallucinations
- prompt injection
- lack of auditability

The core challenge is not simply connecting a model to enterprise data. The
challenge is proving that only approved information reaches the model, that
answers are grounded, and that every governance decision can be audited.

# Solution

TrustCortex implements a governance-first RAG architecture. Retrieval,
classification, policy evaluation, answer generation, validation, and audit
logging are explicit stages in the runtime flow.

Azure AI Search retrieves candidate documents. TrustCortex resolves
Purview-inspired governance metadata, applies role and classification policy,
constructs approved context, and sends only that approved context to Azure AI
Foundry / Azure OpenAI for answer generation.

This preserves enterprise control between retrieval and generation. The model
does not perform hidden retrieval, and blocked content is never included in the
prompt sent to the answer provider.

# Key Capabilities

- Prompt Safety Validation
- Azure AI Search Retrieval
- Purview-Inspired Metadata Governance
- Role-Based Access Control
- Approved Context Construction
- Azure AI Foundry Answer Generation
- Response Validation
- Audit Logging

# Final Architecture

User Question
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
Governance Evaluation
  |
  v
Approved Context
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

Stage responsibilities:

- Prompt Validation blocks unsafe prompts and policy-bypass attempts before
  retrieval.
- Azure AI Search retrieves candidate enterprise documents.
- Purview Metadata Resolution resolves governance metadata such as
  classification, source system, owner department, retention policy, and review
  date.
- Governance Evaluation applies role-based policy to metadata classifications.
- Approved Context Construction builds model context only from documents that
  passed governance.
- Azure AI Foundry generates the answer using only approved context.
- Response Validation checks that the answer remains grounded in approved
  context.
- Audit Logging records prompt safety, retrieval, metadata, policy, response,
  and decision outcomes.

# Azure Services Used

Implemented:

- Azure AI Search
- Azure AI Foundry
- Azure OpenAI Deployments

Governance Simulation:

- Purview-inspired metadata governance model

Planned Integration:

- Microsoft Purview

# Governance Model

TrustCortex V6 uses metadata-driven governance inspired by Microsoft Purview.
Documents are evaluated using governance metadata, not only local sensitivity
fields.

TrustCortex V6 currently uses a Purview-inspired governance model. Governance
metadata is resolved through local metadata providers. The architecture
simulates how Microsoft Purview classifications would be consumed by governance
policy evaluation.

A dedicated metadata resolution layer exists so Microsoft Purview can be
integrated later without changing retrieval, governance, or answer generation
workflows.

Supported classifications:

- Public
- Internal
- Confidential
- HighlyConfidential

Role access rules:

- Engineer can access Public and Internal documents.
- ComplianceOfficer can access Public, Internal, Confidential, and
  HighlyConfidential documents.

If metadata is missing or invalid, TrustCortex fails closed and excludes the
document from approved context.

# Runtime Modes

## Local Safe Mode

SearchProvider = Mock
AnswerProvider = Mock

Runs without Azure cost and is the default mode for local development.

## Azure Retrieval Mode

SearchProvider = Azure
AnswerProvider = Mock

Uses Azure AI Search for retrieval while keeping answer generation local and
cost-controlled.

## Full Azure AI Mode

SearchProvider = Azure
AnswerProvider = AzureFoundry

Uses Azure AI Search for retrieval and Azure AI Foundry / Azure OpenAI for
grounded answer generation after TrustCortex governance filtering.

# Example Governance Scenario

An Engineer asks for a HighlyConfidential payroll incident document.

Azure AI Search may retrieve the document as a candidate. TrustCortex resolves
Purview-inspired metadata and evaluates the document classification against the
Engineer role. Because Engineers can access only Public and Internal documents,
the payroll document is denied.

Azure AI Foundry receives no restricted payroll content. The response includes
governance metadata showing the classification source, evaluated
classification, policy decision, blocked count, and audit status.

# Cost Optimization

TrustCortex uses a mock provider strategy to control Azure spend during
development and demos.

- Mock Search and Mock Answer providers support local safe mode.
- Azure AI Search can be enabled independently from Azure Foundry.
- Azure Foundry is used only when full Azure AI mode is explicitly configured.
- Secrets are stored in user-secrets or secure configuration, not
  `appsettings.json`.

# Specifications

- [Business Use Case](specs/01-business-usecase.md)
- [Architecture](specs/02-architecture.md)
- [Governance Policy](specs/03-governance-policy.md)
- [V1 Acceptance](specs/04-v1-acceptance.md)
- [V2 Azure AI Search Integration](specs/05-v2-azure-search.md)
- [V2 Governed RAG Flow Correction](specs/06-v2-correction-governed-rag-flow.md)
- [V3 Azure Foundry Answer Generation Layer](specs/07-v3-foundry-answer-generation.md)
- [V4 Final Runtime Readiness](specs/08-v4-final-runtime-readiness.md)
- [V5 Real Azure Execution](specs/09-v5-real-azure-execution.md)
- [V6 Purview Governance](specs/11-v6-purview-governance.md)
- [Governance Metadata Contract](governance-metadata-contract.md)
- [V6 Demonstration Scenarios](V6-DEMO-SCENARIOS.md)
- [V5 Demo Guide](DEMO.md)

# Current Scope

TrustCortex demonstrates:

- Azure AI Search enterprise retrieval
- Azure AI Foundry answer generation
- Metadata-driven governance
- Role-based access control
- Approved-context-only prompting
- Response validation
- Audit logging

TrustCortex currently simulates Microsoft Purview metadata using local
governance providers. Direct Microsoft Purview integration is intentionally
left as a future enhancement to keep the project cost-effective and focused on
governance architecture patterns.

# Future Enhancements

- Microsoft Purview Integration (replace simulated metadata provider with live
  Purview classifications)
- Azure AI Content Safety
- Application Insights Telemetry
- Azure AI Evaluation Pipelines
