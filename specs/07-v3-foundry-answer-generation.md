# V3 - Azure Foundry Answer Generation Layer

## Goal

Introduce a provider-based answer generation layer that can use
MockAnswerService or Azure Foundry / Azure OpenAI in future execution.

## Correct Flow

User Question
  |
  v
Input Safety
  |
  v
Enterprise Retrieval
  |
  v
Retrieved Documents
  |
  v
Policy + Governance Filtering
  |
  v
Approved Context
  |
  v
Answer Generation Provider
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response

## Why Approved Context Matters

Only approved documents should be sent to the model. Blocked documents must not
be included in the prompt.

TrustCortex should not use Azure OpenAI "On Your Data" directly in V3 because
that would allow the model service to perform retrieval internally. TrustCortex
needs to enforce document-level governance after retrieval and before answer
generation.

The Application layer builds approved context and passes only allowed documents
to the answer generation provider.

## Provider Strategy

AnswerProvider = Mock uses MockAnswerService.

AnswerProvider = AzureFoundry uses AzureFoundryAnswerService.

## Day 3 Cost-Control Mode

Day 3 implements the AzureFoundryAnswerService abstraction and configuration
but keeps AnswerProvider = Mock by default.

Azure resources for Foundry/OpenAI are not required for Day 3.

## Acceptance Criteria

- Mock answer provider still works.
- AzureFoundryAnswerService exists behind IAnswerService.
- Provider selection is configuration-driven.
- Approved context is converted into a grounded prompt.
- Blocked documents are never passed to answer generation.
- README documents V3 architecture.

## V4 Readiness Notes

V4 keeps the V3 answer provider abstraction and makes the complete runtime mode
matrix explicit:

- Local Safe Mode uses Mock retrieval and Mock answer generation.
- Azure Retrieval Mode uses Azure AI Search retrieval and Mock answer
  generation.
- Full Azure AI Mode uses Azure AI Search retrieval and AzureFoundry answer
  generation.

Azure AI Search remains retrieval only. Azure Foundry / Azure OpenAI remains
answer generation only. TrustCortex continues to own governance orchestration
and must pass only approved context to the configured answer provider.
