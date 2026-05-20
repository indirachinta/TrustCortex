# Governance Policy

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

Block prompts containing:
- ignore previous instructions
- reveal system prompt
- bypass policy
- dump all documents
- show restricted data

## Response Rules

Responses must:
- include citations
- include governance metadata
- include blocked document count
- include audit status