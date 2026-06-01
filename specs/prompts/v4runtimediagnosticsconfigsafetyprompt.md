Now modify code for V4 runtime diagnostics and configuration safety.

Goal:
Add a diagnostics endpoint that shows provider status without exposing secrets.

Add endpoint:

GET /api/admin/runtime-status

Expected response shape:

{
  "searchProvider": "Mock",
  "answerProvider": "Mock",
  "azureSearch": {
    "configured": false,
    "indexName": "trustcortex-documents"
  },
  "azureFoundry": {
    "configured": false,
    "deploymentName": ""
  },
  "costSafety": {
    "mockSearchDefault": true,
    "mockAnswerDefault": true
  }
}

Rules:
- Do not expose API keys.
- Do not expose full secret values.
- Do not call Azure services from this endpoint.
- Only inspect configuration.
- Keep existing /api/admin/search/initialize endpoint unchanged.

Implementation guidance:
- Add Application DTO if needed:
  RuntimeStatusDto
  ProviderStatusDto
  CostSafetyStatusDto

- Add service/interface if useful:
  IRuntimeStatusService

- Infrastructure or API may read IConfiguration directly if simpler.
  For this POC, prefer simple implementation in AdminController if it keeps code clean.

Also improve AzureFoundryAnswerService configuration validation:
- Keep clear exception message:
  "AzureFoundry answer provider is selected, but Endpoint/ApiKey/DeploymentName is missing."
- Make sure it does not validate AzureFoundry settings unless AnswerProvider = AzureFoundry.
- Mock mode should never fail because AzureFoundry settings are empty.

Do not add Azure OpenAI resources.
Do not add Purview.
Do not add Blob Storage.
Keep build passing.