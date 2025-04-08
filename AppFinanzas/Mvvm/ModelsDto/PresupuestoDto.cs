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
        public decimal MontoLimite { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CategoriaGastoId { get; set; }

        // Opcional para mostrar
        public string NombreCategoria { get; set; }
    }
}
