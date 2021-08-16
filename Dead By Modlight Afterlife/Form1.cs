using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dead_By_Modlight_Afterlife
{
    public partial class Form1 : Form
    {
        ///////////////////////////////// => High priority variables
        protected static string ProgramExecutable = System.AppDomain.CurrentDomain.FriendlyName;
        protected const string REGISTRY_MAIN = @"HKEY_CURRENT_USER\SOFTWARE\Dead By Modlight Afterlife";
        protected const string OFFLINEVERSION = "1001";

        ///////////////////////////////// => High priority actions
        protected override void WndProc(ref Message win)
        {
            if (win.Msg == 0x11)
            {
                MessageBox.Show("Dead By Modlight Requires to be closed before PC power off.", ProgramExecutable, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            base.WndProc(ref win);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            REGISTRYCHANGER_VERIFYVALUES();
            NETSC_AVAILABILITYCHECK();
            NETSC_VERSIONCHECK();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FiddlerCore.Stop();
        }

        ///////////////////////////////// => Panel drag-move & buttons
        private void button1_Click(object sender, EventArgs e) => this.Close();
        private void button2_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private async void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Capture = false; await Task.Run(() =>
            {
                Message mouse = Message.Create(Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
                WndProc(ref mouse);
            });
        }

        ///////////////////////////////// => Registry
        private void REGISTRYCHANGER_VERIFYVALUES()
        {
            string REGISTRYVALUE_GAMEEXEPATH = Registry.GetValue(REGISTRY_MAIN, "GameExecutablePath", "NONE").ToString();



            if (REGISTRYVALUE_GAMEEXEPATH != "NONE")
            {
                textBox1.Text = REGISTRYVALUE_GAMEEXEPATH;
                textBox2.Visible = true;
            }
        }

        ///////////////////////////////// => Net Services
        private void NETSC_AVAILABILITYCHECK()
        {
            string availabilityResponse = NetServices.REQUEST_GET("http://api.cranchpalace.info/v1/deadbymodlight/afterlife/availabilityCheck", "");
            if (availabilityResponse == "ERROR")
                return;

            var availabilityJsonResponse = Newtonsoft.Json.Linq.JObject.Parse(availabilityResponse);
            if ((bool)availabilityJsonResponse["isAlive"] == false)
            {
                MessageBox.Show("Modding wasn't available at current moment, possible reasons is technical work or it was patched.", ProgramExecutable, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                this.Close();
            } else {
                Globals.moddingFiles = (string)availabilityJsonResponse["downloadLink"];
                button6.Visible = true;
            }
        }
        private void NETSC_VERSIONCHECK()
        {
            string versionResponse = NetServices.REQUEST_GET("http://api.cranchpalace.info/v1/deadbymodlight/afterlife/versionCheck", $"version={OFFLINEVERSION}");
            if (versionResponse == "ERROR")
                return;

            var versionJsonResponse = Newtonsoft.Json.Linq.JObject.Parse(versionResponse);
            if((bool)versionJsonResponse["isValid"] == false)
            {
                DialogResult isUserAgreeWithUpdate = MessageBox.Show("New Version is out, would you like to update?", ProgramExecutable, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (isUserAgreeWithUpdate == DialogResult.OK) {
                    System.Diagnostics.Process.Start((string)versionJsonResponse["downloadLink"]);
                    this.Close();
                }
            }
        }

        ///////////////////////////////// => Main()
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog gamepathspecify = new OpenFileDialog())
            {
                gamepathspecify.RestoreDirectory = true;
                gamepathspecify.InitialDirectory = Environment.CurrentDirectory;
                gamepathspecify.Filter = "DeadByDaylight-Win64-Shipping (*.exe*)|*DeadByDaylight-Win64-Shipping.exe*;";
                gamepathspecify.FilterIndex = 1;
                if (gamepathspecify.ShowDialog() == DialogResult.OK)
                {
                    Registry.SetValue(REGISTRY_MAIN, "GameExecutablePath", gamepathspecify.FileName);
                    REGISTRYCHANGER_VERIFYVALUES();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "" || textBox2.Text.Length < 16)
            {
                MessageBox.Show("Auth Response field can't be empty.", ProgramExecutable, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            } else if (textBox2.Text.Length < 512) {
                MessageBox.Show("Auth Response can't be that short. Please, make sure you're copied all provided by Fiddler text.", ProgramExecutable, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            using (StreamWriter SW = new StreamWriter(Globals.responseFile, false, System.Text.Encoding.ASCII))
            {
                SW.Write(textBox2.Text);
                SW.Flush();
            }

            FiddlerCore.Start();
            textBox2.ReadOnly = true;
            button4.Enabled = false;
            button4.BackColor = System.Drawing.Color.MediumSpringGreen; 
            button4.Text = "WORKING...";
            button5.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start(textBox1.Text, "-eac-nop-loaded");
        private void button6_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start(Globals.moddingFiles);
    }
}
