using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ModelsDto
{
    public class TipoCambioDto
    {
        public string Casa { get; set; }
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public string CasaFormateada => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Casa);
    }

}
