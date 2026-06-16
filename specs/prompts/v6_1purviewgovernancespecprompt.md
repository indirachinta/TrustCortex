Create specification document 11-v6-purview-governance.md.

Purpose:
Evolve TrustCortex from sensitivity-based governance to metadata-driven governance inspired by Microsoft Purview.

The specification must define:

1. Business Problem
2. Architecture Changes
3. Classification Model
4. Governance Metadata Model
5. Policy Evaluation Rules
6. Audit Requirements
7. API Response Enhancements
8. Acceptance Criteria

Supported classifications:

Public
Internal
Confidential
HighlyConfidential

Metadata source:

Purview

Policy rules:

Engineer:
- Public
- Internal

ComplianceOfficer:
- Public
- Internal
- Confidential
- HighlyConfidential

The document should align with existing V5 architecture and preserve approved-context-only answer generation.