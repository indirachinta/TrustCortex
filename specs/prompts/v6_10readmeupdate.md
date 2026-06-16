Update README.md to represent the final V6 state of TrustCortex.

Goals:

1. Present TrustCortex as a governance-first enterprise AI platform.
2. Reflect completed V6 metadata-driven governance.
3. Clearly explain Azure AI Search, Azure AI Foundry, and Purview-inspired governance responsibilities.
4. Make the README suitable for recruiters, architects, and LinkedIn readers.

Required sections:

# TrustCortex

Short project summary.

# Business Problem

Explain enterprise AI risks:

* sensitive data exposure
* unauthorized retrieval
* hallucinations
* prompt injection
* lack of auditability

# Solution

Explain governance-first RAG architecture.

# Key Capabilities

Include:

* Prompt Safety Validation
* Azure AI Search Retrieval
* Purview-Inspired Metadata Governance
* Role-Based Access Control
* Approved Context Construction
* Azure AI Foundry Answer Generation
* Response Validation
* Audit Logging

# Final Architecture

Use:

User Question
↓
Prompt Validation
↓
Azure AI Search
↓
Purview Metadata Resolution
↓
Governance Evaluation
↓
Approved Context
↓
Azure AI Foundry
↓
Response Validation
↓
Audit Logging
↓
Governed Response

# Azure Services Used

Azure AI Search
Azure AI Foundry
Azure OpenAI Deployments

# Governance Model

Explain classifications:

Public
Internal
Confidential
HighlyConfidential

Explain Engineer vs ComplianceOfficer access rules.

# Runtime Modes

Local Safe Mode
Azure Retrieval Mode
Full Azure AI Mode

# Example Governance Scenario

Engineer requests HighlyConfidential payroll document.

Document retrieved.
Policy denies access.
Azure Foundry receives no restricted content.

# Cost Optimization

Explain Mock Provider strategy.

# Specifications

List all specifications through V6.

# Future Enhancements

Microsoft Purview Integration
Azure AI Content Safety
Application Insights
Evaluation Pipelines

Use professional enterprise architecture language.
