var winAPIHelper = {};

if (typeof Windows !== 'undefined' &&
    typeof Windows.UI !== 'undefined' &&
    typeof Windows.UI.WebUI !== 'undefined') {

    Windows.UI.WebUI.WebUIApplication.addEventListener("activated", function (activatedEventArgs) {
        console.log(activatedEventArgs);

        if (activatedEventArgs.kind == Windows.ApplicationModel.Activation.ActivationKind.protocol ) {
            var query = activatedEventArgs.uri.queryParsed;
            console.log(query);

            for (var i = 0; i < query.length; i++) {
                console.log(query[i].name + '=' + query[i].value);
            }
        }
    });
}

var wnsChannel;

// 注冊 push notification 
if (typeof Windows !== 'undefined' &&
    typeof Windows.UI !== 'undefined' &&
    typeof Windows.UI.Notifications !== 'undefined') {

    Windows.Networking.PushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync()
        .then(function (newChannel) {
            console.log(newChannel);
            wnsChannel = newChannel;
            wnsChannel.addEventListener("pushnotificationreceived", pushNotificationReceivedHandler, false);
        }, function (error) {
            console.log(error);
        });
}

function pushNotificationReceivedHandler(e) {
    var notificationTypeName = "";
    var notificationPayload;
    var pushNotifications = Windows.Networking.PushNotifications;

    // 拆解對應的類型
    switch (e.notificationType) {
        case pushNotifications.PushNotificationType.toast:
            notificationTypeName = "Toast";
            notificationPayload = e.toastNotification.content.getXml();
            break;
        case pushNotifications.PushNotificationType.tile:
            notificationTypeName = "Tile";
            notificationPayload = e.tileNotification.content.getXml();
            break;
        case pushNotifications.PushNotificationType.badge:
            notificationTypeName = "Badge";
            notificationPayload = e.badgeNotification.content.getXml();
            break;
    }

    e.cancel = true;
    // 取得 payload 中的内容
    var xmlDox = new Windows.Data.Xml.Dom.XmlDocument();
    xmlDox.loadXml(notificationPayload);
    var textElements = xmlDox.getElementsByTagName("text")

    var messageDialog = new Windows.UI.Popups.MessageDialog(notificationTypeName, textElements[0].innerText);
    messageDialog.showAsync();
}

winAPIHelper.showToast = function (message, iconUrl) {
    // 檢查是否支援 Windows Runtime API
    if (typeof Windows !== 'undefined' &&
        typeof Windows.UI !== 'undefined' &&
        typeof Windows.UI.Notifications !== 'undefined') {

        var notifications = Windows.UI.Notifications;
        // 利用 ToastTemplateType 列舉選擇要用的範本
        var template = notifications.ToastTemplateType.toastImageAndText01;
        // 轉成 XML
        var toastXml = notifications.ToastNotificationManager.getTemplateContent(template);

        var textElements = toastXml.getElementsByTagName("text");
        textElements[0].appendChild(toastXml.createTextNode(message));
        var imageElements = toastXml.getElementsByTagName("image");
        // 設定 image 的 src 屬性
        var srcAttr = toastXml.createAttribute("src");
        srcAttr.value = iconUrl;
        var attribs = imageElements[0].attributes;
        attribs.setNamedItem(srcAttr);

        // 建立 toast 並發送
        var toast = new notifications.ToastNotification(toastXml);
        var toastNotifier = notifications.ToastNotificationManager.createToastNotifier();
        toastNotifier.show(toast);
    }
};

winAPIHelper.readFile = function (fileName) {
    if (typeof Windows !== 'undefined' &&
        typeof Windows.Storage !== 'undefined' &&
        typeof Windows.Storage.ApplicationData !== 'undefined') {

        var localFolder = Windows.Storage.ApplicationData.current.localFolder;

        localFolder.getFileAsync(fileName)
            .then(function (file) {
                // 抓到檔案並讀取
                return Windows.Storage.FileIO.readTextAsync(file);
            }, function (ex) {
                console.log(ex);
            })
            .done(function (content) {
                console.log(content);
            });
    }
};

winAPIHelper.saveFile = function (fileName, content) {
    if (typeof Windows !== 'undefined' &&
        typeof Windows.Storage !== 'undefined' &&
        typeof Windows.Storage.ApplicationData !== 'undefined') {
        
        var localFolder = Windows.Storage.ApplicationData.current.localFolder;

        localFolder.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.replaceExisting)
            .then(function (file) {
                // 抓到檔案並寫入
                return Windows.Storage.FileIO.writeTextAsync(file, content);
            })
            .done(function () {
                console.log("saved.");
            });
    }
}

winAPIHelper.addUserActivity = function (activityId) {
    if (typeof Windows !== 'undefined' &&
        typeof Windows.ApplicationModel !== 'undefined' &&
        typeof Windows.ApplicationModel.UserActivities !== 'undefined') {

        createActivity(activityId).the(function () {
            console.log("done");
        });
    }
};

async function createActivity(activityId) {
    var channel =  Windows.ApplicationModel.UserActivities.UserActivityChannel.getDefault();
    var activity = await channel.getOrCreateUserActivityAsync(activityId);

    if (activity.state == Windows.ApplicationModel.UserActivities.UserActivityState.new) {
        activity.visualElements.displayText = "new activity";
        activity.activationUri = new Windows.Foundation.Uri('testapp://mainPage?state=new&id=' + activityId);
    } else {
        activity.visualElements.displayText = "published activity";
        activity.activationUri = new Windows.Foundation.Uri('testapp://mainPage?state=published&id=' + activityId);
    }

    activity.contentInfo = Windows.ApplicationModel.UserActivities.UserActivityContentInfo.fromJson('{ "user_id": "pou", "email": "poumason@live.com"}');

    await activity.saveAsync();

    var activitySesion = activity.createSession();
}