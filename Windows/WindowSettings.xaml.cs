using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using ScheduleChangesItems.Windows.Frames.Settings;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        /// <summary>
        /// Окно настроек программы
        /// </summary>
        public WindowSettings()
        {
            InitializeComponent();
            ButtonGeneralSettings.MouseUp += (sender, e) =>
            {
                FrameSettings = null;
                FrameSettings.Navigate(new PageGeneralSettings());
            };
        }
    }
}
