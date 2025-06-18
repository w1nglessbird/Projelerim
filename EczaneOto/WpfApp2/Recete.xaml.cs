using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    public partial class Recete : Window
    {
        string connectionString = @"Server=.\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";

        public Recete()
        {
            InitializeComponent();
        }

        private void ReceteGir_Click(object sender, RoutedEventArgs e)
        {
            ListBox ilacListesi = (ListBox)this.FindName("IlacListesi");

            if (string.IsNullOrWhiteSpace(ReceteKutu.Text))
            {
                MessageBox.Show("Reçete numarası boş olamaz.");
                return;
            }

            if (string.IsNullOrWhiteSpace(tcNo.Text))
            {
                MessageBox.Show("TC numarası boş olamaz.");
                return;
            }

            decimal toplamFiyat = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Reçete ID'yi ve tarihini çekiyoruz
                SqlCommand receteCmd = new SqlCommand("SELECT receteID, tarih FROM Receteler WHERE receteID = @receteID", con);
                receteCmd.Parameters.AddWithValue("@receteID", ReceteKutu.Text);

                SqlDataReader receteReader = receteCmd.ExecuteReader();

                if (!receteReader.Read())
                {
                    receteReader.Close();
                    MessageBox.Show("Girilen reçete bulunamadı.");
                    return;
                }

                int receteID = Convert.ToInt32(receteReader["receteID"]);

                if (receteReader["tarih"] != DBNull.Value)
                {
                    DateTime receteTarihi = Convert.ToDateTime(receteReader["tarih"]);
                    ReceteTarihi.Content = receteTarihi.ToString("dd/MM/yyyy");
                }
                else
                {
                    ReceteTarihi.Content = "Tarih belirtilmemiş";
                }

                receteReader.Close();

                DateTime teslimAlimTarihi = DateTime.Now;
                TeslimAlimTarihi.Content = teslimAlimTarihi.ToString("dd/MM/yyyy");

                SqlCommand hastaCmd = new SqlCommand("SELECT adSoyad FROM Kisiler WHERE ID = @ID", con);
                hastaCmd.Parameters.AddWithValue("@ID", tcNo.Text);

                SqlDataReader hastaReader = hastaCmd.ExecuteReader();

                if (hastaReader.Read())
                {
                    string hastaAdSoyad = hastaReader["adSoyad"].ToString();
                    HastaAdSoyad.Content = hastaAdSoyad;
                }
                else
                {
                    HastaAdSoyad.Content = "Hasta bilgisi bulunamadı";
                }

                hastaReader.Close();

                SqlCommand ilacCmd = new SqlCommand("SELECT barkodNo, ilacAdi FROM ReceteIlaclar WHERE receteID = @receteID", con);
                ilacCmd.Parameters.AddWithValue("@receteID", receteID);

                SqlDataReader reader = ilacCmd.ExecuteReader();
                ilacListesi.Items.Clear();

                var barkodListesi = new List<string>();

                while (reader.Read())
                {
                    string barkod = reader["barkodNo"].ToString();
                    string ad = reader["ilacAdi"].ToString();
                    ilacListesi.Items.Add($"{barkod} - {ad}");
                    barkodListesi.Add(barkod);
                }

                reader.Close();

                foreach (string barkod in barkodListesi)
                {
                    SqlCommand fiyatCmd = new SqlCommand("SELECT fiyat FROM Ilaclar WHERE barkodNo = @barkodNo", con);
                    fiyatCmd.Parameters.AddWithValue("@barkodNo", barkod);

                    SqlDataReader fiyatReader = fiyatCmd.ExecuteReader();

                    if (fiyatReader.Read())
                    {
                        string fiyatStr = fiyatReader["fiyat"].ToString();
                        decimal fiyat;

                        // Türkçe formatta olan fiyat stringini decimal'e çevir
                        if (decimal.TryParse(fiyatStr, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("tr-TR"), out fiyat))
                        {
                            toplamFiyat += fiyat;
                        }
                    }
                    fiyatReader.Close();
                }
            }
            FiyatGor.Content = toplamFiyat.ToString("F2") + " TL";

            SigortaBilgisiGetir();
        }

        private void AlisverisiTamamla_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReceteKutu.Text))
            {
                MessageBox.Show("Reçete numarası boş olamaz.");
                return;
            }

            decimal toplamFiyat = 0;
            int receteID;

            if (!int.TryParse(ReceteKutu.Text, out receteID))
            {
                MessageBox.Show("Geçerli bir Reçete Numarası giriniz.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT barkodNo FROM ReceteIlaclar WHERE receteID = @receteID", con);
                cmd.Parameters.AddWithValue("@receteID", receteID);

                SqlDataReader reader = cmd.ExecuteReader();

                var barkodListesi = new List<string>();
                while (reader.Read())
                {
                    barkodListesi.Add(reader["barkodNo"].ToString());
                }
                reader.Close();

                foreach (string barkod in barkodListesi)
                {
                    SqlCommand fiyatCmd = new SqlCommand("SELECT fiyat, adet FROM Ilaclar WHERE barkodNo = @barkodNo", con);
                    fiyatCmd.Parameters.AddWithValue("@barkodNo", barkod);

                    SqlDataReader fiyatReader = fiyatCmd.ExecuteReader();

                    if (fiyatReader.Read())
                    {
                        int adet = Convert.ToInt32(fiyatReader["adet"]);
                        string fiyatStr = fiyatReader["fiyat"].ToString();

                        if (adet > 0)
                        {
                            decimal fiyat;

                            // Türkçe formatta olan fiyat stringini decimal'e çevir
                            if (decimal.TryParse(fiyatStr, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("tr-TR"), out fiyat))
                            {
                                toplamFiyat += fiyat;
                            }
                            else
                            {
                                MessageBox.Show($"Fiyat okunamadı: {fiyatStr}");
                            }

                            fiyatReader.Close();

                            SqlCommand guncelleCmd = new SqlCommand("UPDATE Ilaclar SET adet = adet - 1 WHERE barkodNo = @barkodNo", con);
                            guncelleCmd.Parameters.AddWithValue("@barkodNo", barkod);
                            guncelleCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            fiyatReader.Close();
                            MessageBox.Show($"İlaç ({barkod}) stokta yok.");
                        }
                    }
                    else
                    {
                        fiyatReader.Close();
                        MessageBox.Show($"İlaç ({barkod}) bulunamadı.");
                    }
                }

                DateTime teslimAlimTarihi = DateTime.Now;

                SqlCommand updateCmd = new SqlCommand("UPDATE Receteler SET fiyat = @fiyat, ID = @ID, teslimAlma = @teslimAlma WHERE receteID = @receteID", con);

                updateCmd.Parameters.AddWithValue("@fiyat", toplamFiyat);
                updateCmd.Parameters.AddWithValue("@ID", tcNo.Text);
                updateCmd.Parameters.AddWithValue("@teslimAlma", teslimAlimTarihi);
                updateCmd.Parameters.AddWithValue("@receteID", receteID);

                updateCmd.ExecuteNonQuery();
            }

            FiyatGor.Content = toplamFiyat.ToString("F2") + " TL";
            MessageBox.Show("Satış tamamlandı.");
        }

        private void SigortaBilgisiGetir()
        {
            if (string.IsNullOrWhiteSpace(tcNo.Text))
            {
                MessageBox.Show("Kullanıcı ID'si boş olamaz.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT guvenceNo FROM Kisiler WHERE ID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", tcNo.Text);

                object guvenceNoObj = cmd.ExecuteScalar();

                if (guvenceNoObj != null)
                {
                    string guvenceNo = guvenceNoObj.ToString();

                    SqlCommand guvenceCmd = new SqlCommand("SELECT guvenceAdi FROM SosyalGuvence WHERE guvenceNo = @guvenceNo", con);
                    guvenceCmd.Parameters.AddWithValue("@guvenceNo", guvenceNo);

                    object guvenceAdiObj = guvenceCmd.ExecuteScalar();

                    if (guvenceAdiObj != null)
                    {
                        SigortalilikTuru.Content = guvenceAdiObj.ToString();
                    }
                    else
                    {
                        SigortalilikTuru.Content = "Sigorta bilgisi bulunamadı.";
                    }
                }
                else
                {
                    SigortalilikTuru.Content = "Kişi bulunamadı.";
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HastaSayfasi hastaSayfasi = new HastaSayfasi();
            hastaSayfasi.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ilacEkleme IlacEkleme = new ilacEkleme();
            IlacEkleme.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Gecmis Gecmis = new Gecmis();
            Gecmis.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBlock_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            Grafikler grafikler = new Grafikler();
            grafikler.Show();
            this.Close();
        }
    }
}