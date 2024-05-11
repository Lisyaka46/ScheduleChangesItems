using ScheduleChangesItems.Classes.TxtPoint;
using System.Windows;
using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogAddTPoint.xaml
    /// </summary>
    public partial class DialogAddPoint : Window
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
        public DialogAddPoint()
        {
            InitializeComponent();
            Height = 112;
            Width = 360;
            ButtonComplete.Click += (sender, e) =>
            {
                try
                {
                    _point = (TextName.Text, Convert.ToInt32(TextValue.Text));
                    Complete = true;
                    Close();
                }
                catch (FormatException) { MessageBox.Show("Неверный формат числа значения позиции тенденции.", "Неверный ввод числа"); }
            };
        }

        /// <summary>
        /// Создать точку позиции тенденции
        /// </summary>
        /// <returns>Позиция тенденции (Может быть пустой)</returns>
        internal (string, int)? GenerateTPoint()
        {
            ShowDialog();
            if (Complete) return _point;
            return null;
        }
    }
}
