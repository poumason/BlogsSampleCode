using Microsoft.Toolkit.Wpf.UI.XamlHost;
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

namespace WpfAppHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            WindowsXamlHost windowsXamlHost = (WindowsXamlHost)sender;
            // 利用 Windows.UI.Xaml.Controls 做為轉換
            Windows.UI.Xaml.Controls.Button button = (Windows.UI.Xaml.Controls.Button)windowsXamlHost.Child;
            Windows.UI.Xaml.Controls.TextBlock txt = new Windows.UI.Xaml.Controls.TextBlock();
            txt.Text = "123";
            button.Content = txt;
        }

        private void MyUWPPage_ChildChanged(object sender, EventArgs e)
        {
            // Hook up x:Bind source
            WindowsXamlHost windowsXamlHost = (WindowsXamlHost)sender;
            global::MyUWPControls.BlankPage1 myUWPPage = windowsXamlHost.GetUwpInternalObject() as global::MyUWPControls.BlankPage1;

            if (myUWPPage != null)
            {
                myUWPPage.WPFMessage = this.WPFMessage;
            }
        }

        public string WPFMessage
        {
            get
            {
                return "Binding from WPF to UWP XAML";
            }
        }
    }
}