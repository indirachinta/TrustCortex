# Governance Metadata Contract

This contract defines the document governance metadata TrustCortex uses for
metadata-driven policy evaluation.

## Fields

- DocumentId
- Classification
- SourceSystem
- OwnerDepartment
- RetentionPolicy
- LastReviewedDate

## Classification Values

- Public
- Internal
- Confidential
- HighlyConfidential

## SourceSystem Value

- Purview

## JSON Examples

### Public Document

```json
{
  "DocumentId": "doc-public-001",
  "Classification": "Public",
  "SourceSystem": "Purview",
  "OwnerDepartment": "Engineering",
  "RetentionPolicy": "Standard-3Years",
  "LastReviewedDate": "2026-06-01"
}
```

### Internal Document

```json
{
  "DocumentId": "doc-internal-001",
  "Classification": "Internal",
  "SourceSystem": "Purview",
  "OwnerDepartment": "Security",
  "RetentionPolicy": "Standard-5Years",
  "LastReviewedDate": "2026-05-15"
}
```

### Confidential Document

```json
{
  "DocumentId": "doc-confidential-001",
  "Classification": "Confidential",
  "SourceSystem": "Purview",
  "OwnerDepartment": "Legal",
  "RetentionPolicy": "Legal-7Years",
  "LastReviewedDate": "2026-04-20"
}
```

### Highly Confidential Document

```json
{
  "DocumentId": "doc-highly-confidential-001",
  "Classification": "HighlyConfidential",
  "SourceSystem": "Purview",
  "OwnerDepartment": "Finance",
  "RetentionPolicy": "Restricted-7Years",
  "LastReviewedDate": "2026-03-10"
}
```
