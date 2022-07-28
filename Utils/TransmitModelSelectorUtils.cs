// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.Utils.TransmitModelSelectorUtils
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eTransmitForRevit.Utils
{
  internal static class TransmitModelSelectorUtils
  {
    public static IEnumerable<Autodesk.Revit.DB.ModelPath> GetModelPathsFromFileOrFolder(string input)
    {
      if (Directory.Exists(input))
        return ((IEnumerable<string>) Directory.GetFiles(input))
                    .Where<string>((Func<string, bool>) (x => Path.GetExtension(x).Equals(".rvt")))
                    .Select<string, Autodesk.Revit.DB.ModelPath>(new Func<string,Autodesk.Revit.DB.ModelPath>(ModelPathUtils.ConvertUserVisiblePathToModelPath));
      if (File.Exists(input))
      {
        List<Autodesk.Revit.DB.ModelPath> fromFileOrFolder = new List<Autodesk.Revit.DB.ModelPath>();
        fromFileOrFolder.Add(ModelPathUtils.ConvertUserVisiblePathToModelPath(input));
        return (IEnumerable<Autodesk.Revit.DB.ModelPath>) fromFileOrFolder;
      }
            Autodesk.Revit.DB.ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(input);
      if (!modelPath.ServerPath)
        return (IEnumerable<Autodesk.Revit.DB.ModelPath>) null;
      List<Autodesk.Revit.DB.ModelPath> fromFileOrFolder1 = new List<Autodesk.Revit.DB.ModelPath>();
      fromFileOrFolder1.Add((Autodesk.Revit.DB.ModelPath)modelPath);
      return (IEnumerable<Autodesk.Revit.DB.ModelPath>) fromFileOrFolder1;
    }
  }
}
