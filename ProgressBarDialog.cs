// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.ProgressBarDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using eTransmitForRevitDB;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace eTransmitForRevitPirat
{
  public class ProgressBarDialog : Form
  {
    protected TransmissionGraph m_graph;
    protected TransmissionOptions m_options;
    protected eTransmitCallbackHandler m_handler;
    protected bool m_success;
    protected bool m_cancel;
    private IContainer components;
    protected Label mainTextLabel;
    protected Label aboveBarLabel;
    protected ProgressBar progressBar1;
    protected Button cancelButton;
    protected Label belowBarLabel;

    public ProgressBarDialog(TransmissionOptions options, eTransmitCallbackHandler handler)
    {
      this.InitializeComponent();
      this.cancelButton.Text = eTransmitResources.Cancel;
      this.m_options = options;
      this.m_handler = handler;
      this.m_handler.setParentDialog(this);
      this.m_success = true;
      this.m_cancel = false;
      this.HelpButtonClicked += new CancelEventHandler(this.progressBarDialog_HelpButtonClicked);
      this.HelpRequested += new HelpEventHandler(this.textBox_HelpRequested);
      this.cancelButton.Click += new EventHandler(this.button1_Click);
      this.FormClosing += new FormClosingEventHandler(this.ProgressBarDialog_FormClosing);
    }

    public bool Succeeded => this.m_success;

    public bool Canceled => this.m_cancel;

    private void button1_Click(object sender, EventArgs e) => this.m_cancel = true;

    public TransmissionGraph GetTransmissionGraph() => this.m_graph;

    private void textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      eTransmitHelpLauncher.LaunchHelp();
      hlpevent.Handled = true;
    }

    private void progressBarDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      eTransmitHelpLauncher.LaunchHelp();
      e.Cancel = true;
    }

    private void ProgressBarDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.DialogResult == DialogResult.OK)
        return;
      this.m_cancel = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ProgressBarDialog));
      this.mainTextLabel = new Label();
      this.aboveBarLabel = new Label();
      this.progressBar1 = new ProgressBar();
      this.cancelButton = new Button();
      this.belowBarLabel = new Label();
      this.SuspendLayout();
      this.mainTextLabel.AutoSize = true;
      this.mainTextLabel.Font = new Font("Microsoft Sans Serif", 14f);
      this.mainTextLabel.Location = new Point(22, 28);
      this.mainTextLabel.Name = "mainTextLabel";
      this.mainTextLabel.Size = new Size(0, 24);
      this.mainTextLabel.TabIndex = 0;
      this.aboveBarLabel.AutoSize = true;
      this.aboveBarLabel.Location = new Point(23, 71);
      this.aboveBarLabel.Name = "aboveBarLabel";
      this.aboveBarLabel.Size = new Size(0, 13);
      this.aboveBarLabel.TabIndex = 1;
      this.progressBar1.Location = new Point(110, 87);
      this.progressBar1.MaximumSize = new Size(273, 33);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new Size(273, 33);
      this.progressBar1.Style = ProgressBarStyle.Marquee;
      this.progressBar1.TabIndex = 2;
      this.cancelButton.DialogResult = DialogResult.Cancel;
      this.cancelButton.Location = new Point(393, 178);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new Size(95, 30);
      this.cancelButton.TabIndex = 3;
      this.cancelButton.UseVisualStyleBackColor = true;
      this.belowBarLabel.AutoSize = true;
      this.belowBarLabel.Location = new Point(23, 123);
      this.belowBarLabel.MaximumSize = new Size(450, 200);
      this.belowBarLabel.Name = "belowBarLabel";
      this.belowBarLabel.Size = new Size(0, 13);
      this.belowBarLabel.TabIndex = 4;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cancelButton;
      this.ClientSize = new Size(500, 220);
      this.Controls.Add((Control) this.belowBarLabel);
      this.Controls.Add((Control) this.cancelButton);
      this.Controls.Add((Control) this.progressBar1);
      this.Controls.Add((Control) this.aboveBarLabel);
      this.Controls.Add((Control) this.mainTextLabel);
      this.HelpButton = true;
      //this.Icon = (Icon) componentResourceManager.GetObject("icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      //this.Name = nameof (ProgressBarDialog);
      this.Name = "ProgressBarDialog";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
