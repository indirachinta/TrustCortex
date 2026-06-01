Update/add tests for V4.

Goal:
Verify runtime safety and provider behavior.

Add or update tests in TrustCortex.Tests.

Required tests:

1. MockMode_DoesNotRequireAzureFoundryConfiguration

Purpose:
Ensure empty AzureFoundry config does not break mock answer mode.

Expected:
- AskQuestionUseCase works with MockAnswerService.
- No AzureFoundry validation is triggered.

2. GroundedPromptBuilder_UsesOnlyApprovedDocuments

Purpose:
Ensure prompt builder includes only documents passed to it.

Setup:
Pass only approved Public/Internal documents.

Expected:
- Prompt contains approved document title/content.
- Prompt does not contain any blocked/restricted document content.
- Prompt includes instruction to answer only from approved context.

3. AzureFoundryAnswerService_EmptyApprovedContext_ReturnsInsufficientInformation

Purpose:
No model call should be needed when there are no approved documents.

Expected:
- Answer = "I do not have enough approved information to answer that question."
- Citations empty.

4. RuntimeStatus_DoesNotExposeSecrets

If runtime status endpoint/service is testable:
Expected:
- response does not contain ApiKey
- response only shows configured true/false

Run:
dotnet build
dotnet test

Fix only related failures.
Do not introduce unrelated refactoring.