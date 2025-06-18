using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    public partial class HastaSayfasi : Window
    {
        private List<Kisi> tumHastalar; // Tüm hastaları saklayacak liste
        private bool isSearchPanelVisible = false;

        public HastaSayfasi()
        {
            InitializeComponent();
        }

        public class Kisi
        {
            public string Id { get; set; }
            public string AdSoyad { get; set; }
            public string KronikHastalik { get; set; }
            public string Telefon { get; set; }
            public string SosyalGuvence { get; set; }
            public string Durum { get; set; }
        }

        public void LoadKisiler()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = "SELECT ID, adSoyad, kronikH, telefon, guvenceNo, durum FROM Kisiler";

            List<Kisi> kisilerListesi = new List<Kisi>();

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
                                kisilerListesi.Add(new Kisi
                                {
                                    Id = reader["ID"].ToString(),
                                    AdSoyad = reader["adSoyad"].ToString(),
                                    KronikHastalik = reader["kronikH"].ToString(),
                                    Telefon = reader["telefon"].ToString(),
                                    SosyalGuvence = reader["guvenceNo"].ToString(),
                                    Durum = reader["durum"].ToString()
                                });
                            }
                        }
                    }
                }

                tumHastalar = kisilerListesi; // Tüm hastaları kaydet
                dataGridKisiler.ItemsSource = kisilerListesi;
                HeaderText.Text = $"👥 Hasta Listesi ({kisilerListesi.Count} hasta)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
            }
        }

        // Hasta arama fonksiyonu - veritabanından arama
        public void SearchHasta(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadKisiler(); // Boşsa tüm hastaları göster
                return;
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = @"SELECT ID, adSoyad, kronikH, telefon, guvenceNo, durum 
                           FROM Kisiler 
                           WHERE adSoyad LIKE @searchTerm 
                           OR telefon LIKE @searchTerm 
                           OR kronikH LIKE @searchTerm 
                           OR guvenceNo LIKE @searchTerm 
                           OR durum LIKE @searchTerm";

            List<Kisi> filteredKisiler = new List<Kisi>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                filteredKisiler.Add(new Kisi
                                {
                                    Id = reader["ID"].ToString(),
                                    AdSoyad = reader["adSoyad"].ToString(),
                                    KronikHastalik = reader["kronikH"].ToString(),
                                    Telefon = reader["telefon"].ToString(),
                                    SosyalGuvence = reader["guvenceNo"].ToString(),
                                    Durum = reader["durum"].ToString()
                                });
                            }
                        }
                    }
                }

                dataGridKisiler.ItemsSource = filteredKisiler;
                HeaderText.Text = $"🔍 Arama Sonuçları ({filteredKisiler.Count} hasta bulundu)";

                if (filteredKisiler.Count == 0)
                {
                    MessageBox.Show($"'{searchTerm}' için sonuç bulunamadı.", "Arama Sonucu",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında bir hata oluştu: " + ex.Message);
            }
        }

        // Lokal arama fonksiyonu - bellekteki verilerden arama (daha hızlı)
        public void SearchHastaLocal(string searchTerm)
        {
            if (tumHastalar == null)
            {
                LoadKisiler();
                return;
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                dataGridKisiler.ItemsSource = tumHastalar;
                HeaderText.Text = $"👥 Hasta Listesi ({tumHastalar.Count} hasta)";
                return;
            }

            var filteredHastalar = tumHastalar.Where(h =>
                (!string.IsNullOrEmpty(h.AdSoyad) && h.AdSoyad.ToLower().Contains(searchTerm.ToLower())) ||
                (!string.IsNullOrEmpty(h.Telefon) && h.Telefon.Contains(searchTerm)) ||
                (!string.IsNullOrEmpty(h.KronikHastalik) && h.KronikHastalik.ToLower().Contains(searchTerm.ToLower())) ||
                (!string.IsNullOrEmpty(h.SosyalGuvence) && h.SosyalGuvence.ToLower().Contains(searchTerm.ToLower())) ||
                (!string.IsNullOrEmpty(h.Durum) && h.Durum.ToLower().Contains(searchTerm.ToLower())) ||
                (!string.IsNullOrEmpty(h.Id) && h.Id.Contains(searchTerm))
            ).ToList();

            dataGridKisiler.ItemsSource = filteredHastalar;
            HeaderText.Text = $"🔍 Arama Sonuçları ({filteredHastalar.Count} hasta bulundu)";

            if (filteredHastalar.Count == 0)
            {
                MessageBox.Show($"'{searchTerm}' için sonuç bulunamadı.", "Arama Sonucu",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Arama panelini göster/gizle
        private void HastaAraButton_Click(object sender, RoutedEventArgs e)
        {
            if (tumHastalar == null)
            {
                LoadKisiler();
            }

            isSearchPanelVisible = !isSearchPanelVisible;
            SearchPanel.Visibility = isSearchPanelVisible ? Visibility.Visible : Visibility.Collapsed;

            if (isSearchPanelVisible)
            {
                SearchTextBox.Focus();
            }
            else
            {
                SearchTextBox.Text = "";
                dataGridKisiler.ItemsSource = tumHastalar;
                HeaderText.Text = $"👥 Hasta Listesi ({tumHastalar?.Count ?? 0} hasta)";
            }
        }

        // Arama butonu click eventi
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim();
            SearchHastaLocal(searchTerm); // Lokal arama kullan (daha hızlı)
            // SearchHasta(searchTerm); // Veritabanı araması istiyorsanız bu satırı kullanın
        }

        // Arama kutusunda Enter'a basınca arama yap
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, null);
            }
        }

        // Anlık arama - her karakter yazıldığında
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Anlık arama istiyorsanız bu kodu etkinleştirin:
            // string searchTerm = SearchTextBox.Text.Trim();
            // SearchHastaLocal(searchTerm);
        }

        // Arama temizle butonu
        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            if (tumHastalar != null)
            {
                dataGridKisiler.ItemsSource = tumHastalar;
                HeaderText.Text = $"👥 Hasta Listesi ({tumHastalar.Count} hasta)";
            }
            SearchTextBox.Focus();
        }

        private void LoadKisilerButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridKisiler.ItemsSource == null)
            {
                LoadKisiler();
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
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