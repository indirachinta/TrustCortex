# Governance Policy

Governance in TrustCortex is a lifecycle control, not a single step before
retrieval. Input safety runs before retrieval, policy filtering runs after
retrieval, and response validation plus audit logging run after answer
generation.

## Roles

Engineer:
- Allowed: Public, Internal
- Blocked: Confidential, Restricted

Manager:
- Allowed: Public, Internal, Confidential
- Blocked: Restricted

ComplianceOfficer:
- Allowed: All

## Prompt Safety Rules

Prompt safety is the first runtime control. It validates the user's question
before Enterprise Retrieval is allowed to run.

Block prompts containing:
- ignore previous instructions
- reveal system prompt
- bypass policy
- dump all documents
- show restricted data

## Document Policy Rules

Azure AI Search and Mock Search retrieve candidate enterprise documents. Those
documents are not automatically approved for answer generation.

Policy filtering runs after retrieval and evaluates document metadata,
including:
- sensitivity level
- allowed roles
- source metadata

Documents that do not satisfy the user's role and policy constraints are
excluded from the approved context. Only approved context can be used for answer
generation.

## Response Rules

Response validation runs after answer generation and before the governed
response is returned.

Responses must:
- include citations
- include governance metadata
- include blocked document count
- include audit status

## Audit Rules

Audit logging runs after answer generation and response validation. Audit
records should capture the user role, retrieved document count, blocked document
count, approved context count, validation status, and final response status.
