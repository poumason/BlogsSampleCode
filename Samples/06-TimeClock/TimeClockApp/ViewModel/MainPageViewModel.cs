using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Power;
using Windows.UI.Xaml;

namespace TimeClockApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer Timer { get; set; }

        private string hhStr;
        public string HHString
        {
            get { return hhStr; }
            set { hhStr = value; OnPropertyChanged("HHString"); }
        }

        private string mmStr;
        public string MMString
        {
            get { return mmStr; }
            set { mmStr = value; OnPropertyChanged("MMString"); }
        }

        private string ssStr;
        public string SSString
        {
            get { return ssStr; }
            set { ssStr = value; OnPropertyChanged("SSString"); }
        }

        private string ttStr;
        public string TTString
        {
            get { return ttStr; }
            set { ttStr = value; OnPropertyChanged("TTString"); }
        }

        private string todayStr;
        public string TodayString
        {
            get { return todayStr; }
            set { todayStr = value; OnPropertyChanged("TodayString"); }
        }

        #region Battery
        private string powerPercent;
        public string PowerPercent
        {
            get { return powerPercent; }
            set { powerPercent = value;  OnPropertyChanged("PowerPercent"); }
        }
        #endregion

        public MainPageViewModel(DispatcherTimer timer)
        {
            Timer = timer;
            timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
            HHString = MMString = SSString = "00";
            powerPercent = "0";
            AddReportUI(Battery.AggregateBattery.GetReport());
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }

        private void Timer_Tick(object sender, object e)
        {
            HHString = DateTime.Now.ToString("HH");
            MMString = DateTime.Now.ToString("mm");
            SSString = DateTime.Now.ToString("ss");
            TTString = DateTime.Now.ToString("tt");
            TodayString = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }

        public void Start()
        {
            Timer?.Start();
        }

        public void Stop()
        {
            Timer?.Stop();
        }

        private async void AggregateBattery_ReportUpdated(Battery sender, object args)
        {           
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AddReportUI(sender.GetReport());
            });
        }

        private  void AddReportUI(BatteryReport report)
        {
            var max = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
            var rem = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
            PowerPercent = ((rem / max) * 100).ToString("F2");
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}