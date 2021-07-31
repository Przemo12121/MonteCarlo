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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace MonteCarlo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int L;
        public string[] map_string;
        List<MapChangeStep> mapChangeList;
        List<double> magnetismList;
        bool sim_being_run = false;
        int current_mcs = 0;
        double temperature;

        bool clear_assistant = false;


        public MainWindow()
        {
            InitializeComponent();
            mapChangeList = new List<MapChangeStep>();
            magnetismList = new List<double>();
        }


        void DataReceiverHelp(string data, ref int counter, ref char c)
        {
            if (c == 'n')
            {
                c = data[0];
            }
            else if (c == 'c')
            {
                if (counter < L)
                {
                    map_string[counter] = data;
                    counter++;
                }
                else
                {
                    MapChangeStep xyz = new MapChangeStep(map_string, Int32.Parse(data)+current_mcs, L);
                    mapChangeList.Add(xyz);
                    c = 'n';
                    counter = 0;
                }
            }
            else if (c == 'k')
            {
                if (counter < L)
                {
                    map_string[counter] = data;
                    counter++;
                }
                else
                {
                    counter = -1;
                }
            }
            else if(c == 'm')
            {
                magnetismList.Add(Double.Parse(data, CultureInfo.InvariantCulture));
                c = 'n';
            }
        }

        void SimulationRunner(string arguments, Process mcs)
        {
            int counter = 0;
            mcs.StartInfo.Arguments = arguments;
            char help = 'n';

            mcs.OutputDataReceived += (sender, arg) => {
                if (counter == -1) 
                { 
                    mcs.CancelOutputRead(); 
                } 
                else 
                { 
                    DataReceiverHelp(arg.Data, ref counter, ref help); 
                } 
            };

            mcs.Start();
            mcs.BeginOutputReadLine();
            mcs.WaitForExit();
        }
        string[] SetupMap(int N)
        {
            string[] map = new string[N];
            double x = double.Parse(input_probability.Text, CultureInfo.InvariantCulture);
            if (x == 0)
            {
                string s;
                for(int i = 0; i<N; i++)
                {
                    s = "";
                    for (int j = 0; j < N; j++)
                    {
                        s += "0";
                    }
                    map[i] = s;
                }
            }
            else if (x == 1)
            {
                string s;
                for (int i = 0; i < N; i++)
                {
                    s = "";
                    for (int j = 0; j < N; j++)
                    {
                        s += "1";
                    }
                    map[i] = s;
                }
            }
            else
            {
                int i = 0;
                double k = Math.Pow(10, (input_probability.Text.Length - 2));
                x *= k;
                string sss = N.ToString() + " " + x.ToString() + " " + k.ToString();
                Process external = new Process();
                external.StartInfo.FileName = "MapGenerator.exe";
                external.StartInfo.Arguments = sss;
                external.StartInfo.UseShellExecute = false;
                external.StartInfo.RedirectStandardOutput = true;
                external.StartInfo.CreateNoWindow = true;
                external.OutputDataReceived += (sender, arg) => { map[i] = arg.Data; i++; if (i == N) { external.CancelOutputRead(); } };
                external.Start();
                external.BeginOutputReadLine();
                external.WaitForExit();
            }
            return map;
        }

        private Canvas SetPixel(int a, int b, char isUp)
        {
            Canvas pixel = new Canvas();
            pixel.Height = 3;
            pixel.Width = 3;

            if (isUp == '1')
            {
                pixel.Background = Brushes.LightSalmon;
            }
            else
            {
                pixel.Background = Brushes.LightSlateGray;
            }
            return pixel;
        }

        private void IntegralsOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!Char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
                else
                {
                    if (((System.Windows.Controls.TextBox)e.OriginalSource).Text.Length == 0)
                    {
                        if (c == '0')
                        {
                            e.Handled = true;
                            break;
                        }
                    }
                    else if (e.OriginalSource == input_length && Int32.Parse(((System.Windows.Controls.TextBox)e.OriginalSource).Text + c) > 150)
                    {
                        e.Handled = true;
                        break;
                    }
                    else if (e.OriginalSource == input_mcs && Int32.Parse(((System.Windows.Controls.TextBox)e.OriginalSource).Text + c) > 1000000)
                    {
                        e.Handled = true;
                        break;
                    }
                }
            }
        }

        private void Doubles_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            foreach (char c in e.Text)
            {
                if (c == '.')
                {
                    if (((System.Windows.Controls.TextBox)e.OriginalSource).Text.Contains("."))
                    {
                        e.Handled = true;
                        break;
                    }
                    else
                    {
                        if(((System.Windows.Controls.TextBox)e.OriginalSource).Text.Length == 0)
                        {
                            ((System.Windows.Controls.TextBox)e.OriginalSource).Text = "0.";
                            e.Handled = true;
                            break;
                        }
                    }
                }
                else if (!Char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
                else if(e.OriginalSource == input_temperature && Double.Parse(((System.Windows.Controls.TextBox)e.OriginalSource).Text + c, CultureInfo.InvariantCulture) > 1000 || ((System.Windows.Controls.TextBox)e.OriginalSource).Text.Length > 4)
                {
                    e.Handled = true;
                    break;
                }
                else if (e.OriginalSource == input_probability && Double.Parse(((System.Windows.Controls.TextBox)e.OriginalSource).Text + c, CultureInfo.InvariantCulture) > 1 || ((System.Windows.Controls.TextBox)e.OriginalSource).Text.Length > 4)
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void LengthNonZero_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(input_length.Text.Length == 0 ||  input_probability.Text.Length == 0 || double.Parse(input_probability.Text, CultureInfo.InvariantCulture) > 1 || Int32.Parse(input_length.Text) <= 0)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void GenerateFirstMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StackPanel column;
            Thickness marg = new Thickness();
            if(mapGrid.Children.Count != 0)
            {
                mapGrid.Children.Clear();
            }
            
            L = Int32.Parse(input_length.Text);

            map_string = SetupMap(L);
            for (int i = 0; i<L; i++)
            {
                column = new StackPanel();
                column.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                column.Width = 3;
                marg.Left = i * 3;
                column.Margin = marg;
                for(int j = 0; j< L; j++)
                {
                    column.Children.Add(SetPixel(i, j, map_string[i][j]));
                }
                mapGrid.Children.Add(column);
            }

        }

        private void MapLoaded_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(map_string == null || input_mcs.Text.Length <= 0 || input_temperature.Text.Length <= 0 || (input_stepcounter.Text.Length > 0 && Int32.Parse(input_stepcounter.Text)> Int32.Parse(input_mcs.Text)) || Int32.Parse(input_mcs.Text) <= 0)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void BeginSimulation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int magnetism = 0;
            
            temperature = double.Parse(input_temperature.Text, CultureInfo.InvariantCulture);
            
            string arguments;
            Process mcs = new Process();
            mcs.StartInfo.FileName = "MonteCarloAlgorithm.exe";
            mcs.StartInfo.UseShellExecute = false;
            mcs.StartInfo.RedirectStandardOutput = true;
            mcs.StartInfo.CreateNoWindow = true;

            arguments = input_temperature.Text + " " + L.ToString();

            if (!sim_being_run)
            {
                MapChangeStep step = new MapChangeStep(map_string, 0, L);
                MapUpdatesCB.Items.Add(0);
                mapChangeList.Add(step);
                for (int i = 0; i < L; i++)
                {
                    for (int j = 0; j < L; j++)
                    {
                        if (map_string[i][j] == '1')
                        {
                            magnetism += 1;
                        }
                        else
                        {
                            magnetism += -1;
                        }
                    }
                }
                magnetismList.Add(((double)magnetism / L / L));
            }
            else
            {
                map_string = (string[])mapChangeList[mapChangeList.Count - 1].map.Clone();
            }

            for (int j = 0; j < L; j++)
            {
                arguments += " ";
                arguments += map_string[j];
            }
            arguments += " ";
            arguments += input_mcs.ToString();
            arguments += " ";
            if (input_stepcounter.Text.Length > 0)
            {
                arguments += input_stepcounter.Text;
            }
            else
            {
                arguments += "-1";
            }

            SimulationRunner(arguments, mcs);
            foreach(MapChangeStep stepp in mapChangeList)
            {
                if(!MapUpdatesCB.Items.Contains(stepp.mcs))
                {
                    MapUpdatesCB.Items.Add(stepp.mcs);
                }
            }
            MapChangeStep xyz = new MapChangeStep(map_string, Int32.Parse(input_mcs.Text)+current_mcs, L);
            mapChangeList.Add(xyz);
            
            MapUpdatesCB.Items.Add(xyz.mcs);
            MapUpdatesCB.SelectedValue = xyz.mcs;


            sim_being_run = true;
            current_mcs += Int32.Parse(input_mcs.Text);
            simRunTB.Text = "Continue simulation";

            input_probability.IsEnabled = false;
            input_temperature.IsEnabled = false;
            input_length.IsEnabled = false;
            genMapBtn.IsEnabled = false;
        }

        private void MapUpdatesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!clear_assistant)
            {
                MapChangeStep chosen = null;
                foreach (MapChangeStep step in mapChangeList)
                {
                    if (step.mcs == Int32.Parse(MapUpdatesCB.SelectedValue.ToString()))
                    {
                        chosen = step;
                        break;
                    }
                }
                for (int j = 0; j < L; j++)
                {
                    for (int k = 0; k < L; k++)
                    {
                        if (chosen.map[j][k] == '1')
                        {
                            if (((Canvas)((StackPanel)mapGrid.Children[j]).Children[k]).Background == Brushes.LightSlateGray)
                            {
                                ((Canvas)((StackPanel)mapGrid.Children[j]).Children[k]).Background = Brushes.LightSalmon;
                            }
                        }
                        else
                        {
                            if (((Canvas)((StackPanel)mapGrid.Children[j]).Children[k]).Background == Brushes.LightSalmon)
                            {
                                ((Canvas)((StackPanel)mapGrid.Children[j]).Children[k]).Background = Brushes.LightSlateGray;
                            }
                        }
                    }
                }
            }
        }

        private void SaveData_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                if (!saveFileDialog1.FileName.Contains(".txt"))
                {
                    saveFileDialog1.FileName += ".txt";
                }
                int i = 0;
                StreamWriter sw = new StreamWriter(@saveFileDialog1.FileName);

                sw.WriteLine("L=" + L.ToString() + "\t" + "T=" + temperature.ToString());
                sw.WriteLine("t[MCS]" + "\t" + "m");
                foreach (double magn in magnetismList)
                {
                    sw.WriteLine(i.ToString() + "\t" + magn.ToString());
                    i++;
                }
                sw.Close();
            }
        }

        private void SaveData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(magnetismList != null && magnetismList.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void RestartSimulation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            map_string = null;
            input_probability.IsEnabled = true;
            input_temperature.IsEnabled = true;
            input_length.IsEnabled = true;
            genMapBtn.IsEnabled = true;
            sim_being_run = false;
            input_probability.Text = null;
            input_temperature.Text = null;
            input_length.Text = null;
            input_mcs.Text = null;
            input_stepcounter.Text = null;
            mapChangeList.Clear();
            magnetismList.Clear();
            mapGrid.Children.Clear();
            clear_assistant = true;
            MapUpdatesCB.Items.Clear();
            clear_assistant = false;
            current_mcs = 0;
            simRunTB.Text = "Begin simulation";
        }
        private void RestartSimulation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sim_being_run)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void mapGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (MapUpdatesCB.Items.Count > 0)
            {
                if (e.Key == Key.Down && MapUpdatesCB.SelectedItem != MapUpdatesCB.Items[MapUpdatesCB.Items.Count - 1])
                {
                    MapUpdatesCB.SelectedItem = MapUpdatesCB.Items[MapUpdatesCB.Items.IndexOf(MapUpdatesCB.SelectedItem)+1];
                }
                else if(e.Key == Key.Up && MapUpdatesCB.SelectedItem != MapUpdatesCB.Items[0])
                {
                    MapUpdatesCB.SelectedItem = MapUpdatesCB.Items[MapUpdatesCB.Items.IndexOf(MapUpdatesCB.SelectedItem) - 1];
                }
            }
        }
    }


    class MapChangeStep
    {
        public int L;
        public string[] map;
        public int mcs;
        public MapChangeStep(string[] mapa, int step, int length)
        {
            this.map = (string[])mapa.Clone();
            this.mcs = step;
            this.L = length;
        }
    }

    #region komendy
    public static class MonteCarloCommands
    {
        public static readonly RoutedUICommand GenerateFirstMap = new RoutedUICommand
   (
       "GenerateFirstMap",
       "GenerateFirstMap",
       typeof(MonteCarloCommands)
   );
        public static readonly RoutedUICommand BeginSimulation = new RoutedUICommand
     (
"BeginSimulation",
"BeginSimulation",
typeof(MonteCarloCommands)
);
        public static readonly RoutedUICommand SaveData = new RoutedUICommand
(
"SaveData",
"SaveData",
typeof(MonteCarloCommands)
);
        public static readonly RoutedUICommand RestartSimulation = new RoutedUICommand
(
"RestartSimulation",
"RestartSimulation",
typeof(MonteCarloCommands)
);
        public static readonly RoutedUICommand CloseWarningWindow = new RoutedUICommand
(
"CloseWarningWindow",
"CloseWarningWindow",
typeof(MonteCarloCommands)
);

    }
    #endregion
}
