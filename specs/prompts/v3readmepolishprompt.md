Update README.md only.

Add a clear V3 section:

## V3 - Foundry-Ready Answer Generation

V3 introduces a provider-based answer generation layer.

AnswerProvider modes:
- Mock
- AzureFoundry

The default is Mock for cost-controlled local execution.

TrustCortex does not send all retrieved documents to the model. It sends only approved context after role and sensitivity filtering.

Architecture:
Azure AI Search retrieves candidate documents.
TrustCortex policy engine filters documents.
Azure Foundry / Azure OpenAI generates answers only from approved context.

Add sample config:

"AnswerProvider": "Mock",
"AzureFoundry": {
  "Endpoint": "",
  "ApiKey": "",
  "DeploymentName": "",
  "MaxTokens": 600,
  "Temperature": 0.2
}

Add note:
Real Azure Foundry/OpenAI resource creation is planned for Day 4 to control cost.