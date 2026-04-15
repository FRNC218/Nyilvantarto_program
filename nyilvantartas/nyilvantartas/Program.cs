using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IngatlanNyilvantarto
{
    //Ingatlan adatai: Cím, helyrajzi szám, méret ($m ^ 2$).
    //Bérlő adatai: Név, szerződés kezdete/ vége, kaució összege.
    //Mérőóra állások: Víz, gáz, villany(havi bontásban).
    //Költségek: Közös költség, javítási alap, biztosítás.
    //Emlékeztető: Mikor esedékes a következő gázkészülék - felülvizsgálat.
    

    class Program
    {
        static List<Ingatlan> ingatlanok = new List<Ingatlan>();
        const string FajlNev = "ingatlanok.txt"; // A fájl a .exe mellett lesz

        static void Main(string[] args)
        {
            // Induláskor betöltjük az adatokat a fájlból
            AdatokBetoltese();

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("================================================");
                Console.WriteLine("      PROFI INGATLAN-NYILVÁNTARTÓ RENDSZER      ");
                Console.WriteLine("================================================");
                Console.ResetColor();
                Console.WriteLine("1. Ingatlanok listázása");
                Console.WriteLine("2. Új ingatlan felvétele");
                Console.WriteLine("3. Pénzügyi statisztika");
                Console.WriteLine("4. Exportalas CSV fájlba");
                Console.WriteLine("5. Ingatlan kiadása");
                Console.WriteLine("6. Kilépés");
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
                        MegjelenitStatisztika();
                        break;
                    case "4":
                        ExportalasCSV();
                        break;
                    case "5":
                        IngatlanKiadasa();
                        break;
                    case "6":
                        Console.WriteLine("Viszlát!");
                        return;
                    default:
                        Console.WriteLine("Nincs ilyen opció!");
                        break;
                }
                Console.WriteLine("\nNyomj meg egy gombot a menübe való visszatéréshez...");
                Console.ReadKey();
            }
        }

        static void Listazas()
        {
            Console.Clear();
            Console.WriteLine("AKTUÁLIS INGATLANLISTA (Színkódolt figyelmeztetésekkel)\n");

            if (ingatlanok.Count == 0)
            {
                Console.WriteLine("A lista üres.");
                return;
            }

            Console.WriteLine("Cím                  | Méret    | Állapot / Bérlő           | Határidő / Megjegyzés");
            Console.WriteLine(new string('-', 95));

            foreach (var i in ingatlanok)
            {
                // ALAPÉRTELMEZETT SZÍN: Fehér
                Console.ResetColor();

                // 1. KRITIKUS: Lejárt gázvizsga vagy 30 napon belül lejáró szerződés -> PIROS
                if (i.GazVizsgaDatum < DateTime.Now || (i.IsKiadva && i.SzerzodesVege < DateTime.Now.AddDays(30)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                // 2. FIGYELMEZTETÉS: Gázvizsga 60 napon belül esedékes -> SÁRGA
                else if (i.GazVizsgaDatum < DateTime.Now.AddDays(60))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                // 3. RENDBEN: Szabad ingatlan -> ZÖLD
                else if (!i.IsKiadva)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine(i.ToString());
            }
            Console.ResetColor();

            Console.WriteLine("\nJelmagyarázat:");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("■ Piros: "); Console.ResetColor(); Console.WriteLine("Lejárt");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("■ Sárga: "); Console.ResetColor(); Console.WriteLine("hamarosan lejár");
            Console.ForegroundColor = ConsoleColor.Green; Console.Write("■ Zöld:  "); Console.ResetColor(); Console.WriteLine("Szabadon kiadható ingatlan");
        }


        static void Hozzaadas()
        {
            Console.Clear();
            Console.WriteLine("ÚJ INGATLAN FELVÉTELE\n");
            try
            {
                Ingatlan uj = new Ingatlan();
                Console.Write("Cím: ");
                uj.Cim = Console.ReadLine();

                Console.Write("Méret (m2): ");
                uj.Meret = double.Parse(Console.ReadLine());

                Console.Write("Kiadva van már? (i/n): ");
                uj.IsKiadva = Console.ReadLine().ToLower() == "i";

                if (uj.IsKiadva)
                {
                    Console.Write("Bérlő neve: ");
                    uj.BerloNeve = Console.ReadLine();
                    Console.Write("Havi bérleti díj (Ft): ");
                    uj.BerletiDij = int.Parse(Console.ReadLine());
                    Console.Write("Szerződés lejárata (éééé-hh-nn): ");
                    uj.SzerzodesVege = DateTime.Parse(Console.ReadLine());
                }
                else
                {
                    uj.BerloNeve = "N/A";
                    uj.BerletiDij = 0;
                    uj.SzerzodesVege = DateTime.Now;
                }

                ingatlanok.Add(uj);
                AdatokMentese(); // Mentés a fájlba minden hozzáadás után
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nIngatlan sikeresen rögzítve és elmentve a fájlba!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nHiba történt: {ex.Message} Kérlek, figyelj a formátumokra!");
                Console.ResetColor();
            }
        }

        static void MegjelenitStatisztika()
        {
            Console.Clear();
            Console.WriteLine("PÉNZÜGYI ÉS INGATLAN STATISZTIKA\n");

            if (ingatlanok.Count == 0)
            {
                Console.WriteLine("Nincs adat a statisztikához.");
                return;
            }

            int osszes = ingatlanok.Count;
            int kiadott = ingatlanok.Count(i => i.IsKiadva);
            int haviBevetel = ingatlanok.Where(i => i.IsKiadva).Sum(i => i.BerletiDij);
            double atlagMeret = ingatlanok.Average(i => i.Meret);

            Console.WriteLine($"Ingatlanok száma összesen: {osszes} db");
            Console.WriteLine($"Ebből kiadva:             {kiadott} db");
            Console.WriteLine($"Kihasználtság:            {(double)kiadott / osszes:P1}"); // P1: Százalékos formátum
            Console.WriteLine($"Összes havi bevétel:      {haviBevetel:N0} Ft");
            Console.WriteLine($"Átlagos ingatlan méret:   {atlagMeret:F1} m2");
        }

        static void AdatokMentese()
        {
            try
            {
                var sorok = ingatlanok.Select(i => i.ToFileFormat());
                File.WriteAllLines(FajlNev, sorok);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a mentés során: " + ex.Message);
            }
        }

        static void AdatokBetoltese()
        {
            if (File.Exists(FajlNev))
            {
                try
                {
                    string[] sorok = File.ReadAllLines(FajlNev);
                    foreach (string sor in sorok)
                    {
                        if (string.IsNullOrWhiteSpace(sor)) continue;
                        string[] adatok = sor.Split(';');
                        ingatlanok.Add(new Ingatlan
                        {
                            Cim = adatok[0],
                            Meret = double.Parse(adatok[1]),
                            IsKiadva = bool.Parse(adatok[2]),
                            BerloNeve = adatok[3],
                            BerletiDij = int.Parse(adatok[4]),
                            SzerzodesVege = DateTime.Parse(adatok[5])
                        });
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("A korábbi adatfájl sérült, új listát kezdünk.");
                }
            }
        }
        static void ExportalasCSV()
        {
            string csvFajlNev = $"ingatlan_riport_{DateTime.Now:yyyyMMdd}.csv";

            try
            {
                using (StreamWriter sw = new StreamWriter(csvFajlNev, false, Encoding.UTF8))
                {
                    // Fejléc írása (Excel számára fontos oszlopnevek)
                    sw.WriteLine("Cím;Méret (m2);Állapot;Bérlő;Havi Díj;Szerződés Vége;Gázvizsga Dátuma");

                    foreach (var i in ingatlanok)
                    {
                        string statusz = i.IsKiadva ? "Kiadva" : "Szabad";
                        // Egy sor összeállítása pontosvesszővel elválasztva
                        sw.WriteLine($"{i.Cim};{i.Meret};{statusz};{i.BerloNeve};{i.BerletiDij};{i.SzerzodesVege:yyyy-MM-dd};{i.GazVizsgaDatum:yyyy-MM-dd}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSikeres exportálás! A fájl neve: {csvFajlNev}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba az exportálás során: " + ex.Message);
            }
        }
        static void IngatlanKiadasa()
        {
            Console.Clear();
            Console.WriteLine("INGATLAN KIADÁSA\n");

            // Csak a szabad ingatlanokat listázzuk ki a választáshoz
            var szabadIngatlanok = ingatlanok.Where(i => !i.IsKiadva).ToList();

            if (szabadIngatlanok.Count == 0)
            {
                Console.WriteLine("Nincs jelenleg szabadon kiadható ingatlan a rendszerben.");
                return;
            }

            Console.WriteLine("Válassz egy szabad ingatlant (sorszám):");
            for (int i = 0; i < szabadIngatlanok.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {szabadIngatlanok[i].Cim} ({szabadIngatlanok[i].Meret} m2)");
            }

            Console.Write("\nSorszám: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= szabadIngatlanok.Count)
            {
                Ingatlan kivalasztott = szabadIngatlanok[index - 1];

                try
                {
                    Console.WriteLine($"\nKiadás folyamatban: {kivalasztott.Cim}");

                    Console.Write("Bérlő neve: ");
                    kivalasztott.BerloNeve = Console.ReadLine();

                    Console.Write("Havi bérleti díj (Ft): ");
                    kivalasztott.BerletiDij = int.Parse(Console.ReadLine());

                    Console.Write("Szerződés lejárata (éééé-hh-nn): ");
                    kivalasztott.SzerzodesVege = DateTime.Parse(Console.ReadLine());

                    kivalasztott.IsKiadva = true; // Átállítjuk a státuszt

                    AdatokMentese(); // Azonnal mentjük a fájlba a változást

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nAz ingatlan sikeresen kiadva!");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nHiba az adatok megadásakor: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("Érvénytelen sorszám.");
            }
        }
    }
}