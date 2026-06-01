# V3 - Azure Foundry Answer Generation Layer

## Goal

Introduce a provider-based answer generation layer that can use
MockAnswerService or Azure Foundry / Azure OpenAI in future execution.

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
