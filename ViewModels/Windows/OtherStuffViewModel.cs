using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlightSimulator.ViewModels.Windows
{
    public class OtherStuffViewModel
    {
        private IMainWinModel model;

        public OtherStuffViewModel(IMainWinModel m)
        {
            this.model = m;
        }

        public ICommand OpenSettCommand
        {
            get
            {
                return model.OpenSettCommand;
            }
        }

        public ICommand ConnectCommand
        {
            get
            {
                return model.ConnectCommand;
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return model.ExitCommand;
            }
        }
    }
}
