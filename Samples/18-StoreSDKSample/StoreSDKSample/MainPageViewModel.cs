using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StoreSDKSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 在 Dev Center dashboard 為 App 建立的實驗 project id
        /// </summary>
        const string ProjectId = "";

        private StoreServicesExperimentVariation variation;
        private StoreServicesCustomEventLogger logger;

        public MainPageViewModel()
        {

        }

        public async Task InitializeExperiment()
        {
            // 取得現在 cache 中的實驗參數
            var result = await StoreServicesExperimentVariation.GetCachedVariationAsync(ProjectId);
            variation = result.ExperimentVariation;

            // 檢查如果有錯誤訊息或是否有新的參數
            if (result.ErrorCode != StoreServicesEngagementErrorCode.None || result.ExperimentVariation.IsStale)
            {
                result = await StoreServicesExperimentVariation.GetRefreshedVariationAsync(ProjectId);

                if (result.ErrorCode == StoreServicesEngagementErrorCode.None)
                {
                    variation = result.ExperimentVariation;
                }
            }

            //// Get the remote variable named "buttonText" and assign the value
            //// to the button.
            //var buttonText = variation.GetString("buttonText", "Grey Button");
            //await button.Dispatcher.RunAsync(
            //    Windows.UI.Core.CoreDispatcherPriority.Normal,
            //    () =>
            //    {
            //        button.Content = buttonText;
            //    });

            //// Log the view event named "userViewedButton" to Dev Center.
            //if (logger == null)
            //{
            //    logger = StoreServicesCustomEventLogger.GetDefault();
            //}

            //logger.LogForVariation(variation, "userViewedButton");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
