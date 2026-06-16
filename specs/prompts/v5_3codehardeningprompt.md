Modify code for V5 real Azure execution readiness.

Goal:
Make AzureFoundryAnswerService safer and demo-ready.

Do not change overall architecture.
Do not add Purview.
Do not add Blob Storage.
Do not add APIM.
Do not add Azure Functions.
Do not remove Mock provider.

Tasks:

1. Review AzureFoundryAnswerService.

Ensure:
- It uses only approved documents passed into GenerateAnswerAsync.
- If approved documents are empty, it returns:
  "I do not have enough approved information to answer that question."
  with no citations.
- It does not call Azure when approved documents are empty.
- It does not expose raw prompt, API key, endpoint, or raw secret values in exceptions.
- It returns citations only from approved documents.
- It has a clear controlled error message if Azure response JSON is unexpected.

2. Review GroundedPromptBuilder.

Ensure prompt includes:

SYSTEM:
You are TrustCortex, a governed enterprise AI assistant.
Answer only using approved context.
If approved context is insufficient, say you do not have enough approved information.
Do not mention blocked or restricted documents that are not present in approved context.
Cite sources from approved context.

USER QUESTION:
{question}

APPROVED CONTEXT:
Document title, source, sensitivity level, content.

3. Review AzureFoundryOptions.

Ensure it has:
- Endpoint
- ApiKey
- DeploymentName
- ApiVersion
- MaxTokens
- Temperature

Default ApiVersion should remain configurable.

4. Review runtime status endpoint.

Ensure:
- It shows provider names.
- It shows configured true/false.
- It does not expose secrets.
- It does not call Azure services.

5. Add comments where useful explaining:
Azure AI Search retrieves candidate documents.
TrustCortex filters approved context.
AzureFoundry generates answer only from approved context.

6. Run:
dotnet build
dotnet test

Fix only related issues.