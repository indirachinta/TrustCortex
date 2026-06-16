# V6 - Purview Governance

## Business Problem

V5 proves that TrustCortex can run in full Azure AI mode with Azure AI Search
retrieval, TrustCortex governance filtering, and AzureFoundry answer generation.

V6 evolves governance from simple sensitivity strings toward metadata-driven
policy decisions inspired by Microsoft Purview. Enterprise documents should be
governed by classification metadata from a trusted catalog source instead of
only local document fields.

The goal is to preserve the V5 approved-context-only answer generation model
while improving governance fidelity, auditability, and future readiness for
enterprise compliance workflows.

## Architecture Changes

V6 keeps the V5 runtime flow:

User Question
  |
  v
Input Safety / Prompt Validation
  |
  v
Azure AI Search Retrieval
  |
  v
Candidate Documents
  |
  v
Purview-Inspired Metadata Resolution
  |
  v
Policy + Governance Filtering
  |
  v
Approved Context
  |
  v
AzureFoundry Answer Generation
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response

Azure AI Search remains responsible for retrieving candidate enterprise
documents. TrustCortex remains responsible for resolving governance metadata,
evaluating policy, constructing approved context, validating responses, and
auditing decisions. AzureFoundry remains responsible for answer generation only
and must receive only approved context.

V6 introduces a governance metadata layer between retrieval and policy
evaluation. The metadata source is Purview.

## Classification Model

Supported classifications:

- Public
- Internal
- Confidential
- HighlyConfidential

Classification values represent the governance sensitivity assigned to a
document by the metadata source. TrustCortex policy decisions must use these
classification values when deciding whether a retrieved document can be included
in approved context.

## Governance Metadata Model

Each retrieved document should be associated with governance metadata before
policy evaluation.

Required metadata:

- DocumentId
- Source
- Classification
- MetadataSource
- AllowedRoles

Metadata source:

- Purview

`MetadataSource` identifies where the governance metadata came from. In V6, the
expected value is `Purview`.

`Classification` must use one of the supported classification values. If
classification metadata is missing, invalid, or cannot be resolved, TrustCortex
must fail closed and exclude the document from approved context.

## Policy Evaluation Rules

Policy evaluation must happen after retrieval and metadata resolution, and
before answer generation.

Engineer can access:

- Public
- Internal

ComplianceOfficer can access:

- Public
- Internal
- Confidential
- HighlyConfidential

Documents outside the user's allowed classifications must be blocked and must
not be included in approved context.

Blocked documents must not be sent to AzureFoundry. The model must not be told
about blocked or restricted documents that are absent from approved context.

## Audit Requirements

Audit logging must capture enough information to explain the governance
decision without exposing secrets or unauthorized document content.

Audit events should include:

- User role
- Prompt safety result
- Documents retrieved
- Documents approved
- Documents blocked
- Classification values evaluated
- Metadata source
- Blocked reason when applicable
- Response validation result
- Audit timestamp

Audit logs must not include API keys, endpoints, raw prompts containing secrets,
or blocked document content.

## API Response Enhancements

The Ask response should continue to include answer, citations, and governance
metadata.

V6 governance metadata should include:

- DocumentsRetrieved
- DocumentsApproved
- DocumentsBlocked
- PolicyCheckPassed
- PromptSafetyPassed
- ResponseGrounded
- AuditLogged
- MetadataSource
- ApprovedClassifications
- BlockedClassifications
- BlockedReason

Citations must come only from approved documents.

## Acceptance Criteria

- TrustCortex preserves the V5 approved-context-only answer generation flow.
- Azure AI Search retrieves candidate documents only.
- Purview is the metadata source for governance classification.
- Supported classifications are Public, Internal, Confidential, and
  HighlyConfidential.
- Engineer receives only Public and Internal approved context.
- ComplianceOfficer can receive Public, Internal, Confidential, and
  HighlyConfidential approved context.
- Documents with missing or invalid classification metadata are blocked.
- Policy filtering happens before AzureFoundry answer generation.
- AzureFoundry receives approved documents only.
- Citations are generated only from approved documents.
- Audit logs include metadata source and classification decision details.
- API response governance metadata reports approved and blocked classification
  outcomes.
- Secrets, endpoints, API keys, and blocked document content are not exposed in
  responses or audit logs.
