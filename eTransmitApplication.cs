// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitApplication
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace eTransmitForRevit
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    internal class eTransmitApplication : IExternalApplication
  {
    public Result OnStartup(UIControlledApplication revitApp)
    {
      RibbonPanel ribbonPanel = revitApp.CreateRibbonPanel(eTransmitResources.eTransmitString);
      string location = typeof (eTransmitApplication).Assembly.Location;
      PushButton pushButton = (PushButton) ribbonPanel.AddItem((RibbonItemData) new PushButtonData(eTransmitResources.eTransmitString, eTransmitResources.TransmitAModel, location, "eTransmitForRevit.eTransmitCommand"));
      ((RibbonItem) pushButton).Enabled = true;
      pushButton.AvailabilityClassName = "eTransmitForRevit.eTransmitAvailabilityCheck";
      ((RibbonItem) pushButton).ToolTip = eTransmitResources.CopiesARevitModelAnd;
      try
      {
        ContextualHelpType contextualHelpType = (ContextualHelpType) 1;
        ((RibbonItem) pushButton).SetContextualHelp(new ContextualHelp(contextualHelpType, "HID_ETRANSMIT_HOME"));
      }
      catch (Exception ex)
      {
        revitApp.ControlledApplication.WriteJournalComment("eTransmit - Couldn't add F1 help. Extended message: " + ex.Message, true);
      }
      try
      {
        Uri uriSource = new Uri(Path.GetDirectoryName(location) + "\\icon_eTransmit_32.ico");
        BitmapImage bitmapImage1 = new BitmapImage(uriSource);
        ((RibbonButton) pushButton).LargeImage = (ImageSource) bitmapImage1;
        Uri uri = new Uri(Path.GetDirectoryName(location) + "\\icon_eTransmit.bmp");
        BitmapImage bitmapImage2 = new BitmapImage(uriSource);
        ((RibbonButton) pushButton).Image = (ImageSource) bitmapImage2;
      }
      catch (Exception ex)
      {
        revitApp.ControlledApplication.WriteJournalComment("eTransmit - unable to find icons. Extended message: " + ex.Message, true);
      }
      ribbonPanel.AddSeparator();
      ribbonPanel.AddStackedItems((RibbonItemData) new PushButtonData("eTransmitHelp", eTransmitResources.Help, location, "eTransmitForRevit.eTransmitHelp")
      {
        AvailabilityClassName = "eTransmitForRevit.eTransmitAvailabilityCheck"
      }, (RibbonItemData) new PushButtonData("eTransmitAbout", eTransmitResources.About, location, "eTransmitForRevit.eTransmitAbout")
      {
        AvailabilityClassName = "eTransmitForRevit.eTransmitAvailabilityCheck"
      });
      return (Result) 0;
    }

    public Result OnShutdown(UIControlledApplication revitApp) => (Result) 0;
  }
}
