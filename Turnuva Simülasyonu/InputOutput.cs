using System;

namespace Turnuva_Simülasyonu
{
    abstract class InputOutput
    {
        /// <summary>
        /// Dosya yolunu kaydetmek ve daha sonra TakımBilgisi classında takım sayısını almak için gerekli.
        /// </summary>
        private static string path;
        public static string DosyaYolu { get { return path; } set { path = value; } }

        /// <summary>
        /// path dosyasıdan takımları alıp Takım dizisine sıralı bir şekilde koy.
        /// </summary>
        /// <param name="path">Takımlar dosyasının yolu.</param>
        /// <returns>Takımlar dizisi; eğer sorun olmazsa, null; hata olursa.</returns>
        public static Takim[] TakimlariAl(string path)
        {
            try
            {
                DosyaYolu = path;
                string[] lines = System.IO.File.ReadAllLines(path); // https://msdn.microsoft.com/en-us/library/ezwyzy7b.aspx
                Takim[] takimlar = new Takim[TakimlarBilgisi.TakimSayisi];
                // Takım sayısı 32 64 ve 128 olmalı.
                if (TakimlarBilgisi.TakimSayisi != 32 && TakimlarBilgisi.TakimSayisi != 64 && TakimlarBilgisi.TakimSayisi != 128)
                {
                    return null;
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] ad_guc = lines[i].Split(',');
                    if (i < TakimlarBilgisi.SeriBasiSayisi)
                    {
                        takimlar[i] = new Takim(ad_guc[0], Convert.ToInt32(ad_guc[1]), true);
                    }
                    else
                    {
                        takimlar[i] = new Takim(ad_guc[0], Convert.ToInt32(ad_guc[1]), false);
                    }
                }
                Array.Sort(takimlar,
    delegate (Takim x, Takim y) { return (-1 * x.TakimGucu).CompareTo(-1 * y.TakimGucu); }); // http://stackoverflow.com/questions/1304278/how-to-sort-an-array-containing-class-objects-by-a-property-value-of-a-class-ins

                return takimlar;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Turnuvayı başlatır ve sonuna kadar kullanıcıdan tuş alarak oynatır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        public static void TurnuvayiOynatVeBitir(Takim[] takimlar)
        {
            Console.WriteLine("Turnuvayı başlatmak için bir tuşa basınız.{0}", Environment.NewLine);
            Console.ReadKey(true);

            MacIslemleri.Kura(takimlar);

            for (int i = 1; i < TakimlarBilgisi.TurSayisi + 1; i++)
            {
                MacIslemleri.BirTurOynat(takimlar);
                MacIslemleri.TurOzeti(takimlar, i);
                Console.WriteLine();
                if (!(i == TakimlarBilgisi.TurSayisi))
                {
                    Console.WriteLine("{0}. turu oynatmak için bir tuşa basınız.{1}", i + 1, Environment.NewLine);
                    Console.ReadKey(true);
                }
            }

            // Kazanan takımı bul ve bilgilerini yazdır
            Console.WriteLine("Kazanan takım bilgilerini görmek için bir tuşa basınız.{0}", Environment.NewLine);
            Console.ReadKey(true);
            for (int i = 0; i < takimlar.Length; i++)
            {
                if (!takimlar[i].TakimElendi)
                {
                    TakimBilgisiYazdir(takimlar, i);
                    break;
                }
            }
        }

        /// <summary>
        /// Tur veya takım arar ve bilgilerini yazdırır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        public static void BilgiYazdirVeProgramiKapat(Takim[] takimlar)
        {
            Console.WriteLine("Tur bilgisi için 1, Takım bilgisi için 2, programı kapatmak için 0 giriniz.");

            ConsoleKeyInfo input = Console.ReadKey(); // https://msdn.microsoft.com/en-us/library/system.consolekey.aspx
            Console.WriteLine();
            char girdi = input.KeyChar;
            if (girdi == '0')
            {
                Environment.Exit(0);
            }
            else if (girdi == '1')
            {
                TurAraVeYazdir(takimlar);
            }
            else if (girdi == '2')
            {
                TakimAraVeYazdir(takimlar);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Takımı adıyla arar ve bulduysa takım bilgilerini yazdırır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <returns>Takım bulunduysa 0 döndürür, bulunamadıysa -1.</returns>
        private static int TakimAraVeYazdir(Takim[] takimlar)
        {
            Console.WriteLine("Takım adı giriniz: ");
            string aranacak_takim = Console.ReadLine();
            for (int i = 0; i < TakimlarBilgisi.TakimSayisi; i++)
            {
                if (aranacak_takim.Equals(takimlar[i].TakimAdi, StringComparison.OrdinalIgnoreCase) == true) // https://msdn.microsoft.com/en-us/library/cc165449.aspx
                {
                    TakimBilgisiYazdir(takimlar, i);
                    return 0;
                }
            }
            Console.WriteLine("Takım bulunamadı.");
            return -1;
        }

        /// <summary>
        /// Turu numarasıyla arar ve bulduysa bilgilerini yazdırır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <returns>Tur geçerliyse 0, geçerli değilse -1.</returns>
        private static int TurAraVeYazdir(Takim[] takimlar)
        {
            Console.WriteLine("Tur numarası giriniz");
            string input = Console.ReadLine(); // https://msdn.microsoft.com/en-us/library/system.consolekey.aspx
            Console.WriteLine();
            bool isNumeric = int.TryParse(input, out int n); // http://stackoverflow.com/a/894271
            if (isNumeric)
            {
                if (MacIslemleri.TurOzeti(takimlar, n) == -1)
                {
                    Console.WriteLine("Tur sayısı 1 ile {0} arasında olmalıdır.", TakimlarBilgisi.TurSayisi);
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("Lütfen sayı giriniz.");
                return -1;
            }
        }

        /// <summary>
        /// Verilen Takim dizisinden takimIndisi indisli takımın bilgilerini yazdırır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <param name="takimIndisi">Takımın indisi.</param>
        public static void TakimBilgisiYazdir(Takim[] takimlar, int takimIndisi)
        {
            string kazandi = "";
            if (!takimlar[takimIndisi].TakimElendi)
            {
                kazandi = "Kazanan ";
            }

            Console.WriteLine("{0}Takım Adı: {1}", kazandi, takimlar[takimIndisi].TakimAdi);
            Console.WriteLine("{0}Takım {1}", kazandi, takimlar[takimIndisi].TakimSeriBasiMi ? "seribaşı." : "seribaşı değil.");
            Console.WriteLine("{0}Takım Nosu: {1}", kazandi, (takimIndisi + 1).ToString());

            int attigiSkor = 0;
            int yedigiSkor = 0;
            for (int i = 0; i < takimlar[takimIndisi].Maclar.Length; i++)
            {
                if (takimlar[takimIndisi].Maclar[i] == null)
                {
                    break;
                }

                MacYazdir(takimlar, takimIndisi, i);

                if (takimlar[takimIndisi].Maclar[i].ATakimi == takimlar[takimIndisi])
                {
                    attigiSkor += takimlar[takimIndisi].Maclar[i].ASkoru;
                    yedigiSkor += takimlar[takimIndisi].Maclar[i].BSkoru;
                }
                else
                {
                    attigiSkor += takimlar[takimIndisi].Maclar[i].BSkoru;
                    yedigiSkor += takimlar[takimIndisi].Maclar[i].ASkoru;
                }
            }
            Console.WriteLine("{0}Takımın Attığı Sayı: {1}", kazandi, attigiSkor.ToString());
            Console.WriteLine("{0}Takımın Yediği Sayı: {1}", kazandi, yedigiSkor.ToString());
            Console.WriteLine("{0}Takım Averajı: {1}", kazandi, (attigiSkor - yedigiSkor).ToString());
            Console.WriteLine();
        }

        /// <summary>
        /// Takimlar dizisinden takimIndisi indisli takımın turNumarasi numaralı maçını yazdırır.
        /// turNumarası geçersiz ise fonksiyon işlem yapmaz.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <param name="takimIndisi">Takımın indisi.</param>
        /// <param name="turNumarasi">Tur numarası.</param>
        internal static void MacYazdir(Takim[] takimlar, int takimIndisi, int turNumarasi)
        {
            if (takimlar[takimIndisi].Maclar[turNumarasi] == null)
            {
                return;
            }
            Console.WriteLine("{0}. Tur: {1} ({3}) - ({4}) {2}", turNumarasi + 1,
                takimlar[takimIndisi].Maclar[turNumarasi].ATakimi.TakimAdi, takimlar[takimIndisi].Maclar[turNumarasi].BTakimi.TakimAdi,
                takimlar[takimIndisi].Maclar[turNumarasi].ASkoru, takimlar[takimIndisi].Maclar[turNumarasi].BSkoru);
        }

        public static void DosyaHatasi()
        {
            Console.WriteLine("Dosyanın formatı doğru değil. Lütfen virgüllerle ayırdığınız takım adı ve güç puanlarını giriniz. {0}"
                + "Takım sayısı 32, 64 ve 128 olabilir. Boş satır bırakmadığınıza emin olunuz.", Environment.NewLine);
            Console.ReadKey(true);
        }

    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array) // hazır shuffle fonksiyonu(http://stackoverflow.com/a/110570)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
