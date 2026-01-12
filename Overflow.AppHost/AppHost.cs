using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("production")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(8080);
    });

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithDataVolume("keycloak-data")
    .WithRealmImport("../infra/realms")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEndpoint(6001, 8080, "keycloak", isExternal: true);
    //.WithEnvironment("VIRTUAL_HOST", "id.overflow.local")
    //.WithEnvironment("VIRTUAL_PORT", "8080"); ;

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
    //.WithArgs("--data-dir", "/data", "--api-key", typesenseApiKey, "--enable-cors")
    .WithEnvironment("TYPESENSE_DATA_DIR", "/data")
    .WithEnvironment("TYPESENSE_ENABLE_CORS", "true")
    .WithEnvironment("TYPESENSE_API_KEY", typesenseApiKey)
    .WithVolume("typesense-data", "/data")
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typesenseContainer = typesense.GetEndpoint("typesense");

var questionDb = postgres.AddDatabase("questionDb");

// RabbitMQ Container
var rabbitMq  = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port:15672);



// SERVICE CONFIG

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WithReference(rabbitMq)
    .WaitFor(keycloak, WaitBehavior.StopOnResourceUnavailable)  // 2 dakika timeout
    .WaitFor(questionDb, WaitBehavior.StopOnResourceUnavailable)  // 1 dakika timeout
    .WaitFor(rabbitMq, WaitBehavior.StopOnResourceUnavailable);  // 2 dakika timeout


var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithEnvironment("typesense-api-key", typesenseApiKey)
    .WithReference(typesenseContainer)
    .WithReference(rabbitMq)
    .WaitFor(typesense, WaitBehavior.StopOnResourceUnavailable)  // 2 dakika timeout
    .WaitFor(rabbitMq, WaitBehavior.StopOnResourceUnavailable);  // 2 dakika timeout


// YARP Configuration
var yarp = builder.AddYarp("gateway")
    .WithConfiguration(yarpBuilder =>
    {
        yarpBuilder.AddRoute("/api/questions/{**catch-all}", questionService);
        yarpBuilder.AddRoute("/api/tags/{**catch-all}", questionService);
        yarpBuilder.AddRoute("search/{**catch-all}", searchService);
    })
     .WithHostPort(8001)
     .WithEnvironment("ASPNETCORE_URLS", "http://*:8001")  // ✅ Format düzeltildi
     .WithEnvironment("ASPNETCORE_FORWARDEDHEADERS_ENABLED", "true") // ✅ Forwarded headers için
     .WithEndpoint(port: 8001, targetPort: 8001, scheme: "http", name: "gateway", isExternal: true)
     .WithEnvironment("VIRTUAL_HOST","api.overflow.local")
     .WithEnvironment("VIRTUAL_PORT","8001");

if (!builder.Environment.IsDevelopment())
{
    builder.AddContainer("nginx-proxy", "nginxproxy/nginx-proxy", "1.8")
        .WithEndpoint(80,80,"nginx",isExternal:true)
        .WithBindMount("/var/run/docker.sock","/tmp/docker.sock",isReadOnly:true);
}


builder.Build().Run();
