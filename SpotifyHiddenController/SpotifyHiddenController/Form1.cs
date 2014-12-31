using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleHotKey;
using MyoSharp.Communication;

namespace SpotifyHiddenController
{
    public partial class Form1 : Form
    {
       
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

        public Form1()
        {
            InitializeComponent();
            SetHook();
        }

        private void SetHook()
        {
            // Set system-wide hook.
            _hookHandle = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KbHookProc,
                    (IntPtr)0,
                    0);
        }

        private int KbHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var hookStruct = (KbLLHookStruct)Marshal.PtrToStructure(lParam, typeof(KbLLHookStruct));

                // Quick and dirty check. You may need to check if this is correct. See GetKeyState for more info.
                bool ctrlDown = GetKeyState(VK_LCONTROL) != 0 || GetKeyState(VK_RCONTROL) != 0;

                if (ctrlDown && hookStruct.vkCode == 0x56) // Ctrl+V
                {

                    this.textBox1.Text += "trev\n";
                    //Clipboard.SetText("Hi"); // Replace this with yours something here...
                }
            }

            // Pass to other keyboard handlers. Makes the Ctrl+V pass through.
            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_hookHandle);
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

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {

        }

    }
}
