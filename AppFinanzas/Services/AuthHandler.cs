using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using Microsoft.Maui.Storage;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AppFinanzas.Services
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly HttpClient _refreshClient = new HttpClient();
        private readonly string _baseUrl = "https://localhost:7086/api";

        public AuthHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Agregar header Authorization si existe token
            var token = SesionActual.Token ?? Preferences.Default.Get("jwt", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Intentar refrescar token
                var refresh = Preferences.Default.Get("refreshToken", string.Empty);
                try
                {
                    if (string.IsNullOrEmpty(refresh))
                    {
                        // intentar cargar de SecureStorage
                        refresh = SecureStorage.GetAsync("refreshToken").GetAwaiter().GetResult();
                    }
                }
                catch { }

                if (!string.IsNullOrEmpty(refresh))
                {
                    var refreshed = await TryRefreshTokenAsync(refresh);
                    if (refreshed)
                    {
                        // Reintentar la petición original con nuevo token
                        var newToken = SesionActual.Token ?? Preferences.Default.Get("jwt", string.Empty);
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                            // Clonar la request original (simple approach: create new request with same content)
                            var newRequest = await CloneHttpRequestMessageAsync(request);
                            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                            return await base.SendAsync(newRequest, cancellationToken);
                        }
                    }
                }
            }

            return response;
        }

        private async Task<bool> TryRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var dto = new { refreshToken };
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _refreshClient.PostAsync($"{_baseUrl}/auth/refresh", content);
                if (!response.IsSuccessStatusCode) return false;

                var body = await response.Content.ReadAsStringAsync();
                var loginResp = JsonSerializer.Deserialize<LoginResponseDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (loginResp?.Token != null)
                {
                    SesionActual.Token = loginResp.Token;
                    try { await SecureStorage.SetAsync("jwt", loginResp.Token); } catch { Preferences.Default.Set("jwt", loginResp.Token); }
                }
                if (loginResp?.RefreshToken != null)
                {
                    try { await SecureStorage.SetAsync("refreshToken", loginResp.RefreshToken); } catch { Preferences.Default.Set("refreshToken", loginResp.RefreshToken); }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            if (req.Content != null)
            {
                var ms = new System.IO.MemoryStream();
                await req.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                {
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
                }
            }

            // Copy the request headers
            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            clone.Version = req.Version;
            return clone;
        }
    }
}
