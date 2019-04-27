using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlightSimulator.ViewModels.Windows
{
    class AutoPilotViewModel
    {
        IMainWinModel model;
        //CTOR
        public AutoPilotViewModel(IMainWinModel m)
        {
            model = m;
        }
        public ICommand AutoPilotCommand
        {
            get
            {
                return model.AutoPilotCommand;
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return model.ClearCommand;
            }
        }
    }
}
