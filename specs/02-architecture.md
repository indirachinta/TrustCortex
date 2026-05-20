# TrustCortex Architecture

React UI / Swagger
        ↓
ASP.NET Core API
        ↓
Governance Pipeline
        ↓
Policy Engine
        ↓
Prompt Safety Layer
        ↓
Azure AI Search
        ↓
Azure OpenAI
        ↓
Response Validator
        ↓
Audit Log

## V1  Scope

V1  uses:
- local policy metadata
- mock search service
- mock answer generation
- Azure AI Search Free tier
- Azure Content Safety resource