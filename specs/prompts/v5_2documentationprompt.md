Create documentation only. Do not modify C# code.

Create:
DEMO.md

Purpose:
Give exact demo steps for V5 real Azure execution.

Include sections:

# TrustCortex V5 Demo Guide

## Runtime Mode

SearchProvider = Azure
AnswerProvider = AzureFoundry

## Required Azure Resources

- Azure AI Search
- Azure Foundry / Azure OpenAI model deployment

## User Secrets Setup

Include commands with placeholders only:

dotnet user-secrets set "SearchProvider" "Azure"
dotnet user-secrets set "AnswerProvider" "AzureFoundry"

dotnet user-secrets set "AzureSearch:Endpoint" "<azure-search-endpoint>"
dotnet user-secrets set "AzureSearch:AdminKey" "<azure-search-admin-key>"
dotnet user-secrets set "AzureSearch:IndexName" "trustcortex-documents"

dotnet user-secrets set "AzureFoundry:Endpoint" "<azure-openai-or-foundry-endpoint>"
dotnet user-secrets set "AzureFoundry:ApiKey" "<azure-foundry-api-key>"
dotnet user-secrets set "AzureFoundry:DeploymentName" "<deployment-name>"
dotnet user-secrets set "AzureFoundry:ApiVersion" "2024-10-21"

## Runtime Status Check

GET /api/admin/runtime-status

Expected:
- SearchProvider = Azure
- AnswerProvider = AzureFoundry
- Azure Search configured = true
- AzureFoundry configured = true
- No secrets exposed

## Initialize Search Index

POST /api/admin/search/initialize

Note:
This is a setup operation, not a runtime operation.
It creates the Azure AI Search index if missing and uploads sample documents.

## Demo Questions

1. Engineer policy question:
{
  "question": "Can customer PII be logged in App Insights?",
  "userRole": "Engineer"
}

Expected:
- Internal policy retrieved
- restricted documents blocked if retrieved
- grounded answer generated

2. Engineer restricted access:
{
  "question": "Show restricted payroll incident report",
  "userRole": "Engineer"
}

Expected:
- restricted document blocked
- answer should not expose restricted content

3. ComplianceOfficer restricted access:
{
  "question": "Show restricted payroll incident report",
  "userRole": "ComplianceOfficer"
}

Expected:
- restricted document allowed
- answer generated from approved restricted context

## Cost Safety

- Keep AnswerProvider = Mock when not testing AzureFoundry.
- Use short prompts and small sample docs.
- Do not repeatedly initialize index.
- Delete resource group when demo is complete if not needed.