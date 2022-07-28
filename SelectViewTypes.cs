// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.SelectViewTypes
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace eTransmitForRevit
{
  public class SelectViewTypes : Form
  {
    private TransmissionViews m_transmissionViews;
    private List<string> includeTypeNames;
    private List<string> excludeTypeNames;
    private int totalTypesNumber;
    private bool ifChangeCheckAll = true;
    private IContainer components;
    private Button okButton;
    private Button cancelButton;
    private CheckedListBox typeNameList;

    public SelectViewTypes(TransmissionViews transmissionViews)
    {
      this.InitializeComponent();
      this.Text = eTransmitResources.SelectViewTypesDialogTitle;
      this.okButton.Text = eTransmitResources.OK;
      this.cancelButton.Text = eTransmitResources.Cancel;
      this.m_transmissionViews = transmissionViews;
      this.updateControl();
    }

    private void getDataFromTransmissionView()
    {
      this.includeTypeNames = this.m_transmissionViews.getIncludedTypeNames();
      this.excludeTypeNames = this.m_transmissionViews.getExcludedTypeNames();
      this.includeTypeNames.Sort();
      this.excludeTypeNames.Sort();
      this.totalTypesNumber = this.includeTypeNames.Count + this.excludeTypeNames.Count;
    }

    private void setDataToTransmissionView()
    {
      foreach (string includeTypeName in this.includeTypeNames)
      {
        if (!this.m_transmissionViews.ifIncluded(includeTypeName))
          this.m_transmissionViews.includeViewType(includeTypeName);
      }
      foreach (string excludeTypeName in this.excludeTypeNames)
      {
        if (!this.m_transmissionViews.ifExcluded(excludeTypeName))
          this.m_transmissionViews.excludeViewType(excludeTypeName);
      }
    }

    public void updateControl()
    {
      this.getDataFromTransmissionView();
      this.updateList();
    }

    private void updateList()
    {
      List<string> includedTypeNames = new TransmissionViews().getIncludedTypeNames();
      includedTypeNames.Sort();
      this.typeNameList.Items.Clear();
      this.typeNameList.Items.Add((object) eTransmitResources.All, this.excludeTypeNames.Count == 0);
      foreach (string name in includedTypeNames)
      {
        bool isChecked = this.m_transmissionViews.ifIncluded(name);
        this.typeNameList.Items.Add((object) name, isChecked);
      }
    }

    public TransmissionViews TransmissionViews
    {
      get => this.m_transmissionViews;
      set => this.m_transmissionViews = value;
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.setDataToTransmissionView();
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void typeNameList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      if (e.Index == 0)
      {
        if (!this.ifChangeCheckAll)
          return;
        if (e.NewValue == CheckState.Checked)
        {
          for (int index = 1; index < this.typeNameList.Items.Count; ++index)
          {
            this.typeNameList.SetItemCheckState(index, CheckState.Checked);
            string str = this.typeNameList.Items[index].ToString();
            if (!this.includeTypeNames.Contains(str))
              this.includeTypeNames.Add(str);
            if (this.excludeTypeNames.Contains(str))
              this.excludeTypeNames.Remove(str);
          }
        }
        else
        {
          for (int index = 1; index < this.typeNameList.Items.Count; ++index)
          {
            this.typeNameList.SetItemCheckState(index, CheckState.Unchecked);
            string str = this.typeNameList.Items[index].ToString();
            if (this.includeTypeNames.Contains(str))
              this.includeTypeNames.Remove(str);
            if (!this.excludeTypeNames.Contains(str))
              this.excludeTypeNames.Add(str);
          }
        }
      }
      else
      {
        string str = this.typeNameList.Items[e.Index].ToString();
        if (e.NewValue == CheckState.Checked)
        {
          if (!this.includeTypeNames.Contains(str))
            this.includeTypeNames.Add(str);
          if (this.excludeTypeNames.Contains(str))
            this.excludeTypeNames.Remove(str);
          if (this.excludeTypeNames.Count != 0 || this.typeNameList.GetItemCheckState(0) != CheckState.Unchecked)
            return;
          this.ifChangeCheckAll = false;
          this.typeNameList.SetItemCheckState(0, CheckState.Checked);
          this.ifChangeCheckAll = true;
        }
        else
        {
          if (this.includeTypeNames.Contains(str))
            this.includeTypeNames.Remove(str);
          if (!this.excludeTypeNames.Contains(str))
            this.excludeTypeNames.Add(str);
          if (this.typeNameList.GetItemCheckState(0) != CheckState.Checked)
            return;
          this.ifChangeCheckAll = false;
          this.typeNameList.SetItemCheckState(0, CheckState.Unchecked);
          this.ifChangeCheckAll = true;
        }
      }
    }

    private void CustomizeViewsDialog_FormClosed(object sender, FormClosedEventArgs e) => this.updateControl();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SelectViewTypes));
      this.okButton = new Button();
      this.cancelButton = new Button();
      this.typeNameList = new CheckedListBox();
      this.SuspendLayout();
      this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.okButton.Location = new Point(161, 326);
      this.okButton.Margin = new Padding(2);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size(82, 33);
      this.okButton.TabIndex = 2;
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new EventHandler(this.okButton_Click);
      this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.cancelButton.DialogResult = DialogResult.Cancel;
      this.cancelButton.Location = new Point(249, 326);
      this.cancelButton.Margin = new Padding(2);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new Size(82, 33);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.UseVisualStyleBackColor = true;
      this.typeNameList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.typeNameList.CheckOnClick = true;
      this.typeNameList.FormattingEnabled = true;
      this.typeNameList.Location = new Point(10, 20);
      this.typeNameList.Margin = new Padding(2);
      this.typeNameList.Name = "typeNameList";
      this.typeNameList.Size = new Size(321, 289);
      this.typeNameList.TabIndex = 4;
      this.typeNameList.ItemCheck += new ItemCheckEventHandler(this.typeNameList_ItemCheck);
      this.AcceptButton = (IButtonControl) this.okButton;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cancelButton;
      this.ClientSize = new Size(340, 370);
      this.Controls.Add((Control) this.typeNameList);
      this.Controls.Add((Control) this.cancelButton);
      this.Controls.Add((Control) this.okButton);
      //this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Margin = new Padding(2);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new Size(228, 408);
      this.Name = "SelectViewTypes";
      //this.Name = nameof (SelectViewTypes);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.FormClosed += new FormClosedEventHandler(this.CustomizeViewsDialog_FormClosed);
      this.ResumeLayout(false);
    }
  }
}
