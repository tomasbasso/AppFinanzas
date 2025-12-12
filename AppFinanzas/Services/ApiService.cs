using AppFinanzas.Data;

using AppFinanzas.Mvvm.ModelsDto;

using Microsoft.Maui.Storage;

using System;

using System.Collections.Generic;

using System.Net.Http;

using System.Net.Http.Headers;

using System.Text;

using System.Text.Json;

using System.Threading.Tasks;



#nullable disable



namespace AppFinanzas.Services

{

    public class ApiService

    {

        private readonly HttpClient _client;

        private readonly string _baseUrl = "https://localhost:7086/api"; // API local

        //private readonly string _baseUrl = "http://apifinanzas.runasp.net/api\""; // API de monster



        public ApiService()

        {

            // AuthHandler para manejar Authorization y el refresh en un solo lugar

            var handler = new AuthHandler(new HttpClientHandler());

            _client = new HttpClient(handler);

        }



        private void AddAuthHeader()

        {

            var token = SesionActual.Token ?? Preferences.Default.Get("jwt", string.Empty);



            if (string.IsNullOrEmpty(token))

            {

                try

                {

                    token = SecureStorage.GetAsync("jwt").GetAwaiter().GetResult();

                }

                catch

                {

                  

                }

            }



            if (!string.IsNullOrEmpty(token))

            {

                if (_client.DefaultRequestHeaders.Contains("Authorization"))

                    _client.DefaultRequestHeaders.Remove("Authorization");



                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            }

        }



        // Logout: borra token y datos guardados en el cliente

        public async Task LogoutAsync()

        {

            try { if (_client.DefaultRequestHeaders.Contains("Authorization")) _client.DefaultRequestHeaders.Remove("Authorization"); } catch { }

            SesionActual.Token = null;

            SesionActual.Usuario = null;

            try { Preferences.Default.Remove("jwt"); } catch { }

            try { Preferences.Default.Remove("refreshToken"); } catch { }

            try { Preferences.Default.Remove("usuarioEmail"); } catch { }

            try { SecureStorage.Remove("jwt"); } catch { try { await SecureStorage.SetAsync("jwt", string.Empty); } catch { } }

            try { SecureStorage.Remove("refreshToken"); } catch { try { await SecureStorage.SetAsync("refreshToken", string.Empty); } catch { } }

        }



       

        public async Task<LoginResponseDto> LoginAsync(string email, string contrasena)

        {

            var loginData = new { email, contrasena };

            var json = JsonSerializer.Serialize(loginData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");



            var response = await _client.PostAsync($"{_baseUrl}/Usuarios/login", content);



            if (!response.IsSuccessStatusCode)

                throw new Exception("Login fallido");



            var result = await response.Content.ReadAsStringAsync();



            var loginResp = JsonSerializer.Deserialize<LoginResponseDto>(result, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });



            // Guardo los tokens y actualizo la sesion en memoria

            if (loginResp?.Token != null)

            {

                SesionActual.Token = loginResp.Token;

                try { await SecureStorage.SetAsync("jwt", loginResp.Token); } catch { Preferences.Default.Set("jwt", loginResp.Token); }

            }

            if (loginResp?.RefreshToken != null)

            {

                try { await SecureStorage.SetAsync("refreshToken", loginResp.RefreshToken); } catch { Preferences.Default.Set("refreshToken", loginResp.RefreshToken); }

            }



            if (loginResp?.Usuario != null)

            {

                SesionActual.Usuario = loginResp.Usuario;

            }



            return loginResp!;

        }



        // CUENTAS (piden token)

        public async Task<List<CuentaDto>> GetCuentasAsync()

        {

            Console.WriteLine("Authorization header:");

            if (_client.DefaultRequestHeaders.TryGetValues("Authorization", out var headers))

            {

                foreach (var header in headers)

                    Console.WriteLine(header);

            }

            else

            {

                Console.WriteLine("No se encontrÃ³ el header Authorization.");

            }



            AddAuthHeader();



            var response = await _client.GetAsync($"{_baseUrl}/Cuentas");



            if (!response.IsSuccessStatusCode)

            {

                var errorContent = await response.Content.ReadAsStringAsync();

                throw new Exception($"Error al obtener cuentas bancarias. CÃ³digo: {(int)response.StatusCode} - {response.ReasonPhrase}\nDetalle: {errorContent}");

            }



            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CuentaDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }





        public async Task CrearCuentaAsync(CuentaDto nuevaCuenta)

        {

            AddAuthHeader();

            var payload = new
            {
                nombre = nuevaCuenta.Nombre,
                banco = nuevaCuenta.Banco,
                saldoInicial = nuevaCuenta.Saldo,
                saldoActual = nuevaCuenta.SaldoActual,
                tipoCuenta = nuevaCuenta.TipoCuenta,
                usuarioId = nuevaCuenta.UsuarioId
            };

            var json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");



            var response = await _client.PostAsync($"{_baseUrl}/Cuentas", content);

            if (!response.IsSuccessStatusCode)

                throw new Exception("No se pudo crear la cuenta.");

        }



        public async Task EditarCuentaAsync(CuentaDto cuenta)

        {

            AddAuthHeader();

            var payload = new
            {
                nombre = cuenta.Nombre,
                banco = cuenta.Banco,
                saldoInicial = cuenta.Saldo,
                saldoActual = cuenta.SaldoActual,
                tipoCuenta = cuenta.TipoCuenta,
                usuarioId = cuenta.UsuarioId
            };



            var json = JsonSerializer.Serialize(payload);

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



        // TRANSACCIONES

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

                throw new Exception("No se pudo editar la transacciÃ³n");

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



           

            var responseContent = await response.Content.ReadAsStringAsync();

            var transaccionCreada = JsonSerializer.Deserialize<TransaccionDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });



            Console.WriteLine($"TransacciÃ³n creada: {transaccionCreada?.Descripcion}");

        }

        public async Task EliminarTransaccionAsync(int transaccionId)

        {

            AddAuthHeader();



            var response = await _client.DeleteAsync($"{_baseUrl}/Transacciones/{transaccionId}");



            if (!response.IsSuccessStatusCode)

                throw new Exception("No se pudo eliminar la transacciÃ³n");

        }



        // PRESUPUESTOS

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

            
            var endpoints = new[] { "CategoriasGasto", "CategoriaGasto", "CategoriaGastos", "CategoriasGastos" };
            Exception? lastError = null;

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var response = await _client.GetAsync($"{_baseUrl}/{endpoint}");
                    if (!response.IsSuccessStatusCode)
                    {
                        lastError = new Exception($"CÃ³digo {(int)response.StatusCode} en /{endpoint}");
                        continue;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<CategoriaGastoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (data != null)
                        return data;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }
            }

            throw new Exception($"Error al obtener las categorias de gasto. Intentos: {string.Join(", ", endpoints)}. Detalle: {lastError?.Message}");
        }

        public async Task<List<CategoriaIngresoDto>> GetCategoriasIngresoAsync()
        {
            AddAuthHeader();

            
            var endpoints = new[] { "CategoriasIngreso", "CategoriaIngreso", "CategoriasIngresos", "CategoriaIngresos" };
            Exception? lastError = null;

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var response = await _client.GetAsync($"{_baseUrl}/{endpoint}");
                    if (!response.IsSuccessStatusCode)
                    {
                        lastError = new Exception($"CÃ³digo {(int)response.StatusCode} en /{endpoint}");
                        continue;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<CategoriaIngresoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (data != null)
                        return data;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }
            }

            throw new Exception($"Error al obtener las categorias de ingreso. Intentos: {string.Join(", ", endpoints)}. Detalle: {lastError?.Message}");
        }

        // METAS DE AHORRO
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

        // TIPO DE CAMBIO 
        public async Task<List<TipoCambioDto>> GetTiposCambioExternosAsync()
        {
          
            var response = await _client.GetAsync("https://dolarapi.com/v1/dolares");

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo obtener el tipo de cambio externo");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TipoCambioDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // PERFIL 
        public async Task<UsuarioDto> GetPerfilAsync()
        {
            AddAuthHeader();

            var response = await _client.GetAsync($"{_baseUrl}/Usuarios/me");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener el perfil del usuario");

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UsuarioDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // USUARIOS
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
                throw new Exception($"No se pudo eliminar el usuario. CÃ³digo: {(int)response.StatusCode} - {error}");
            }
        }
    }
}

