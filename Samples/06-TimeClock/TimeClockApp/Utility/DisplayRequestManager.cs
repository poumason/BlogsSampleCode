using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;

namespace TimeClockApp
{
    public class DisplayRequestManager
    {
        #region singleton
        private static object locker = new object();

        private static DisplayRequestManager instance;
        public static DisplayRequestManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new DisplayRequestManager();
                        }
                    }
                }

                return instance;
            }
        }
        #endregion

        private DisplayRequest DisplayRequest { get; set; }

        private bool keepOpenScreen = false;
        public bool KeepOpenedScreen
        {
            get
            {
                return keepOpenScreen;
            }
            set
            {
                if (keepOpenScreen != value)
                {
                    keepOpenScreen = value;
                    UpdateDisplayRequestState();
                }
            }
        }      

        private DisplayRequestManager()
        {
            DisplayRequest = new DisplayRequest();
        }

        private void UpdateDisplayRequestState()
        {
            if (KeepOpenedScreen)
            {
                RequestDisplayActive();
            }
            else
            {
                RequestDisplayRelease();
            }
        }

        /// <summary>
        /// 發出不關閉螢幕的請求
        /// </summary>
        private void RequestDisplayActive()
        {
            try
            {
                if (DisplayRequest == null)
                {
                    DisplayRequest = new DisplayRequest();
                }

                DisplayRequest.RequestActive();
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 釋放不關閉螢幕的請求
        /// </summary>
        private void RequestDisplayRelease()
        {
            try
            {
                if (DisplayRequest == null)
                {
                    DisplayRequest = new DisplayRequest();
                }

                DisplayRequest.RequestRelease();
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}