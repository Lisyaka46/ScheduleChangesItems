using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            GeneralSettings.Click += (sender, e) =>
            {
                GeneralSettings.Content = "dfn";
                FrameSettings.Navigate(new Uri("/Windows/Frames/Settings/PageGeneralSettings.xaml"));
                FrameSettings.Refresh();
            };
        }
    }
}
