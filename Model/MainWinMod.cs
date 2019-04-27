using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightSimulator.Model.Interface;
using FlightSimulator.Views;
using FlightSimulator.ViewModels.Windows;
using System.Windows.Controls;
using FlightSimulator.ViewModels;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Media;
using FlightSimulator.Model.EventArgs;

namespace FlightSimulator.Model
{
    public class MainWinMod : BaseNotify
    {
        private TextBox AutoPilotBox;
        private double lon;
        private double lat;
        private double throttle;
        private double aileron;
        private double elevator;
        private double rudder;
        private double flaps;
        private volatile TcpListener listener;
        private volatile TcpClient info;
        private volatile TcpClient commands;
        private volatile NetworkStream inputStream;
        private volatile NetworkStream outputStream;
        private volatile BinaryReader reader;
        private volatile StreamWriter writer;
        private volatile string inputedCommands;
        Dictionary<string, string> paths;
        private volatile Queue<ICommand> exec;
        //CTOR
        public MainWinMod(Joystick j, TextBox tb)
        {
            this.AutoPilotBox = tb;
            this.listener = null;
            this.info = null;
            this.commands = null;
            this.inputStream = null;
            this.outputStream = null;
            this.reader = null;
            this.writer = null;
            this.paths = new Dictionary<string, string>();
            this.initPaths();
            this.throttle = 0;
            this.rudder = 0;
            this.flaps = 0;
            this.aileron = 0;
            this.elevator = 0;
            this.inputedCommands = "";
            this.exec = new Queue<ICommand>();
        }

        private void initPaths()
        {
            paths["aileron"] = "/controls/flight/aileron";
            paths["elevator"] = "/controls/flight/elevator";
            paths["rudder"] = "/controls/flight/rudder";
            paths["flaps"] = "/controls/flight/flaps";
            paths["throttle"] = "/controls/engines/current-engine/throttle";
        }

        public double Lon
        {
            set { this.lon = value; NotifyPropertyChanged("Lon"); }
            get { return this.lon; }
        }

        public double Lat
        {
            set { this.lat = value; NotifyPropertyChanged("Lat"); }
            get { return this.lat; }
        }
        //open settings window command-action
        private void Sett_Click()
        {
            SettingsWindow settWin = new SettingsWindow(new SettingsWindowViewModel(new ApplicationSettingsModel()));
            settWin.Show();
        }
        public ICommand OpenSettCommand
        {
            get
            {
                return new CommandHandler(() => Sett_Click());
            }
        }
        //open connection for FlightGear to connect on info channel and connect to FlightGear on commands channel
        private void Connect_Click(string FlightServerIP, int FlightInfoPort, int FlightCommandPort)
        {
            new Thread(delegate ()
            {
                try
                {   //set up connection for FlightGear to connect to...
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(FlightServerIP), FlightInfoPort);
                    listener = new TcpListener(ep);
                    listener.Start();   //start listening...
                    //accept connection from FlightGear
                    info = listener.AcceptTcpClient();
                    listener.Stop();
                    //set up commands channel via connecting to FlightGear as a client
                    ep = new IPEndPoint(IPAddress.Parse(FlightServerIP), FlightCommandPort);
                    commands = new TcpClient();
                    commands.Connect(ep);
                    inputStream = info.GetStream();
                    outputStream = commands.GetStream();
                    reader = new BinaryReader(inputStream);
                    writer = new StreamWriter(outputStream, Encoding.UTF8);
                    sendMessage(paths["throttle"], 0); //a message to show we mean bussiness...
                    //accept Lon and Lat values inputed through the info channel in the background...
                    string[] vals = null;
                    string message = null;
                    while (reader != null)
                    {
                        message = reader.ReadString();
                        vals = message.Split(',');
                        try
                        {
                            //Lon should be first...
                            Lon = Double.Parse(vals[0]);
                            //Lat should be second...
                            Lat = Double.Parse(vals[1]);
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception)
                {
                    this.closeResources();
                }
            }).Start();
        }
        public ICommand ConnectCommand
        {
            get
            {
                return new CommandHandler(() => Connect_Click(Properties.Settings.Default.FlightServerIP, Properties.Settings.Default.FlightInfoPort,
                    Properties.Settings.Default.FlightCommandPort));
            }
        }
        //clears the buffer
        private void Clear_Click(TextBox tb)
        {
            tb.Background = Brushes.White;
            tb.Text = "";
        }
        public ICommand ClearCommand
        {
            get
            {
                return new CommandHandler(() => Clear_Click(AutoPilotBox));
            }
        }
        //closes channels
        private void closeResources()
        {
            try
            {
                if (this.info != null) { this.info.Close(); this.info = null; }
                if (this.commands != null) { this.commands.Close(); this.commands = null; }
                if (this.inputStream != null) { this.inputStream.Close(); this.inputStream = null; }
                if (this.outputStream != null) { this.outputStream.Close(); this.outputStream = null; }
                if (this.reader != null) { this.reader.Close(); this.reader = null; }
                if (this.writer != null) { this.writer.Close(); this.writer = null; }
                if (this.listener != null) { this.listener.Stop(); this.listener = null; }
            }
            catch (Exception) { }
        }
        //will be called upon closing main window
        private void X_Click()
        {
            closeResources();
        }
        public ICommand ExitCommand
        {
            get
            {
                return new CommandHandler(() => X_Click());
            }
        }
        //will be called upon sending a set of commands via the auto pilot box
        private void AutoPilotOk_Click(TextBox tb)
        {
            AutoPilotBox.Background = Brushes.White;
            inputedCommands = tb.Text;
            try
            {
                string[] lines = inputedCommands.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int i = 0, j = 0, length = 0, size = lines.Length;
                for (; i < size; ++i)
                {
                    string[] toExec = lines[i].Split(' ');
                    double parse = 0;
                    length = toExec.Length;
                    for (j = 0; j < length; ++j)
                    {
                        if (lines[i] != "" && toExec.Length == 3)
                        {
                            //case command began with a variable name binded to a path with the form: "var = val"
                            if (paths.Keys.Contains<string>(toExec[0]) && toExec[1] == "=" && Double.TryParse(toExec[2], out parse))
                            {
                                if (toExec[0] == "throttle")
                                {
                                    exec.Enqueue(new CommandHandler(() => sendMessage(paths["throttle"], parse)));
                                }
                                if (toExec[0] == "rudder")
                                {
                                    exec.Enqueue(new CommandHandler(() => sendMessage(paths["rudder"], parse)));
                                }
                                if (toExec[0] == "aileron")
                                {
                                    exec.Enqueue(new CommandHandler(() => sendMessage(paths["aileron"], parse)));
                                }
                                if (toExec[0] == "elevator")
                                {
                                    exec.Enqueue(new CommandHandler(() => sendMessage(paths["elevator"], parse)));
                                }
                                if (toExec[0] == "flaps")
                                {
                                    exec.Enqueue(new CommandHandler(() => sendMessage(paths["flaps"], parse)));
                                }
                            }
                        }
                        //case command is of the raw form: "set path val"
                        if (toExec[0] == "set" && (paths.Values.Contains<string>(toExec[1]) || paths.Values.Contains<string>("/" + toExec[1]))
                        && Double.TryParse(toExec[2], out parse))
                        {
                            foreach (KeyValuePair<string, string> kvPair in paths)
                            {
                                if (kvPair.Value == toExec[1] || kvPair.Value == "/" + toExec[1])
                                {
                                    if (kvPair.Value == "/controls/engines/current-engine/throttle")
                                    {
                                        exec.Enqueue(new CommandHandler(() => sendMessage(paths["throttle"], parse)));
                                    }
                                    if (kvPair.Value == "/controls/flight/rudder")
                                    {
                                        exec.Enqueue(new CommandHandler(() => sendMessage(paths["rudder"], parse)));
                                    }
                                    if (kvPair.Value == "/controls/flight/aileron")
                                    {
                                        exec.Enqueue(new CommandHandler(() => sendMessage(paths["aileron"], parse)));
                                    }
                                    if (kvPair.Value == "/controls/flight/elevator")
                                    {
                                        exec.Enqueue(new CommandHandler(() => sendMessage(paths["elevator"], parse)));
                                    }
                                    if (kvPair.Value == "/controls/flight/flaps")
                                    {
                                        exec.Enqueue(new CommandHandler(() => sendMessage(paths["flaps"], parse)));
                                    }
                                }
                            }
                        }
                    }
                }
                if (exec.Count != 0)
                {
                    new Thread(delegate ()
                    {
                        while (exec.Count != 0)
                        {
                            exec.Dequeue().Execute(null);
                            if (exec.Count != 0)
                            {
                                System.Threading.Thread.Sleep(2000); //sleep for 2s...
                            }
                        }
                    }).Start();
                }
            }
            catch (Exception) { }
        }

        public ICommand AutoPilotCommand
        {
            get
            {
                return new CommandHandler(() => AutoPilotOk_Click(AutoPilotBox));
            }
        }

        private void sendMessage(string FlightGearPath, double valToSend)
        {
            if (writer != null)
            {
                try
                {
                    string toSend = "set ";
                    toSend += FlightGearPath;
                    toSend += " ";
                    toSend += valToSend.ToString();
                    toSend += " \r\n";
                    writer.Write(toSend);
                    writer.Flush();
                }
                catch (Exception) { }
            }
        }

        public double Throttle
        {
            set { throttle = value; sendMessage(paths["throttle"], value); }
            get { return throttle; }
        }

        public double Rudder
        {
            set { rudder = value; sendMessage(paths["rudder"], value); }
            get { return rudder; }
        }

        public double Flaps
        {
            set { flaps = value; sendMessage(paths["flaps"], value); }
            get { return flaps; }
        }

        public double Aileron
        {
            set { aileron = value; sendMessage(paths["aileron"], value); }
            get { return aileron; }
        }

        public double Elevator
        {
            set { elevator = value; sendMessage(paths["elevator"], value); }
            get { return elevator; }
        }
    }
}