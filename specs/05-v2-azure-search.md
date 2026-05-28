# V2 - Azure AI Search Integration

## Goal

Integrate Azure AI Search as the real retrieval provider for TrustCortex while keeping mock search fallback for cost-controlled local execution.

## Scope

- Create Azure resource group.
- Create Azure AI Search Free tier.
- Create search index.
- Add sample enterprise documents.
- Implement AzureAiSearchService.
- Use configuration flag to switch between Mock and Azure search.

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