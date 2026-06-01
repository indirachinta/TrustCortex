using System.Text;
using TrustCortex.Application.DTOs;

namespace TrustCortex.Infrastructure.Answers;

public sealed class GroundedPromptBuilder
{
    private const string SystemInstruction =
        "You are TrustCortex, a governed enterprise AI assistant. Answer only from approved context. If context is insufficient, say you do not have enough approved information.";

    public string Build(string question, IReadOnlyList<SearchDocument> approvedDocuments)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("System Instruction:");
        prompt.AppendLine(SystemInstruction);
        prompt.AppendLine();
        prompt.AppendLine("User Question:");
        prompt.AppendLine(question);
        prompt.AppendLine();
        prompt.AppendLine("Approved Context:");

        if (approvedDocuments.Count == 0)
        {
            prompt.AppendLine("No approved documents were provided.");
            return prompt.ToString();
        }

        for (var index = 0; index < approvedDocuments.Count; index++)
        {
            var document = approvedDocuments[index];

            prompt.AppendLine($"Document {index + 1}");
            prompt.AppendLine($"Title: {document.Title}");
            prompt.AppendLine($"Source: {document.Source}");
            prompt.AppendLine($"Sensitivity Level: {document.Sensitivity}");
            prompt.AppendLine("Content:");
            prompt.AppendLine(document.Content);
            prompt.AppendLine();
        }

        return prompt.ToString();
    }
}
