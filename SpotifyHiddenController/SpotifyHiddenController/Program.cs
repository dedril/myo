using System;
using System.CodeDom;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleHotKey;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Poses;
using SpotifyHiddenController;

namespace SpotifyHiddenController
{
    /// <summary>
    /// Simple app that takes commands from the myo armband and sends media keystrokes to your spotify app such that the 
    /// spotify app doesn't need to be the active window, allowing you to change your music with the myo app while working.
    /// 
    /// Big thanks to
    /// - https://github.com/tayfuzun/MyoSharp
    /// for writing the C# hookup to the myo and providing clear examples.
    /// 
    /// - http://stackoverflow.com/questions/8459162/user32-api-custom-postmessage-for-automatisation
    /// for instructions about using SendMessage.
    /// 
    /// </summary>
    public class Runner
    {
        private static Debugger debugger;
        delegate void SetTextCallback(string s);

        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            debugger = new SpotifyHiddenController.Debugger();
            Application.Run(debugger);
        }


        public static void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (debugger.TxtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                debugger.TxtLog.Invoke(d, new object[] { text });
            }
            else
            {
                debugger.TxtLog.AppendText(text);
            }
        }
      

     

        //need both VK control (for the key) and MOD control (the modifier)
        //for the ctrl hot key to work.
        private static uint VK_CONTROL = 0x11;
        private static uint MOD_CONTROL = 0x0002;


    }
}