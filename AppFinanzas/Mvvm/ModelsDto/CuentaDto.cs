using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        public string Nombre { get; set; }
        public string Banco { get; set; }
        public decimal Saldo { get; set; }
    }
}
