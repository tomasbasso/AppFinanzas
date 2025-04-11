using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class TransaccionDto
    {
        public int TransaccionId { get; set; }
        public string Tipo { get; set; } // "Ingreso" o "Gasto"
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int CuentaId { get; set; }
        public int? CategoriaGastoId { get; set; }
        public int? CategoriaIngresoId { get; set; }
        public int UsuarioId { get; set; } 
    }

}
