namespace CineMatch.Extensions
{
    public static class OmdbClientExtension
    {
        public static IServiceCollection AddOmdbClient(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterOmdbApiKeyHandler(services, configuration);

            services.AddHttpClient("OmdbClient", client => ConfigureClient(client, configuration))
                    .AddHttpMessageHandler<OmdbApiKeyHandler>();

            return services;
        }

        private static void RegisterOmdbApiKeyHandler(IServiceCollection services, IConfiguration configuration)
        {
            var apiKey = configuration["ExternalApis:OmdbApiKey"];

            services.AddTransient(provider => new OmdbApiKeyHandler(apiKey!));
        }

        private static void ConfigureClient(HttpClient client, IConfiguration configuration)
        {
            var baseUrl = configuration["ExternalApis:OmdbBaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException();

            client.BaseAddress = new Uri(baseUrl);
        }
    }
}
