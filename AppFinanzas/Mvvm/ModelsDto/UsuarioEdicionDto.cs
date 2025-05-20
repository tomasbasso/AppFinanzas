using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class UsuarioEdicionDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public string Contrasena { get; set; }
    }
}
