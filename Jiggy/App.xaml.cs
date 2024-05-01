using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Jiggy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            using (SingleInstanceApp singleInstance = new SingleInstanceApp())
            {
                if (!singleInstance.IsSingleInstance())
                    return;

                Jiggy.App app = new Jiggy.App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
    internal class SingleInstanceApp : IDisposable
    {
        Mutex mutex = new Mutex(false, "Jiggy");

        public bool IsSingleInstance() => mutex.WaitOne(0, false);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                mutex.ReleaseMutex();
            }
            catch { }
            if (mutex != null)
            {
                mutex.Close();
            }
            mutex = null;
        }
    }
}
