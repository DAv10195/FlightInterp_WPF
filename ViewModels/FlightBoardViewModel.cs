using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulator.ViewModels
{
    public class FlightBoardViewModel : BaseNotify
    {
        private MainWinMod model;
        //CTOR
        public FlightBoardViewModel(MainWinMod m)
        {
            model = m;
        }

        public double Lon
        {
            get { return model.Lon; }
            set { NotifyPropertyChanged("Lon"); }
        }

        public double Lat
        {
            get { return model.Lat; }
            set { NotifyPropertyChanged("Lat"); }
        }

        public void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainWinMod m = sender as MainWinMod;
            if (m != null && m == model)
            {
                if (e.PropertyName.Equals("Lat"))
                {
                    Lat = model.Lat;
                }
                if (e.PropertyName.Equals("Lon"))
                {
                    Lon = model.Lon;
                }
            }
        }
    }
}
