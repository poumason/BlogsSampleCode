using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace JavaScriptExternal
{
    [AllowForWeb]
    public sealed class JavaScriptExternalObject
    {
        /// <summary>
        /// 提供給外部使用時收聽的事件
        /// </summary>
        public event EventHandler<string> FromJavaScriptMessage;

        /// <summary>
        /// 提供給 JavaScript 呼叫的方法
        /// </summary>
        public void onOpenNativeShareDialog(string json)
        {
            FromJavaScriptMessage?.Invoke(null, json);
        }

        public void notify(string message)
        {
            FromJavaScriptMessage?.Invoke(null, message);
        }
    }
}