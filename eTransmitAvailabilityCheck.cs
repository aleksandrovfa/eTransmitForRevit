// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitAvailabilityCheck
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace eTransmitForRevit
{
  internal class eTransmitAvailabilityCheck : IExternalCommandAvailability
  {
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
  }
}
