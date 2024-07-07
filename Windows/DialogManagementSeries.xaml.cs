using ScheduleChangesItems.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogAddSeries.xaml
    /// </summary>
    public partial class DialogManagementSeries : Window
    {
        private string[] NameSeriesesInicialized;

        private System.Drawing.Color DefaultColor;

        private System.Drawing.Color SelectColor;

        private System.Drawing.Color SelectColorAuto;

        private readonly ColorAnimation ColorAnim;

        private bool Complete = false;

        /// <summary>
        /// Диалоговое окно для создания коллекции
        /// </summary>
        public DialogManagementSeries()
        {
            InitializeComponent();
            foreach (string types in Enum.GetNames(typeof(SeriesChartType))) ComboBoxStyleChart.Items.Add(types);
            CheckBoxAutoSelectColor.IsChecked = true;
            DefaultColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            SelectColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(80, 80, 80));
            DefaultColor = System.Drawing.Color.FromArgb(0, 0, 0);
            SelectColor = System.Drawing.Color.FromArgb(80, 80, 80);
            SelectColorAuto = System.Drawing.Color.FromArgb(80, 80, 80);
            ColorAnim = new ColorAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(100)),
            };
            DefaultColorView.MouseLeftButtonUp += (sender, e) => SetColorDefault(DefaultColor);
            SelectColorView.MouseLeftButtonUp += (sender, e) =>
            {
                if (SetColorSelect((CheckBoxAutoSelectColor.IsChecked ?? false) ? SelectColorAuto : SelectColor)) CheckBoxAutoSelectColor.IsChecked = false;
            };
            CheckBoxAutoSelectColor.Checked += (sender, e) =>
            {
                GenAutoColor(DefaultColor);
                ChangeTypeSelColor();
            };
            CheckBoxAutoSelectColor.Unchecked += (sender, e) => ChangeTypeSelColor();
            ButtonCreateSeries.MouseUp += (sender, e) =>
            {
                if (TextBoxNameSeries.Text.Length > 0 && !NameSeriesesInicialized.Contains(TextBoxNameSeries.Text))
                {
                    Complete = true;
                    Close();
                }
                else
                {
                    ColorAnimation anim = new ColorAnimation()
                    {
                        To = Colors.White,
                        Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                    };
                    TextBoxNameSeries.Background = new SolidColorBrush(Colors.Red);
                    TextBoxNameSeries.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                }
            };
        }

        /// <summary>
        /// Изменить данные коллекции с помощью диалогового окна
        /// </summary>
        /// <param name="NamesInitSeries">Массив уже проинициализированных имён коллекций</param>
        /// <param name="NameSeries">Имя изменяемой коллекции</param>
        /// <param name="VisSeries">Визуализационный объект коллекции</param>
        /// <param name="chartType">Тип отображения коллекции который является стартовым значением для выбора типа отображения коллекции</param>
        /// <returns>Коллекция (может быть пустой)</returns>
        public (Series, VisualizationSeries)? ChangeInfoSeries(string[] NamesInitSeries, string NameSeries, VisualizationSeries VisSeries, SeriesChartType chartType)
        {
            NameSeriesesInicialized = NamesInitSeries;
            TextBoxNameSeries.Text = NameSeries;
            DefaultColor = VisSeries.ColorDefault;
            SelectColor = VisSeries.ColorSelect;
            ComboBoxStyleChart.SelectedIndex = (int)chartType;
            DefaultColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(VisSeries.ColorDefault.R, VisSeries.ColorDefault.G, VisSeries.ColorDefault.B));
            SelectColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(VisSeries.ColorSelect.R, VisSeries.ColorSelect.G, VisSeries.ColorSelect.B));
            CheckBoxAutoSelectColor.IsChecked = false;
            ButtonCreateSeries.Texting = "Изменить";
            Icon = App.ImageSourceFromBitmap(Properties.Resources.Edit);
            Title = "Изменение коллекции";
            ShowDialog();
            if (Complete)
            {
                Series s = new Series
                {
                    Color = DefaultColor,
                    BorderColor = DefaultColor,
                    BorderWidth = 8,
                    Name = TextBoxNameSeries.Text,
                    ChartType = (SeriesChartType)ComboBoxStyleChart.SelectedIndex,
                };
                VisSeries = new VisualizationSeries(DefaultColor, (CheckBoxAutoSelectColor.IsChecked ?? false) ? SelectColorAuto : SelectColor);
                return (s, VisSeries);
            }
            return null;
        }


        /// <summary>
        /// Сгенерировать с помощью диалогового окна колекцию
        /// </summary>
        /// <param name="NamesSeries">Массив уже проинициализированных имён колекций</param>
        /// <returns>Коллекция (может быть пустой)</returns>
        public (Series, VisualizationSeries)? GenSeries(string[] NamesSeries)
        {
            NameSeriesesInicialized = NamesSeries;
            ComboBoxStyleChart.SelectedIndex = (int)SeriesChartType.Column;
            ButtonCreateSeries.Texting = "Создать";
            Icon = App.ImageSourceFromBitmap(Properties.Resources.Add);
            Title = "Добавление коллекции";
            ShowDialog();
            if (Complete)
            {
                Series s = new Series
                {
                    Color = DefaultColor,
                    BorderColor = DefaultColor,
                    BorderWidth = 8,
                    Name = TextBoxNameSeries.Text,
                    ChartType = (SeriesChartType)ComboBoxStyleChart.SelectedIndex,
                };
                VisualizationSeries VisSeries = new VisualizationSeries(DefaultColor, (CheckBoxAutoSelectColor.IsChecked ?? false) ? SelectColorAuto : SelectColor);
                return (s, VisSeries);
            }
            return null;
        }

        /// <summary>
        /// Изменить тип цвета выделения
        /// </summary>
        private void ChangeTypeSelColor()
        {
            System.Drawing.Color Coloring = CheckBoxAutoSelectColor.IsChecked ?? false ? SelectColorAuto : SelectColor;
            ColorAnim.To = System.Windows.Media.Color.FromRgb(Coloring.R, Coloring.G, Coloring.B);
            SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
        }

        /// <summary>
        /// Сгенерировать автоматически ярче цвет чем исходный
        /// </summary>
        /// <param name="color">Исходный цвет</param>
        private void GenAutoColor(System.Drawing.Color color)
        {
            byte r, g, b;
            r = color.R + 80 <= 255 ? (byte)(color.R + 80) : (byte)(color.R - (80 - (255 - color.R)));
            g = color.G + 80 <= 255 ? (byte)(color.G + 80) : (byte)(color.G - (80 - (255 - color.G)));
            b = color.B + 80 <= 255 ? (byte)(color.B + 80) : (byte)(color.B - (80 - (255 - color.B)));
            ColorAnim.To = System.Windows.Media.Color.FromArgb(color.A, r, g, b);
            SelectColorAuto = System.Drawing.Color.FromArgb(color.A, r, g, b);
            SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
        }

        /// <summary>
        /// Присвоить цвет обычного состояния параметру
        /// </summary>
        /// <param name="StartColor">Стартовый цвет</param>
        private void SetColorDefault(System.Drawing.Color StartColor)
        {
            System.Drawing.Color? color = ColorInDialogGen(StartColor);
            if (color.HasValue)
            {
                System.Drawing.Color Vcol = color.Value;
                ColorAnim.To = System.Windows.Media.Color.FromArgb(Vcol.A, Vcol.R, Vcol.G, Vcol.B);
                DefaultColor = Vcol;
                DefaultColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
                if (CheckBoxAutoSelectColor.IsChecked ?? false) GenAutoColor(Vcol);
            }
        }

        /// <summary>
        /// Присвоить цвет выделеного состояния параметру
        /// </summary>
        /// <param name="StartColor">Стартовый цвет</param>
        /// <returns>Изменился цвет или нет</returns>
        private bool SetColorSelect(System.Drawing.Color StartColor)
        {
            System.Drawing.Color? color = ColorInDialogGen(StartColor);
            if (color.HasValue)
            {
                System.Drawing.Color Vcol = color.Value;
                ColorAnim.To = System.Windows.Media.Color.FromRgb(Vcol.R, Vcol.G, Vcol.B);
                SelectColor = Vcol;
                SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
            }
            return color.HasValue;
        }

        /// <summary>
        /// Узнать цвет с помощью диалогового окна
        /// </summary>
        /// <param name="StartColorDialog">Стартовый цвет в диалоговом окне</param>
        /// <returns>Возможный цвет</returns>
        private System.Drawing.Color? ColorInDialogGen(System.Drawing.Color StartColorDialog)
        {
            ColorDialog dialog = new ColorDialog
            {
                Color = StartColorDialog
            };
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes) return dialog.Color;
            return null;
        }
    }
}
