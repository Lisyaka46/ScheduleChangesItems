using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ScheduleChangesItems.Classes;

namespace ScheduleChangesItems
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
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
