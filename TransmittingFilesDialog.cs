// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.TransmittingFilesDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using eTransmitForRevitDB;
using System;
using System.IO;
using System.Windows.Forms;

namespace eTransmitForRevitPirat
{
  internal class TransmittingFilesDialog : ProgressBarDialog
  {
    private int m_transmittedFileCount;
    private int m_totalFileCount;
    private bool m_isRecording;

    public TransmittingFilesDialog(
      TransmissionGraph graph,
      TransmissionOptions options,
      eTransmitCallbackHandler handler)
      : base(options, handler)
    {
      this.m_graph = graph;
      this.m_transmittedFileCount = 0;
      this.m_totalFileCount = graph.GetIncludedFileCount();
      this.mainTextLabel.Text = eTransmitResources.SavingFilesPleaseWait;
      this.aboveBarLabel.Text = string.Format(eTransmitResources.NOfMFilesCompleted, (object) 0, (object) this.m_totalFileCount);
      this.belowBarLabel.Text = "";
      this.progressBar1.Style = ProgressBarStyle.Continuous;
      this.progressBar1.Minimum = 0;
      this.progressBar1.Maximum = this.m_totalFileCount;
      this.progressBar1.Step = 1;
      this.m_handler.CompletedOneFile += new PostTransmitHandler(this.m_handler_CompletedOneFile);
      this.m_handler.AboutToTransmitFile += new PreTransmitHandler(this.m_handler_AboutToTransmitFile);
      this.m_isRecording = false;
    }

    public void DisplayProgressWhileTransmittingFiles(UIApplication uiApp, bool isRecording)
    {
      this.m_isRecording = isRecording;
      if (this.m_isRecording)
      {
        this.Show();
        DateTime dateTime = DateTime.Now.AddMilliseconds(1000.0);
        while (DateTime.Now < dateTime)
        {
          this.Activate();
          Application.DoEvents();
        }
      }
      this.m_success = new eTransmit(uiApp.Application).TransmitFiles(this.m_graph, this.m_options, (TransmissionCallbackHandler) this.m_handler);
      if (!this.m_isRecording)
        return;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void m_handler_AboutToTransmitFile(object sender, FileEventArgs e)
    {
      DateTime dateTime = DateTime.Now.AddMilliseconds(50.0);
      if (this.m_options.OpenAndUpgrade)
        dateTime = DateTime.Now.AddMilliseconds(400.0);
      string fileName = Path.GetFileName(ModelPathUtils.ConvertModelPathToUserVisiblePath(e.File.GetPath()));
      this.belowBarLabel.Text = string.Format(e.File.MainModel ? eTransmitResources.SavingMainModel : eTransmitResources.SavingLinkedFile, (object) fileName);
      while (DateTime.Now < dateTime && this.m_isRecording)
      {
        this.Activate();
        Application.DoEvents();
      }
    }

    private void m_handler_CompletedOneFile(object sender, FileEventArgs e)
    {
      DateTime dateTime = DateTime.Now.AddMilliseconds(50.0);
      if (this.m_options.OpenAndUpgrade)
        dateTime = DateTime.Now.AddMilliseconds(400.0);
      ++this.m_transmittedFileCount;
      this.aboveBarLabel.Text = string.Format(eTransmitResources.NOfMFilesCompleted, (object) this.m_transmittedFileCount, (object) this.m_totalFileCount);
      this.progressBar1.PerformStep();
      while (DateTime.Now < dateTime && this.m_isRecording)
      {
        this.Activate();
        Application.DoEvents();
      }
    }
  }
}
