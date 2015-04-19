// Using all these things (or not)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Application main namespace
namespace Delta_Slide_Show
{
    // Splash
    //-------------------------------------------------------------------------
    // This class is for the starting image. Just 4 fun.
    public partial class Splash : Form
    {
        private bool easter = false;
        // Constructor
        public Splash()
        {
            InitializeComponent();
        }

        // When it loads start the timer
        private void Form1_Load(object sender, EventArgs e)
        {
            loadTimer.Start();
        }

        // When titleLabel is clicked, let the easter out
        private void label1_Click(object sender, EventArgs e)
        {
            if (easter)
            {
                this.BackgroundImage = global::Delta_Slide_Show.Properties.Resources.delta2;
                this.easter = false;
            }
            else
            {
                this.BackgroundImage = global::Delta_Slide_Show.Properties.Resources.delta42;
                this.easter = true;
            }
        }

        // When the timer reaches 2 seconds, it stops and close the window
        private void loadTimer_Tick(object sender, EventArgs e)
        {
            loadTimer.Stop();
            new MainWindow().Show();
            this.Hide();
        }
    }
}
