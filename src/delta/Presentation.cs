using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeltaPresentation;
using Microsoft.Win32;

namespace Delta_Presentation
{
    public partial class Presentation : Form
    {
        private List<string> sources;
        private string filename;
        private int index = 0;
        private Size normalSize;
        private int timer = 0;

        public Presentation()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                try
                {
                    this.filename = args[1];
                    this.sources = DeltaFileManager.Load(this.filename).Values.ToList<string>();
                    Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Nenhuma apresentação foi selecionada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            string[] split = args[0].Split('\\');
            string exename = split[split.Length - 1].Replace("\"", "");
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

        private void UpdateSlide()
        {
            string foldername = filename.Substring(0, filename.Length - filename.Split('\\')[filename.Split('\\').Length - 1].Length);
            foldername = foldername.Replace('\\', '/');
            this.webBrowser1.DocumentText = "<!DOCTYPE html><html><head><base href=\"file://" + foldername + "\" target=\"_blank\"></head></html>" + this.sources[this.index];
        }

        private void Start()
        {
            this.index = 0;
            UpdateSlide();
        }

        private void NextSlide()
        {
            this.index++;
            if (this.index >= sources.Count) this.index = sources.Count - 1;
            UpdateSlide();
        }

        private void PrevSlide()
        {
            this.index--;
            if (this.index < 0) this.index = 0;
            UpdateSlide();
        }

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (this.timer-- != 0) return; 
            if (e.KeyCode == Keys.Right)
                NextSlide();
            else if (e.KeyCode == Keys.Left)
                PrevSlide();
            this.timer = 1;
        }
    }
}
