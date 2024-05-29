using Microsoft.Win32;
using ScheduleChangesItems.Classes;
using ScheduleChangesItems.Windows;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Media.Animation;

namespace ScheduleChangesItems
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Количество отображаемых значений тенденции
        /// </summary>
        private int CountPrintingTrend = 9;


        /// <summary>
        /// Файл с которым работает программа
        /// </summary>
        private string DirectoryJobFile = null;

        /// <summary>
        /// Константное название программы
        /// </summary>
        private const string TitleValue = "Program Thend";

        /// <summary>
        /// Активный индекс отображения информации на графике
        /// </summary>
        private int SelectedIndexSeries;

        /// <summary>
        /// Активный индекс отображения информации позиций на графике
        /// </summary>
        private List<int> SelectedIndexPoint;

        /// <summary>
        /// Цвет обычного состояния позиции тенденции
        /// </summary>
        private System.Drawing.Color DefaultColorPointSeries;

        /// <summary>
        /// Цвет выделенной позиции тенденции
        /// </summary>
        private System.Drawing.Color SelectedColorPointSeries;

        /// <summary>
        /// Массив визуализационных объектов коллекции
        /// </summary>
        private List<VisualizationSeries> VisCollection;

        /// <summary>
        /// Манимация для состояния позиции тенденции
        /// </summary>
        private readonly ColorAnimation AnimStatusPoint;

        /// <summary>
        /// Удаление карты
        /// </summary>
        /// <param name="hObject">DeleteObject</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Конвертация из карты цвета в ImageSource
        /// </summary>
        /// <param name="bmp">Карта цвета</param>
        /// <returns>ImageSource</returns>
        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }


        /// <summary>
        /// Инициализация главного окна формы
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AnimStatusPoint = new ColorAnimation(Colors.White, Colors.Black, TimeSpan.FromMilliseconds(600));
            Title = TitleValue;
            ChartPoint.ChartAreas[0].AxisY.Minimum = 0d;
            TextStatusPoint.Foreground = new SolidColorBrush(Colors.Black);
            ButtonDeveloper.Click += (sender, e) =>
            {
                DialogDeveloper dialogDeveloper = new DialogDeveloper();
                dialogDeveloper.ShowDialog();
            };
            ButtonOpenFile.MouseUp += (sender, e) =>
            {
                DirectoryJobFile = GetGirectoryOpenFile();
                if (DirectoryJobFile != null)
                {
                    (Series[], VisualizationSeries[])? ReadSeries = TxtPointFileManipulate.ReadFile(DirectoryJobFile);
                    if (ReadSeries.HasValue)
                    {
                        if (VisCollection != null) VisCollection.Clear();
                        else VisCollection = new List<VisualizationSeries>();
                        VisCollection.AddRange(ReadSeries.Value.Item2);
                        OpenData(ReadSeries.Value.Item1);
                    }
                    else DirectoryJobFile = null;
                }
            };
            Closing += (sender, e) =>
            {
                if (DirectoryJobFile != null)
                {
                    MessageBoxResult Result = MessageBox.Show("Вы уверены что хотите закрыть приложение не сохранив изменённые данные?",
                        $"Закрытие файла {Path.GetFileName(DirectoryJobFile)}", MessageBoxButton.YesNo);
                    if (Result == MessageBoxResult.Yes)
                    {
                        ChartPoint.Series.Clear();
                        DirectoryJobFile = null;
                    }
                    else if (Result == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            };
            ListPointsBox.SelectionChanged += (sender, e) =>
            {
                if (ListPointsBox.SelectedIndex == -1) return;
                if (ListPointsBox.SelectedIndex > (CountPrintingTrend - 1) / 2)
                {
                    if (ListPointsBox.SelectedIndex < ListPointsBox.Items.Count - (CountPrintingTrend - 1) / 2)
                    {
                        ChartPoint.ChartAreas[0].AxisX.Minimum = ListPointsBox.SelectedIndex - (CountPrintingTrend - 1) / 2;
                        ChartPoint.ChartAreas[0].AxisX.Maximum = ListPointsBox.SelectedIndex + 1 + (CountPrintingTrend - 1) / 2;
                    }
                    else
                    {
                        ChartPoint.ChartAreas[0].AxisX.Maximum = ListPointsBox.Items.Count + 1;
                        ChartPoint.ChartAreas[0].AxisX.Minimum = ListPointsBox.Items.Count - (CountPrintingTrend - 1);
                    }
                }
                else
                {
                    ChartPoint.ChartAreas[0].AxisX.Minimum = 0;
                    ChartPoint.ChartAreas[0].AxisX.Maximum = CountPrintingTrend;
                }
                if (SelectedIndexPoint[SelectedIndexSeries] != -1 && SelectedIndexPoint[SelectedIndexSeries] < ChartPoint.Series[SelectedIndexSeries].Points.Count)
                    ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]].Color = DefaultColorPointSeries;
                SelectedIndexPoint[SelectedIndexSeries] = ListPointsBox.SelectedIndex;
                if (SelectedIndexPoint[SelectedIndexSeries] != -1)
                    ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]].Color = SelectedColorPointSeries;
                UpdateLimitChating();
                _ = UpdateInformation();
            };
            ListSeriesesBox.SelectionChanged += (sender, e) =>
            {
                if (ListSeriesesBox.SelectedIndex == -1) return;
                UpdateChartPoint(ListSeriesesBox.SelectedIndex);
            };
            ButtonAddNewPoint.MouseUp += (sender, e) =>
            {
                DialogManagementPoint MPoint = new DialogManagementPoint();
                (string, int)? point = MPoint.GenerateTPoint();
                if (point != null)
                {
                    ListPointsBox.Items.Add(point.Value.Item2);

                    ChartPoint.Series[SelectedIndexSeries].Points.AddY(point.Value.Item2);
                    ChartPoint.Series[SelectedIndexSeries].Points[ChartPoint.Series[SelectedIndexSeries].Points.Count - 1].AxisLabel = point.Value.Item1;
                    ListPointsBox.SelectedIndex++;
                    if (!ButtonRemovePoint.IsEnabled)
                    {
                        ButtonRemovePoint.IsEnabled = true;
                        ButtonChangePoint.IsEnabled = true;
                    }
                }
            };
            ButtonChangePoint.MouseUp += (sender, e) =>
            {
                double X = ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]].XValue;
                DialogManagementPoint MPoint = new DialogManagementPoint();
                (string, int)? point = MPoint.ChangeInfoPoint(ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]]);
                if (point.HasValue)
                {
                    ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]] = new DataPoint(X, point.Value.Item2)
                    {
                        AxisLabel = point.Value.Item1
                    };
                    ListPointsBox.Items[SelectedIndexPoint[SelectedIndexSeries]] = point.Value.Item2;
                    ListPointsBox.SelectedIndex = ListPointsBox.Items.Count - 1;
                }
            };
            ButtonSaveFile.MouseUp += (sender, e) =>
            {
                string SeriesesTaging = SeriesTagging(ChartPoint.Series);
                File.WriteAllText(DirectoryJobFile, SeriesesTaging);
                DirectoryJobFile = null;
                ListSeriesesBox.SelectedIndex = -1;
                ListPointsBox.SelectedIndex = -1;
                ListSeriesesBox.Items.Clear();
                ListPointsBox.Items.Clear();
                ChartPoint.Series.Clear();
                TextAllGive.Text = $"Всего было создано: ?";
                TextAllRemove.Text = $"Всего было использовано: ?";
                TextMinProcent.Text = "Значение относительно минимума: ?%";
                TextMaxProcent.Text = "Значение относительно максимума: ?%";
                TextTrend.Text = "Тенденция относительно предыдущего значения: ?%";
                TextAllTrendChange.Text = $"Общий процент изменения тенденции: ?%";

                //ButtonCreateFile.IsEnabled = true;
                ButtonOpenFile.IsEnabled = true;
                ButtonSaveFile.IsEnabled = false;
                ButtonAddNewPoint.IsEnabled = false;
                ButtonRemovePoint.IsEnabled = false;
                ButtonChangePoint.IsEnabled = false;
                ButtonAddNewSeries.IsEnabled = false;
                ButtonChangeSeries.IsEnabled = false;
                ButtonRemoveSeries.IsEnabled = false;
            };
            ButtonRemovePoint.MouseUp += (sender, e) =>
            {
                MessageBoxResult Result = MessageBox.Show($"Вы точно хотите удалить точку \"{ListPointsBox.SelectedItem}\" из коллекции \"{ListSeriesesBox.SelectedItem}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                {
                    int index = ListPointsBox.SelectedIndex;
                    ChartPoint.Series[SelectedIndexSeries].Points.RemoveAt(index);
                    ListPointsBox.Items.RemoveAt(index);
                    if (index < ListPointsBox.Items.Count) ListPointsBox.SelectedIndex = index;
                    else if (index == ListPointsBox.Items.Count && ListPointsBox.Items.Count > 0) ListPointsBox.SelectedIndex = index - 1;
                    else
                    {
                        ListPointsBox.SelectedIndex = -1;
                        SelectedIndexPoint[SelectedIndexSeries] = -1;
                        ButtonRemovePoint.IsEnabled = false;
                        ButtonChangePoint.IsEnabled = false;
                    }
                    UpdateLimitChating();
                }
            };
            ButtonAddNewSeries.MouseUp += (sender, e) =>
            {
                DialogManagementSeries dialogSeries = new DialogManagementSeries();
                (Series, VisualizationSeries)? s = dialogSeries.GenSeries(ChartPoint.Series.Select((i) => i.Name).ToArray());
                if (s.HasValue)
                {
                    ButtonAddNewPoint.IsEnabled = true;
                    ButtonRemoveSeries.IsEnabled = true;
                    ButtonChangeSeries.IsEnabled = true;
                    VisCollection.Add(s.Value.Item2);
                    ChartPoint.Series.Add(s.Value.Item1);
                    ListSeriesesBox.Items.Add(s.Value.Item1.Name);
                    SelectedIndexPoint.Add(-1);
                    ListSeriesesBox.SelectedIndex = ListSeriesesBox.Items.Count - 1;
                    //UpdateChartPoint(ListSeriesesBox.Items.Count - 1);
                }
            };
            ButtonChangeSeries.MouseUp += (sender, e) =>
            {
                string NameSeries = ListSeriesesBox.Items[SelectedIndexSeries].ToString();
                DialogManagementSeries dialogSeries = new DialogManagementSeries();
                (string, VisualizationSeries)? SeriesChangeInfo =
                    dialogSeries.ChangeInfoSeries(
                        ChartPoint.Series.Select((i) => i.Name).Where((i) => !i.Equals(NameSeries)).ToArray(), NameSeries, VisCollection[SelectedIndexSeries]);
                if (SeriesChangeInfo.HasValue)
                {
                    VisCollection[SelectedIndexSeries] = SeriesChangeInfo.Value.Item2;
                    ChartPoint.Series[SelectedIndexSeries].Name = SeriesChangeInfo.Value.Item1;
                    ChartPoint.Series[SelectedIndexSeries].Color = VisCollection[SelectedIndexSeries].ColorDefault;
                    ChartPoint.Update();
                    int index = SelectedIndexSeries;
                    ListSeriesesBox.Items[SelectedIndexSeries] = SeriesChangeInfo.Value.Item1;
                    ListSeriesesBox.SelectedIndex = index;
                }
            };
            ButtonRemoveSeries.MouseUp += (sender, e) =>
            {
                MessageBoxResult Result = MessageBox.Show($"Вы точно хотите удалить коллекцию \"{ListSeriesesBox.SelectedItem}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                {
                    int index = ListSeriesesBox.SelectedIndex;
                    ListSeriesesBox.Items.RemoveAt(index);
                    ChartPoint.Series.RemoveAt(index);
                    ListPointsBox.Items.Clear();
                    if (index < ListSeriesesBox.Items.Count) ListSeriesesBox.SelectedIndex = index;
                    else if (index == ListSeriesesBox.Items.Count && ListSeriesesBox.Items.Count > 0) ListSeriesesBox.SelectedIndex = index - 1;
                    else
                    {
                        ListSeriesesBox.SelectedIndex = -1;
                        SelectedIndexSeries = -1;
                        ButtonRemoveSeries.IsEnabled = false;
                        ButtonChangeSeries.IsEnabled = false;
                        ButtonRemovePoint.IsEnabled = false;
                        ButtonChangePoint.IsEnabled = false;
                        ButtonAddNewPoint.IsEnabled = false;
                    }
                }
            };
            ButtonSettings.MouseUp += (sender, e) =>
            {
                WindowSettings settings = new WindowSettings();
                settings.ShowDialog();
            };
        }

        /// <summary>
        /// Конвертировать объект Series в синтаксис тегов
        /// </summary>
        /// <param name="MassSeries">Объекты коллекции</param>
        /// <returns>Синтаксис тегов</returns>
        private string SeriesTagging(SeriesCollection MassSeries)
        {
            if (MassSeries == null) return string.Empty;
            string Text = string.Empty;
            VisualizationSeries VisSeries;
            Series series;
            for (int i = 0; i < MassSeries.Count; i++)
            {
                VisSeries = VisCollection[i];
                series = MassSeries[i];
                Text += "<Series_Init>\n";
                Text += $"<Name>{series.Name}<\n";
                Text += $"<Hex>#{VisSeries.ColorDefault.A:X2}{VisSeries.ColorDefault.R:X2}{VisSeries.ColorDefault.G:X2}{VisSeries.ColorDefault.B:X2}<\n";
                Text += $"<Select_Hex>#{VisSeries.ColorSelect.A:X2}{VisSeries.ColorSelect.R:X2}{VisSeries.ColorSelect.G:X2}{VisSeries.ColorSelect.B:X2}<\n";
                foreach (DataPoint p in series.Points)
                {
                    Text += $"<Point>{p.YValues[0]}<\n";
                    if (p.Name.Length > 0) Text += $"<Point_Name>{p.Name}<\n";
                }
                Text += "<~>\n";
                if (i < MassSeries.Count - 1) Text += "\n";
            }
            return Text;
        }

        /// <summary>
        /// Узнать директорию открываемого файла
        /// </summary>
        /// <returns>Директория открываемого файла (Может быть пустой)</returns>
        private static string GetGirectoryOpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = "Trend", // Default file name
                DefaultExt = ".txtpoint", // Default file extension
                Filter = "Алгоритм тенденции (.txtpoint)|*.txtpoint" // Filter files by extension
            };
            if (dialog.ShowDialog() ?? false) return dialog.FileName;
            return null;
        }

        /// <summary>
        /// Визуализировать дату
        /// </summary>
        private void OpenData(Series[] Data)
        {
            Title = $"{TitleValue}: {Path.GetFileName(DirectoryJobFile)}";

            SelectedIndexSeries = Data.Length - 1;
            SelectedIndexPoint = new List<int>();
            SelectedIndexPoint.AddRange(Data.Select((i) => i.Points.Count - 1));

            foreach (Series s in Data)
            {
                ListSeriesesBox.Items.Add(s.Name);
                s.ChartArea = ChartPoint.ChartAreas[0].Name;
                Array.ForEach(s.Points.ToArray(), (i) => ListPointsBox.Items.Add(i.YValues[0]));
                s.Enabled = false;
                ChartPoint.Series.Add(s);
            }

            //ButtonCreateFile.IsEnabled = false;
            ButtonOpenFile.IsEnabled = false;
            //ButtonRemovePoint.IsEnabled = true;
            if (ListSeriesesBox.Items.Count > 0)
            {
                ButtonAddNewPoint.IsEnabled = true;
                ButtonRemoveSeries.IsEnabled = true;
                ButtonChangeSeries.IsEnabled = true;
            }
            ButtonAddNewSeries.IsEnabled = true;
            ButtonSaveFile.IsEnabled = true;

            if (SelectedIndexSeries != -1)
            {
                ChartPoint.Series[SelectedIndexSeries].Enabled = true;
                ListSeriesesBox.SelectedIndex = SelectedIndexSeries;
                ListPointsBox.SelectedIndex = SelectedIndexPoint[SelectedIndexSeries];
                DefaultColorPointSeries = VisCollection[SelectedIndexSeries].ColorDefault;
                SelectedColorPointSeries = VisCollection[SelectedIndexSeries].ColorSelect;
            }
        }

        /// <summary>
        /// Обновляет отображение данных тенденции
        /// </summary>
        private async void UpdateChartPoint(int index)
        {
            Series s = ChartPoint.Series[index];

            if (SelectedIndexSeries < ChartPoint.Series.Count && SelectedIndexSeries != -1)
                ChartPoint.Series[SelectedIndexSeries].Enabled = false;
            ListPointsBox.Items.Clear();

            SelectedIndexSeries = index;

            DefaultColorPointSeries = VisCollection[SelectedIndexSeries].ColorDefault;
            SelectedColorPointSeries = VisCollection[SelectedIndexSeries].ColorSelect;

            ChartPoint.Series[SelectedIndexSeries].Enabled = true;
            Array.ForEach(s.Points.ToArray(), (i) => ListPointsBox.Items.Add(i.YValues[0]));
            ListPointsBox.SelectedIndex = SelectedIndexPoint[index];
            UpdateLimitChating();
            ButtonRemovePoint.IsEnabled = s.Points.Count > 0;
            ButtonChangePoint.IsEnabled = s.Points.Count > 0;
            await Task.FromResult(UpdateInformation());
        }

        private void UpdateLimitChating()
        {
            if (ChartPoint.Series[SelectedIndexSeries].Points.Count > 0)
            {
                List<double> Points = new List<double>();
                for (int i = ChartPoint.ChartAreas[0].AxisX.Minimum > 0d ? (int)ChartPoint.ChartAreas[0].AxisX.Minimum - 1 : 0;
                    i < ChartPoint.ChartAreas[0].AxisX.Maximum - 1 && i < ChartPoint.Series[SelectedIndexSeries].Points.Count; i++)
                    Points.Add(ChartPoint.Series[SelectedIndexSeries].Points[i].YValues[0]);
                double Max = Points.Max() * 1.6;
                double Min = Points.Min() * 1.6;
                if (Math.Abs(Min) + Max == 0) return;
                ChartPoint.ChartAreas[0].AxisY.Maximum = Max;
                if (Min < 0) ChartPoint.ChartAreas[0].AxisY.Minimum = Min;
                else ChartPoint.ChartAreas[0].AxisY.Minimum = 0;
                ChartPoint.ChartAreas[0].AxisY.Interval = Math.Round((Math.Abs(Min) + Max) / 12, 2);
            }
        }

        private async Task UpdateInformation()
        {
            Series s = ChartPoint.Series[SelectedIndexSeries];
            int index = SelectedIndexPoint[SelectedIndexSeries];
            double ActNum = s.Points[index].YValues[0];
            double[] Points = s.Points.Select((i) => i.YValues[0]).ToArray();
            double Consumption = await Task.Run(() =>
            {
                double x = 0d;
                for (int i = 1; i < s.Points.Count; i++)
                {
                    if (s.Points[i - 1].YValues[0] > s.Points[i].YValues[0])
                    {
                        x += s.Points[i - 1].YValues[0] - s.Points[i].YValues[0];
                    }
                }
                return x;
            });
            double Trend;
            double TrendChange = 0d;

            if (index - 1 > -1) Trend =
                    await MathTrend(s.Points[index - 1].YValues[0], ActNum);
            else if (ActNum < 0) Trend = -1d;
            else Trend = 1d;
            double TrendMax = await MathTrend(Points.Max(), ActNum);
            double TrendMin = await MathTrend(Points.Min(), ActNum);

            if (index > 0)
            {
                double Num = ActNum;
                Func<double, double, bool> func = null;
                if (ActNum <= s.Points[index - 1].YValues[0]) func = (x, y) => x <= y;
                else if (ActNum >= s.Points[index - 1].YValues[0]) func = (x, y) => x >= y;
                else TrendChange = 0;
                if (func != null)
                {
                    for (int i = index - 1; i > -1; i--)
                    {
                        if (i == -1) break;
                        if (func.Invoke(Num, s.Points[i].YValues[0])) Num = s.Points[i].YValues[0];
                        else break;
                    }
                    TrendChange = await MathTrend(Num, ActNum);
                }
            }
            else TrendChange = (int)Trend;

            TextAllGive.Text = $"Всего было создано: {s.Points.Sum((i) => i.YValues[0])}";
            TextAllRemove.Text = $"Всего было использовано: {Consumption}";
            TextMinProcent.Text = $"Значение относительно минимума: {GenStringProcent(TrendMin)}";
            TextMaxProcent.Text = $"Значение относительно максимума: {GenStringProcent(TrendMax)}";
            TextTrend.Text = $"Тенденция относительно предыдущего значения: {GenStringProcent(Trend)}";
            TextAllTrendChange.Text = $"Общий процент изменения тенденции: {GenStringProcent(TrendChange)}";
            if (ActNum == Points.Max()) TextStatusPoint.Text = "Максимальное значение";
            else if (ActNum == Points.Min()) TextStatusPoint.Text = "Минимальное значение";
            else
            {
                TextStatusPoint.Text = "Промежуточное значение";
                return;
            }
            TextStatusPoint.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, AnimStatusPoint);
        }

        /// <summary>
        /// Сгенерировать строку отображаемого процента
        /// </summary>
        /// <remarks>
        /// (0,01 => +1%) *** (0,001 => +<![CDATA[<]]>0%) *** (-0,01 => -1%) *** (-0,001 => -<![CDATA[<]]>0%)
        /// </remarks>
        /// <param name="Procent">Вычисленный процент</param>
        /// <returns>Строка отображаемая процент</returns>
        private string GenStringProcent(double Procent) =>
            $"{(Procent > 0 ? "+" : string.Empty)}" +
            $"{(Math.Abs(Procent) >= 0.01d ? $"{(int)(Procent * 100)}%" : Math.Abs(Procent) > 0 ? "<0%" : "0%")}";

        /// <summary>
        /// Вычислить процентное соотношение между двумя числами
        /// </summary>
        /// <param name="PreNum">Прошлое число</param>
        /// <param name="ActNum">Текущее число</param>
        /// <returns>процентное соотношение</returns>
        private async Task<double> MathTrend(double PreNum, double ActNum)
        {
            return await Task.Run(() =>
            {
                double Difference = ActNum - PreNum;
                if (Difference == 0) return 0d;
                else if (PreNum == 0)
                {
                    if (ActNum < 0) return -1d;
                    else return 1d;
                }
                else if (ActNum < 0 && PreNum < 0 || ActNum > 0 && PreNum < 0) return -(Difference / PreNum);
                else return Difference / PreNum;
            });
        }
    }
}
