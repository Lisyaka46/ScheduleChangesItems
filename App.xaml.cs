using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ScheduleChangesItems.Classes;

namespace ScheduleChangesItems
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Конвертация карты цвета в объект данных изображения
        /// </summary>
        /// <param name="bmp">Карта цвета</param>
        /// <returns>Объект данных изображения</returns>
        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        /// <summary>
        /// Структура константных значений директорий программы
        /// </summary>
        public struct Pathes
        {
            /// <summary>
            /// <b>ДИРЕКТОРИЯ</b> файла настроек
            /// </summary>
            public const string PathSettings = "Settings.txt";
        }

        /// <summary>
        /// Объект настроек программы
        /// </summary>
        public static ObjectSettings Setting { get; } = new ObjectSettings();
    }
}
