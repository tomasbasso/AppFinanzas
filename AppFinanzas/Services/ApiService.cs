using AppFinanzas.Mvvm.ModelsDto;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        ////////////LOGIN
        public async Task<UsuarioDto> LoginAsync(string email, string contrasena)
        {
            var loginData = new { email, contrasena };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Usuarios/login", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login fallido");

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UsuarioDto>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }


        //////////// CUENTAS BANCARIAS
        public async Task<List<CuentaDto>> GetCuentasAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/Cuentas");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener cuentas bancarias");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CuentaDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CrearCuentaAsync(CuentaDto nuevaCuenta)
        {
            var json = JsonSerializer.Serialize(nuevaCuenta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Cuentas", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo crear la cuenta.");
        }

        public async Task EditarCuentaAsync(CuentaDto cuenta)
        {
            var dto = new
            {
                cuenta.Nombre,
                cuenta.Banco,
                SaldoInicial = cuenta.Saldo,
                cuenta.TipoCuenta,
                cuenta.UsuarioId
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/Cuentas/{cuenta.CuentaId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(); // <-- Acá vemos qué falló
                throw new Exception($"Error al editar cuenta: {response.StatusCode} - {errorBody}");
            }
        }

        public async Task EliminarCuentaAsync(int cuentaId)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}/Cuentas/{cuentaId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar la cuenta.");
        }

        ////////////TRANSACCIONES 
        public async Task<List<TransaccionDto>> GetTransaccionesAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/Transacciones");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener transacciones");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TransaccionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task EditarTransaccionAsync(TransaccionDto transaccion)
        {
            var json = JsonSerializer.Serialize(transaccion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/Transacciones/{transaccion.TransaccionId}", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo editar la transacción");
        }
        public async Task CrearTransaccionAsync(TransaccionDto transaccion)
        {
            var json = JsonSerializer.Serialize(transaccion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Transacciones", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {error}");
            }

            // Opcional: podés leer la transacción creada si la necesitas
            var responseContent = await response.Content.ReadAsStringAsync();
            var transaccionCreada = JsonSerializer.Deserialize<TransaccionDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"Transacción creada: {transaccionCreada?.Descripcion}");
        }
        public async Task EliminarTransaccionAsync(int transaccionId)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}/Transacciones/{transaccionId}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar la transacción");
        }


        ////////////PRESUPUESTOS
        public async Task<List<PresupuestoDto>> GetPresupuestosAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/Presupuestos");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener presupuestos");

        var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PresupuestoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        ////////////METAS AHORRO
        
        public async Task<List<MetaAhorroDto>> GetMetasAhorroAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/MetasAhorro");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener metas de ahorro");

        var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MetaAhorroDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        ////////// TIPO DE CAMBIO 

        public async Task<List<TipoCambioDto>> GetTiposCambioAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/TipoCambio");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener tipos de cambio");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TipoCambioDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /////// PERFIL
        public async Task<UsuarioDto> GetPerfilAsync()
        {
            var response = await _client.GetAsync($"{_baseUrl}/Usuarios/me");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener el perfil del usuario");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UsuarioDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
