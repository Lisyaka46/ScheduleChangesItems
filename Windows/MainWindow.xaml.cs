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

namespace ScheduleChangesItems
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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
        private Color DefaultColorPointSeries;

        /// <summary>
        /// Цвет выделенной позиции тенденции
        /// </summary>
        private Color SelectedColorPointSeries;

        /// <summary>
        /// Цвет выделенной позиции тенденции
        /// </summary>
        private List<Color> SelectedColorsPoint;

        /// <summary>
        /// Инициализация главного окна формы
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Title = TitleValue;
            ChartPoint.ChartAreas[0].AxisY.Minimum = 0d;
            ButtonDeveloper.Click += (sender, e) =>
            {
                DialogDeveloper dialogDeveloper = new DialogDeveloper();
                dialogDeveloper.ShowDialog();
            };
            ButtonOpenFile.Click += (sender, e) =>
            {
                DirectoryJobFile = GetGirectoryOpenFile();
                if (DirectoryJobFile != null)
                {
                    (Series[], Color[])? ReadSeries = TxtPointFileManipulate.ReadFile(DirectoryJobFile);
                    if (ReadSeries.HasValue)
                    {
                        if (SelectedColorsPoint != null) SelectedColorsPoint.Clear();
                        else SelectedColorsPoint = new List<Color>();
                        SelectedColorsPoint.AddRange(ReadSeries.Value.Item2);
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
                        $"Закрытие файла {System.IO.Path.GetFileName(DirectoryJobFile)}", MessageBoxButton.YesNo);
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
                if (SelectedIndexPoint[SelectedIndexSeries] != -1 && SelectedIndexPoint[SelectedIndexSeries] < ChartPoint.Series[SelectedIndexSeries].Points.Count)
                    ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]].Color = DefaultColorPointSeries;
                SelectedIndexPoint[SelectedIndexSeries] = ListPointsBox.SelectedIndex;
                if (SelectedIndexPoint[SelectedIndexSeries] != -1)
                    ChartPoint.Series[SelectedIndexSeries].Points[SelectedIndexPoint[SelectedIndexSeries]].Color = SelectedColorPointSeries;
                _ = UpdateInformation();
            };
            ListSeriesesBox.SelectionChanged += (sender, e) =>
            {
                if (ListSeriesesBox.SelectedIndex == -1) return;
                UpdateChartPoint(ListSeriesesBox.SelectedIndex);
            };
            ButtonAddNewPoint.Click += (sender, e) =>
            {
                DialogAddPoint addPoint = new DialogAddPoint();
                (string, int)? point = addPoint.GenerateTPoint();
                if (point != null)
                {
                    ListPointsBox.Items.Add(point.Value.Item2);

                    ChartPoint.Series[SelectedIndexSeries].Points.AddY(point.Value.Item2);
                    ChartPoint.Series[SelectedIndexSeries].Points[ChartPoint.Series[SelectedIndexSeries].Points.Count - 1].AxisLabel = point.Value.Item1;
                    ListPointsBox.SelectedIndex++;
                    if (!ButtonRemovePoint.IsEnabled) ButtonRemovePoint.IsEnabled = true;
                    UpdateLimitChating();
                    _ = UpdateInformation();
                }
            };
            ButtonSaveFile.Click += (sender, e) =>
            {
                string[] SeriesesTaging = ChartPoint.Series.Select((i) => TSeriesTagging(i)).ToArray();
                File.WriteAllLines(DirectoryJobFile, SeriesesTaging);
                DirectoryJobFile = null;
                ListSeriesesBox.SelectedIndex = -1;
                ListPointsBox.SelectedIndex = -1;
                ListSeriesesBox.Items.Clear();
                ListPointsBox.Items.Clear();
                ChartPoint.Series.Clear();
                TextAllGive.Text = $"Всего было создано: X";
                TextAllRemove.Text = $"Всего было использовано: X";
                TextAllTrend.Text = "Тенденция относительно начального значения: X%";
                TextTrend.Text = "Тенденция относительно предыдущего значения: X%";

                //ButtonCreateFile.IsEnabled = true;
                ButtonOpenFile.IsEnabled = true;
                ButtonSaveFile.IsEnabled = false;
                ButtonAddNewPoint.IsEnabled = false;
                ButtonRemovePoint.IsEnabled = false;
                ButtonAddNewSeries.IsEnabled = false;
                ButtonRemoveSeries.IsEnabled = false;
            };
            ButtonRemovePoint.Click += (sender, e) =>
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
                    }
                    UpdateLimitChating();
                }
            };
            ButtonAddNewSeries.Click += (sender, e) =>
            {
                DialogAddSeries dialogAddSeries = new DialogAddSeries();
                (Series, Color)? s = dialogAddSeries.GenSeries(ChartPoint.Series.Select((i) => i.Name).ToArray());
                if (s.HasValue)
                {
                    ButtonAddNewPoint.IsEnabled = true;
                    ButtonRemoveSeries.IsEnabled = true;
                    SelectedColorsPoint.Add(s.Value.Item2);
                    ChartPoint.Series.Add(s.Value.Item1);
                    ListSeriesesBox.Items.Add(s.Value.Item1.Name);
                    SelectedIndexPoint.Add(-1);
                    ListSeriesesBox.SelectedIndex = ListSeriesesBox.Items.Count - 1;
                    //UpdateChartPoint(ListSeriesesBox.Items.Count - 1);
                }
            };
            ButtonRemoveSeries.Click += (sender, e) =>
            {
                // добавить удаление коллекции
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
                        ButtonRemovePoint.IsEnabled = false;
                        ButtonAddNewPoint.IsEnabled = false;
                    }
                }
            };
        }

        /// <summary>
        /// Конвертировать объект TSepies в синтаксис тегов
        /// </summary>
        /// <param name="series">Объект коллекции</param>
        /// <returns>Синтаксис тегов</returns>
        private string TSeriesTagging(Series series)
        {
            if (series == null) return string.Empty;
            string Text = string.Empty;
            Text += "<Series_Init>\n";
            Text += $"<Name>{series.Name}<\n";
            Text += $"<Hex>#{series.Color.A:X2}{series.Color.R:X2}{series.Color.G:X2}{series.Color.B:X2}<\n";
            foreach (DataPoint p in series.Points)
            {
                Text += $"<Point>{p.YValues[0]}<\n";
                if (p.Name.Length > 0) Text += $"<Point_Name>{p.Name}<\n";
            }
            Text += "<~>\n";
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

            //ButtonCreateFile.IsEnabled = false;
            ButtonOpenFile.IsEnabled = false;
            ButtonSaveFile.IsEnabled = true;
            ButtonAddNewPoint.IsEnabled = true;
            //ButtonRemovePoint.IsEnabled = true;
            ButtonRemoveSeries.IsEnabled = true;
            ButtonAddNewSeries.IsEnabled = true;

            foreach (Series s in Data)
            {
                ListSeriesesBox.Items.Add(s.Name);
                s.ChartArea = ChartPoint.ChartAreas[0].Name;
                Array.ForEach(s.Points.ToArray(), (i) => ListPointsBox.Items.Add(i.YValues[0]));
                s.Enabled = false;
                ChartPoint.Series.Add(s);
            }
            if (SelectedIndexSeries != -1)
            {
                ChartPoint.Series[SelectedIndexSeries].Enabled = true;
                ListSeriesesBox.SelectedIndex = SelectedIndexSeries;
                ListPointsBox.SelectedIndex = SelectedIndexPoint[SelectedIndexSeries];
                DefaultColorPointSeries = ChartPoint.Series[SelectedIndexSeries].Color;
                SelectedColorPointSeries = SelectedColorsPoint[SelectedIndexSeries];
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

            DefaultColorPointSeries = s.Color;
            SelectedColorPointSeries = SelectedColorsPoint[SelectedIndexSeries];

            ChartPoint.Series[SelectedIndexSeries].Enabled = true;
            Array.ForEach(s.Points.ToArray(), (i) => ListPointsBox.Items.Add(i.YValues[0]));
            ListPointsBox.SelectedIndex = SelectedIndexPoint[index];
            UpdateLimitChating();
            ButtonRemovePoint.IsEnabled = s.Points.Count > 0;
            await Task.FromResult(UpdateInformation());
        }

        private void UpdateLimitChating()
        {
            if (ChartPoint.Series[SelectedIndexSeries].Points.Count > 0)
            {
                ChartPoint.ChartAreas[0].AxisY.Maximum = ChartPoint.Series[SelectedIndexSeries].Points.Max((i) => i.YValues[0]) * 1.1;
                ChartPoint.ChartAreas[0].AxisY.Interval = Math.Round(ChartPoint.ChartAreas[0].AxisY.Maximum / 6, 2);
            }
        }

        private async Task UpdateInformation()
        {
            Series s = ChartPoint.Series[SelectedIndexSeries];

            double Max = s.Points.Sum((i) => i.YValues[0]);
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
            int Trend = await Task.Run(() =>
            {
                if (SelectedIndexPoint[SelectedIndexSeries] == -1) return 0;
                else if (SelectedIndexPoint[SelectedIndexSeries] == 0) return 100;
                else
                {
                    return (int)((s.Points[SelectedIndexPoint[SelectedIndexSeries]].YValues[0] / s.Points[SelectedIndexPoint[SelectedIndexSeries] - 1].YValues[0] - 1) * 100);
                }
            });
            TextAllGive.Text = $"Всего было создано: {Max}";
            TextAllRemove.Text = $"Всего было использовано: {Consumption}";
            TextAllTrend.Text = "Тенденция относительно начального значения: " +
                $"{(int)(s.Points.Count > 0 ? (s.Points[SelectedIndexPoint[SelectedIndexSeries]].YValues[0] / s.Points[0].YValues[0] - 1) * 100 : 0)}%";
            TextTrend.Text = $"Тенденция относительно предыдущего значения: {Trend}%";
        }
    }
}
