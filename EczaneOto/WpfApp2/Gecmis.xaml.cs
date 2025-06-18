using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Globalization;
using static WpfApp2.ilacEkleme;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System.IO;


namespace WpfApp2
{
    public partial class Gecmis : Window
    {
        public Gecmis()
        {
            InitializeComponent();
        }

        // Reçeteler sınıfı - Model
        public class Receteler
        {
            public long ReceteID { get; set; }
            public long? ID { get; set; }
            public decimal? Fiyat { get; set; }
            public string Tarih { get; set; }
            public string FiyatFormatli => Fiyat?.ToString("N2", new CultureInfo("tr-TR")) ?? "0,00";
        }

        public class GunlukSatis
        {
            public string Tarih { get; set; }
            public int SatisAdeti { get; set; }
            public decimal ToplamTutar { get; set; }
            public string ToplamTutarFormatli => ToplamTutar.ToString("N2", new CultureInfo("tr-TR"));
        }

        public class AylikSatisIstatistigi
        {
            public string Ay { get; set; }
            public int SatisAdeti { get; set; }
            public decimal ToplamTutar { get; set; }
            public string ToplamTutarFormatli => ToplamTutar.ToString("N2", new CultureInfo("tr-TR"));
        }

        public class YillikSatisIstatistigi
        {
            public string Yil { get; set; }
            public int SatisAdeti { get; set; }
            public decimal ToplamTutar { get; set; }
            public string ToplamTutarFormatli => ToplamTutar.ToString("N2", new CultureInfo("tr-TR"));
        }

        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";

        private readonly CultureInfo turkishCulture = new CultureInfo("tr-TR");

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HastaSayfasi hastaSayfasi = new HastaSayfasi();
            hastaSayfasi.Show();
            this.Close();
        }

        // İlaç Ekleme sayfası açma
        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ilacEkleme IlacEkleme = new ilacEkleme();
            IlacEkleme.Show();
            this.Close();
        }

        // Ana Sayfa açma
        private void TextBlock_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Recete Recete = new Recete();
            Recete.Show();
            this.Close();
        }

        private string GetTurkishMonthName(int month)
        {
            string[] turkishMonths = {
                "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
            };

            return month >= 1 && month <= 12 ? turkishMonths[month - 1] : "Bilinmeyen";
        }
        

        private void VerileriYukle()
        {
            string query = @"SELECT receteID, ID, 
                            CAST(ISNULL(fiyat, 0) as decimal(18,2)) as fiyat, 
                            tarih 
                            FROM Receteler 
                            ORDER BY tarih DESC";
            List<Receteler> receteListesi = new List<Receteler>();

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
                                var fiyatValue = reader["fiyat"];
                                System.Diagnostics.Debug.WriteLine($"SQL'den gelen fiyat: {fiyatValue} (Tip: {fiyatValue?.GetType().Name})");

                                receteListesi.Add(new Receteler
                                {
                                    ReceteID = Convert.ToInt64(reader["receteID"]),
                                    ID = reader["ID"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["ID"]),
                                    Fiyat = fiyatValue == DBNull.Value ? (decimal?)null : Convert.ToDecimal(fiyatValue),
                                    Tarih = Convert.ToDateTime(reader["tarih"]).ToString("dd/MM/yyyy")
                                });
                            }
                        }
                    }
                }

                dataGridReceteler.ItemsSource = receteListesi;

                if (receteListesi.Count == 0)
                {
                    MessageBox.Show("Veri bulunamadı! Tablo boş.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GunlukSatisIstatistikleriYukle()
        {
            string query = @"
                SELECT 
                    CONVERT(date, tarih) as Tarih,
                    CAST(ISNULL(fiyat, 0) as decimal(18,2)) as fiyat
                FROM Receteler 
                WHERE tarih >= DATEADD(day, -30, GETDATE())
                ORDER BY Tarih DESC";

            List<GunlukSatis> gunlukSatisList = new List<GunlukSatis>();
            Dictionary<DateTime, List<decimal>> tarihFiyatMap = new Dictionary<DateTime, List<decimal>>();

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
                                DateTime tarih = Convert.ToDateTime(reader["Tarih"]);

                                decimal fiyat = Convert.ToDecimal(reader["fiyat"]);

                                System.Diagnostics.Debug.WriteLine($"Günlük satış - Tarih: {tarih:dd/MM/yyyy}, Fiyat: {fiyat}");

                                if (!tarihFiyatMap.ContainsKey(tarih))
                                {
                                    tarihFiyatMap[tarih] = new List<decimal>();
                                }
                                tarihFiyatMap[tarih].Add(fiyat);
                            }
                        }
                    }
                }

                foreach (var tarihGrup in tarihFiyatMap.OrderByDescending(x => x.Key))
                {
                    gunlukSatisList.Add(new GunlukSatis
                    {
                        Tarih = tarihGrup.Key.ToString("dd/MM", turkishCulture),
                        SatisAdeti = tarihGrup.Value.Count,
                        ToplamTutar = tarihGrup.Value.Sum()
                    });
                }

                dataGridGunlukSatis.ItemsSource = gunlukSatisList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Günlük satış istatistikleri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AylikSatisIstatistikleriYukle()
        {
            string query = @"
                SELECT 
                    YEAR(tarih) as Yil,
                    MONTH(tarih) as Ay,
                    CAST(ISNULL(fiyat, 0) as decimal(18,2)) as fiyat
                FROM Receteler 
                WHERE tarih >= DATEADD(month, -12, GETDATE())
                ORDER BY Yil DESC, Ay DESC";

            List<AylikSatisIstatistigi> aylikSatisList = new List<AylikSatisIstatistigi>();
            Dictionary<string, List<decimal>> ayFiyatMap = new Dictionary<string, List<decimal>>();

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
                                int yil = Convert.ToInt32(reader["Yil"]);
                                int ay = Convert.ToInt32(reader["Ay"]);
                                decimal fiyat = Convert.ToDecimal(reader["fiyat"]);
                                string ayAdi = GetTurkishMonthName(ay) + " " + yil;
                                string ayKey = $"{yil}-{ay:D2}"; // Sıralama için key

                                System.Diagnostics.Debug.WriteLine($"Aylık satış - Ay: {ayAdi}, Fiyat: {fiyat}");

                                if (!ayFiyatMap.ContainsKey(ayKey))
                                {
                                    ayFiyatMap[ayKey] = new List<decimal>();
                                }
                                ayFiyatMap[ayKey].Add(fiyat);
                            }
                        }
                    }
                }

                foreach (var ayGrup in ayFiyatMap.OrderByDescending(x => x.Key))
                {
                    string[] ayParts = ayGrup.Key.Split('-');
                    int yil = int.Parse(ayParts[0]);
                    int ay = int.Parse(ayParts[1]);

                    string ayAdi = GetTurkishMonthName(ay) + " " + yil;

                    aylikSatisList.Add(new AylikSatisIstatistigi
                    {
                        Ay = ayAdi,
                        SatisAdeti = ayGrup.Value.Count,
                        ToplamTutar = ayGrup.Value.Sum()
                    });
                }

                dataGridAylikSatis.ItemsSource = aylikSatisList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aylık satış istatistikleri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void YillikSatisIstatistikleriYukle()
        {
            string query = @"
                SELECT 
                    YEAR(tarih) as Yil,
                    CAST(ISNULL(fiyat, 0) as decimal(18,2)) as fiyat
                FROM Receteler 
                WHERE tarih >= DATEADD(year, -5, GETDATE())
                ORDER BY Yil DESC";

            List<YillikSatisIstatistigi> yillikSatisList = new List<YillikSatisIstatistigi>();
            Dictionary<int, List<decimal>> yilFiyatMap = new Dictionary<int, List<decimal>>();

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
                                int yil = Convert.ToInt32(reader["Yil"]);
                                decimal fiyat = Convert.ToDecimal(reader["fiyat"]);

                                System.Diagnostics.Debug.WriteLine($"Yıllık satış - Yıl: {yil}, Fiyat: {fiyat}");

                                if (!yilFiyatMap.ContainsKey(yil))
                                {
                                    yilFiyatMap[yil] = new List<decimal>();
                                }
                                yilFiyatMap[yil].Add(fiyat);
                            }
                        }
                    }
                }

                foreach (var yilGrup in yilFiyatMap.OrderByDescending(x => x.Key))
                {
                    yillikSatisList.Add(new YillikSatisIstatistigi
                    {
                        Yil = yilGrup.Key.ToString(),
                        SatisAdeti = yilGrup.Value.Count,
                        ToplamTutar = yilGrup.Value.Sum()
                    });
                }

                dataGridYillikSatis.ItemsSource = yillikSatisList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yıllık satış istatistikleri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OzetIstatistikleriGuncelle()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Bugünkü satış
                    string bugunkuQuery = "SELECT COUNT(*) FROM Receteler WHERE CONVERT(date, tarih) = CONVERT(date, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(bugunkuQuery, conn))
                    {
                        int bugunkuSatis = Convert.ToInt32(cmd.ExecuteScalar());
                        BugunkuSatis.Text = bugunkuSatis.ToString("N0", turkishCulture);
                    }

                    string haftalikQuery = @"SELECT COUNT(*) FROM Receteler 
                                           WHERE tarih >= DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0) 
                                           AND tarih < DATEADD(week, DATEDIFF(week, 0, GETDATE()) + 1, 0)";
                    using (SqlCommand cmd = new SqlCommand(haftalikQuery, conn))
                    {
                        int haftalikSatis = Convert.ToInt32(cmd.ExecuteScalar());
                        HaftalikSatis.Text = haftalikSatis.ToString("N0", turkishCulture);
                    }

                    string aylikQuery = @"SELECT COUNT(*) FROM Receteler 
                                        WHERE MONTH(tarih) = MONTH(GETDATE()) 
                                        AND YEAR(tarih) = YEAR(GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(aylikQuery, conn))
                    {
                        int aylikSatis = Convert.ToInt32(cmd.ExecuteScalar());
                        AylikSatis.Text = aylikSatis.ToString("N0", turkishCulture);
                    }

                    string toplamQuery = "SELECT COUNT(*) FROM Receteler";
                    using (SqlCommand cmd = new SqlCommand(toplamQuery, conn))
                    {
                        int toplamSatis = Convert.ToInt32(cmd.ExecuteScalar());
                        ToplamSatis.Text = toplamSatis.ToString("N0", turkishCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GecmisGoster_Click(object sender, RoutedEventArgs e)
        {
            VerileriYukle();
        }

        private void IstatistikGoster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OzetIstatistikleriGuncelle();
                GunlukSatisIstatistikleriYukle();
                AylikSatisIstatistikleriYukle();     // Yeni eklenen
                YillikSatisIstatistikleriYukle();    // Yeni eklenen

            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler güncellenirken genel bir hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OzetIstatistikleriGuncelle();
            GunlukSatisIstatistikleriYukle();
            AylikSatisIstatistikleriYukle();
            YillikSatisIstatistikleriYukle();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void PdfCiktiAl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "PDF Raporu Kaydet",
                    FileName = $"Recete_Raporu_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string htmlContent = GenerateHtmlReport();

                    ConvertHtmlToPdf(htmlContent, saveFileDialog.FileName);

                    MessageBox.Show($"PDF raporu başarıyla oluşturuldu!\nKonum: {saveFileDialog.FileName}",
                                  "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (MessageBox.Show("PDF dosyasını açmak ister misiniz?", "Dosya Aç",
                                      MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDF oluşturulurken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExcelCiktiAl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv",
                    Title = "Excel Raporu Kaydet",
                    FileName = $"Recete_Raporu_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (saveFileDialog.FileName.EndsWith(".csv"))
                    {
                        CreateCsvReport(saveFileDialog.FileName);
                    }
                    else
                    {
                        
                    }

                    MessageBox.Show($"Excel raporu başarıyla oluşturuldu!\nKonum: {saveFileDialog.FileName}","Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                                  
                    if (MessageBox.Show("Excel dosyasını açmak ister misiniz?", "Dosya Aç",MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)                 
                    {
                        Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel oluşturulurken hata oluştu: {ex.Message}","Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                            
            }
        }

        private string GenerateHtmlReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<title>Reçete Raporu</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(@"
                body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; color: #333; }
                .header { text-align: center; margin-bottom: 30px; border-bottom: 3px solid #38b2ac; padding-bottom: 20px; }
                .header h1 { color: #2d3748; margin: 0; font-size: 28px; }
                .header p { color: #718096; margin: 5px 0; }
                .stats-container { display: flex; justify-content: space-around; margin: 30px 0; }
                .stat-box { 
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                    color: white; padding: 20px; border-radius: 10px; text-align: center; 
                    box-shadow: 0 4px 6px rgba(0,0,0,0.1); min-width: 120px;
                }
                .stat-box h3 { margin: 0; font-size: 24px; }
                .stat-box p { margin: 5px 0 0 0; opacity: 0.9; }
                table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                th, td { border: 1px solid #e2e8f0; padding: 12px; text-align: left; }
                th { background: #38b2ac; color: white; font-weight: bold; }
                tr:nth-child(even) { background-color: #f8f9fa; }
                tr:hover { background-color: #e2e8f0; }
                .section { margin: 30px 0; }
                .section h2 { color: #2d3748; border-left: 4px solid #38b2ac; padding-left: 10px; }
                .footer { text-align: center; margin-top: 40px; padding-top: 20px; border-top: 1px solid #e2e8f0; color: #718096; }
            ");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h1>📄 Geçmiş Reçete Sistemi Raporu</h1>");
            sb.AppendLine($"<p>Rapor Tarihi: {DateTime.Now:dd MMMM yyyy, HH:mm}</p>");
            sb.AppendLine("</div>");

            // İstatistikler
            sb.AppendLine("<div class='stats-container'>");
            sb.AppendLine($"<div class='stat-box'><h3>{BugunkuSatis.Text}</h3><p>Bugün</p></div>");
            sb.AppendLine($"<div class='stat-box'><h3>{HaftalikSatis.Text}</h3><p>Bu Hafta</p></div>");
            sb.AppendLine($"<div class='stat-box'><h3>{AylikSatis.Text}</h3><p>Bu Ay</p></div>");
            sb.AppendLine($"<div class='stat-box'><h3>{ToplamSatis.Text}</h3><p>Toplam</p></div>");
            sb.AppendLine("</div>");

            // Ana Reçete Tablosu
            sb.AppendLine("<div class='section'>");
            sb.AppendLine("<h2>Reçete Listesi</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Reçete No</th><th>T.C. No</th><th>Fiyat (₺)</th><th>Tarih</th></tr>");

            if (dataGridReceteler.ItemsSource is List<Receteler> receteler)
            {
                foreach (var recete in receteler)
                {
                    sb.AppendLine($"<tr>");
                    sb.AppendLine($"<td>{recete.ReceteID}</td>");
                    sb.AppendLine($"<td>{recete.ID?.ToString() ?? "Belirtilmemiş"}</td>");
                    sb.AppendLine($"<td>{recete.FiyatFormatli} ₺</td>");
                    sb.AppendLine($"<td>{recete.Tarih}</td>");
                    sb.AppendLine($"</tr>");
                }
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            // Günlük Satış Tablosu
            if (dataGridGunlukSatis.ItemsSource is List<GunlukSatis> gunlukSatis)
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<h2>Günlük Satış İstatistikleri</h2>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Tarih</th><th>Satış Adedi</th><th>Toplam Tutar (₺)</th></tr>");

                foreach (var satis in gunlukSatis)
                {
                    sb.AppendLine($"<tr>");
                    sb.AppendLine($"<td>{satis.Tarih}</td>");
                    sb.AppendLine($"<td>{satis.SatisAdeti}</td>");
                    sb.AppendLine($"<td>{satis.ToplamTutarFormatli} ₺</td>");
                    sb.AppendLine($"</tr>");
                }

                sb.AppendLine("</table>");
                sb.AppendLine("</div>");
            }

            // Footer
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>Bu rapor Geçmiş Reçete Sistemi tarafından otomatik olarak oluşturulmuştur.</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
        private void ConvertHtmlToPdf(string htmlContent, string pdfPath)
        {
            try
            {
                string tempHtmlPath = Path.GetTempFileName() + ".html";
                File.WriteAllText(tempHtmlPath, htmlContent, Encoding.UTF8);
                string chromeArgs = $"--headless --disable-gpu --print-to-pdf=\"{pdfPath}\" \"{tempHtmlPath}\"";

                ProcessStartInfo psi = new ProcessStartInfo();
                string[] chromePaths = {
                    @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
                    @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
                };

                string chromePath = chromePaths.FirstOrDefault(File.Exists);

                if (chromePath == null)
                {
                    throw new Exception("Chrome veya Edge bulunamadı. Lütfen Chrome veya Edge yükleyin.");
                }

                psi.FileName = chromePath;
                psi.Arguments = chromeArgs;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit(10000);
                }

                if (File.Exists(tempHtmlPath))
                    File.Delete(tempHtmlPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF oluşturma hatası: {ex.Message}");
            }
        }

        private void CreateCsvReport(string filePath)
        {
            var sb = new StringBuilder();

            // BOM ekle (Excel'de Türkçe karakterlerin doğru görünmesi için)
            sb.Append('\uFEFF');

            sb.AppendLine("Reçete No;T.C. No;Fiyat;Tarih");

            if (dataGridReceteler.ItemsSource is List<Receteler> receteler)
            {
                foreach (var recete in receteler)
                {
                    string tcNo = recete.ID?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(tcNo))
                    {
                        tcNo = "=\"" + tcNo + "\"";
                    }

                    string fiyat = recete.Fiyat?.ToString("F2") ?? "0,00";

                    // Tarihi metin olarak zorla
                    string tarih = recete.Tarih ?? "";
                    if (!string.IsNullOrEmpty(tarih))
                    {
                        tarih = "=\"" + tarih + "\"";
                    }

                    sb.AppendLine($"{recete.ReceteID};{tcNo};{fiyat};{tarih}");
                }
            }

            // Dosyaya yaz
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void TextBlock_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            Grafikler grafikler = new Grafikler();
            grafikler.Show();
            this.Close();

        }
    }
}
