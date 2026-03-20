namespace nyilvantartas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace IngatlanNyilvantarto
    {
        // 1. Az Ingatlan osztály definiálása
       

        class Program
        {
            static List<Ingatlan> ingatlanok = new List<Ingatlan>();

            static void Main(string[] args)
            {
                // Teszt adatok hozzáadása
                MintaAdatok();

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== INGATLAN-NYILVÁNTARTÓ RENDSZER ===");
                    Console.WriteLine("1. Ingatlanok listázása");
                    Console.WriteLine("2. Új ingatlan felvétele");
                    Console.WriteLine("3. Kilépés");
                    Console.Write("\nVálassz opciót: ");

                    string opcio = Console.ReadLine();

                    switch (opcio)
                    {
                        case "1":
                            Listazas();
                            break;
                        case "2":
                            Hozzaadas();
                            break;
                        case "3":
                            return;
                        default:
                            Console.WriteLine("Érvénytelen opció!");
                            break;
                    }
                    Console.WriteLine("\nNyomj egy gombot a folytatáshoz...");
                    Console.ReadKey();
                }
            }

            static void Listazas()
            {
                Console.WriteLine("\nCím                  | Méret  | Állapot / Bérlő                      | Határidő");
                Console.WriteLine("--------------------------------------------------------------------------------");
                foreach (var i in ingatlanok)
                {
                    Console.WriteLine(i.ToString());
                }
            }

            static void Hozzaadas()
            {
                Ingatlan uj = new Ingatlan();
                Console.Write("Cím: "); uj.Cim = Console.ReadLine();
                Console.Write("Méret (m2): "); uj.Meret = double.Parse(Console.ReadLine());
                Console.Write("Kiadva? (i/n): "); uj.IsKiadva = Console.ReadLine().ToLower() == "i";

                if (uj.IsKiadva)
                {
                    Console.Write("Bérlő neve: "); uj.BerloNeve = Console.ReadLine();
                    Console.Write("Havi díj: "); uj.BerletiDij = int.Parse(Console.ReadLine());
                }
                uj.SzerzodesVege = DateTime.Now.AddYears(1); // Alapértelmezett 1 év

                ingatlanok.Add(uj);
                Console.WriteLine("Sikeresen hozzáadva!");
            }

            static void MintaAdatok()
            {
                ingatlanok.Add(new Ingatlan { Cim = "Fő utca 1.", Meret = 50, IsKiadva = true, BerloNeve = "Kovács János", BerletiDij = 150000, SzerzodesVege = new DateTime(2026, 05, 10) });
                ingatlanok.Add(new Ingatlan { Cim = "Tölgyfa u. 12.", Meret = 35, IsKiadva = false, SzerzodesVege = DateTime.Now });
            }
        }
    }
}
