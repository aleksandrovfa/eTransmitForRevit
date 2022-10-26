// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitAboutBox
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using eTransmitForRevitDB;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace eTransmitForRevit
{
  internal class eTransmitAboutBox : Form
  {
    private IContainer components;
    private Button okButton;
    private TextBox textBoxDescription;
    private TableLayoutPanel tableLayoutPanel;

    public eTransmitAboutBox()
    {
      this.InitializeComponent();
      this.Text = string.Format(eTransmitResources.AboutTitle, (object) TransmissionOptions.ServiceVersion);
      this.textBoxDescription.Text = string.Format(eTransmitResources.AboutText, (object) TransmissionOptions.ServiceVersion, (object) this.AssemblyCopyright);
      this.okButton.Text = eTransmitResources.OK;
    }

    public string AssemblyTitle
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
        if (customAttributes.Length != 0)
        {
          AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute) customAttributes[0];
          if (assemblyTitleAttribute.Title != "")
            return assemblyTitleAttribute.Title;
        }
        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public string AssemblyDescription
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
        return customAttributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute) customAttributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false);
        return customAttributes.Length == 0 ? "" : ((AssemblyProductAttribute) customAttributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
        return customAttributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute) customAttributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
        return customAttributes.Length == 0 ? "" : ((AssemblyCompanyAttribute) customAttributes[0]).Company;
      }
    }

    private void okButton_Click(object sender, EventArgs e) => this.Close();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.okButton = new Button();
      this.textBoxDescription = new TextBox();
      this.tableLayoutPanel = new TableLayoutPanel();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.okButton.DialogResult = DialogResult.Cancel;
      this.okButton.Location = new Point(339, 232);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size(75, 30);
      this.okButton.TabIndex = 24;
      this.okButton.Click += new EventHandler(this.okButton_Click);
      this.textBoxDescription.Dock = DockStyle.Fill;
      this.textBoxDescription.Location = new Point(6, 3);
      this.textBoxDescription.Margin = new Padding(6, 3, 3, 3);
      this.textBoxDescription.Multiline = true;
      this.textBoxDescription.Name = "textBoxDescription";
      this.textBoxDescription.ReadOnly = true;
      this.textBoxDescription.ScrollBars = ScrollBars.Both;
      this.textBoxDescription.Size = new Size(408, 214);
      this.textBoxDescription.TabIndex = 23;
      this.textBoxDescription.TabStop = false;
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel.Controls.Add((Control) this.textBoxDescription, 0, 0);
      this.tableLayoutPanel.Controls.Add((Control) this.okButton, 0, 1);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(9, 9);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 2;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 83.33334f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel.Size = new Size(417, 265);
      this.tableLayoutPanel.TabIndex = 0;
      this.AcceptButton = (IButtonControl) this.okButton;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(435, 283);
      this.Controls.Add((Control) this.tableLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (eTransmitAboutBox);
      this.Padding = new Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
