// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.SuccessDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace eTransmitForRevitPirat
{
  public class SuccessDialog : Form
  {
    private IEnumerable<string> m_transmittedFileNames;
    private string m_transmittedFileDirectory;
    private IEnumerable<string> m_fileNamesToOpen;
    private Application m_revitApp;
    private IContainer components;
    private Button okButton;
    private Label label1;
    private Button openButton;

    public SuccessDialog(
      IEnumerable<string> transmittedFileNames,
      string transmittedFileDirectory,
      bool succeeded,
      bool generateReport,
      Autodesk.Revit.ApplicationServices.Application revitApp)
    {
      this.m_transmittedFileNames = transmittedFileNames;
      this.m_transmittedFileDirectory = transmittedFileDirectory;
      this.m_fileNamesToOpen = transmittedFileNames.Select<string, string>((Func<string, string>) (x => Path.Combine(transmittedFileDirectory, x)));
      this.m_revitApp = revitApp;
      this.InitializeComponent();
      this.Text = eTransmitResources.eTransmitString;
      this.okButton.Text = eTransmitResources.OK;
      this.openButton.Text = eTransmitResources.OpenFolder;
      this.HelpRequested += new HelpEventHandler(this.textBox_HelpRequested);
      this.HelpButtonClicked += new CancelEventHandler(this.successDialog_HelpButtonClicked);
      string str = transmittedFileNames.Count<string>() == 1 ? transmittedFileNames.First<string>() : string.Format("{0} files", (object) transmittedFileNames.Count<string>());
      if (succeeded)
        this.label1.Text = transmittedFileNames.Count<string>() == 1 ? string.Format(eTransmitResources.SuccessfullySaved, (object) transmittedFileNames.First<string>(), (object) transmittedFileDirectory) : string.Format(eTransmitResources.SuccessfullySavedFolder, (object) transmittedFileNames.Count<string>(), (object) transmittedFileDirectory);
      else if (generateReport)
        this.label1.Text = transmittedFileNames.Count<string>() == 1 ? string.Format(eTransmitResources.ErrorsWhileSaving, (object) transmittedFileNames.First<string>(), (object) transmittedFileDirectory) : string.Format(eTransmitResources.ErrorsWhileSavingFolder, (object) transmittedFileNames.Count<string>(), (object) transmittedFileDirectory);
      else
        this.label1.Text = transmittedFileNames.Count<string>() == 1 ? string.Format(eTransmitResources.ErrorsWhileSavingNoReport, (object) transmittedFileNames.First<string>(), (object) transmittedFileDirectory) : string.Format(eTransmitResources.ErrorsWhileSavingFolderNoReport, (object) transmittedFileNames.Count<string>(), (object) transmittedFileDirectory);
    }

    private void textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      eTransmitHelpLauncher.LaunchHelp();
      hlpevent.Handled = true;
    }

    private void successDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      eTransmitHelpLauncher.LaunchHelp();
      e.Cancel = true;
    }

    public IEnumerable<string> GetFileNames() => this.m_fileNamesToOpen;

    private void button1_Click(object sender, EventArgs e) => this.Close();

    private void button2_Click(object sender, EventArgs e)
    {
      try
      {
        new Process()
        {
          StartInfo = {
            FileName = this.m_transmittedFileDirectory
          }
        }.Start();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(eTransmitResources.UnableToOpenFolder);
        this.m_revitApp.WriteJournalComment("eTransmit - Unable to open the transmittal folder. Extended message: " + ex.Message, true);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SuccessDialog));
      this.okButton = new Button();
      this.label1 = new Label();
      this.openButton = new Button();
      this.SuspendLayout();
      this.okButton.Location = new Point(192, 171);
      this.okButton.Margin = new Padding(4, 4, 4, 4);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size((int) sbyte.MaxValue, 37);
      this.okButton.TabIndex = 2;
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new EventHandler(this.button1_Click);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(28, 31);
      this.label1.Margin = new Padding(4, 0, 4, 0);
      this.label1.MaximumSize = new Size(433, 1231);
      this.label1.Name = "label1";
      this.label1.Size = new Size(8, 17);
      this.label1.TabIndex = 4;
      this.label1.Text = "\r\n";
      this.openButton.Location = new Point(328, 171);
      this.openButton.Margin = new Padding(4, 4, 4, 4);
      this.openButton.Name = "openButton";
      this.openButton.Size = new Size((int) sbyte.MaxValue, 37);
      this.openButton.TabIndex = 5;
      this.openButton.UseVisualStyleBackColor = true;
      this.openButton.Click += new EventHandler(this.button2_Click);
      this.AcceptButton = (IButtonControl) this.okButton;
      this.AutoScaleDimensions = new SizeF(8f, 16f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(468, 214);
      this.Controls.Add((Control) this.openButton);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.okButton);
      this.HelpButton = true;
      //this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Margin = new Padding(4, 4, 4, 4);
      this.MaximizeBox = false;
      this.MaximumSize = new Size(486, 259);
      this.MinimizeBox = false;
      this.MinimumSize = new Size(486, 259);
      //this.Name = nameof (SuccessDialog);
      this.Name = "SuccessDialog";
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
