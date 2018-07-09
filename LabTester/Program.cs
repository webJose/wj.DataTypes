using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wj.DataTypes;

namespace LabTester
{
    class Program
    {
        private const int ColumnWidth = 25;
        private static void OutputMoney(in Money money)
        {
            Console.WriteLine($"{{1,-{ColumnWidth}}}{{0,{ColumnWidth}:I}}{{0,{ColumnWidth}:G}}{{0,{ColumnWidth}:O}}{{0,{ColumnWidth}:O4}}", money, money.RegionCulture.Culture.NativeName);
        }
        static void Main(string[] args)
        {
            Random r = new Random();
            string fileName = "moneyoutput.txt";
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    TextWriter oldWriter = Console.Out;
                    Console.SetOut(sw);
                    foreach (RegionCulture rc in Money.AllMoneyRegions)
                    {
                        Money m = new Money(r.Next(-99999, 99999), rc);
                        OutputMoney(m);
                    }
                    Console.SetOut(oldWriter);
                }
            }
            ProcessStartInfo psi = new ProcessStartInfo(fileName);
            psi.UseShellExecute = true;
            using (Process p = Process.Start(psi))
            { }
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            Console.WriteLine($"Total cultures.  {cultures.Length}");
            Console.WriteLine();
            Console.WriteLine("{0,-30}{1,-10}{2,-20}", "Culture Name", "Currency", "Region Name");
            foreach(CultureInfo ci in cultures)
            {
                RegionInfo ri = null;
                try
                {
                    ri = new RegionInfo(ci.LCID);
                }
                catch { }
                //if (ri != null) continue;
                Console.WriteLine("{0,-30}{1,-10}{2,-20}", ci.Name, ri?.ISOCurrencySymbol, ri?.TwoLetterISORegionName);
            }
        }
    }
}
