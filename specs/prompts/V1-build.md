Implement V 1 of TrustCortex.

Architecture:
- API → Application
- Infrastructure → Application

Projects:
- TrustCortex.Api
- TrustCortex.Application
- TrustCortex.Infrastructure
- TrustCortex.Tests

Follow all specs in /specs.

Implement:
- Swagger
- GET /health
- POST /api/ask

Application layer should contain:
- interfaces
- DTOs
- governance pipeline
- use cases
- validation contracts

Infrastructure layer should contain:
- mock search implementation
- mock answer implementation
- prompt safety implementation
- audit logger

Implement interfaces:
- IPolicyEngine
- IPromptSafetyService
- ISearchService
- IAnswerService
- IAuditLogger
- IResponseValidator

Use dependency injection.

Do NOT integrate Azure OpenAI yet.

POST /api/ask request:
{
  "question": "Can customer PII be logged in App Insights?",
  "userRole": "Engineer"
}

Response:
{
  "answer": "...",
  "citations": [],
  "governance": {
    "policyCheckPassed": true,
    "promptSafetyPassed": true,
    "documentsBlocked": 1,
    "blockedReason": "RestrictedSensitivity",
    "responseGrounded": true,
    "auditLogged": true
  }
}