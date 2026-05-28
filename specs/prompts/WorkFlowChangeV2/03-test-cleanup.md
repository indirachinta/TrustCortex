Now update tests and fix build issues.

Run through TrustCortex.Tests and update tests for the corrected governed RAG lifecycle.

Update:
- TrustCortex.Tests/AskQuestionUseCaseTests.cs
- any test setup files if needed

Required tests:

1. Unsafe prompt is blocked before retrieval.
Expected:
- PromptSafetyPassed = false
- DocumentsRetrieved = 0
- DocumentsApproved = 0
- DocumentsBlocked = 0
- AuditLogged = true

2. Engineer safe question retrieves documents then blocks restricted/confidential documents.
Expected:
- PromptSafetyPassed = true
- DocumentsRetrieved > 0
- DocumentsApproved > 0
- DocumentsBlocked > 0
- AuditLogged = true

3. ComplianceOfficer can access restricted documents.
Expected:
- PromptSafetyPassed = true
- DocumentsRetrieved > 0
- DocumentsApproved == DocumentsRetrieved
- DocumentsBlocked = 0
- PolicyCheckPassed = true

After tests are updated:
- run dotnet build
- run dotnet test
- fix all compilation/test failures

Do not introduce unrelated refactoring.