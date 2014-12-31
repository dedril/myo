using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SpotifyHiddenController
{
    public partial class Debugger : Form
    {
        private KeyboardService keyboard;
        private SpotifysService spotify;
        private MyoService myo;

        public Debugger()
        {
            InitializeComponent();

            keyboard = new KeyboardService();
            spotify = new SpotifysService();
            myo = new MyoService(spotify, keyboard);

        }

        /// <summary>
        /// Allows us to write to the log textbox from any thread.
        /// </summary>
        public TextBox TxtLog
        {
            get { return this.txtLog; }
        }


        private void Debugger_Load(object sender, EventArgs e)
        {

        }

        private void Debugger_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (keyboard != null)
            {
                keyboard.UnHook();
            }
            if (myo != null)
            {
                myo.Dispose();
            }
        }


       
    }
}
