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
        static string AktualitasFajlUtvonal = "ingatlanok.txt";

        static void Main(string[] args)
        {
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
                Console.WriteLine("6. Betöltés");
                Console.WriteLine("7. dsadasd");
                Console.WriteLine("0. Kilépés");
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
                        AdatokBetoltese();
                        break;
                    case "7":
                        UjFajlLetrehozasa();
                        break;
                    case "0":
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
                File.WriteAllLines(AktualitasFajlUtvonal, sorok);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a mentés során: " + ex.Message);
            }
        }

        static void AdatokBetoltese()
        {
            Console.Clear();
            string[] fajlok = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");

            if (fajlok.Length == 0)
            {
                Console.WriteLine("Nincs elérhető .txt fájl.");
                return;
            }

            Console.WriteLine("Válassz fájlt a betöltéshez:");
            for (int i = 0; i < fajlok.Length; i++)
                Console.WriteLine($"{i + 1}. {Path.GetFileName(fajlok[i])}");

            Console.Write("\nSorszám: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= fajlok.Length)
            {
                AktualitasFajlUtvonal = fajlok[index - 1];
                ProbaBetoltes(AktualitasFajlUtvonal);
            }
        }

        static void ProbaBetoltes(string utvonal)
        {
            try
            {
                ingatlanok.Clear();
                string[] sorok = File.ReadAllLines(utvonal);

                foreach (string sor in sorok)
                {
                    if (string.IsNullOrWhiteSpace(sor)) continue;
                    string[] a = sor.Split(';');

                    // Minimum 6 oszlop kell (az eredeti fájlodban annyi van)
                    if (a.Length < 6) continue;

                    try
                    {
                        Ingatlan uj = new Ingatlan
                        {
                            Cim = a[0],
                            Meret = double.Parse(a[1]),
                            IsKiadva = bool.Parse(a[2]),
                            BerloNeve = a[3],
                            BerletiDij = int.Parse(a[4]),
                            SzerzodesVege = DateTime.Parse(a[5])
                        };

                        // Ha van 7. oszlop (Gázvizsga), betöltjük, ha nincs, adunk egy mait
                        if (a.Length >= 7)
                        {
                            uj.GazVizsgaDatum = DateTime.Parse(a[6]);
                        }
                        else
                        {
                            uj.GazVizsgaDatum = DateTime.Now.AddYears(1);
                        }

                        ingatlanok.Add(uj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Hiba a sor feldolgozásakor: {sor} -> {ex.Message}");
                    }
                }
                Console.WriteLine($"\nSikeresen betöltve: {ingatlanok.Count} ingatlan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a fájl megnyitásakor: " + ex.Message);
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

                    kivalasztott.IsKiadva = true; 

                    AdatokMentese(); 

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

        static void UjFajlLetrehozasa()
        {
            Console.Clear();
            Console.WriteLine("ÚJ ADATFÁJL LÉTREHOZÁSA\n");
            Console.Write("Add meg az új fájl nevét (pl. ingatlanok_2024): ");
            string nev = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nev))
            {
                Console.WriteLine("A név nem lehet üres!");
                return;
            }

            if (!nev.EndsWith(".txt")) nev += ".txt";

            if (File.Exists(nev))
            {
                Console.WriteLine("Ez a fájl már létezik! Használd a betöltés funkciót.");
                return;
            }

            try
            {
                File.WriteAllText(nev, "");

                AktualitasFajlUtvonal = nev;
                ingatlanok.Clear(); 

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSikeresen létrehozva és betöltve: {nev}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a fájl létrehozásakor: " + ex.Message);
            }
        }
    }
}