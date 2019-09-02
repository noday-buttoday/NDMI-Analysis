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
using System.Windows.Shapes;

namespace SatelliteDataPolling
{
    /// <summary>
    /// InitSettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InitSettingControl : Window
    {
        public InitSettingControl()
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch (btn.Content.ToString())
            {
                case "자동":
                    SatelliteDataPolling.SettingWindow.SettingClass.Instance.AutoFlag = true;
                    SatelliteDataPolling.SettingWindow.SettingClass.Instance.SaveConfig();
                    break;
                case "수동":
                    SatelliteDataPolling.SettingWindow.SettingClass.Instance.AutoFlag = false;
                    SatelliteDataPolling.SettingWindow.SettingClass.Instance.SaveConfig();
                    break;
            }

            MainWindow main = new MainWindow();

            main.Show();
            main.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            this.Close();
        }
    }
}
