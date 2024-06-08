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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScheduleChangesItems.Windows.Frames.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageGeneralSettings.xaml
    /// </summary>
    public partial class PageGeneralSettings : Page
    {
        /// <summary>
        /// Страница <b>общих</b> настроек программы
        /// </summary>
        public PageGeneralSettings()
        {
            InitializeComponent();
            VisiblyMaxMin.IsChecked = App.Setting.VisiblyMax_and_Min;
            VisiblyMaxMin.Click += (sender, e) =>
            {
                App.Setting.VisiblyMax_and_Min = VisiblyMaxMin.IsChecked.Value;
            };
        }
    }
}
