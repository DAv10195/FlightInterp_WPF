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
using FlightSimulator.Views;
using FlightSimulator.Model;
using FlightSimulator.ViewModels.Windows;
using FlightSimulator.Model.Interface;
using FlightSimulator.ViewModels;
using System.ComponentModel;

namespace FlightSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManuelPilotViewModel vm;
        OtherStuffViewModel osvm;
        //CTOR
        public MainWindow()
        {
            InitializeComponent();
            IMainWinModel m = new MainWinMod(AutoPilotBox);
            vm = new ManuelPilotViewModel(m, joystick);
            FlightBoardViewModel fvm = new FlightBoardViewModel(m);
            AutoPilotViewModel avm = new AutoPilotViewModel(m);
            osvm = new OtherStuffViewModel(m);
            flightBoard.VM = fvm;
            m.PropertyChanged += fvm.Model_PropertyChanged;
            joystick.Moved += vm.JoyStick_Handler;
            this.DataContext = vm;
            autoPilotButt.DataContext = avm;
            clearButt.DataContext = avm;
            connectButt.DataContext = osvm;
            settButt.DataContext = osvm;
            Application.Current.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            osvm.ExitCommand.Execute(null);
        }

        private void decideBoxColor(object sender, TextChangedEventArgs args)
        {
            if (AutoPilotBox.Text != "")
            {
                AutoPilotBox.Background = Brushes.Pink;
            }
            else
            {
                AutoPilotBox.Background = Brushes.White;
            }
        }
    }
}