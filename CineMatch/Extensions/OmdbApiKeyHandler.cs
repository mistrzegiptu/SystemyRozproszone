using Microsoft.AspNetCore.WebUtilities;

namespace CineMatch.Extensions
{
    public class OmdbApiKeyHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        public OmdbApiKeyHandler(string apiKey)
        {
            _apiKey = apiKey ?? string.Empty;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(request.RequestUri != null)
            {
                string currentUrl = request.RequestUri.ToString();
                string urlWithApiKey = QueryHelpers.AddQueryString(currentUrl, "apikey", _apiKey);

                request.RequestUri = new Uri(urlWithApiKey);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
