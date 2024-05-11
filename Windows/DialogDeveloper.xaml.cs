using ScheduleChangesItems.Classes.TxtPoint;
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
using ScheduleChangesItems.Classes;

namespace ScheduleChangesItems.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogDeveloper.xaml
    /// </summary>
    public partial class DialogDeveloper : Window
    {
        /// <summary>
        /// Диалоговое окно разработчика для проверки логики
        /// </summary>
        public DialogDeveloper()
        {
            InitializeComponent();
            ButtonCheckReadTag.Click += (sender, e) =>
            {
                Tag tag = TxtPointFileManipulate.ReadTag(TextBoxTag.Text);
                TextName.Text = $"Name: \"{tag.Name}\"";
                TextValue.Text = $"Value: \"{tag.Value}\"";
            };
        }
    }
}
