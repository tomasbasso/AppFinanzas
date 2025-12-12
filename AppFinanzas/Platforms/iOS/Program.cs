using ObjCRuntime;
using UIKit;

namespace AppFinanzas
{
    public class Program
    {
        // Punto de entrada principal de la app.
        static void Main(string[] args)
        {
            // si queres usar otra clase Application Delegate distinta de AppDelegate
            // indicalo aca.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}

