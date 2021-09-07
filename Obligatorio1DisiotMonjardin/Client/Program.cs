using System;
using System.Collections.Generic;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("ver", () => Console.WriteLine("opcion ver XD"));
            opciones.Add("comprar", () => Console.WriteLine("no compre este juego"));
            opciones.Add("eliminar", () => Console.WriteLine("seguro que lo quiere borrar"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            opciones.Add("explota", () => Main(args));

            CliMenu.showMenu(opciones, "Menuuuu");
        }
    }
}
