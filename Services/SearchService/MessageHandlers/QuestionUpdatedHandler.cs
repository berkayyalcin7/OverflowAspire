using Contracts;
using System.Text.RegularExpressions;
using Typesense;

namespace SearchService.MessageHandlers
{
    public class QuestionUpdatedHandler(ITypesenseClient client)
    {
        public async Task HandleAsync(QuestionUpdated message)
        {
            // Parially update anonymous object
            await client.UpdateDocument("questions", message.QuestionId, new
            {
                message.Title,
                Content = StripHtml(message.Content),
                Tags = message.Tags.ToArray()
            });
        }

        private static string StripHtml(string Content)
        {
            return Regex.Replace(Content, "<.*?>", string.Empty);
        }
    }
}
