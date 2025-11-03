using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using BCR_Reader_Pro.Service;
using VSP.CONTROLLER;

namespace BCR_Reader_Pro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //InitDevices();

            // 하드웨어 가속 강제 적용
            RenderOptions.ProcessRenderMode = RenderMode.Default;
            CVSDeviceCtrlManager.Instance.Initialize();

            SequenceManager.Instance.CreateThread();
            //Add OpenLibrary 
            //InitOpenLibrary();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SequenceManager.Instance.ThreadEnd();
        }
        //private void InitOpenLibrary()


    }

}
