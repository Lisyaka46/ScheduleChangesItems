using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Media.Animation;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogCreateThreadFile.xaml
    /// </summary>
    public partial class DialogCreateThreadFile : Window
    {
        /// <summary>
        /// Состояние отмены создания файла
        /// </summary>
        private bool CancelAcces = true;

        /// <summary>
        /// Диалоговое окно создания файла тенденции
        /// </summary>
        public DialogCreateThreadFile()
        {
            InitializeComponent();
            TextBoxDirectory.Background = new SolidColorBrush(Colors.White);
            ButtonReview.MouseUp += (sender, e) =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = TextBoxNameFile.Text, // Default file name
                    DefaultExt = ".txtpoint", // Default file extension
                    Filter = "Алгоритм тенденции (.txtpoint)|*.txtpoint", // Filter files by extension
                    Title = "Добавление директории к файлу"
                };
                if (saveFileDialog.ShowDialog() ?? false)
                {
                    TextBoxNameFile.Text = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    TextBoxDirectory.Text = saveFileDialog.FileName;
                }
            };
            TextBoxNameFile.GotMouseCapture += (sender, e) => TextBoxNameFile.SelectAll();
            ButtonComlete.MouseUp += (sender, e) =>
            {
                if (TextBoxDirectory.Text.Length > 0)
                {
                    CancelAcces = false;
                    Close();
                }
                else
                {
                    TextBoxDirectory.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Colors.Red, Colors.White, TimeSpan.FromMilliseconds(1200)));
                }
            };
            ButtonCancel.MouseUp += (sender, e) =>
            {
                CancelAcces = true;
                Close();
            };
        }

        /// <summary>
        /// Создать файл тенденции через диалоговое окно
        /// </summary>
        /// <returns>Директория созданного файла тенденции</returns>
        public string CreateFile()
        {
            ShowDialog();
            return CancelAcces ? string.Empty : TextBoxDirectory.Text;
        }
    }
}
