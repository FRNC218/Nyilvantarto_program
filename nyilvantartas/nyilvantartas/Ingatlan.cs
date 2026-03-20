using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nyilvantartas
{
    public class Ingatlan
    {
        public string Cim { get; set; }
        public double Meret { get; set; } // m2
        public string BerloNeve { get; set; }
        public int BerletiDij { get; set; }
        public DateTime SzerzodesVege { get; set; }
        public bool IsKiadva { get; set; }

        public override string ToString()
        {
            string statusz = IsKiadva ? $"Kiadva ({BerloNeve}) - {BerletiDij} Ft/hó" : "SZABAD";
            return $"{Cim.PadRight(20)} | {Meret} m2 | {statusz} | Lejárat: {SzerzodesVege.ToShortDateString()}";
        }
    }
}
