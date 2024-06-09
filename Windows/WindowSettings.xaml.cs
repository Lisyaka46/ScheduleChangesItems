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
using ScheduleChangesItems.Windows.Pages.Settings;
using System.Windows.Navigation;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        /// <summary>
        /// Объект данных страницы "Общих" настроек программы
        /// </summary>
        private readonly PageGeneralSettings GeneralSettings = new PageGeneralSettings();

        private readonly PageTextSettings TextSettings = new PageTextSettings();

        /// <summary>
        /// Окно настроек программы
        /// </summary>
        public WindowSettings()
        {
            InitializeComponent();
            FrameSettings.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            ButtonGeneralSettings.MouseUp += (sender, e) =>
            {
                FrameSettings.Navigate(GeneralSettings);
                //if (!FrameSettings.CanGoBack) FrameSettings.Navigate(new PageGeneralSettings());
            };

            ButtonTextSettings.MouseUp += (sender, e) =>
            {
                FrameSettings.Navigate(TextSettings);
                //if (!FrameSettings.CanGoBack) FrameSettings.Navigate(new PageGeneralSettings());
            };
        }
    }
}
