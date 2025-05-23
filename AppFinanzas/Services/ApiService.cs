﻿using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        // Función para agregar el token JWT al header de Authorization
        private void AddAuthHeader()
        {
            var token = SesionActual.Token ?? Preferences.Default.Get<string>("jwt", null);

            if (!string.IsNullOrEmpty(token))
            {
                if (_client.DefaultRequestHeaders.Contains("Authorization"))
                    _client.DefaultRequestHeaders.Remove("Authorization");

                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        ////////////// LOGIN (no requiere token)
        public async Task<LoginResponseDto> LoginAsync(string email, string contrasena)
        {
            var loginData = new { email, contrasena };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Usuarios/login", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login fallido");

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<LoginResponseDto>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        //////////// CUENTAS BANCARIAS (requieren token)
        public async Task<List<CuentaDto>> GetCuentasAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Cuentas");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener cuentas bancarias");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CuentaDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CrearCuentaAsync(CuentaDto nuevaCuenta)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(nuevaCuenta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Cuentas", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo crear la cuenta.");
        }

        public async Task EditarCuentaAsync(CuentaDto cuenta)
        {
            AddAuthHeader();

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
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al editar cuenta: {response.StatusCode} - {errorBody}");
            }
        }

        public async Task EliminarCuentaAsync(int cuentaId)
        {
            AddAuthHeader();

            var response = await _client.DeleteAsync($"{_baseUrl}/Cuentas/{cuentaId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar la cuenta.");
        }

        //////////// TRANSACCIONES
        public async Task<List<TransaccionDto>> GetTransaccionesAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Transacciones");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener transacciones");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TransaccionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task EditarTransaccionAsync(TransaccionDto transaccion)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(transaccion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/Transacciones/{transaccion.TransaccionId}", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo editar la transacción");
        }
        public async Task CrearTransaccionAsync(TransaccionDto transaccion)
        {
            AddAuthHeader();

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
            AddAuthHeader();

            var response = await _client.DeleteAsync($"{_baseUrl}/Transacciones/{transaccionId}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar la transacción");
        }

        //////////// PRESUPUESTOS
        public async Task<List<PresupuestoDto>> GetPresupuestosAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Presupuestos");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener presupuestos");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PresupuestoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task EliminarPresupuestoAsync(int presupuestoId)
        {
            AddAuthHeader();

            var response = await _client.DeleteAsync($"{_baseUrl}/Presupuestos/{presupuestoId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar el presupuesto.");
        }
        public async Task CrearPresupuestoAsync(PresupuestoDto presupuesto)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(presupuesto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Presupuestos", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {error}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var presupuestoCreado = JsonSerializer.Deserialize<PresupuestoDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"Presupuesto creado: {presupuestoCreado?.MontoLimite}");
        }

        public async Task EditarPresupuestoAsync(PresupuestoDto presupuesto)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(presupuesto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/Presupuestos/{presupuesto.PresupuestoId}", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo editar el presupuesto");
        }

        public async Task<List<CategoriaGastoDto>> GetCategoriasGastoAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/CategoriasGasto");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener las categorías de gasto.");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoriaGastoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        //////////// METAS AHORRO
        public async Task<List<MetaAhorroDto>> GetMetasAhorroAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/MetasAhorro");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener metas de ahorro");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MetaAhorroDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task EliminarMetaAhorroAsync(int MetaId)
        {
            AddAuthHeader();

            var response = await _client.DeleteAsync($"{_baseUrl}/MetasAhorro/{MetaId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo eliminar la meta ahorro.");
        }
        public async Task EditarMetaAhorroAsync(MetaAhorroDto meta_ahorro)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(meta_ahorro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/MetasAhorro/{meta_ahorro.MetaId}", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo editar esta meta de ahorro");
        }
        public async Task CrearMetaAhorroAsync(MetaAhorroDto meta_ahorro)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(meta_ahorro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/MetasAhorro", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {error}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var MetaCreada = JsonSerializer.Deserialize<MetaAhorroDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"Meta creada:{MetaCreada}");
        }

        ////////// TIPO DE CAMBIO (EXTERNO, NO PROTEGIDO)
        public async Task<List<TipoCambioDto>> GetTiposCambioExternosAsync()
        {
            // Este NO requiere token
            var response = await _client.GetAsync("https://dolarapi.com/v1/dolares");

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo obtener el tipo de cambio externo");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TipoCambioDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /////// PERFIL (protegido)
        public async Task<UsuarioDto> GetPerfilAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Usuarios/me");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener el perfil del usuario");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UsuarioDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        ////// USUARIOS (protegido)
        public async Task<List<UsuarioDto>> GetUsuariosAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Usuarios");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener usuarios");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UsuarioDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CrearUsuarioAsync(UsuarioRegistroDto nuevoUsuario)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(nuevoUsuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_baseUrl}/Usuarios/register", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {error}");
            }
        }

        public async Task ActualizarUsuarioAsync(int usuarioId, UsuarioEdicionDto usuario)
        {
            AddAuthHeader();

            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_baseUrl}/Usuarios/{usuarioId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {error}");
            }
        }

        public async Task EliminarUsuarioAsync(int usuarioId)
        {
            AddAuthHeader();

            var response = await _client.DeleteAsync($"{_baseUrl}/Usuarios/{usuarioId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"No se pudo eliminar el usuario: {error}");
            }
        }
    }
}
