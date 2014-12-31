using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SpotifyHiddenController
{
    public class SpotifysService
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

        public SpotifysService()
        {
        }

        public void Pause()
        {
            Runner.SetText("Pausing Spotify" + Environment.NewLine);

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.PlayPause));
        }

        public void NextTrack()
        {
            Runner.SetText("Next Track Spotify" + Environment.NewLine);

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.NextTrack));
        }

        public void PreviousTrack()
        {
            Runner.SetText("Previous Track Spotify" + Environment.NewLine);

            var hwndSpotify = GetSpotifyWindowHandle();
            SendMessage(hwndSpotify, WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.PreviousTrack));
        }

        private IntPtr GetSpotifyWindowHandle()
        {
            Process[] processes = Process.GetProcessesByName("Spotify");
            var hwndSpotify = processes[0].MainWindowHandle;
            return hwndSpotify;
        }

    }
}
