// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.AdditionalFilesDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace eTransmitForRevit
{
  public class AdditionalFilesDialog : Form
  {
    private List<string> m_additionalFiles;
    private List<string> tempFiles = new List<string>();
    private string m_lastUsedPath;
    private IContainer components;
    private Label label1;
    private ListView fileList;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private Button removeFile;
    private Button addFile;
    private Button cancelButton;
    private Button okButton;

    public AdditionalFilesDialog(List<string> additionalFiles)
    {
      this.InitializeComponent();
      this.Text = eTransmitResources.AddOtherFilesTitle;
      this.okButton.Text = eTransmitResources.OK;
      this.cancelButton.Text = eTransmitResources.Cancel;
      this.label1.Text = eTransmitResources.CurrentFiles;
      this.fileList.Columns[0].Text = eTransmitResources.FileName;
      this.fileList.Columns[1].Text = eTransmitResources.Location;
      this.m_additionalFiles = additionalFiles;
      this.m_lastUsedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      this.updateFileList();
    }

    public void updateFileList()
    {
      this.tempFiles.Clear();
      foreach (string additionalFile in this.m_additionalFiles)
        this.tempFiles.Add(additionalFile);
      this.fileList.Items.Clear();
      foreach (string tempFile in this.tempFiles)
        this.fileList.Items.Add(new ListViewItem(Path.GetFileName(tempFile))
        {
          SubItems = {
            tempFile
          }
        });
    }

    private void addFile_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.InitialDirectory = this.m_lastUsedPath;
      openFileDialog.Multiselect = true;
      openFileDialog.CheckFileExists = true;
      openFileDialog.CheckPathExists = true;
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      List<string> list = ((IEnumerable<string>) openFileDialog.FileNames).ToList<string>();
      this.fileList.SelectedItems.Clear();
      foreach (string str in list)
      {
        if (!this.tempFiles.Contains(str))
        {
          this.tempFiles.Add(str);
          ListViewItem listViewItem = new ListViewItem(Path.GetFileName(str));
          listViewItem.SubItems.Add(str);
          this.fileList.Items.Add(listViewItem);
          listViewItem.Selected = true;
        }
      }
      if (list.Count > 0)
        this.m_lastUsedPath = Path.GetDirectoryName(list[0]);
      this.fileList.Focus();
    }

    private void removeFile_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem selectedItem in this.fileList.SelectedItems)
      {
        this.tempFiles.Remove(selectedItem.SubItems[1].Text);
        this.fileList.Items.Remove(selectedItem);
      }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.m_additionalFiles.Clear();
      foreach (string tempFile in this.tempFiles)
        this.m_additionalFiles.Add(tempFile);
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void AdditionalFilesDialog_FormClosed(object sender, FormClosedEventArgs e) => this.updateFileList();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AdditionalFilesDialog));
      this.label1 = new Label();
      this.fileList = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.removeFile = new Button();
      this.addFile = new Button();
      this.cancelButton = new Button();
      this.okButton = new Button();
      this.SuspendLayout();
      this.label1.Location = new Point(12, 22);
      this.label1.Name = "label1";
      this.label1.Size = new Size(413, 23);
      this.label1.TabIndex = 0;
      this.fileList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.fileList.Columns.AddRange(new ColumnHeader[2]
      {
        this.columnHeader1,
        this.columnHeader2
      });
      this.fileList.FullRowSelect = true;
      this.fileList.LabelWrap = false;
      this.fileList.Location = new Point(15, 48);
      this.fileList.Name = "fileList";
      this.fileList.Size = new Size(530, 167);
      this.fileList.TabIndex = 1;
      this.fileList.UseCompatibleStateImageBehavior = false;
      this.fileList.View = View.Details;
      this.columnHeader1.Text = "";
      this.columnHeader1.Width = 85;
      this.columnHeader2.Text = "";
      this.columnHeader2.Width = 439;
      this.removeFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.removeFile.BackgroundImage = (Image) componentResourceManager.GetObject("removeFile.BackgroundImage");
      this.removeFile.BackgroundImageLayout = ImageLayout.Center;
      this.removeFile.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.removeFile.ForeColor = Color.Red;
      this.removeFile.Location = new Point(52, 225);
      this.removeFile.Name = "removeFile";
      this.removeFile.Size = new Size(30, 30);
      this.removeFile.TabIndex = 2;
      this.removeFile.TextAlign = ContentAlignment.TopCenter;
      this.removeFile.UseVisualStyleBackColor = true;
      this.removeFile.Click += new EventHandler(this.removeFile_Click);
      this.addFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.addFile.BackgroundImage = (Image) componentResourceManager.GetObject("addFile.BackgroundImage");
      this.addFile.BackgroundImageLayout = ImageLayout.Center;
      this.addFile.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.addFile.ForeColor = Color.LimeGreen;
      this.addFile.Location = new Point(17, 225);
      this.addFile.Name = "addFile";
      this.addFile.Size = new Size(30, 30);
      this.addFile.TabIndex = 2;
      this.addFile.TextAlign = ContentAlignment.TopCenter;
      this.addFile.UseVisualStyleBackColor = true;
      this.addFile.Click += new EventHandler(this.addFile_Click);
      this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.cancelButton.DialogResult = DialogResult.Cancel;
      this.cancelButton.Location = new Point(452, 261);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new Size(90, 30);
      this.cancelButton.TabIndex = 4;
      this.cancelButton.UseVisualStyleBackColor = true;
      this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.okButton.Location = new Point(345, 261);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size(90, 30);
      this.okButton.TabIndex = 3;
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new EventHandler(this.okButton_Click);
      this.AutoScaleDimensions = new SizeF(8f, 16f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cancelButton;
      this.ClientSize = new Size(559, 312);
      this.Controls.Add((Control) this.cancelButton);
      this.Controls.Add((Control) this.okButton);
      this.Controls.Add((Control) this.addFile);
      this.Controls.Add((Control) this.removeFile);
      this.Controls.Add((Control) this.fileList);
      this.Controls.Add((Control) this.label1);
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new Size(577, 357);
      this.Name = nameof (AdditionalFilesDialog);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.FormClosed += new FormClosedEventHandler(this.AdditionalFilesDialog_FormClosed);
      this.ResumeLayout(false);
    }
  }
}
