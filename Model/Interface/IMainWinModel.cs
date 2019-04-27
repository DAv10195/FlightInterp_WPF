using FlightSimulator.Model.EventArgs;
using FlightSimulator.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlightSimulator.Model.Interface
{
    public interface IMainWinModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propName);
        double Aileron { get; set; }
        double Throttle { get; set; }
        double Rudder { get; set; }
        double Elevator { get; set; }
        double Flaps { get; set; }
        double Lon { get; set; }
        double Lat { get; set; }
        ICommand OpenSettCommand { get; }
        ICommand ExitCommand { get; }
        ICommand ClearCommand { get; }
        ICommand AutoPilotCommand { get; }
        ICommand ConnectCommand { get; }
    }
}
