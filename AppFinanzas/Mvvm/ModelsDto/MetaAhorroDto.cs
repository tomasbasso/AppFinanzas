using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class MetaAhorroDto
    {
        public int MetaId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public decimal MontoObjetivo { get; set; }
        public DateTime FechaLimite { get; set; }
        public decimal ProgresoActual { get; set; }
    }
}