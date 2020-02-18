using System;
using System.IO;

namespace Turnuva_Simülasyonu
{
    class Program
    {
        static void Main(string[] args)
        {
            // Takımları al
            Takim[] takimlar = InputOutput.TakimlariAl(Path.Combine(Environment.CurrentDirectory), "Takımlar.txt");
            if (takimlar == null)
            {
                InputOutput.DosyaHatasi();
                return;
            }

            InputOutput.TurnuvayiOynatVeBitir(takimlar);

            while (true)
            {
                InputOutput.BilgiYazdirVeProgramiKapat(takimlar);
            }
        }
    }
}