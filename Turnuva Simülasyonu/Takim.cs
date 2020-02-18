using System;
using System.IO;

namespace Turnuva_Simülasyonu
{
    abstract class TakimlarBilgisi
    {
        private static readonly Random rand;

        /// <summary>
        /// Takım sayısı.
        /// </summary>
        public static int TakimSayisi => File.ReadAllLines(InputOutput.DosyaYolu).Length; // https://msdn.microsoft.com/en-us/library/x9fsa0sw.aspx
        /// <summary>
        /// Seri başı sayısı.
        /// </summary>
        public static int SeriBasiSayisi => TakimSayisi / 8;
        /// <summary>
        /// Tur
        /// </summary>
        public static int TurSayisi => (int)Math.Log(TakimSayisi, 2);
        public static Random Rand { get { return rand; } }

        static TakimlarBilgisi()
        {
            rand = new Random();
        }
    }

    class Takim
    {
        private readonly string ad;
        private readonly int guc;
        private readonly bool seribasi;
        private MacBilgisi[] macDizisi = new MacBilgisi[TakimlarBilgisi.TurSayisi];

        /// <summary>
        /// Takımın elendi bilgisi.
        /// </summary>
        public bool TakimElendi { get; set; }
        /// <summary>
        /// Takımın adı.
        /// </summary>
        public string TakimAdi { get { return ad; } }
        /// <summary>
        /// Takımın gücü (100 üzerinden).
        /// </summary>
        public int TakimGucu { get { return guc; } }
        /// <summary>
        /// Takım seri başı ise true.
        /// </summary>
        public bool TakimSeriBasiMi { get { return seribasi; } }
        /// <summary>
        /// Takımın maç geçmişi.
        /// </summary>
        public MacBilgisi[] Maclar { get { return macDizisi; } set { macDizisi = value; } }

        /// <summary>
        /// Yeni takım nesnesi oluşturur.
        /// </summary>
        /// <param name="ad">Takım adı.</param>
        /// <param name="guc">Takımın gücü 0 ile 100 arasında olmalıdır.</param>
        /// <param name="seribasi">Takımın seri başı bilgisi.</param>
        public Takim(string ad, int guc, bool seribasi)
        {
            this.ad = ad;
            if (guc < 0)
            {
                this.guc = 0;
            }
            else if (guc > 100)
            {
                this.guc = 100;
            }
            else
            {
                this.guc = guc;
            }
            this.seribasi = seribasi;
            TakimElendi = false;
        }
    }

    class MacBilgisi
    {
        private readonly int aSkoru;
        private readonly int bSkoru;
        private readonly Takim a;
        private readonly Takim b;

        /// <summary>
        /// A takımının skoru.
        /// </summary>
        public int ASkoru { get { return aSkoru; } }
        /// <summary>
        /// B takımının skoru.
        /// </summary>
        public int BSkoru { get { return bSkoru; } }
        /// <summary>
        /// A takımı nesnesi.
        /// </summary>
        public Takim ATakimi { get { return a; } }
        /// <summary>
        /// B takımı nesnesi.
        /// </summary>
        public Takim BTakimi { get { return b; } }

        public MacBilgisi(Takim a, Takim b, int a_skoru, int b_skoru)
        {
            aSkoru = a_skoru;
            bSkoru = b_skoru;
            this.a = a;
            this.b = b;
        }
    }
}