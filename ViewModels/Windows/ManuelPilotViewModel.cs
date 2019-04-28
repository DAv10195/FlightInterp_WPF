using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightSimulator.Model.Interface;
using System.Windows.Controls;
using FlightSimulator.Model;
using FlightSimulator.Views;
using FlightSimulator.Model.EventArgs;

namespace FlightSimulator.ViewModels.Windows
{
    public class ManuelPilotViewModel : BaseNotify
    {
        private IMainWinModel model;
        private Joystick joystick;
        //CTOR
        public ManuelPilotViewModel(IMainWinModel m, Joystick j)
        {
            model = m;
            joystick = j;
        }

        public double Throttle
        {
            set
            {
                model.Throttle = value;
            }
            get
            {
                return model.Throttle;
            }
        }

        public double Rudder
        {
            set
            {
                model.Rudder = value;
            }
            get
            {
                return model.Rudder;
            }
        }

        public double Flaps
        {
            set
            {
                model.Flaps = value;
            }
            get
            {
                return model.Flaps;
            }
        }

        public double Aileron
        {
            set
            {
                model.Aileron = value;
            }
            get
            {
                return model.Aileron;
            }
        }

        public double Elevator
        {
            set
            {
                model.Elevator = value;
            }
            get
            {
                return model.Elevator;
            }
        }

        public void JoyStick_Handler(Joystick sender, VirtualJoystickEventArgs args)
        {
            if (sender == joystick)
            {
                Aileron = args.Aileron;
                Elevator = args.Elevator;
            }
        }
    }
}
