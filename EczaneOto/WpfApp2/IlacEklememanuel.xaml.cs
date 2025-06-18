using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using static WpfApp2.ilacEkleme;

namespace WpfApp2
{
  
    public partial class IlacEklememanuel : Window
    {
        public IlacEklememanuel()
        {
            InitializeComponent();
        }

        public void VeriEkle(string barkodNo, string ilacAdi, string firmaAdi, string ilacTuru, string fiyat, string adet, DateTime sonKullanmaTarihi, string gorselDosyaAdi)
        {
            try
            {
                string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
                string query = "INSERT INTO Ilaclar (barkodNo, ilacAdi, firmaAdi, ilacTuru, fiyat, adet, sonKullanmaTarihi, resimYolu) " +
                               "VALUES (@barkodNo, @ilacAdi, @firmaAdi, @ilacTuru, @fiyat, @adet, @sonKullanmaTarihi, @resimYolu)";

                using (SqlConnection baglan = new SqlConnection(connectionString))
                {
                    baglan.Open();
                    using (SqlCommand komut = new SqlCommand(query, baglan))
                    {
                        komut.Parameters.AddWithValue("@barkodNo", barkodNo);
                        komut.Parameters.AddWithValue("@ilacAdi", ilacAdi);
                        komut.Parameters.AddWithValue("@firmaAdi", firmaAdi);
                        komut.Parameters.AddWithValue("@ilacTuru", ilacTuru);
                        komut.Parameters.AddWithValue("@fiyat", fiyat);
                        komut.Parameters.AddWithValue("@adet", adet);
                        komut.Parameters.AddWithValue("@sonKullanmaTarihi", sonKullanmaTarihi);
                        komut.Parameters.AddWithValue("@resimYolu", gorselDosyaAdi);

                        int sonuc = komut.ExecuteNonQuery();
                        MessageBox.Show(sonuc > 0 ? "İlaç başarıyla eklendi!" : "Ekleme başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }



        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnEkle_Click(object sender, RoutedEventArgs e)
        {
            string barkodNo = textBox1.Text;
            string ilacAdi = textBox2.Text;
            string firmaAdi = textBox3.Text;
            string ilacTuru = textBox4.Text;
            string fiyat = textBox5.Text;
            string adet = textBox6.Text;
            DateTime sonKullanmaTarihi = datePickerSKT.SelectedDate ?? DateTime.Now;

            VeriEkle(barkodNo, ilacAdi, firmaAdi, ilacTuru, fiyat, adet, sonKullanmaTarihi, secilenGorselDosyaAdi);
        }

        private string secilenGorselDosyaAdi = "";
        private void FotoYukle_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "PNG Fotoğraf Seç";
            openFileDialog.Filter = "PNG Resimleri|*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string dosyaYolu = openFileDialog.FileName;
                txtDosyaYolu.Text = dosyaYolu;

                // Görseli göster
                BitmapImage bitmap = new BitmapImage(new Uri(dosyaYolu));
                imgGoster.Source = bitmap;

                // Dosyayı uygulama içindeki IlacGorselleri klasörüne kopyala
                string klasorYolu = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IlacGorselleri");
                if (!Directory.Exists(klasorYolu))
                    Directory.CreateDirectory(klasorYolu);

                secilenGorselDosyaAdi = System.IO.Path.GetFileName(dosyaYolu);
                string hedefYol = System.IO.Path.Combine(klasorYolu, secilenGorselDosyaAdi);
                File.Copy(dosyaYolu, hedefYol, true); // üzerine yazılsın
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
