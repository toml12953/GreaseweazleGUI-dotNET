﻿// Updateorm.cs
//
// Greaseweazle GUI Wrapper
//
// Copyright (c) 2019 Don Mankin <don.mankin@yahoo.com>
//
// MIT License
//
// See the file LICENSE for more details, or visit <https://opensource.org/licenses/MIT>.

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
    public partial class UpdateForm : Form
    {
        #region declarations
        private Form m_frmChooser = null;
        private const int WM_CLOSE = 0x0010;
        private string m_sUpdateFolder = "";
        private string m_sUpdateFilename = "firmware.upd";
        private string m_sUSBPort = "UNKNOWN";
        private bool m_bUSBSupport = false;
        private bool m_bLegacyUSB = true;
        private bool m_bWindowsEXE = false;
        #endregion

        #region UpdateForm
        public UpdateForm(ChooserForm newForm)
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

            // set defaults
            if (m_sUpdateFolder.Length == 0)
                m_sUpdateFolder = sExeDir;

            // bootloader support enabled in v0.16
            if (ChooserForm.m_GWToolsVersion < (decimal)0.16)
                this.chkBootLoader.BackColor = Color.FromArgb(255, 182, 193);
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
            // update firmware
            ChooserForm.m_Ini.IniWriteValue("gbUpdateFirmware", "m_sUpdateFilename", m_sUpdateFilename);
            ChooserForm.m_Ini.IniWriteValue("gbUpdateFirmware", "m_sUpdateFolder", m_sUpdateFolder);
            ChooserForm.m_Ini.IniWriteValue("gbWriteToDisk", "txtUpdateCommandLine", txtUpdateCommandLine.Text);
        }
        #endregion

        #region iniReadFile
        public void iniReadFile()
        {
            string sRet;

            // update firmware
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbUpdateFirmware", "m_sUpdateFilename", "garbage").Trim())) != "garbage")
                m_sUpdateFilename = sRet;
            if ((sRet = (ChooserForm.m_Ini.IniReadValue("gbUpdateFirmware", "m_sUpdateFolder", "garbage").Trim())) != "garbage")
                m_sUpdateFolder = sRet;

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

        #region UpdateForm_Load
        private void UpdateForm_Load(object sender, EventArgs e)
        {
            // read inifile
            iniReadFile();

            // initialize status label
            this.toolStripStatusLabel.Text = ChooserForm.m_sStatusLine.Trim();
            this.toolStripStatusLabel.BackColor = ChooserForm.m_StatusColor;
            this.statusStrip.BackColor = ChooserForm.m_StatusColor;

            CreateCommandLine();
        }
        #endregion
   
        #region btnLaunch_Click
        private void btnLaunch_Click(object sender, EventArgs e)
        {
            LaunchPython();
        }
        #endregion

        #region CreateCommandLine
        private void CreateCommandLine()
        {
            if (true == m_bWindowsEXE)
                txtUpdateCommandLine.Text = "gw.exe update";
            else
                txtUpdateCommandLine.Text = "python.exe " + ChooserForm.m_sGWscript + " update";
            if (chkBootLoader.Checked == true)
                txtUpdateCommandLine.Text += " --bootloader";
            if ((m_bLegacyUSB == false) && (m_bUSBSupport == true) && (m_sUSBPort != "UNKNOWN"))
                txtUpdateCommandLine.Text += " --device=" + m_sUSBPort;
            if (chkBootLoader.Checked == false)
                txtUpdateCommandLine.Text += " \"" + m_sUpdateFolder + "\\" + m_sUpdateFilename + "\"";
            if ((m_bLegacyUSB == true) && (m_bUSBSupport == true) && (m_sUSBPort != "UNKNOWN"))
                txtUpdateCommandLine.Text += " " + m_sUSBPort;
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
            startInfo.Arguments = "/K " + "\"" + txtUpdateCommandLine.Text + "\"";
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

        #region btnSelectUpdateFile_Click
        private void btnSelectUpdateFile_Click(object sender, EventArgs e)
        {
            // select file and folder where file is to be read from
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.RestoreDirectory = true; // make sure directory is set to executable path
            openDialog.InitialDirectory = m_sUpdateFolder;
            openDialog.Multiselect = false;
            openDialog.Title = "Select an image";
            openDialog.Filter = "UPD Files (*.upd)|*.upd" + "|" + "All Files (*.*)|*.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                m_sUpdateFilename = openDialog.SafeFileName;
                m_sUpdateFolder = Path.GetDirectoryName(openDialog.FileName);
                CreateCommandLine();
            }
        }
        #endregion

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

        #region chkBootLoader_CheckedChanged
        private void chkBootLoader_CheckedChanged(object sender, EventArgs e)
        {
            CreateCommandLine();
        }
        #endregion
    }
}
