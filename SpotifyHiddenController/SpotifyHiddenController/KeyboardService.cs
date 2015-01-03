using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyHiddenController
{
    /// <summary>
    /// Keeps track of the state of the keyboard and sets up the keyboard hooks
    /// </summary>
    public class KeyboardService
    {
        private KeyboardHook keyboardHook;

        public bool CtrlKeyHeld { get; private set; }
        //helps us measure if we are still holding the key
        private DateTime CtrlKeyLastPress = DateTime.MinValue;

        public event CtrlHandler CtrlPressed;
        public event CtrlHandler CtrlReleased;
        public delegate void CtrlHandler(object keyboardService, EventArgs e);

        /// <summary>
        /// The Ctrl key.
        /// </summary>
        public const int VK_LCONTROL = 0xA2;

        public KeyboardService()
        {
            keyboardHook = new KeyboardHook();

            keyboardHook.HotKeyPressed += new EventHandler(KeyPressed);
            keyboardHook.SetHook((int)VK_LCONTROL); 
        }

        public void UnHook()
        {
            keyboardHook.UnHook();
        }

        private void KeyPressed(object sender, EventArgs e)
        {
            if (!CtrlKeyHeld)
            {
                Runner.SetText("hot key pressed" + Environment.NewLine);
                CtrlKeyHeld = true;

                if (CtrlPressed != null)
                {
                    CtrlPressed(this,null);
                }
            }

            //we need to update this each milisecond the ctrl key is held down, otherwise it will timeout quickly even if the user is holding it down
            CtrlKeyLastPress = DateTime.Now;

            //in a separate non-blocking thread, wait and see if the user lets go of the CTRL key
            Task.Factory.StartNew(() => CheckIfUserStillHoldingHotKey());
        }

        private void CheckIfUserStillHoldingHotKey()
        {
            Thread.Sleep(1000);
            if (CtrlKeyHeld &&
                TimeSpan.FromTicks(DateTime.Now.Ticks - CtrlKeyLastPress.Ticks).TotalMilliseconds > 1000)
            {
                //then we are no longer holding the key
                CtrlKeyHeld = false;
                Runner.SetText("No longer holding down hot key" + Environment.NewLine);

                if (CtrlReleased != null)
                {
                    CtrlReleased(this, null);
                }
            }
        }
    }
}
