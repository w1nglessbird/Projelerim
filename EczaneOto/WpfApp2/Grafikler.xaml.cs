using System;
using System.Collections.Generic;
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
using LiveCharts.Wpf;
using LiveCharts;
using System.Data.SqlClient;
using System.Globalization;
using System.ComponentModel;

namespace WpfApp2
{

    public partial class Grafikler : Window
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=EczaneOtomasyon;Trusted_Connection=True;";
        private CultureInfo turkishCulture = new CultureInfo("tr-TR");

        public Func<double, string> Formatter { get; set; }

        public Grafikler()
        {
            InitializeComponent();
            Formatter = value => value.ToString("C", turkishCulture);
            this.Loaded += Grafikler_Loaded;
        }

        private void Grafikler_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            try
            {
                GunlukSatisGrafigi();
                EnCokSatilanUrunlerGrafigi();
                AylikSatisKarsilastirmaGrafigi();
                YillikPerformansTrendi();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Grafikler yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GunlukSatisGrafigi()
        {
            string query = @"
                SELECT 
                    CONVERT(date, tarih) as Tarih,
                    COUNT(*) as SatisAdeti,
                    CAST(ISNULL(SUM(TRY_CAST(fiyat AS decimal(18,2))), 0) as decimal(18,2)) as ToplamTutar
                FROM Receteler 
                WHERE tarih >= DATEADD(day, -30, GETDATE())
                  AND ISNUMERIC(fiyat) = 1
                GROUP BY CONVERT(date, tarih)
                ORDER BY Tarih";

            List<string> gunler = new List<string>();
            List<double> satisAdetleri = new List<double>();

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
                                int satisAdeti = Convert.ToInt32(reader["SatisAdeti"]);

                                gunler.Add(tarih.ToString("dd/MM", turkishCulture));
                                satisAdetleri.Add(satisAdeti);
                            }
                        }
                    }
                }

                var lineSeries = new LineSeries
                {
                    Title = "Günlük Satışlar",
                    Values = new ChartValues<double>(satisAdetleri),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    Stroke = new SolidColorBrush(Colors.White),
                    Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255))
                };

                LineChart.Series.Clear();
                LineChart.Series.Add(lineSeries);
                LineChart.AxisX.Clear();
                LineChart.AxisX.Add(new Axis
                {
                    Title = "Günler",
                    Labels = gunler,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 10
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Günlük satış grafiği yüklenirken hata: {ex.Message}", "Hata");
            }
        }

        private void EnCokSatilanUrunlerGrafigi()
        {
            string query = @"
        SELECT TOP 5 
            ilacAdi,
            COUNT(*) AS SatisAdeti
        FROM ReceteIlaclar
        WHERE receteID IN (
            SELECT receteID 
            FROM Receteler 
            WHERE tarih >= DATEADD(month, -3, GETDATE())
        )
        GROUP BY ilacAdi
        ORDER BY COUNT(*) DESC";

            try
            {
                var pieSeriesCollection = new SeriesCollection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var colors = new[] {
                        Color.FromRgb(255, 206, 84),
                        Color.FromRgb(75, 192, 192),
                        Color.FromRgb(153, 102, 255),
                        Color.FromRgb(255, 159, 64),
                        Color.FromRgb(199, 199, 199)
                    };
                            int colorIndex = 0;

                            while (reader.Read())
                            {
                                string ilacAdi = reader["ilacAdi"].ToString();
                                int satisAdeti = Convert.ToInt32(reader["SatisAdeti"]);

                                pieSeriesCollection.Add(new PieSeries
                                {
                                    Title = ilacAdi,
                                    Values = new ChartValues<double> { satisAdeti },
                                    Fill = new SolidColorBrush(colors[colorIndex % colors.Length]),
                                    DataLabels = true,
                                    LabelPoint = chartPoint => $"{chartPoint.Y} adet"
                                });

                                colorIndex++;
                            }
                        }
                    }
                }

                if (pieSeriesCollection.Count == 0)
                {
                    pieSeriesCollection = new SeriesCollection
            {
                new PieSeries {
                    Title = "Veri Yok",
                    Values = new ChartValues<double> { 1 },
                    Fill = new SolidColorBrush(Colors.Gray)
                }
            };
                }

                PieChart.Series = pieSeriesCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"En çok satılan ürünler grafiği yüklenirken hata: {ex.Message}", "Hata");
            }
        }

        private void AylikSatisKarsilastirmaGrafigi()
        {
            string query = @"
                SELECT 
                    YEAR(tarih) as Yil,
                    MONTH(tarih) as Ay,
                    CAST(ISNULL(SUM(TRY_CAST(fiyat AS decimal(18,2))), 0) as decimal(18,2)) as ToplamTutar
                FROM Receteler 
                WHERE tarih >= DATEADD(month, -12, GETDATE())
                  AND ISNUMERIC(fiyat) = 1
                GROUP BY YEAR(tarih), MONTH(tarih)
                ORDER BY YEAR(tarih), MONTH(tarih)";

            List<string> aylar = new List<string>();
            List<double> tutarlar = new List<double>();

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
                                decimal toplamTutar = Convert.ToDecimal(reader["ToplamTutar"]);

                                string ayAdi = GetTurkishMonthName(ay) + " " + yil.ToString().Substring(2);
                                aylar.Add(ayAdi);
                                tutarlar.Add((double)toplamTutar);
                            }
                        }
                    }
                }

                var columnSeries = new ColumnSeries
                {
                    Title = "Aylık Satış",
                    Values = new ChartValues<double>(tutarlar),
                    Fill = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
                    DataLabels = true,
                    LabelPoint = chartPoint => chartPoint.Y.ToString("C0", turkishCulture)
                };

                BarChart.Series.Clear();
                BarChart.Series.Add(columnSeries);
                BarChart.AxisX.Clear();
                BarChart.AxisX.Add(new Axis
                {
                    Title = "Aylar",
                    Labels = aylar,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 10,
                    LabelsRotation = 45
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aylık karşılaştırma grafiği yüklenirken hata: {ex.Message}", "Hata");
            }
        }

        private void YillikPerformansTrendi()
        {
            string query = @"
                SELECT 
                    YEAR(tarih) as Yil,
                    CAST(ISNULL(SUM(TRY_CAST(fiyat AS decimal(18,2))), 0) as decimal(18,2)) as ToplamTutar
                FROM Receteler 
                WHERE tarih >= DATEADD(year, -5, GETDATE())
                  AND ISNUMERIC(fiyat) = 1
                GROUP BY YEAR(tarih)
                ORDER BY YEAR(tarih)";

            List<string> yillar = new List<string>();
            List<double> tutarlar = new List<double>();

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
                                decimal toplamTutar = Convert.ToDecimal(reader["ToplamTutar"]);

                                yillar.Add(yil.ToString());
                                tutarlar.Add((double)toplamTutar);
                            }
                        }
                    }
                }

                var areaSeries = new LineSeries
                {
                    Title = "Yıllık Toplam",
                    Values = new ChartValues<double>(tutarlar),
                    AreaLimit = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 3,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                };

                AreaChart.Series.Clear();
                AreaChart.Series.Add(areaSeries);
                AreaChart.AxisX.Clear();
                AreaChart.AxisX.Add(new Axis
                {
                    Title = "Yıllar",
                    Labels = yillar,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 12
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yıllık trend grafiği yüklenirken hata: {ex.Message}", "Hata");
            }
        }

        private string GetTurkishMonthName(int monthNumber)
        {
            string[] turkishMonths = {
                "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
            };

            if (monthNumber >= 1 && monthNumber <= 12)
                return turkishMonths[monthNumber - 1];

            return "Bilinmeyen";
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ReceteOlustur_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
            this.Close();
        }

        private void Hastalar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HastaSayfasi hastaSayfasi = new HastaSayfasi();
            hastaSayfasi.Show();
            this.Close();
        }

        private void Recete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Recete recete = new Recete();
            recete.Show();
            this.Close();
        }

        private void Stok_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ilacEkleme IlacEkleme = new ilacEkleme();
            IlacEkleme.Show();
            this.Close();
        }

        private void GecmisRecete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Gecmis Gecmis = new Gecmis();
            Gecmis.Show();
            this.Close();
        }

        private void YenileButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeCharts();
           
        }
    }
}