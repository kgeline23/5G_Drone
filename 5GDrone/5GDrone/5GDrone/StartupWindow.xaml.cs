using _5GDrone.Net;
using Networking;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Networking.Client;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace _5GDrone
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {

        private Client client;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;


        public StartupWindow(Client client)
        {
            InitializeComponent();




            if (client != null)
                MessageBox.Show(" DroneClient is not null");

            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);
        }

        public StartupWindow()
            : this(null) { }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {

            //start drone by executing the python code
            //this will execute a python file
            //searchPaths.Add(@"C:\Python27\Lib"); 
            /*
            Microsoft.Scripting.Hosting.ScriptEngine pythonEngine = IronPython.Hosting.Python.CreateEngine();
            Microsoft.Scripting.Hosting.ScriptSource pythonScript = pythonEngine.CreateScriptSourceFromFile("..\\..\\PythonServer.py");
            pythonScript.Execute();
            */

            /*
                        var engine = Python.CreateEngine();
                        var searchPaths = engine.GetSearchPaths();
                        searchPaths.Add(@"C:\Users\the5g\Documents\GitHub\5G_Drone\5GDrone\5GDrone\packages\DynamicLanguageRuntime.1.1.2");
                        searchPaths.Add(@"C:\Python27\Lib");
                        searchPaths.Add(@"C:\Users\the5g\Documents\GitHub\5G_Drone\5GDrone\5GDrone");
                        searchPaths.Add(@"C:\Users\the5g\Documents\GitHub\5G_Drone\5GDrone\5GDrone\5GDrone\PythonServer.py");

                        engine.SetSearchPaths(searchPaths);
                        var mainfile = @"C:\Users\the5g\Documents\GitHub\5G_Drone\5GDrone\5GDrone\5GDrone\PythonServer.py";
                        var scope = engine.CreateScope();
                        engine.CreateScriptSourceFromFile(mainfile).Execute();
                        //var result = scope.GetVariable("res");
                        // Console.WriteLine(result);
                        //Console.ReadKey();
            */

            //Basic engine to run python script. - Part1
            ScriptEngine engine = Python.CreateEngine();
            string pythonScriptPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            ScriptSource source = engine.CreateScriptSourceFromFile(pythonScriptPath + "/PythonServer.py");
            ScriptScope scope = engine.CreateScope();
            source.Execute(scope);
            /*
            //Passing values
            Object myclass = engine.Operations.Invoke(scope.GetVariable("pythonScriptClass"));
            object[] parameters = new object[] { "Hi", 3 };
            engine.Operations.InvokeMember(myclass, "theMethod", parameters);
            */

            //client.connect()

            //if drone was able to start then swap windows
            MainWindow main = new MainWindow(this.client);
            main.Show();
            this.Close();
            //else output error message


        }

    }
}

