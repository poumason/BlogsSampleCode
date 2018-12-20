<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PWASampleWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=1280" />
    <link rel="manifest" href="manifest.json" />
    <script src="js/directionalnavigation-1.0.0.0.js"></script>
    <script src="js/winapihelper.js"></script>
    <style>
        .grow {
            transition: all .2s ease-in-out;
        }

            .grow:focus {
                transform: scale(1.1);
                border-color: red;
                border-style: solid;
                border-width: thin;
            }

        .list {
            display: -webkit-flex; /* Safari */
            -webkit-flex-wrap: wrap; /* Safari 6.1+ */
            display: flex;
            flex-wrap: wrap;
            border-top: thin solid lightgray;
        }
    </style>
    <script type="text/javascript">
        navigator.gamepadInputEmulation = "gamepad";

        //Windows.UI.ViewManagement.ApplicationViewScaling.trySetDisableLayoutScaling(true);
        //Windows.UI.ViewManagement.ApplicationView.getForCurrentView().setDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.useCoreWindow);

        function resize() {
            var result1 = Windows.UI.ViewManagement.ApplicationViewScaling.trySetDisableLayoutScaling(true);
            var result2 = Windows.UI.ViewManagement.ApplicationView.getForCurrentView().setDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.useCoreWindow);

            var msg = result1 + "," + result2;

            var messageDialog = new Windows.UI.Popups.MessageDialog("Hello, world!", msg);
            messageDialog.showAsync();

            console.log("ok");
        }

        function requestSendToast() {
            winAPIHelper.showToast("title", "https://p2.bahamut.com.tw/B/2KU/46/0001465046.JPG");
        }

        if (window.Windows && Windows.UI.Popups) {
            document.addEventListener('contextmenu', function (e) {

                // Build the context menu
                var menu = new Windows.UI.Popups.PopupMenu();
                menu.commands.append(new Windows.UI.Popups.UICommand("Option 1", null, 1));
                menu.commands.append(new Windows.UI.Popups.UICommandSeparator);
                menu.commands.append(new Windows.UI.Popups.UICommand("Option 2", null, 2));

                // Convert from webpage to WinRT coordinates
                function pageToWinRT(pageX, pageY) {
                    var zoomFactor = document.documentElement.msContentZoomFactor;
                    return {
                        x: (pageX - window.pageXOffset) * zoomFactor,
                        y: (pageY - window.pageYOffset) * zoomFactor
                    };
                }

                // When the menu is invoked, execute the requested command
                menu.showAsync(pageToWinRT(e.pageX, e.pageY)).done(function (invokedCommand) {
                    if (invokedCommand !== null) {
                        switch (invokedCommand.id) {
                            case 1:
                                console.log('Option 1 selected');
                                // Invoke code for option 1
                                break;
                            case 2:
                                console.log('Option 2 selected');
                                // Invoke code for option 2
                                break;
                            default:
                                break;
                        }
                    } else {
                        // The command is null if no command was invoked.
                        console.log("Context menu dismissed");
                    }
                });
            }, false);
        }
    </script>
</head>
<body style="background-color: #888888;" onload="resize();">
    <div>
        <input type="button" value="show toast" onclick="requestSendToast();" />

        <input type="button" value="reload" onclick="window.location.reload();" />
        
        <input type="button" value="read file" onclick="winAPIHelper.readFile('test.txt');" />
        
        <input type="button" value="write file" onclick="winAPIHelper.saveFile('test.txt', 'from PWA sample');" />
        
        <input type="button" value="add user activity" onclick="winAPIHelper.addUserActivity('test_user_activity_id');" />
    </div>
    <form id="form1" runat="server">
        <div id="chartList" runat="server" class="list">
        </div>
    </form>
</body>
</html>