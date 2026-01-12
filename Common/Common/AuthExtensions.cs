using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddKeyCloakAuthentication(this IServiceCollection services)
        {
            // Keycloak Authentication
            services
                .AddAuthentication()
                .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "overflow", options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "overflow";
                    // Bunu eklemediğimizde Invalid issuer hatası alıyoruz. 
                    //options.Authority = "http://keycloak:6001/realms/overflow";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidIssuers = [
                            "http://keycloak/realms/overflow",
                            "http://localhost:6001/realms/overflow",
                            "http://id.overflow.local/realms/overflow"
                        ]
                    };
                });


            return services;
        }
    }
}
