using ScheduleChangesItems.Classes.TxtPoint;
using System.Windows;
using System;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogAddTPoint.xaml
    /// </summary>
    public partial class DialogManagementPoint : Window
    {
        /// <summary>
        /// Создаваемая позиция
        /// </summary>
        private (string, int) _point = (string.Empty, 0);

        /// <summary>
        /// Состояние успешного создания
        /// </summary>
        private bool Complete = false;

        /// <summary>
        /// Инициализация диалогового окна создающего позицию тенденции
        /// </summary>
        public DialogManagementPoint()
        {
            InitializeComponent();
            ButtonComplete.MouseUp += (sender, e) =>
            {
                try
                {
                    _point = (TextName.Text, Convert.ToInt32(TextValue.Text));
                    Complete = true;
                    Close();
                }
                catch (FormatException) { MessageBox.Show("Неверный формат числа значения позиции тенденции.", "Неверный ввод числа"); }
            };
            TextValue.Background = new SolidColorBrush(Colors.White);
            ButtonValue_min1.MouseUp += (sender, e) => StringTextUpNum(-1);
            ButtonValue_min5.MouseUp += (sender, e) => StringTextUpNum(-5);
            ButtonValue_min10.MouseUp += (sender, e) => StringTextUpNum(-10);
            ButtonValue_min15.MouseUp += (sender, e) => StringTextUpNum(-15);
            ButtonValue_min30.MouseUp += (sender, e) => StringTextUpNum(-30);

            ButtonValue_pl1.MouseUp += (sender, e) => StringTextUpNum(1);
            ButtonValue_pl5.MouseUp += (sender, e) => StringTextUpNum(5);
            ButtonValue_pl10.MouseUp += (sender, e) => StringTextUpNum(10);
            ButtonValue_pl18.MouseUp += (sender, e) => StringTextUpNum(18);
            ButtonValue_pl30.MouseUp += (sender, e) => StringTextUpNum(30);
            ButtonValue_pl45.MouseUp += (sender, e) => StringTextUpNum(45);
        }

        private void StringTextUpNum(int NumChanging)
        {
            try
            {
                TextValue.Text = (Convert.ToInt32(TextValue.Text) + NumChanging).ToString();
            }
            catch
            {
                ColorAnimation animation = new ColorAnimation(Colors.Red, Colors.White, TimeSpan.FromMilliseconds(1200));
                TextValue.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
        }

        /// <summary>
        /// Создать точку позиции тенденции
        /// </summary>
        /// <returns>Позиция тенденции (Может быть пустой)</returns>
        internal (string, int)? GenerateTPoint(int StartValue)
        {
            int ComMn = (int)(StartValue - StartValue / 3.14d);
            ButtonValue_Com.Texting = $"{(ComMn > 0 ? "+" : string.Empty)}{ComMn}";
            ButtonValue_Com.MouseUp += (sender, e) => StringTextUpNum(ComMn);
            ButtonValue_mn2.MouseUp += (sender, e) => StringTextUpNum(StartValue);
            Title = "Добавление точки тенденции";
            Icon = App.ImageSourceFromBitmap(Properties.Resources.Add);
            ButtonComplete.Texting = "Создать";
            TextValue.Text = StartValue.ToString();
            ShowDialog();
            if (Complete) return _point;
            return null;
        }

        /// <summary>
        /// Редактировать точку тенденции
        /// </summary>
        /// <param name="point">Редактируемая точка</param>
        /// <returns>Позиция тенденции (Может быть пустой)</returns>
        internal (string, int)? ChangeInfoPoint(DataPoint point)
        {
            int Num = (int)point.YValues[0], ComMn = (int)(Num - Num / 3.14d);
            ButtonValue_Com.Texting = $"{(ComMn > 0 ? "+" : string.Empty)}{ComMn}";
            ButtonValue_Com.MouseUp += (sender, e) => StringTextUpNum((int)(Num - Num / 3.14d));
            ButtonValue_mn2.MouseUp += (sender, e) => StringTextUpNum(Num);
            TextName.Text = point.AxisLabel;
            TextValue.Text = Num.ToString();
            Title = "Изменение точки тенденции";
            Icon = App.ImageSourceFromBitmap(Properties.Resources.Edit);
            ButtonComplete.Texting = "Изменить";
            ShowDialog();
            if (Complete) return _point;
            return null;
        }
    }
}
