﻿// WriteForm.cs
//
// Greaseweazle GUI Wrapper
//
// Copyright (c) 2019 Don Mankin <don.mankin@yahoo.com>
//
// MIT License
//
// See the file LICENSE for more details, or visit <https://opensource.org/licenses/MIT>.using System;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Greaseweazle
{
    public partial class WriteForm : Form
    {
        #region declarations
        private const int WM_CLOSE = 0x0010;
        private string m_sWriteDiskFolder = "";
        private string m_sWTDFilename = "mydisk.scp";
        private string m_sUSBPort = "UNKNOWN";
        private bool m_bWindowsEXE = false;
        private bool m_bUSBSupport = false;
        private bool m_bLegacyUSB = true;
        private Form m_frmChooser = null;
        #endregion

        #region WriteForm
        public WriteForm(ChooserForm newForm)
        {
            m_frmChooser = newForm;
            InitializeComponent();
            InitializeMyStuff();
        }
        #endregion

        #region InitializeMyStuff
        private void InitializeMyStuff()
        {
            // disable maximize box
            this.MaximizeBox = false;

            // set working directory to executable directory
            string sExeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(sExeDir);

            if (m_sWriteDiskFolder.Length == 0)
                m_sWriteDiskFolder = sExeDir;
        }
        #endregion

        #region btnBack_Click
        private void btnBack_Click(object sender, EventArgs e)
        {
            iniWriteFile();
            m_frmChooser.Show();
            this.Close();
        }
        #endregion

        #region iniWriteFile
        public void iniWriteFile()
        {
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "m_sWTDFilename", m_sWTDFilename);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkDoubleStep", (chkDoubleStep.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkEraseEmpty", (chkEraseEmpty.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkWTDAdjustSpeed", (chkWTDAdjustSpeed.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "rbWriteDoubleSided", (rbWriteDoubleSided.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "rbWriteSingleSided", (rbWriteSingleSided.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "txtWriteLastCyl", txtWriteLastCyl.Text);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkWriteLastCyl", (chkWriteLastCyl.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "txtWriteFirstCyl", txtWriteFirstCyl.Text);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkWriteFirstCyl", (chkWriteFirstCyl.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "txtDriveSelectWTD", txtDriveSelectWTD.Text);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "chkDriveSelectWTD", (chkDriveSelectWTD.Checked == true).ToString());
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "m_sWriteDiskFolder", m_sWriteDiskFolder);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "txtRTDCommandLine", txtWTDCommandLine.Text);
        }
        #endregion

        #region iniReadFile
        public void iniReadFile()
        {
            string sRet;

            // found out the controller type
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbType", "rbF7", "garbage").Trim())) != "garbage")
            {
                if (sRet == "False")
                {
                    chkDriveSelectWTD.BackColor = Color.FromArgb(255, 182, 193);
                    txtDriveSelectWTD.BackColor = Color.FromArgb(255, 182, 193);
                }
            }

            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "m_sWTDFilename", "garbage").Trim())) != "garbage")
                m_sWTDFilename = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "m_sWriteDiskFolder", "garbage").Trim())) != "garbage")
                m_sWriteDiskFolder = sRet;

            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkDoubleStep", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkDoubleStep.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkEraseEmpty", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkEraseEmpty.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkWTDAdjustSpeed", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkWTDAdjustSpeed.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "rbWriteDoubleSided", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    rbWriteDoubleSided.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "rbWriteSingleSided", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    rbWriteSingleSided.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "txtWriteLastCyl", "garbage").Trim())) != "garbage")
                txtWriteLastCyl.Text = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkWriteLastCyl", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkWriteLastCyl.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "txtWriteFirstCyl", "garbage").Trim())) != "garbage")
                txtWriteFirstCyl.Text = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkWriteFirstCyl", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkWriteFirstCyl.Checked = true;
            }
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "txtDriveSelectWTD", "garbage").Trim())) != "garbage")
                txtDriveSelectWTD.Text = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWriteToDisk", "chkDriveSelectWTD", "garbage").Trim())) != "garbage")
            {
                if (sRet == "True")
                    chkDriveSelectWTD.Checked = true;
            }

            // usb port
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbUSBPorts", "m_sUSBPort", "garbage").Trim())) != "garbage")
                m_sUSBPort = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbUSBPorts", "mnuUSBSupport", "garbage").Trim())) != "garbage")
                m_bUSBSupport = (sRet == "True");
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbUSBPorts", "chkLegacyUSB", "garbage").Trim())) != "garbage")
                m_bLegacyUSB = (sRet == "True");

            // windows executable
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbWindowsEXE", "mnuWindowsEXE", "garbage").Trim())) != "garbage")
                m_bWindowsEXE = (sRet == "True");
        }
        #endregion

        #region CreateCommandLine
        private void CreateCommandLine()
        {
            if (true == m_bWindowsEXE)
                txtWTDCommandLine.Text = "gw.exe write";
            else
                txtWTDCommandLine.Text = "python.exe " + ChooserForm.m_sGWscript + " write";
            if (chkWriteFirstCyl.Checked == true)
                txtWTDCommandLine.Text += " --scyl=" + txtWriteFirstCyl.Text;
            if (chkWriteLastCyl.Checked == true)
                txtWTDCommandLine.Text += " --ecyl=" + txtWriteLastCyl.Text;
            if (rbWriteSingleSided.Checked == true)
                txtWTDCommandLine.Text += " --single-sided";
            if (chkWTDAdjustSpeed.Checked == true)
                txtWTDCommandLine.Text += " --adjust-speed";
            if (chkDoubleStep.Checked == true)
                txtWTDCommandLine.Text += " --double-step";
            if (chkWTDAdjustSpeed.Checked == true)
                txtWTDCommandLine.Text += " --erase-empty";
            if ((chkDriveSelectWTD.Enabled == true) && (chkDriveSelectWTD.Checked == true))
                txtWTDCommandLine.Text += " --drive=" + txtDriveSelectWTD.Text;
            if ((m_bLegacyUSB == false) && (m_bUSBSupport == true) && (m_sUSBPort != "UNKNOWN"))
                txtWTDCommandLine.Text += " --device=" + m_sUSBPort;
            txtWTDCommandLine.Text += " " + "\"" + m_sWriteDiskFolder + "\\" + m_sWTDFilename + "\"";
            if ((m_bLegacyUSB == true) && (m_bUSBSupport == true) && (m_sUSBPort != "UNKNOWN"))
                txtWTDCommandLine.Text += " " + m_sUSBPort;
        }
        #endregion

        #region LaunchPython
        private void LaunchPython()
        {
            // only allow one instance at a time
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                if (theprocess.Id > 0)
                {
                    if (ChooserForm.m_ProcessId == theprocess.Id)
                    {
                        System.Windows.Forms.MessageBox.Show("You must first close the previous Greaseweazle command console", "Oops!");
                        return;
                    }
                }
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "C:\\WINDOWS\\SYSTEM32\\cmd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            txtWTDCommandLine.Text = txtWTDCommandLine.Text.Trim(); // remove empty usb port if it exists
            startInfo.Arguments = "/K " + "\"" + txtWTDCommandLine.Text + "\"";
            try
            {
                Process exeProcess = Process.Start(startInfo);
                ChooserForm.m_ProcessId = exeProcess.Id;
            }
            catch (Exception e)
            {
                string sMessage = e.Message.ToString();
                MessageBox.Show(this, "An error has occured\n" + sMessage, "Oops!");
            }
        }
        #endregion

        #region WriteForm_Load
        private void WriteForm_Load(object sender, EventArgs e)
        {
            // read inifile
            iniReadFile();

            // initialize status label
            this.toolStripStatusLabel.Text = ChooserForm.m_sStatusLine.Trim();
            this.toolStripStatusLabel.BackColor = ChooserForm.m_StatusColor;
            this.statusStrip.BackColor = ChooserForm.m_StatusColor;

            // version options check
            if ((ChooserForm.m_GWToolsVersion < (decimal)0.05) || (ChooserForm.m_GWToolsVersion > (decimal)0.12))
            {
                if (ChooserForm.m_StatusColor == Color.FromArgb(173, 255, 47))  // current version
                    this.chkWTDAdjustSpeed.Text = "Adjust Speed - OBSOLETE";
                else                                                            // different version
                    this.chkWTDAdjustSpeed.BackColor = Color.FromArgb(255, 182, 193);
                this.chkWTDAdjustSpeed.Checked = false;
            }

            if (ChooserForm.m_GWToolsVersion < (decimal)0.20)
            {
                this.chkDoubleStep.BackColor = Color.FromArgb(255, 182, 193);
                this.chkDoubleStep.Checked = false;
                this.chkEraseEmpty.BackColor = Color.FromArgb(255, 182, 193);
                this.chkEraseEmpty.Checked = false;
            }

            CreateCommandLine();
        }
        #endregion

        #region btnLaunch_Click
        private void btnLaunch_Click(object sender, EventArgs e)
        {
            LaunchPython();
        }
        #endregion

        #region btnWTDSelectFile_Click
        private void btnWTDSelectFile_Click(object sender, EventArgs e)
        {
            // select file and folder where file is to be read from
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.RestoreDirectory = true; // make sure directory is set to executable path
            openDialog.InitialDirectory = m_sWriteDiskFolder;
            openDialog.Multiselect = false;
            openDialog.Title = "Select an image";
            openDialog.Filter = "Formats (*.HFE; *.SCP; *.IPF;)| *.HFE; *.SCP; *.IPF; | All files(*.*) | *.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                m_sWTDFilename = openDialog.SafeFileName;
                m_sWriteDiskFolder = Path.GetDirectoryName(openDialog.FileName);
                CreateCommandLine();
            }
        }
        #endregion

        #region changed

        private void chkWriteLastCyl_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkWriteFirstCyl_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkWTDAdjustSpeed_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkDriveSelectWTD_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void gbWriteToDisk_Enter(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void rbWriteSingleSided_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void rbWriteDoubleSided_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void txtWriteFirstCyl_TextChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void txtDriveSelectWTD_TextChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkDoubleStep_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkEraseEmpty_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        private void chkLegacySS_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }

        #endregion // changed

        #region WndProc
        protected override void WndProc(ref Message m) // capture close message so we can save our settings
        {
            if (m.Msg == WM_CLOSE)
            {
                // write inifile
                iniWriteFile();

                // show main form
                ChooserForm.m_frmChooser.Show();
            }
            base.WndProc(ref m);
        }
        #endregion
    }
}
