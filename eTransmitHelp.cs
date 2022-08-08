// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitHelp
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace eTransmitForRevitPirat
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    internal class eTransmitHelp : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements)
    {
      try
      {
        eTransmitHelpLauncher.LaunchHelp();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Unable to launch eTransmit help.");
        commandData.Application.Application.WriteJournalComment("eTransmit - unable to launch help. Extended message: " + ex.Message, true);
      }
      return (Result) 0;
    }
  }
}
