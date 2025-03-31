using AppFinanzas.Mvvm.ModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppFinanzas.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7086/api";

        public ApiService()
        {
            _client = new HttpClient();
        }

        public async Task<UsuarioDto> LoginAsync(string email, string contrasena)
        {
            var loginData = new { email, contrasena };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Usuarios/login", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login fallido");

            // Si la API no devuelve nada, solo retornás el usuario actual
            return new UsuarioDto { Email = email };
        }

    }

}
