using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        public string Nombre { get; set; }

        [JsonPropertyName("saldoInicial")]
        public decimal Saldo { get; set; }

        [JsonPropertyName("saldoActual")]
        public decimal SaldoActual { get; set; }

        [JsonPropertyName("banco")]
        public string Banco { get; set; }

        [JsonPropertyName("tipoCuenta")]
        public string TipoCuenta { get; set; }
        public int UsuarioId { get; set; }
    }

}
