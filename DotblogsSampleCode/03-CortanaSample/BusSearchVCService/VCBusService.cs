using BusSearchModel;
using BusSearchModel.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace BusSearchVCService
{
    public sealed class VCBusService : IBackgroundTask
    {
        VoiceCommandServiceConnection vcConnection;
        BackgroundTaskDeferral serviceDeferral;
        CancellationTokenSource cancelToken;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            // Get trigger details , it from Cortana
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails != null && triggerDetails.Name == "com.poumason.uwp.VCBusService")
            {
                // 取得 VoiceCommandServiceConnection
                vcConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                vcConnection.VoiceCommandCompleted += VcConnection_VoiceCommandCompleted;

                VoiceCommand vCommand = await vcConnection.GetVoiceCommandAsync();
                // 判断 command name
                switch (vCommand.CommandName)
                {
                    case "SearchWhereBusList":
                        foreach (var item in vCommand.Properties)
                        {
                            Debug.WriteLine(String.Format("{0}={1}", item.Key, item.Value));
                        }
                        await HandleSearchWhereBusList();
                        break;
                    case "CheckStartBusStation":
                        ShowHaveOtherBusConfirm();
                        break;
                    default:
                        // 需要注册这个处理，以相容那些不再使用的 VCDs.
                        LaunchAppInForeground();
                        break;
                }
            }
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "开启 App 中...请稍后";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = "";
            await vcConnection.RequestAppLaunchAsync(response);
        }

        private async Task HandleSearchWhereBusList()
        {
            await ShowProgressScreen("处理中 ...");

            // 提供 VoiceCommenContentTile 给用户选择
            List<BusData> result = await new SearchService().SearchBusList();
            result = result.Take(5).ToList();
            List<VoiceCommandContentTile> vcTiles = new List<VoiceCommandContentTile>();
            int i = 0;
            foreach (var item in result)
            {
                vcTiles.Add(new VoiceCommandContentTile
                {
                    Title = item.Name,
                    TextLine1 = String.Format("{0}_{1}", item.Name, i),
                    ContentTileType = VoiceCommandContentTileType.TitleWithText,
                    Image = await StorageFile.GetFileFromApplicationUriAsync(
                        new Uri("ms-appx:///BusSearchVCService/Images/GreyTile.png")),
                    AppContext = item
                });
                i += 1;
            }
            
            // Create a VoiceCommandUserMessage for the initial question.
            VoiceCommandUserMessage userMsg = new VoiceCommandUserMessage();
            userMsg.DisplayMessage = userMsg.SpokenMessage = "请选择";
            // Create a VoiceCommandUserMessage for the second question,
            // in case Cortana needs to reprompt.
            VoiceCommandUserMessage repeatMsg = new VoiceCommandUserMessage();
            repeatMsg.DisplayMessage = repeatMsg.SpokenMessage = "请重新选择";

            //如果要使用 RequestDisambiguationAsync 需要使用 CreateResponseForPrompt
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponseForPrompt(userMsg, repeatMsg, vcTiles);            
            // 显示多个选择
            var vcResult = await vcConnection.RequestDisambiguationAsync(response);
            // 取得选择结果
            BusData selectedItem = vcResult.SelectedItem.AppContext as BusData;
            VoiceCommandUserMessage resultMsg = new VoiceCommandUserMessage();
            resultMsg.DisplayMessage = resultMsg.SpokenMessage = "选择:" + selectedItem.Name;
            VoiceCommandResponse response1 = VoiceCommandResponse.CreateResponse(resultMsg);
            await vcConnection.ReportSuccessAsync(response1);
        }

        private async void ShowHaveOtherBusConfirm()
        {
            List<VoiceCommandContentTile> vcTiles = new List<VoiceCommandContentTile>();
            vcTiles.Add(new VoiceCommandContentTile
            {
                Title = "目前已经没有公车了，是否继续查询？",
                ContentTileType = VoiceCommandContentTileType.TitleWithText,
                Image = await StorageFile.GetFileFromApplicationUriAsync(
                            new Uri("ms-appx:///BusSearchVCService/Images/GreyTile.png"))
            });

            // Create a VoiceCommandUserMessage for the initial question.
            VoiceCommandUserMessage userMsg = new VoiceCommandUserMessage();
            userMsg.DisplayMessage = userMsg.SpokenMessage = "请选择";
            // Create a VoiceCommandUserMessage for the second question,
            // in case Cortana needs to reprompt.
            VoiceCommandUserMessage repeatMsg = new VoiceCommandUserMessage();
            repeatMsg.DisplayMessage = repeatMsg.SpokenMessage = "请重新选择";

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponseForPrompt(userMsg, repeatMsg, vcTiles);

            var vcConfirm = await vcConnection.RequestConfirmationAsync(response);
            if (vcConfirm != null)
            {
                if (vcConfirm.Confirmed)
                {
                    await HandleSearchWhereBusList();
                }
                else
                {
                    VoiceCommandUserMessage cancelMsg = new VoiceCommandUserMessage();
                    cancelMsg.DisplayMessage = repeatMsg.SpokenMessage = "非常抱歉";
                    VoiceCommandResponse response1 = VoiceCommandResponse.CreateResponse(cancelMsg);
                    vcConnection.ReportFailureAsync(response1);
                }
            }
        }

        private async Task ShowProgressScreen(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await vcConnection.ReportProgressAsync(response);
        }

        private void VcConnection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (serviceDeferral != null)
            {
                serviceDeferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            vcConnection = null;
            if (cancelToken!= null)
            {
                cancelToken.Cancel();
            }
            cancelToken = null;
            if (serviceDeferral!= null)
            {
                serviceDeferral.Complete();
            }
        }
    }
}