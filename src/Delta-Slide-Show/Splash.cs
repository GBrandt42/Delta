// Namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Delta_Slide_Show
//-----------------------------------------------------------------------------
// Namespace padrão do editor
namespace Delta_Slide_Show
{
    // Classe da janela de Splash
    public partial class Splash : Form
    {
        // Variável privada de páscoa
        private bool easter = false;
        
        // Construtor
        public Splash()
        {
            InitializeComponent();
        }

        // Ao carregar o splash, inicia o timer
        private void Form1_Load(object sender, EventArgs e)
        {
            loadTimer.Start();
        }

        // Mostra a essência da páscoa quando clicado
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

        // Fecha a janela quando o timer atinge 2 segundos
        private void loadTimer_Tick(object sender, EventArgs e)
        {
            loadTimer.Stop();
            new MainWindow().Show();
            this.Hide();
        }
    }
}
