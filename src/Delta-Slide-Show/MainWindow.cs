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
using System.IO;
using Microsoft.Win32;

// Delta_Slide_Show
//-----------------------------------------------------------------------------------------
// Namespace padrão do editor
namespace Delta_Slide_Show
{
    // Classe da janela principal
    public partial class MainWindow : Form
    {
        // Variáveis privadas
        private Dictionary<int, string> sources = new Dictionary<int, string>();
        private string lastSaved = "";

        // Construtor
        public MainWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = true;
            this.slideListView.SelectedIndex = 0;
        }

        // Quando fechar a janela
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // Quando o texto da janela  HTML mudar
        private void textBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            string foldername = lastSaved.Substring(0, lastSaved.Length - lastSaved.Split('\\')[lastSaved.Split('\\').Length - 1].Length);
            foldername = foldername.Replace('\\', '/');
            this.webBrowser1.DocumentText = "<!DOCTYPE html><html><head><base href=\"file://" + foldername + "\" target=\"_blank\"></head></html>" + this.textBox1.Text;
            if (this.slideListView.SelectedIndex < 0) this.slideListView.SelectedIndex = 0;
            sources[this.slideListView.SelectedIndex] = this.textBox1.Text;
        }

        // Quando o botão de criar novo slide for pressionado
        private void novoSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addSlide();
        }

        // Criação de um slide
        private void addSlide(string source = "<html>\n\t<head>\n\t\t<meta charset='utf-8'>\n\t</head>\n\t<body></body>\n</html>")
        {
            sources[slideListView.Items.Count] = source;
            slideListView.Items.Add("Slide " + (slideListView.Items.Count + 1));
        }

        // Quando o botão de salvar for clicado
        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastSaved == null)
            {
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != string.Empty)
                    DeltaPresentation.DeltaFileManager.Save(slideDictionary(), saveFileDialog.FileName);
                lastSaved = saveFileDialog.FileName;
            }
            else
            {
                DeltaPresentation.DeltaFileManager.Save(slideDictionary(), lastSaved);
            }
        }
    
        // Quando o botão de abrir for clicado
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != string.Empty)
            {
                this.lastSaved = this.openFileDialog.FileName;
                slideListView.Items.Clear();
                int i = 0;
                foreach (KeyValuePair<string, string> pair in DeltaPresentation.DeltaFileManager.Load(openFileDialog.FileName))
                {
                    addSlide(pair.Value);
                    this.slideListView.Items[i++] = pair.Key;
                }
                this.slideListView.SelectedIndex = 0;
            }
        }

        // Quando o item selecionado na caixa de list mudar
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.slideListView.SelectedIndex < 0) return;
            try
            {
                this.textBox1.Text = sources[this.slideListView.SelectedIndex];
            }
            catch (Exception)
            {

            }
            this.textBox2.Text = this.slideListView.Items[this.slideListView.SelectedIndex].ToString();
        }

        // Aquisição de um Dictionary com os slides e seus nomes
        private Dictionary<string, string> slideDictionary()
        {
            Dictionary<string, string> slides = new Dictionary<string, string>();
            foreach (KeyValuePair<int, string> pair in sources)
            {
                slides.Add(this.slideListView.Items[pair.Key].ToString(), pair.Value);
            }
            return slides;
        }

        // Quando o botão Salvar Como for apertado
        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != string.Empty)
                DeltaPresentation.DeltaFileManager.Save(slideDictionary(), saveFileDialog.FileName);
            lastSaved = saveFileDialog.FileName;
        }

        // Quando o bobtão de apagar slide for apertado
        private void apagarSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slideListView.Items.Count > 1)
                slideListView.Items.RemoveAt(slideListView.SelectedIndex);
        }

        // Quando o texto da janela de nome do slide mudar
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.slideListView.Items[this.slideListView.SelectedIndex] = this.textBox2.Text;
        }

        // Quando a janela carregar
        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Size = Screen.PrimaryScreen.Bounds.Size;
            string[] split = Environment.GetCommandLineArgs()[0].Split('\\');
            string exename = split[split.Length - 1].Replace("\"", "");
            
            // Alteração do registro para mostrar o HTML corretamente
            if (Environment.Is64BitOperatingSystem)
                if ((int)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", exename, RegistryValueKind.DWord) == 9999) return;
            else if ((int)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", exename, RegistryValueKind.DWord) == 9999) return;
            try
            {
                if (Environment.Is64BitOperatingSystem)
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", exename, 9999, RegistryValueKind.DWord);
                else
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", exename, 9999, RegistryValueKind.DWord);
            }
            catch (System.UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Falha ao acessar o registro, alguns componentes podem não aparecer corretamente. Para corrigir abra o programa como administrador.", "Falha ao acessar o registro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Quando o botão de nova apresentação por pressionado
        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this, "Quer salvar a apresentação atual?", "Salvar", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                salvarComoToolStripMenuItem_Click(sender, e);
                lastSaved = "";
                this.slideListView.Items.Clear();
                this.sources.Clear();
                addSlide();
            }
            else if (result == DialogResult.No)
            {
                lastSaved = "";
                this.slideListView.Items.Clear();
                addSlide();
            }
        }

        // Quando um botão for pressionado dentro da caixa de lista
        private void slideListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                apagarSlideToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Insert)
            {
                addSlide();
            }
        }

        // Quando o botão de visualizar apresentação for apertado
        private void doComeçoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastSaved == "") salvarComoToolStripMenuItem_Click(sender, e);
            if (lastSaved == "") return;
            System.Diagnostics.Process.Start("delta.exe", lastSaved);
        }

    }
}
