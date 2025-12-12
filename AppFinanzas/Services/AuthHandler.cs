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
            // Agrego el header Authorization si tengo token
            var token = SesionActual.Token ?? Preferences.Default.Get("jwt", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Intento refrescar el token
                var refresh = Preferences.Default.Get("refreshToken", string.Empty);
                try
                {
                    if (string.IsNullOrEmpty(refresh))
                    {
                        // si no hay, pruebo leerlo de SecureStorage
                        refresh = SecureStorage.GetAsync("refreshToken").GetAwaiter().GetResult();
                    }
                }
                catch { }

                if (!string.IsNullOrEmpty(refresh))
                {
                    var refreshed = await TryRefreshTokenAsync(refresh);
                    if (refreshed)
                    {
                        // Vuelvo a mandar la request con el token nuevo
                        var newToken = SesionActual.Token ?? Preferences.Default.Get("jwt", string.Empty);
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                            // Clono la request con el mismo contenido
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

            // Copio el body con un MemoryStream para la request clonada
            if (req.Content != null)
            {
                var ms = new System.IO.MemoryStream();
                await req.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copio los headers del contenido
                if (req.Content.Headers != null)
                {
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
                }
            }

            // Copio los headers de la request
            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            clone.Version = req.Version;
            return clone;
        }
    }
}

