Update or add tests for V5 real Azure execution readiness.

Do not require real Azure calls in tests.

Add tests using fake/mock services.

Required tests:

1. AzureFoundryAnswerService_EmptyApprovedContext_DoesNotCallAzure

Expected:
- returns insufficient approved information message
- citations empty

2. GroundedPromptBuilder_DoesNotIncludeBlockedDocuments

Setup:
Pass only approved documents to prompt builder.
Also create a blocked document in the test but do not pass it to builder.

Expected:
- prompt contains approved document content
- prompt does not contain blocked document content

3. RuntimeStatus_DoesNotExposeSecrets

Expected:
- runtime status does not contain ApiKey
- runtime status shows configured true/false only

4. AskQuestionUseCase_AzureFoundryProviderReceivesOnlyApprovedContext

Use fake answer service to capture approved documents.

Expected:
- Engineer does not pass Restricted documents to answer service
- ComplianceOfficer can pass Restricted documents when retrieved and approved

Run:
dotnet build
dotnet test

Fix only related failures.