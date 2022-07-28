// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitCallbackHandler
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using eTransmitForRevitDB;

namespace eTransmitForRevit
{
  public class eTransmitCallbackHandler : TransmissionCallbackHandler
  {
    private TransmissionOptions m_options;
    private ProgressBarDialog m_parentDialog;

    public event PreTransmitHandler AboutToTransmitFile;

    public event PostTransmitHandler CompletedOneFile;

    public eTransmitCallbackHandler() => this.m_options = new TransmissionOptions();

    public void setParentDialog(ProgressBarDialog modelDialog) => this.m_parentDialog = modelDialog;

    public override void PreTransmitPackage(
      ref TransmissionGraph graph,
      ref TransmissionOptions options)
    {
      base.PreTransmitPackage(ref graph, ref options);
      this.m_options = new TransmissionOptions(options);
    }

    public override void PreTransmitFile(ref AReferencedFile file, ref TransmissionGraph graph)
    {
      if (this.AboutToTransmitFile == null)
        return;
      this.AboutToTransmitFile((object) this, new FileEventArgs(file));
    }

    public override void PostTransmitFile(AReferencedFile file, TransmissionGraph graph)
    {
      if (this.CompletedOneFile == null)
        return;
      this.CompletedOneFile((object) this, new FileEventArgs(file));
    }

    public override bool CancelOperation() => this.m_parentDialog.Canceled;
  }
}
