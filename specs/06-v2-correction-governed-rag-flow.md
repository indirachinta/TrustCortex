# V2 Correction - Governed RAG Flow

## Purpose

This correction aligns TrustCortex documentation with the governed RAG lifecycle
that the runtime should implement.

Governance is not a single pre-retrieval step. It is applied before retrieval,
after retrieval, and after answer generation.

## Correct Runtime Flow

User Question
  |
  v
Input Safety / Prompt Validation
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
Answer Generation
  |
  v
Response Validation
  |
  v
Audit Logging
  |
  v
Governed Response

## Stage Definitions

Input Safety / Prompt Validation happens before retrieval. It blocks unsafe
questions, prompt injection attempts, and requests that try to bypass policy.

Enterprise Retrieval uses Azure AI Search or Mock Search to retrieve candidate
enterprise documents. Retrieved documents are not automatically approved for
use in the answer.

Policy + Governance Filtering happens after retrieval. It evaluates document
metadata such as sensitivity level, allowed roles, source, and other governance
attributes.

Approved Context is the subset of retrieved documents that passed policy
filtering. Only approved context can be sent to answer generation.

Answer Generation creates the draft answer from the user's question and the
approved context.

Response Validation runs after answer generation. It checks that the answer
contains required citations, governance metadata, blocked document counts, and
other response policy requirements.

Audit Logging runs after answer generation and response validation. It records
the retrieval, filtering, generation, and validation decisions that produced the
governed response.

## Required Documentation Alignment

- Architecture diagrams and descriptions must show input safety before
  retrieval.
- Retrieval documentation must describe Azure AI Search and Mock Search as
  candidate document providers.
- Governance documentation must show policy filtering after retrieval using
  document metadata.
- Answer generation documentation must state that only approved context is used.
- Response documentation must show validation and audit logging after answer
  generation.
