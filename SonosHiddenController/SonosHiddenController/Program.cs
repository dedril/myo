using System;
using System.CodeDom;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Poses;

namespace Myo
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
    public class SonosHiddenController
    {
        //helps us get the spotify window handle.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        //allows us to send keystrokes to the spotify window.
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        const uint WM_APPCOMMAND = 0x0319;

        public enum SpotifyAction : long
        {
            PlayPause = 917504,
            Mute = 524288,
            VolumeDown = 589824,
            VolumeUp = 655360,
            Stop = 851968,
            PreviousTrack = 786432,
            NextTrack = 720896
        }
        
        #region Methods
        public static void Main()
        {
            // create a hub to manage Myos
            using (var channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create())))
            using (var hub = Hub.Create(channel))
            {
                // listen for when a Myo connects
                hub.MyoConnected += (sender, e) =>
                {
                    Console.WriteLine("Myo {0} has connected!", e.Myo.Handle);

                    e.Myo.PoseChanged += Myo_PoseChanged;
                    e.Myo.Locked += Myo_Locked;
                    e.Myo.Unlocked += Myo_Unlocked;

                };

                // start listening for Myo data
                channel.StartListening();

                while (true)
                {
                    //forever loop
                }
            }
        }
        #endregion

        #region Event Handlers
        private static void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Console.WriteLine("{0} arm Myo detected {1} pose!", e.Myo.Arm, e.Myo.Pose);

            switch (e.Myo.Pose)
            {
                case Pose.FingersSpread:
                    PauseSpotify();
                    break;

                case Pose.WaveOut:
                    if (e.Myo.Arm == Arm.Right)
                    {
                        NextTrackSpotify();
                    }
                    else
                    {
                        PreviousTrackSpotify(); 
                    }
                    break;

                case Pose.WaveIn:
                    if (e.Myo.Arm == Arm.Right)
                    {
                        PreviousTrackSpotify();
                    }
                    else
                    {
                        NextTrackSpotify();
                    }
                    break;
            }
        }

        private static void Myo_Unlocked(object sender, MyoEventArgs e)
        {
            Console.WriteLine("{0} arm Myo has unlocked!", e.Myo.Arm);
        }

        private static void Myo_Locked(object sender, MyoEventArgs e)
        {
            Console.WriteLine("{0} arm Myo has locked!", e.Myo.Arm);
        }
        #endregion

        #region Spotify Actions

        private static void PauseSpotify()
        {
            Console.WriteLine("Pausing Spotify");

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.PlayPause));
        }

        private static void NextTrackSpotify()
        {
            Console.WriteLine("Next Track Spotify");

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.NextTrack));
        }

        private static void PreviousTrackSpotify()
        {
            Console.WriteLine("Previous Track Spotify");

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.PreviousTrack));
        }

        private static IntPtr GetSpotifyWindowHandle()
        {
            Process[] processes = Process.GetProcessesByName("Spotify");
            var hwndSpotify = processes[0].MainWindowHandle;
            return hwndSpotify;
        }


        #endregion
    }
}