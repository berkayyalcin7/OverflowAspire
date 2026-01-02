var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak",6001)
    .WithDataVolume("keycloak-data");

// LOCAL PC ' DE POSTRGE OLDUĞU İÇİN PORT'U 5434 OLARAK DEĞİŞTİRDİK.
var postgres = builder.AddPostgres("postgres", port: 5434)
    .WithDataVolume("postgres-data")
    .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
    .WithPgAdmin(container =>
    {
        container.WithImage("dpage/pgadmin4:latest");
        
        // pgAdmin ayarları
        container.WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False");
        container.WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False");
        
        // Upgrade check'i tamamen kapat - BU HATAYI ÇÖZER
        container.WithEnvironment("PGADMIN_CONFIG_UPGRADE_CHECK_ENABLED", "False");
        
        // Tüm SSL doğrulamalarını kapat
        container.WithEnvironment("PYTHONHTTPSVERIFY", "0");
        container.WithEnvironment("REQUESTS_CA_BUNDLE", "");
        container.WithEnvironment("CURL_CA_BUNDLE", "");
        container.WithEnvironment("SSL_CERT_FILE", "");
        container.WithEnvironment("PYTHONSSLVERIFY", "0");
        // Python'un SSL context'ini devre dışı bırak
        container.WithEnvironment("SSL_CERT_DIR", "");
    });

var typesenseApiKey = builder.AddParameter("typesense-api-key",secret:true);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithArgs("--data-dir", "/data", "--api-key", typesenseApiKey, "--enable-cors")
    .WithVolume("typesense-data", "/data")
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typesenseContainer = typesense.GetEndpoint("typesense");

var questionDb = postgres.AddDatabase("questionDb");

// SERVICE CONFIG

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WaitFor(keycloak)
    .WaitFor(questionDb);


var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithEnvironment("typesense-api-key",typesenseApiKey)
    .WithReference(typesenseContainer)
    .WaitFor(typesense);


builder.Build().Run();
