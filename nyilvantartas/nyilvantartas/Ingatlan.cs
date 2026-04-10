using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngatlanNyilvantarto
{
    public class Ingatlan
    {
        public string Cim { get; set; }
        public double Meret { get; set; }
        public bool IsKiadva { get; set; }
        public string BerloNeve { get; set; }
        public int BerletiDij { get; set; }
        public DateTime SzerzodesVege { get; set; }
        public DateTime GazVizsgaDatum { get; set; }

        public string ToFileFormat()
        {
            return $"{Cim};{Meret};{IsKiadva};{BerloNeve};{BerletiDij};{SzerzodesVege}";
        }

        public override string ToString()
        {
            string statusz = IsKiadva ? $"Kiadva ({BerloNeve}) - {BerletiDij:N0} Ft" : "SZABAD";
            return $"{Cim.PadRight(20)} | {Meret,5} m2 | {statusz.PadRight(35)} | Lejárat: {SzerzodesVege.ToShortDateString()}";
        }
    }
}
