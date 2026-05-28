Update documentation only. Do not modify C# code yet.

Goal: Align TrustCortex specs and README with the corrected governed RAG lifecycle.

Correct runtime flow:

User Question
 ↓
Input Safety / Prompt Validation
 ↓
Enterprise Retrieval
 ↓
Retrieved Documents
 ↓
Policy + Governance Filtering
 ↓
Approved Context
 ↓
Answer Generation
 ↓
Response Validation
 ↓
Audit Logging
 ↓
Governed Response

Update:
- specs/02-architecture.md
- specs/03-governance-policy.md
- specs/05-v2-azure-search.md if relevant
- README.md

Create:
- specs/06-v2-correction-governed-rag-flow.md

Make documentation clearly explain:
1. Governance is not a single pre-retrieval step.
2. Input safety happens before retrieval.
3. Azure AI Search / Mock Search retrieves candidate documents.
4. Policy filtering happens after retrieval using document metadata.
5. Only approved context goes to answer generation.
6. Response validation and audit logging happen after answer generation.

Do not change code in this pass.