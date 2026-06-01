Improve AzureFoundryAnswerService for V4 robustness.

Do not change the interface.
Do not change AskQuestionUseCase flow.

Update AzureFoundryAnswerService and GroundedPromptBuilder if needed.

Requirements:

1. Prompt structure
GroundedPromptBuilder should produce clear sections:

SYSTEM:
You are TrustCortex, a governed enterprise AI assistant.
Answer only using approved context.
If approved context is insufficient, say you do not have enough approved information.
Do not mention blocked or restricted documents.
Include concise answer and cite sources when possible.

USER QUESTION:
{question}

APPROVED CONTEXT:
[Document 1]
Title:
Source:
Sensitivity:
Content:

2. AzureFoundryAnswerService should:
- Return "I do not have enough approved information..." if approved documents are empty.
- Use only approved documents.
- Return citations based only on approved documents.
- Avoid leaking raw prompt in exception messages.
- Include clear failure message if Azure provider call fails.
- Keep API version configurable if possible.

3. Add AzureFoundryOptions property:
ApiVersion

Default:
2024-10-21

Update appsettings.json:
"AzureFoundry": {
  "Endpoint": "",
  "ApiKey": "",
  "DeploymentName": "",
  "ApiVersion": "2024-10-21",
  "MaxTokens": 600,
  "Temperature": 0.2
}

4. Build endpoint URL using ApiVersion from options.

5. Keep AnswerProvider = Mock as default.

Run:
dotnet build