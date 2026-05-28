# V2 - Azure AI Search Integration

## Goal

Integrate Azure AI Search as the real retrieval provider for TrustCortex while keeping mock search fallback for cost-controlled local execution.

Azure AI Search participates in the Enterprise Retrieval stage of the governed
RAG lifecycle. It retrieves candidate documents only; it does not replace policy
filtering or response governance.

## Scope

- Create Azure resource group.
- Create Azure AI Search Free tier.
- Create search index.
- Add sample enterprise documents.
- Implement AzureAiSearchService.
- Use configuration flag to switch between Mock and Azure search.
- Return candidate documents with metadata needed for downstream policy
  filtering.

## Correct Runtime Position

Azure AI Search and Mock Search run after Input Safety / Prompt Validation and
before Policy + Governance Filtering.

Flow excerpt:

Input Safety / Prompt Validation
  |
  v
Enterprise Retrieval with Azure AI Search or Mock Search
  |
  v
Retrieved Documents
  |
  v
Policy + Governance Filtering
  |
  v
Approved Context

Search results are candidate documents. Policy filtering must run after
retrieval using document metadata such as sensitivity level and allowed roles.
Only approved context is sent to answer generation.

## Out of Scope

- Azure OpenAI answer generation
- Microsoft Purview integration
- API Management
- Azure Functions
- App Insights
- Production authentication

## Acceptance Criteria

- Application still builds and tests pass.
- Mock search continues to work.
- Azure AI Search can be enabled through configuration.
- Search results return title, content, source, sensitivity level, and allowed roles.
- Policy filtering still controls which documents are visible to each role.
- Policy filtering happens after Azure AI Search or Mock Search returns
  candidate documents.
