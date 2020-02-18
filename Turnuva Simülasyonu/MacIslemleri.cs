using System;

namespace Turnuva_Simülasyonu
{
    class MacIslemleri
    {
        private static int oynananTur = 0;
        private static int[][,] eslesme = new int[TakimlarBilgisi.TurSayisi + 1][,];
        /// <summary>
        /// Oynanan tur sayısını tutar.
        /// </summary>
        private static int OynananTur { get { return oynananTur; } set { oynananTur = value; } }
        /// <summary>
        /// Eşleşmeleri tutan dizidir. 0. indis, kura sonuçları; (tur sayısı). indis ise kazanan takımı verir.
        /// </summary>
        private static int[][,] Eslesme { get { return eslesme; } set { eslesme = value; } }

        /// <summary>
        /// İlk kura çekimini yapar. Kura çekimi zaten yapılmışsa fonksiyon işlem yapmaz.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <returns>Kura sonucunu döndürür.</returns>
        public static int[,] Kura(Takim[] takimlar)
        {
            if (Eslesme[0] != null) //Kura çekilmişse
            {
                return null;
            }

            int elenenler = 0;
            for (int i = 0; i < takimlar.Length; i++)
            {
                if (takimlar[i].TakimElendi)
                {
                    elenenler++;
                }
            }

            int elenmeyenler = takimlar.Length - elenenler;
            int[,] eslesme = new int[2, elenmeyenler / 2];

            for (int i = 0; i < TakimlarBilgisi.SeriBasiSayisi; i++)
            {
                if (i % 2 == 0)
                {
                    eslesme[0, 4 * i] = i;
                }
                if (i % 2 == 1)
                {
                    eslesme[0, (TakimlarBilgisi.TakimSayisi / 2) - (4 * i)] = i;
                }
            }

            int[] seribasiOlmayanTakimlar = new int[TakimlarBilgisi.TakimSayisi - TakimlarBilgisi.SeriBasiSayisi];

            for (int i = TakimlarBilgisi.SeriBasiSayisi; i < TakimlarBilgisi.TakimSayisi; i++)
            {
                seribasiOlmayanTakimlar[i - TakimlarBilgisi.SeriBasiSayisi] = i;
            }

            TakimlarBilgisi.Rand.Shuffle(seribasiOlmayanTakimlar);

            for (int i = 0, j = 0; i < TakimlarBilgisi.TakimSayisi / 2; i++)
            {
                if (i % 4 == 0)
                {
                    eslesme[1, i] = seribasiOlmayanTakimlar[j];
                }
                else
                {
                    eslesme[0, i] = seribasiOlmayanTakimlar[j];
                    j++;
                    eslesme[1, i] = seribasiOlmayanTakimlar[j];
                }
                j++;
            }

            Eslesme[0] = eslesme;
            return eslesme;
        }

        /// <summary>
        /// Kura çekilmemişse kurayı çeker ve ilk maçları oynatır. Kura çekilmişse bir sonraki turu oynatır.
        /// Final maçı da oynanılmışsa fonksiyon işlem yapmaz.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        public static void BirTurOynat(Takim[] takimlar)
        {
            if (OynananTur == 0)
            {
                if (Eslesme[0] == null) //Kura çekilmemiş demektir
                {
                    Eslesme[0] = Kura(takimlar); // Kurayı çek
                }
                Eslesme[1] = TuruOynat(takimlar, Eslesme[0]); // İlk turu oynat
            }
            else if (OynananTur < TakimlarBilgisi.TurSayisi)
            {
                Eslesme[OynananTur + 1] = TuruOynat(takimlar, Eslesme[OynananTur]);
            }
            else
            {
                return;
            }
            OynananTur++;
        }

        /// <summary>
        /// Aldığı eşleşmeleri oynatır.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <param name="eslesme">Kura fonksiyonundan dönen eşleşme dizisi.</param>
        /// <returns>Bir sonraki turun karşılaşmalarının dizisini döndürür.</returns>
        private static int[,] TuruOynat(Takim[] takimlar, int[,] eslesme)
        {
            // Final maçı oynanmış demektir
            if (eslesme.GetLength(0) == 1 && eslesme.GetLength(1) == 1)
            {
                return null;
            }

            // Oynanmayan sadece final maçı var
            if (eslesme.GetLength(1) == 1)
            {
                int[,] kazanan = new int[1, 1];
                bool sonuc = MaciOynat(takimlar[eslesme[0, 0]], takimlar[eslesme[1, 0]]);
                if (sonuc) // A kazandı
                {
                    kazanan[0, 0] = eslesme[0, 0];
                }
                else // B kazandı
                {
                    kazanan[0, 0] = eslesme[1, 0];
                }

                return kazanan;
            }

            // Birden fazla oynanmayan maç var
            int[,] kazananlar = new int[2, eslesme.GetLength(1) / 2];
            for (int i = 0; i < eslesme.GetLength(1); i++)
            {
                bool sonuc = MaciOynat(takimlar[eslesme[0, i]], takimlar[eslesme[1, i]]);
                if (sonuc) // A kazandı
                {
                    kazananlar[i % 2, i / 2] = eslesme[0, i];
                }
                else // B kazandı
                {
                    kazananlar[i % 2, i / 2] = eslesme[1, i];
                }
            }

            return kazananlar;
        }

        /// <summary>
        /// Verilen tur numarasının maç sonuçlarını yazdırır. Hatalı tur numarası girişinde işlem yapılmaz.
        /// </summary>
        /// <param name="takimlar">Takımların bulunduğu Takım[].</param>
        /// <param name="turNumarasi">Yazdırılacak tur numarası.</param>
        /// <returns>Tur geçerliyse 0, geçerli değilse -1.</returns>
        public static int TurOzeti(Takim[] takimlar, int turNumarasi)
        {
            if (turNumarasi <= 0 || turNumarasi > TakimlarBilgisi.TurSayisi)
            {
                return -1;
            }

            turNumarasi--;
            for (int i = 0; i < Eslesme[turNumarasi].GetLength(1); i++)
            {
                InputOutput.MacYazdir(takimlar, Eslesme[turNumarasi][0, i], turNumarasi);
            }
            return 0;
        }

        /// <summary>
        /// a kazanırsa true döndürür, b kazanırsa false. Kaybeden takımın elendi fieldını true yapar.
        /// </summary>
        /// <param name="a">A takımı objesi.</param>
        /// <param name="b">B takımı objesi.</param>
        /// <returns>a kazanırsa true, b kazanırsa false.</returns>
        private static bool MaciOynat(Takim a, Takim b)
        {
            int a_gucu = (int)Math.Pow(a.TakimGucu, 4);
            int b_gucu = (int)Math.Pow(b.TakimGucu, 4);
            int temp = a_gucu + b_gucu;
            int rand = TakimlarBilgisi.Rand.Next(0, temp);
            int[] skorlar;
            if (rand < a_gucu)
            {
                if (a.TakimGucu >= b.TakimGucu)
                {
                    skorlar = SkorUret(a.TakimGucu - b.TakimGucu, true);
                }
                else
                {
                    skorlar = SkorUret(b.TakimGucu - a.TakimGucu, false);
                }
                // Oynanmamış ilk maça maç skorunu yerleştir.
                for (int i = 0; i < a.Maclar.Length; i++)
                {
                    if (a.Maclar[i] == null)
                    {
                        a.Maclar[i] = new MacBilgisi(a, b, skorlar[1], skorlar[0]);
                        b.Maclar[i] = a.Maclar[i];
                        break;
                    }
                }

                b.TakimElendi = true;

                return true;
            }
            else
            {
                if (b.TakimGucu >= a.TakimGucu)
                {
                    skorlar = SkorUret(b.TakimGucu - a.TakimGucu, true);
                }
                else
                {
                    skorlar = SkorUret(a.TakimGucu - b.TakimGucu, false);
                }
                // Oynanmamış ilk maça maç skorunu yerleştir.
                for (int i = 0; i < a.Maclar.Length; i++)
                {
                    if (a.Maclar[i] == null)
                    {
                        a.Maclar[i] = new MacBilgisi(a, b, skorlar[0], skorlar[1]);
                        b.Maclar[i] = a.Maclar[i];
                        break;
                    }
                }

                a.TakimElendi = true;
                return false;
            }
        }

        /// <summary>
        /// Bir maç skoru üretir. Döndürdüğü dizinin ilk elemanında kaybeden takımın skoru olur.
        /// Takımların güçleri eşitse buyukTakimKazandi'nın değeri önemsizdir.
        /// </summary>
        /// <param name="takimGucFarki">Takımların güçleri farkı.</param>
        /// <param name="buyukTakimKazandi">Büyük takımın kazandığı ile ilgili bilgi.</param>
        /// <returns>Maç skoru.</returns>
        private static int[] SkorUret(int takimGucFarki, bool buyukTakimKazandi)
        {
            int[] skorlar = new int[2];

            int baseSkor = TakimlarBilgisi.Rand.Next(65, 91);
            int skorFarki;
            if (buyukTakimKazandi)
            {
                if (takimGucFarki <= 5)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 16);
                }
                else if (takimGucFarki <= 10)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 23);
                }
                else if (takimGucFarki <= 20)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 26);
                }
                else if (takimGucFarki <= 30)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 31);
                }
                else
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 41);
                }
            }
            else
            {
                if (takimGucFarki <= 5)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 16);
                }
                else if (takimGucFarki <= 10)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 11);
                }
                else if (takimGucFarki <= 20)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 9);
                }
                else if (takimGucFarki <= 30)
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 6);
                }
                else
                {
                    skorFarki = TakimlarBilgisi.Rand.Next(1, 4);
                }
            }
            int kucukTakimSayisi = skorFarki / 2;
            int buyukTakimSayisi = (int)Math.Ceiling((double)skorFarki / 2);
            skorlar[0] = baseSkor - kucukTakimSayisi;
            skorlar[1] = baseSkor + buyukTakimSayisi;

            return skorlar;
        }
    }
}