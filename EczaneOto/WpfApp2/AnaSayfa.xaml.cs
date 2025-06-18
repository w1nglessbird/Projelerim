using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    public partial class AnaSayfa : Window
    {
        public AnaSayfa()
        {
            InitializeComponent();
        }

        public void LoadIlaclar()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = "SELECT ilacAdi FROM Ilaclar";

            List<string> ilacListesi = new List<string>();

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
                                ilacListesi.Add(reader["ilacAdi"].ToString());
                            }
                        }
                    }
                }

                listBoxIlaclar.ItemsSource = ilacListesi;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
            }
        }

        private void listBoxIlaclar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxIlaclar.SelectedItem != null)
            {
                string selectedIlac = listBoxIlaclar.SelectedItem.ToString();

                string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
                string query = "SELECT resimYolu FROM Ilaclar WHERE ilacAdi = @ilacAdi";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ilacAdi", selectedIlac);
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                string resimDosyasi = result.ToString();
                                string tamYol = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IlacGorselleri", resimDosyasi);

                                if (System.IO.File.Exists(tamYol))
                                {
                                    IlacGorsel.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(tamYol));
                                }
                                else
                                {
                                    IlacGorsel.Source = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Görsel yüklenemedi: " + ex.Message);
                }
            }
        }

        private void ReceteyiOlustur_Click(object sender, RoutedEventArgs e)
        {
            if (ReceteIlacListBox.Items.Count == 0)
            {
                MessageBox.Show("Reçeteye ilaç eklenmemiş.");
                return;
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Yeni reçete oluştur
                    string insertRecete = "INSERT INTO Receteler DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                    SqlCommand receteCmd = new SqlCommand(insertRecete, conn);
                    int receteID = Convert.ToInt32(receteCmd.ExecuteScalar());

                    // 2. Her ilaç için barkod bul ve reçeteye ekle
                    foreach (string ilacAdi in ReceteIlacListBox.Items)
                    {
                        string barkodQuery = "SELECT barkodNo FROM Ilaclar WHERE ilacAdi = @ilacAdi";
                        SqlCommand barkodCmd = new SqlCommand(barkodQuery, conn);
                        barkodCmd.Parameters.AddWithValue("@ilacAdi", ilacAdi);

                        object barkodObj = barkodCmd.ExecuteScalar();

                        if (barkodObj != null)
                        {
                            int barkodNo = Convert.ToInt32(barkodObj);

                            string insertIlac = "INSERT INTO ReceteIlaclar (receteID, barkodNo, ilacAdi) VALUES (@receteID, @barkodNo, @ilacAdi)";
                            SqlCommand insertCmd = new SqlCommand(insertIlac, conn);
                            insertCmd.Parameters.AddWithValue("@receteID", receteID);
                            insertCmd.Parameters.AddWithValue("@barkodNo", barkodNo);
                            insertCmd.Parameters.AddWithValue("@ilacAdi", ilacAdi);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    // 3. UI'ya reçete numarasını yaz
                    ReceteOl.Text = receteID.ToString();
                    MessageBox.Show("Reçete başarıyla oluşturuldu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void LoadIlaclarButton_Click(object sender, RoutedEventArgs e)
        {
            LoadIlaclar();
        }

        private void ReceteyeEkle_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxIlaclar.SelectedItem != null)
            {
                string secilenIlac = listBoxIlaclar.SelectedItem.ToString();
                if (!ReceteIlacListBox.Items.Contains(secilenIlac))
                {
                    ReceteIlacListBox.Items.Add(secilenIlac);
                }
                else
                {
                    MessageBox.Show("Bu ilaç zaten reçetede mevcut.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen eklenecek bir ilaç seçiniz.");
            }
        }

        private void RecetedenKaldir_Click(object sender, RoutedEventArgs e)
        {
            if (ReceteIlacListBox.SelectedItem != null)
            {
                string secilenIlac = ReceteIlacListBox.SelectedItem.ToString();
                ReceteIlacListBox.Items.Remove(secilenIlac);
            }
            else
            {
                MessageBox.Show("Lütfen kaldırılacak bir ilaç seçiniz.");
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
            Recete Recete = new Recete();
            Recete.Show();
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