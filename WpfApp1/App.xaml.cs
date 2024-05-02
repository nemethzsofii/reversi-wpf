using System.Windows;
using WpfApp1.Data;
using WpfApp1.Model;
using WpfApp1.ViewModels;
using System.Windows.Threading;
using System;
using WpfApp1.View2;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ReversiViewModel _viewModel;
        private readonly DispatcherTimer _timer;
        private readonly IReversiDataAccess _dataAccess = new ReversiFileDataAccess();
        private readonly WpfData _data;
        private readonly Logic _model;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            _data = new WpfData();
            _dataAccess = new ReversiFileDataAccess();
            _model = new Logic(_data, _dataAccess);
            _viewModel = new ReversiViewModel(_model);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => _model.TimerTick();

            _viewModel.TimerStarted += (_, _) => StartTimer();
            _viewModel.TimerStopped += (_, _) => StopTimer();

            _viewModel.MessageBoxNemSikerult += (_, _) => MnS();
            _viewModel.MessageBoxSikerult += (_, _) => MS();
            _viewModel.MessageBoxPause += (_, _) => MP();

            //StartTimer();
        }

        public void StartTimer() => _timer.Start();
        public void StopTimer() => _timer.Stop();

        public void MnS() => MnSS();
        public void MS() => MSS();
        public void MP() => MsP();
        public static void MSS()
        {
            MessageBox.Show("Success:)");
        }
        public static void MsP()
        {
            MessageBox.Show("Game Paused!");
        }

        public static void MnSS()
        {
            MessageBox.Show("Error! Something went wrong!");
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();

            window.DataContext = _viewModel;

            window.Show();
        }
    }
}
