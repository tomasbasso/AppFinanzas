using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class CategoriaGastoDto
    {
        public int CategoriaGastoId { get; set; }
        public string Nombre { get; set; }
        public int? UsuarioId { get; set; }

        // public UsuarioDto Usuario { get; set; } 
        
    }
}

