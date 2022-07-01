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
using TuringMachineClassLib;

namespace TuringMachineApp
{
    /// <summary>
    /// Логика взаимодействия для CreateConfigDialogWindow.xaml
    /// </summary>
    public partial class CreateConfigDialogWindow : Window
    {
        public TuringMachineConfig TMConfig { get; set; }

        public CreateConfigDialogWindow()
        {
            InitializeComponent();
            TMConfig = new TuringMachineConfig();
            this.DataContext = TMConfig;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
