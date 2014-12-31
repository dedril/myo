using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Poses;

namespace SpotifyHiddenController
{
    /// <summary>
    /// You need to start this service in a separate thread, because it 
    /// </summary>
    public class MyoService:IDisposable
    {

        private SpotifysService spotify;
        private KeyboardService keyboard;

        private IHub hub;
        private IChannel channel;

        public MyoService(SpotifysService spotify, KeyboardService keyboard)
        {
            this.spotify = spotify;
            this.keyboard = keyboard;

            // create a hub to manage Myos
            channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));
            hub = Hub.Create(channel);

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
        }



        private void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            if (keyboard != null && 
                keyboard.CtrlKeyHeld)
            {
                Runner.SetText(string.Format("{0} arm Myo detected {1} pose!", e.Myo.Arm, e.Myo.Pose) + Environment.NewLine);

                switch (e.Myo.Pose)
                {
                    case Pose.FingersSpread:
                        spotify.Pause();
                        break;

                    case Pose.WaveOut:
                        if (e.Myo.Arm == Arm.Right)
                        {
                            spotify.NextTrack();
                        }
                        else
                        {
                            spotify.PreviousTrack();
                        }
                        break;

                    case Pose.WaveIn:
                        if (e.Myo.Arm == Arm.Right)
                        {
                            spotify.PreviousTrack();
                        }
                        else
                        {
                            spotify.NextTrack();
                        }
                        break;
                }
            }
        }

        private void Myo_Unlocked(object sender, MyoEventArgs e)
        {
            Runner.SetText(string.Format("{0} arm Myo has unlocked!", e.Myo.Arm) + Environment.NewLine);
        }

        private void Myo_Locked(object sender, MyoEventArgs e)
        {
            Runner.SetText(string.Format("{0} arm Myo has locked!", e.Myo.Arm) + Environment.NewLine);
        }

        public void Dispose()
        {
            if (hub != null)
            {
                hub.Dispose();
            }
            if (channel != null)
            {
                channel.Dispose();
            }
        }
    }
}
