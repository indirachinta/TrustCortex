Update architecture documentation to reflect the final V6 architecture.

Requirements:

1. Remove historical V1/V2/V3/V4/V5 sections that describe intermediate states.
2. Present only the final architecture.
3. Explain component responsibilities clearly.

Sections:

# Architecture Overview

# Runtime Flow

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

# Component Responsibilities

Prompt Validation
Azure AI Search
Purview Metadata Resolution
Governance Evaluation
Approved Context Construction
Azure AI Foundry
Response Validation
Audit Logging

# Governance Principles

Governance before generation.
Approved-context-only prompting.
Metadata-driven authorization.
Role-based access control.
Auditability.

# Azure Service Responsibilities

Azure AI Search:
Enterprise retrieval only.

Azure AI Foundry:
Answer generation only.

TrustCortex:
Governance orchestration.

# Why TrustCortex Does Not Use Azure OpenAI On Your Data

Explain governance visibility and policy enforcement benefits.

# Example Request Lifecycle

Engineer requests HighlyConfidential payroll report.

Show retrieval success.
Show governance denial.
Show no restricted content sent to model.
