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
    public partial class DialogAddSeries : Window
    {
        private string[] NameSeriesesInicialized;

        private System.Drawing.Color DefColor;

        private System.Drawing.Color SelColor;

        private System.Drawing.Color SelColorAuto;

        private readonly ColorAnimation ColorAnim;

        private bool Complete = false;

        /// <summary>
        /// Диалоговое окно для создания коллекции
        /// </summary>
        public DialogAddSeries()
        {
            InitializeComponent();
            CheckBoxAutoSelectColor.IsChecked = true;
            DefaultColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            SelectColorView.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(80, 80, 80));
            DefColor = System.Drawing.Color.FromArgb(0, 0, 0);
            SelColor = System.Drawing.Color.FromArgb(80, 80, 80);
            SelColorAuto = System.Drawing.Color.FromArgb(80, 80, 80);
            ColorAnim = new ColorAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(100)),
            };
            DefaultColorView.MouseLeftButtonUp += (sender, e) => SetColorDefault();
            SelectColorView.MouseLeftButtonUp += (sender, e) =>
            {
                SetColorSelect();
                CheckBoxAutoSelectColor.IsChecked = false;
            };
            CheckBoxAutoSelectColor.Checked += (sender, e) =>
            {
                GenAutoColor(DefColor);
                ChangeTypeSelColor();
            };
            CheckBoxAutoSelectColor.Unchecked += (sender, e) => ChangeTypeSelColor();
            ButtonCreateSeries.Click += (sender, e) =>
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
        /// Сгенерировать с помощью диалогового окна колекцию
        /// </summary>
        /// <returns>Коллекция (может быть пустой)</returns>
        public (Series, System.Drawing.Color)? GenSeries(string[] NamesSeries)
        {
            NameSeriesesInicialized = NamesSeries;
            ShowDialog();
            if (Complete)
            {
                Series s = new Series
                {
                    Color = DefColor,
                    Name = TextBoxNameSeries.Text
                };
                return (s, (CheckBoxAutoSelectColor.IsChecked ?? false) ? SelColorAuto : SelColor);
            }
            return null;
        }

        /// <summary>
        /// Изменить тип цвета выделения
        /// </summary>
        private void ChangeTypeSelColor()
        {
            if (CheckBoxAutoSelectColor.IsChecked ?? false)
                ColorAnim.To = System.Windows.Media.Color.FromArgb(SelColorAuto.A, SelColorAuto.R, SelColorAuto.G, SelColorAuto.B);
            else
                ColorAnim.To = System.Windows.Media.Color.FromArgb(SelColor.A, SelColor.R, SelColor.G, SelColor.B);
            SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
        }

        /// <summary>
        /// Сгенерировать автоматически ярче цвет чем исходный
        /// </summary>
        /// <param name="color">Исходный цвет</param>
        private void GenAutoColor(System.Drawing.Color color)
        {
            byte r, g, b;
            r = color.R + 80 <= 255 ? (byte)(color.R + 80) : (byte)(255 - ((color.R + 80) % 256));
            g = color.G + 80 <= 255 ? (byte)(color.G + 80) : (byte)(255 - ((color.G + 80) % 256));
            b = color.B + 80 <= 255 ? (byte)(color.B + 80) : (byte)(255 - ((color.B + 80) % 256));
            ColorAnimation ColorAnim = new ColorAnimation
            {
                From = ((SolidColorBrush)SelectColorView.Fill).Color,
                To = System.Windows.Media.Color.FromArgb(color.A, r, g, b),
                Duration = new Duration(TimeSpan.FromMilliseconds(100)),
            };
            SelColorAuto = System.Drawing.Color.FromArgb(color.A, r, g, b);
            SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
        }

        /// <summary>
        /// Присвоить цвет обычного состояния параметру
        /// </summary>
        private void SetColorDefault()
        {
            System.Drawing.Color? color = ColorInDialogGen();
            if (color.HasValue)
            {
                System.Drawing.Color Vcol = color.Value;
                ColorAnim.To = System.Windows.Media.Color.FromArgb(Vcol.A, Vcol.R, Vcol.G, Vcol.B);
                DefColor = Vcol;
                DefaultColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
                if (CheckBoxAutoSelectColor.IsChecked ?? false) GenAutoColor(Vcol);
            }
        }

        /// <summary>
        /// Присвоить цвет выделеного состояния параметру
        /// </summary>
        private void SetColorSelect()
        {
            System.Drawing.Color? color = ColorInDialogGen();
            if (color.HasValue)
            {
                System.Drawing.Color Vcol = color.Value;
                ColorAnim.To = System.Windows.Media.Color.FromArgb(Vcol.A, Vcol.R, Vcol.G, Vcol.B);
                SelColor = Vcol;
                SelectColorView.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
            }
        }

        /// <summary>
        /// Узнать цвет с помощью диалогового окна
        /// </summary>
        /// <returns>Возможный цвет</returns>
        private System.Drawing.Color? ColorInDialogGen()
        {
            ColorDialog dialog = new ColorDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes) return dialog.Color;
            return null;
        }
    }
}
