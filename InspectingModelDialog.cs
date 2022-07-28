// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.InspectingModelDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace eTransmitForRevit
{
  public class InspectingModelDialog : ProgressBarDialog
  {
    private int m_fileCount;
    private long m_bytes;
    private Application m_revitApp;

    public InspectingModelDialog(
      TransmissionOptions options,
      eTransmitCallbackHandler handler,
      Application revitApp)
      : base(options, handler)
    {
      this.m_revitApp = revitApp;
      this.m_fileCount = 0;
      this.m_bytes = 0L;
      this.mainTextLabel.Text = eTransmitResources.InspectingModelPleaseWait;
      this.aboveBarLabel.Text = string.Format(eTransmitResources.FilesAndBytes, (object) this.m_fileCount, (object) this.GetBytesString());
      this.belowBarLabel.Text = "";
    }

    private string GetBytesString()
    {
      long bytes = this.m_bytes;
      if (bytes < 10024L)
        return string.Format(eTransmitResources.Bytes, (object) bytes);
      long num1 = bytes / 1024L;
      if (num1 < 10024L)
        return string.Format(eTransmitResources.KBytes, (object) num1);
      long num2 = num1 / 1024L;
      return num2 < 10024L ? string.Format(eTransmitResources.MBytes, (object) num2) : string.Format(eTransmitResources.GBytes, (object) (num2 / 1024L));
    }

    public void DisplayProgressWhileBuildingGraph(
      IEnumerable<Autodesk.Revit.DB.ModelPath> mainModelPaths,
      bool isRecording)
    {
      DateTime dateTime1 = DateTime.Now.AddMilliseconds(500.0);
      DateTime dateTime2 = DateTime.Now.AddMilliseconds(50.0);
      if (isRecording)
      {
        this.Show();
        while (DateTime.Now < dateTime2)
                    System.Windows.Forms.Application.DoEvents();
      }
      eTransmit.FileAdded += new FileAddedHandler(this.graph_FileAdded);
      IList<string> additionalFiles = (IList<string>) new List<string>();
      TransmissionGraph transmissionGraph = eTransmit.CreateTransmissionGraph(mainModelPaths, (TransmissionCallbackHandler) this.m_handler, this.m_revitApp, ref additionalFiles);
      this.m_options.AdditionalFiles.AddRange((IEnumerable<string>) additionalFiles);
      if (isRecording)
      {
        while (DateTime.Now < dateTime1)
                    System.Windows.Forms.Application.DoEvents();
      }
      if (this.m_options.GetIncludedTypes().Count < 5)
      {
        this.aboveBarLabel.Text = eTransmitResources.RemovingExcludedFileTypes;
        DateTime dateTime3 = DateTime.Now.AddMilliseconds(200.0);
        transmissionGraph.ExcludeByType(this.m_options);
        if (isRecording)
        {
          while (DateTime.Now < dateTime3)
                        System.Windows.Forms.Application.DoEvents();
        }
        if (this.m_handler.CancelOperation())
        {
          if (!isRecording)
            return;
          this.Close();
          return;
        }
      }
      transmissionGraph.addAdditionalFiles(this.m_options.AdditionalFiles);
      this.aboveBarLabel.Text = eTransmitResources.RemovingParentlessFiles;
      transmissionGraph.ExcludeParentlessFiles();
      this.m_graph = transmissionGraph;
      if (!isRecording)
        return;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    public void graph_FileAdded(object sender, FileEventArgs e)
    {
      ++this.m_fileCount;
      this.m_bytes += e.File.Size;
      this.aboveBarLabel.Text = string.Format(eTransmitResources.FilesAndBytes, (object) this.m_fileCount, (object) this.GetBytesString());
      DateTime dateTime = DateTime.Now.AddMilliseconds(100.0);
      while (DateTime.Now < dateTime)
                System.Windows.Forms.Application.DoEvents();
    }
  }
}
