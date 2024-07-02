using htyö_GUI.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace htyö_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Creating database
            base.OnStartup(e);

            DbInitializer dbInitializer = new DbInitializer();
            dbInitializer.InitializeDatabase();
        }
    }
}
