using Typesense;

namespace SearchService.MessageHandlers
{
    public class AnswerCountUpdatedHandler(ITypesenseClient client)
    {
        public async Task Handle(Contracts.AnswerCountUpdated message)
        {
            await client.UpdateDocument("questions",message.QuestionId, new
            {
                answerCount = message.AnswerCount
            });
        }
    }
}
