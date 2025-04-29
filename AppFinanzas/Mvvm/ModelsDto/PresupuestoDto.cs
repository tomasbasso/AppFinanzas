using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class PresupuestoDto
    {
        public int PresupuestoId { get; set; }
        public int UsuarioId { get; set; }
        public int CategoriaGastoId { get; set; }
        public decimal MontoLimite { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }

        // Opcional para mostrar
        public string NombreCategoria { get; set; }
     
    }
}
