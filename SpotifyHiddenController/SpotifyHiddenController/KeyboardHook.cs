using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleHotKey;

namespace SpotifyHiddenController
{
    /// <summary>
    /// Manages setting up the hook and the events for the keyboard actions.
    /// Thanks http://pastebin.com/uCSvqwb4
    /// </summary>
    public class KeyboardHook
    {
        public event EventHandler HotKeyPressed;

        private static int _hookHandle = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int nVirtKey);

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public const int WH_KEYBOARD_LL = 13;

        public const int VK_LCONTROL = 0xA2;
        public const int VK_RCONTROL = 0xA3;

        /// <summary>
        /// Necessary or the life of the delegate will be short and will be garbage collected.
        /// Thanks http://stackoverflow.com/questions/6193711/call-has-been-made-on-garbage-collected-delegate-in-c
        /// </summary>
        private HookProc hookProcDelegate;

        public KeyboardHook()
        {
            hookProcDelegate = KbHookProc;
        }

        private int keyCode;
        public void SetHook(int keyCode)
        {
            this.keyCode = keyCode;

            // Set system-wide hook.
            _hookHandle = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    hookProcDelegate,
                    (IntPtr)0,
                    0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_hookHandle);
        }

        private int KbHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var hookStruct = (KbLLHookStruct)Marshal.PtrToStructure(lParam, typeof(KbLLHookStruct));

                // Quick and dirty check. You may need to check if this is correct. See GetKeyState for more info.
                bool ctrlDown = GetKeyState(keyCode) != 0;

                if (ctrlDown) // Ctrl
                {
                    if (HotKeyPressed != null)
                    {
                        HotKeyPressed(this,null);
                    }
                }
            }

            // Pass to other keyboard handlers. Makes the Ctrl pass through.
            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        //Declare the wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class KbLLHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }


       

    }
}
