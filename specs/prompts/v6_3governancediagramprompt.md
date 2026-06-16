Update architecture documentation.

Replace sensitivity-based governance with metadata-driven governance.

New flow:

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
Azure Foundry
↓
Response Validation
↓
Audit Logging

Document responsibilities of each stage.