# Architecture Overview

TrustCortex is a governance-first enterprise AI platform for secure retrieval
augmented generation on Azure. The final V6 architecture separates enterprise
retrieval, metadata-driven governance, answer generation, response validation,
and audit logging into explicit runtime stages.

Azure AI Search retrieves candidate documents. TrustCortex resolves
Purview-inspired metadata, evaluates role and classification policy, constructs
approved context, validates responses, and records audit evidence. Azure AI
Foundry generates answers only from approved context supplied by TrustCortex.

# Runtime Flow

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

# Component Responsibilities

## Prompt Validation

Validates the user question before retrieval. Blocks unsafe prompts,
policy-bypass attempts, and prompt injection patterns before enterprise
documents are retrieved.

## Azure AI Search

Retrieves candidate enterprise documents for the user question. Azure AI Search
does not decide whether the user is authorized to use each retrieved document.

## Purview Metadata Resolution

Resolves governance metadata for each retrieved document. Metadata includes
classification, source system, owner department, retention policy, and last
reviewed date. In V6, the metadata source is Purview-inspired sample metadata.

## Governance Evaluation

Applies TrustCortex policy rules to the resolved metadata. Documents are
approved or blocked based on the user's role and each document's governance
classification.

## Approved Context Construction

Builds the context sent to the answer provider using only documents approved by
governance evaluation. Blocked documents are excluded completely.

## Azure AI Foundry

Generates a grounded answer using only the approved context provided by
TrustCortex. Azure AI Foundry does not perform hidden retrieval or policy
filtering.

## Response Validation

Checks that the generated answer is grounded in approved context and does not
introduce unsupported claims.

## Audit Logging

Records the decision trail for prompt safety, retrieval, metadata resolution,
policy evaluation, response validation, and final governance outcome. Audit
records include classification, source system, and policy decision details.

# Governance Principles

- Governance before generation.
- Approved-context-only prompting.
- Metadata-driven authorization.
- Role-based access control.
- Auditability.

# Azure Service Responsibilities

## Azure AI Search

Enterprise retrieval only.

## Azure AI Foundry

Answer generation only.

## TrustCortex

Governance orchestration.

TrustCortex owns prompt validation, metadata resolution, policy evaluation,
approved context construction, response validation, and audit logging.

# Why TrustCortex Does Not Use Azure OpenAI On Your Data

TrustCortex does not use Azure OpenAI On Your Data because retrieval and
governance must remain visible and enforceable before model generation.

Using On Your Data directly would move retrieval inside the model call. That
reduces TrustCortex's ability to inspect candidate documents, resolve governance
metadata, apply role-based policy, block unauthorized context, and audit exactly
which content was approved or denied.

By controlling retrieval and filtering before answer generation, TrustCortex
ensures that Azure AI Foundry receives only approved context. This preserves
governance visibility, policy enforcement, and auditability.

# Example Request Lifecycle

An Engineer requests a HighlyConfidential payroll report.

1. Prompt Validation accepts the question as safe.
2. Azure AI Search retrieves the payroll report as a candidate document.
3. Purview Metadata Resolution identifies the document classification as
   HighlyConfidential.
4. Governance Evaluation compares the classification against the Engineer role.
5. The policy denies access because Engineers can access only Public and
   Internal documents.
6. Approved Context Construction excludes the payroll report.
7. Azure AI Foundry receives no restricted payroll content.
8. Response Validation and Audit Logging record the governed denial and the
   metadata-driven policy decision.
