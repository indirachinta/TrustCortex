Now modify code for Day 3 / V3.

Goal:
Add provider-based answer generation so TrustCortex can switch between MockAnswerService and AzureFoundryAnswerService using configuration.

Do not add Azure resources.
Do not remove MockAnswerService.
Do not change the corrected governed RAG flow.
Do not use Azure OpenAI "On Your Data" integration.
Do not let the model retrieve documents directly.
The model must receive only approved context from TrustCortex.

Current flow must remain:
Input Safety
 ↓
Search
 ↓
Policy Filtering
 ↓
Approved Context
 ↓
Answer Generation
 ↓
Response Validation
 ↓
Audit

Required code changes:

1. Add appsettings config in TrustCortex.Api/appsettings.json:

"AnswerProvider": "Mock",
"AzureFoundry": {
  "Endpoint": "",
  "ApiKey": "",
  "DeploymentName": "",
  "MaxTokens": 600,
  "Temperature": 0.2
}

Do not put real secrets in appsettings.

2. Create options class:

TrustCortex.Infrastructure/Answers/AzureFoundryOptions.cs

Properties:
- Endpoint
- ApiKey
- DeploymentName
- MaxTokens
- Temperature

3. Create prompt builder:

TrustCortex.Infrastructure/Answers/GroundedPromptBuilder.cs

Purpose:
Convert approved documents into a grounded model prompt.

Rules:
- Include system instruction:
  "You are TrustCortex, a governed enterprise AI assistant. Answer only from approved context. If context is insufficient, say you do not have enough approved information."
- Include user question.
- Include approved document snippets with title, source, sensitivity level, and content.
- Do not include blocked documents.
- Keep prompt readable and deterministic.

4. Create AzureFoundryAnswerService:

TrustCortex.Infrastructure/Answers/AzureFoundryAnswerService.cs

It should implement IAnswerService.

Behavior:
- Read AzureFoundryOptions through IOptions<AzureFoundryOptions>.
- Validate Endpoint, ApiKey, and DeploymentName.
- Use approved documents only.
- Build prompt using GroundedPromptBuilder.
- Call Azure Foundry / Azure OpenAI chat completion endpoint.
- Return AnswerDraft with:
  - generated answer
  - citations from approved documents

Important:
If SDK package choice is unclear, use a clean wrapper structure and leave implementation ready for Azure OpenAI compatible endpoint.
Prefer current Microsoft-recommended SDK approach if available.
Keep code compile-safe.

If SDK integration creates package/version issues, implement AzureFoundryAnswerService with HttpClient and clear TODO comments for the exact API call, but keep the provider abstraction compile-safe.

5. Update TrustCortex.Infrastructure/DependencyInjection.cs

Current:
IAnswerService always maps to MockAnswerService.

Change to:
var answerProvider = configuration["AnswerProvider"] ?? "Mock";

if answerProvider == "AzureFoundry":
- configure AzureFoundryOptions
- register GroundedPromptBuilder
- register IAnswerService as AzureFoundryAnswerService

else:
- register IAnswerService as MockAnswerService

6. Keep SearchProvider logic unchanged.

7. Ensure AskQuestionUseCase remains unchanged unless required.

The important behavior:
AskQuestionUseCase already passes approvedDocuments to IAnswerService.
That means AzureFoundryAnswerService must receive only approved context.

8. Add clear logging/exception message if AzureFoundry provider is selected but config is missing:
"AzureFoundry answer provider is selected, but Endpoint/ApiKey/DeploymentName is missing."

9. Build must pass.