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
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Kullanıcı adı ve şifre boş olamaz!";
                return;
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
            string query = "SELECT COUNT(*) FROM kullaniciler WHERE kullaniciAdi = @kullaniciAdi AND sifre = @sifre";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@kullaniciAdi", username);
                        command.Parameters.AddWithValue("@sifre", password);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            lblMessage.Text = "Giriş başarılı!";

                            // Giriş başarılıysa AnaSayfa'ya yönlendir
                            AnaSayfa anaSayfa = new AnaSayfa();
                            anaSayfa.Show(); // Yeni pencereyi aç
                            this.Close(); // Mevcut pencereyi kapat
                        }
                        else
                        {
                            lblMessage.Text = "Kullanıcı adı veya şifre hatalı!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Bir hata oluştu: " + ex.Message;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}

        


