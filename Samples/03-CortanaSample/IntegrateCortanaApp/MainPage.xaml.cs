using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntegrateCortanaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OnLoadVCDFiles(object sender, RoutedEventArgs e)
        {
            // 加入定义好的 Voice Command Definition file
            var vcdFolder = await Package.Current.InstalledLocation.GetFolderAsync("Commands");
            var vcdfile = await vcdFolder.GetFileAsync("Command1.xml");
            var installedCS = VoiceCommandDefinitionManager.InstalledCommandDefinitions;
            foreach (var item in installedCS)
            {               
                Debug.WriteLine(item.Key);
            }
            
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdfile);
        }

        private async void OnSearchBus(object sender,RoutedEventArgs e)
        {
            BusSearchModel.SearchService service = new BusSearchModel.SearchService();
            //var result = await service.SearchBusWithLocation();
            //if (result!= null)
            //{
            //    Debug.WriteLine(result.Count);
            //}
            var result = await service.SearchBusList();
        }
    }
}