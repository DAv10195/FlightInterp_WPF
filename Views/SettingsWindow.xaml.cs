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
using FlightSimulator.ViewModels.Windows;

namespace FlightSimulator.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SettingsWindowViewModel vm; //ViewModel

        public SettingsWindow(SettingsWindowViewModel viewMod)
        {
            InitializeComponent();
            vm = viewMod;
            vm.ReloadSettings();
            this.DataContext = vm;
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveSettings();
            this.Close();
        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
