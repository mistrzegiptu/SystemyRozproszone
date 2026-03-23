using System.Net.Http.Headers;

namespace CineMatch.Extensions
{
    public static class TmdbClientExtension
    {
        public static IServiceCollection AddTmdbClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("TmdbClient", client => ConfigureClient(client, configuration));

            return services;
        }

        private static void ConfigureClient(HttpClient client, IConfiguration configuration)
        {
            var baseUrl = configuration["ExternalApis:TmdbBaseUrl"];
            var token = configuration["ExternalApis:TmdbToken"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException();

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
