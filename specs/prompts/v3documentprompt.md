Update documentation only. Do not modify C# code yet.

We are starting Day 3 / V3 of TrustCortex.

Current completed flow:
User Question
 ↓
Input Safety / Prompt Validation
 ↓
Enterprise Retrieval through Mock Search or Azure AI Search
 ↓
Retrieved Documents
 ↓
Policy + Governance Filtering
 ↓
Approved Context
 ↓
Mock Answer Generation
 ↓
Response Validation
 ↓
Audit Logging
 ↓
Governed Response

V3 goal:
Add an answer generation provider abstraction for Azure Foundry / Azure OpenAI while keeping MockAnswerService as the default provider for cost control.

Important design decision:
TrustCortex should not use Azure OpenAI "On Your Data" directly in V3 because that would allow the model service to perform retrieval internally. TrustCortex needs to enforce document-level governance after retrieval and before answer generation. Therefore, the Application layer will build approved context and pass only allowed documents to the answer generation provider.

Create a new spec:
specs/07-v3-foundry-answer-generation.md

Content should include:
# V3 - Azure Foundry Answer Generation Layer

## Goal
Introduce a provider-based answer generation layer that can use MockAnswerService or Azure Foundry / Azure OpenAI in future execution.

## Correct Flow
User Question
 ↓
Input Safety
 ↓
Enterprise Retrieval
 ↓
Retrieved Documents
 ↓
Policy + Governance Filtering
 ↓
Approved Context
 ↓
Answer Generation Provider
 ↓
Response Validation
 ↓
Audit Logging
 ↓
Governed Response

## Why Approved Context Matters
Only approved documents should be sent to the model. Blocked documents must not be included in the prompt.

## Provider Strategy
AnswerProvider = Mock uses MockAnswerService.
AnswerProvider = AzureFoundry uses AzureFoundryAnswerService.

## V3 3 Cost-Control Mode
V3 3 implements the AzureFoundryAnswerService abstraction and configuration but keeps AnswerProvider = Mock by default.
Azure resources for Foundry/OpenAI are not required for Day 3.

## Acceptance Criteria
- Mock answer provider still works.
- AzureFoundryAnswerService exists behind IAnswerService.
- Provider selection is configuration-driven.
- Approved context is converted into a grounded prompt.
- Blocked documents are never passed to answer generation.
- README documents V3 architecture.

Update:
- specs/02-architecture.md
- README.md

Add a V3 section explaining:
- Foundry/OpenAI is the reasoning layer.
- Azure AI Search is the retrieval layer.
- TrustCortex governance sits between retrieval and answer generation.
- Mock provider remains default for cost safety.

Do not modify code in this pass.