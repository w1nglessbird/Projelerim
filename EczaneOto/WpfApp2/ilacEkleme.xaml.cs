using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static WpfApp2.ilacEkleme;

namespace WpfApp2
{
    public partial class ilacEkleme : Window
    {
        public ilacEkleme()
        {
            InitializeComponent();
            this.Loaded += (s, e) => ShowToast("İlaç stoğu kritik seviyede! Devam edilsin mi?");
            KritikStok();
        }

        public class Ilaclar
        {
            public string BarkodNo { get; set; }
            public string IlacAdi { get; set; }
            public string FirmaAdi { get; set; }
            public string IlacTuru { get; set; }
            public string Fiyat { get; set; }
            public string Adet { get; set; }
            public string SonKullanmaTarihi { get; set; }
        }

        public class KritikS
        {
            public string BarkodNo { get; set; }
            public string IlacAdi { get; set; }
            public string FirmaAdi { get; set; }
            public string Adet { get; set; }
        }

        private List<string> dusukStokIlaçlar = new List<string>();
        private List<string> silinecekIlaclar = new List<string>();
        private AnaSayfa gk = Application.Current.Windows.OfType<AnaSayfa>().FirstOrDefault(x => x.IsActive);


        public void LoadIlaclar()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = "SELECT barkodNo, ilacAdi, firmaAdi, ilacTuru, fiyat, adet, sonKullanmaTarihi FROM Ilaclar";

            List<Ilaclar> ilaclarListesi = new List<Ilaclar>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ilaclarListesi.Add(new Ilaclar
                                {
                                    BarkodNo = reader["barkodNo"].ToString(),
                                    IlacAdi = reader["ilacAdi"].ToString(),
                                    FirmaAdi = reader["firmaAdi"].ToString(),
                                    IlacTuru = reader["ilacTuru"].ToString(),
                                    Fiyat = reader["fiyat"].ToString(),
                                    Adet = reader["adet"].ToString(),
                                    SonKullanmaTarihi = Convert.ToDateTime(reader["sonKullanmaTarihi"]).ToString("yyyy-MM-dd")
                                });
                            }
                        }
                    }
                }

                listBoxIlaclar.ItemsSource = ilaclarListesi;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public void ShowToast(string message)
        {
            KontrolDusukStok();
            KontrolSonKullanmaTarihi();

            if (dusukStokIlaçlar.Count > 0 || silinecekIlaclar.Count > 0)
            {
                if (dusukStokIlaçlar.Count > 0 && silinecekIlaclar.Count > 0)
                {
                    ToastText.Text = "Bazı ilaçların stoğu azaldı ve bazılarının tarihi geçti!";
                }
                else if (dusukStokIlaçlar.Count > 0)
                {
                    ToastText.Text = dusukStokIlaçlar.Count == 1
                        ? $"{dusukStokIlaçlar[0]} ilacının stoğu azaldı. Stok eklemek ister misiniz?"
                        : "Bazı ilaçların stoğu azaldı. Stok eklemek ister misiniz?";
                }
                else if (silinecekIlaclar.Count > 0)
                {
                    ToastText.Text = silinecekIlaclar.Count == 1
                        ? "Bir ilacın son kullanma tarihi geçti!"
                        : "Bazı ilaçların son kullanma tarihi geçti!";
                }

                ToastContainer.Visibility = Visibility.Visible;
            }
            else
            {
                ToastContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void KontrolDusukStok()
        {
            dusukStokIlaçlar.Clear();
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = @"
                SELECT i.ilacAdi 
                FROM Ilaclar i
                WHERE i.sonKullanmaTarihi > GETDATE()
                GROUP BY i.ilacAdi
                HAVING SUM(CAST(i.adet AS INT)) < 10";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dusukStokIlaçlar.Add(reader["ilacAdi"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok kontrolü sırasında hata: " + ex.Message);
            }
        }

        public void KritikStok()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = @"
                SELECT 
                    barkodNo, 
                    ilacAdi, 
                    MAX(firmaAdi) as firmaAdi, 
                    SUM(CAST(adet AS INT)) as ToplamAdet
                FROM Ilaclar
                WHERE sonKullanmaTarihi > GETDATE()
                GROUP BY barkodNo, ilacAdi
                HAVING SUM(CAST(adet AS INT)) < 10"
            ;

            List<KritikS> ilaclarListesi = new List<KritikS>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ilaclarListesi.Add(new KritikS
                                {
                                    BarkodNo = reader["barkodNo"].ToString(),
                                    IlacAdi = reader["ilacAdi"].ToString(),
                                    FirmaAdi = reader["firmaAdi"].ToString(),
                                    Adet = reader["ToplamAdet"].ToString()
                                });
                            }
                        }
                    }
                }

                kritikGrid.ItemsSource = ilaclarListesi;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
            }
        }

        private void BtnToastEvet_Click(object sender, RoutedEventArgs e)
        {
            if (silinecekIlaclar.Count > 0)
            {
                SilSonKullanmaGecmisIlaclar();
            }

            if (dusukStokIlaçlar.Count > 0)
            {
                StokEkle();
            }

            ToastContainer.Visibility = Visibility.Collapsed;
            LoadIlaclar();
        }

        private void BtnToastHayir_Click(object sender, RoutedEventArgs e)
        {
            ToastContainer.Visibility = Visibility.Collapsed;
        }

        private void StokEkle()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string barkodQuery = @"
                SELECT DISTINCT barkodNo, ilacAdi, firmaAdi, ilacTuru, fiyat, resimYolu 
                FROM Ilaclar 
                WHERE ilacAdi IN ('" + string.Join("','", dusukStokIlaçlar) + @"') 
                AND sonKullanmaTarihi > GETDATE()";
                    List<(string barkod, string name, string firm, string type, decimal price, string resimYolu)> medications =
                        new List<(string, string, string, string, decimal, string)>();
                    using (SqlCommand cmd = new SqlCommand(barkodQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                medications.Add((
                                    reader["barkodNo"].ToString(),
                                    reader["ilacAdi"].ToString(),
                                    reader["firmaAdi"].ToString(),
                                    reader["ilacTuru"].ToString(),
                                    Convert.ToDecimal(reader["fiyat"]),
                                    reader["resimYolu"].ToString()
                                ));
                            }
                        }
                    }
                    foreach (var med in medications)
                    {
                        string insertQuery = @"
                    INSERT INTO Ilaclar 
                    (barkodNo, ilacAdi, firmaAdi, ilacTuru, fiyat, adet, sonKullanmaTarihi, resimYolu) 
                    VALUES 
                    (@barkodNo, @ilacAdi, @firmaAdi, @ilacTuru, @fiyat, @adet, @sonTarih, @resimYolu)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@barkodNo", med.barkod);
                            insertCmd.Parameters.AddWithValue("@ilacAdi", med.name);
                            insertCmd.Parameters.AddWithValue("@firmaAdi", med.firm);
                            insertCmd.Parameters.AddWithValue("@ilacTuru", med.type);
                            insertCmd.Parameters.AddWithValue("@fiyat", med.price);
                            insertCmd.Parameters.AddWithValue("@adet", 20); 
                            insertCmd.Parameters.AddWithValue("@sonTarih", DateTime.Now.AddYears(2));
                            insertCmd.Parameters.AddWithValue("@resimYolu", med.resimYolu);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show($"{medications.Count} ilacın stoğu yenilendi!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok ekleme sırasında hata: " + ex.Message);
            }
        }

        public void KontrolSonKullanmaTarihi()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = "SELECT barkodNo, ilacAdi, sonKullanmaTarihi FROM Ilaclar";

            silinecekIlaclar.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime sonTarih;
                                if (DateTime.TryParse(reader["sonKullanmaTarihi"].ToString(), out sonTarih))
                                {
                                    if (sonTarih < DateTime.Today)
                                    {
                                        string barkod = reader["barkodNo"].ToString();
                                        string ilacAdi = reader["ilacAdi"].ToString();
                                        silinecekIlaclar.Add($"{ilacAdi} ({barkod})");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tarih kontrolünde hata oluştu: " + ex.Message);
            }
        }


        public void SilSonKullanmaGecmisIlaclar()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First delete expired medications
                    string deleteQuery = "DELETE FROM Ilaclar WHERE sonKullanmaTarihi < GETDATE()";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        int deletedCount = deleteCmd.ExecuteNonQuery();
                        MessageBox.Show($"{deletedCount} adet tarihi geçmiş ilaç silindi.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme işlemi sırasında hata: " + ex.Message);
            }
        }

        private void ilacEkle_Click(object sender, RoutedEventArgs e)
        {
            IlacEklememanuel ekle = new IlacEklememanuel();

            if (gk != null && gk.IsLoaded)
            {
                ekle.Owner = gk;
            }

            ekle.ShowDialog();
        }

        private void ilaclarYukle_Click(object sender, RoutedEventArgs e)
        {
            LoadIlaclar();
            KontrolSonKullanmaTarihi();
        }

        private void kritikStok_Click(object sender, RoutedEventArgs e)
        {
            KritikStok();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HastaSayfasi hastaSayfasi = new HastaSayfasi();
            hastaSayfasi.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
            this.Close();
        }
        private void TextBlock_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Recete recete = new Recete();
            recete.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Gecmis gecmis = new Gecmis();
            gecmis.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            Grafikler grafikler = new Grafikler();
            grafikler.Show();
            this.Close();
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
