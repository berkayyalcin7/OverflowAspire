using Typesense;

namespace SearchService.Data
{
    public static class SearchInitializer
    {
        public static async Task EnsureIndexExists(ITypesenseClient client)
        {
			const string schemaName = "questions";
            try
			{
				await client.RetrieveCollection(schemaName);
                Console.WriteLine($"Collection {schemaName} has been already created.");

            }
			catch (TypesenseApiNotFoundException)
			{
                Console.WriteLine($"Collection {schemaName} has not been created yet");

                var schema = new Schema(schemaName, new List<Field>
            {
                new Field("id", FieldType.String) { Facet = false, Optional = false },
                new Field("title", FieldType.String) { Facet = false, Optional = false },
                new Field("content", FieldType.String) { Facet = false, Optional = false },
                new Field("tags", FieldType.StringArray) { Facet = true, Optional = true },
                new Field("createdAt", FieldType.Int64) { Facet = true, Optional = false },
                new Field("hasAcceptedAnswer", FieldType.Bool) { Facet = true, Optional = false },
                new Field("answerCount", FieldType.Int32) { Facet = true, Optional = false },
            })
                {
                    DefaultSortingField = "createdAt"
                };

                await client.CreateCollection(schema);
                Console.WriteLine($"Collection {schemaName} has been created");
            }
        }
    }
}
