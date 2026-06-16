Refactor policy engine.

Current behavior:

Uses document sensitivity.

New behavior:

Uses GovernanceMetadata.Classification.

Rules:

Engineer:
- Public
- Internal

ComplianceOfficer:
- Public
- Internal
- Confidential
- HighlyConfidential

Preserve existing policy evaluation flow.

Update unit tests.