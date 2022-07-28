// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitReport
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace eTransmitForRevit
{
  internal class eTransmitReport
  {
    private HashSet<AReferencedFile> m_includedFiles;
    private HashSet<AReferencedFile> m_excludedFiles;
    private HashSet<AReferencedFile> m_errorFiles;
    private HashSet<AReferencedFile> m_noInspectForLinksFiles;
    private HashSet<AReferencedFile> m_upgradeErrorFiles;
    private IEnumerable<AReferencedFile> m_mainModels;
    private Dictionary<string, HashSet<FailureStorage>> m_errors;

    public eTransmitReport(TransmissionGraph graph)
    {
      this.m_includedFiles = new HashSet<AReferencedFile>();
      this.m_excludedFiles = new HashSet<AReferencedFile>();
      this.m_errorFiles = new HashSet<AReferencedFile>();
      this.m_noInspectForLinksFiles = new HashSet<AReferencedFile>();
      this.m_upgradeErrorFiles = new HashSet<AReferencedFile>();
      this.m_mainModels = graph.GetMainModels();
      this.m_errors = graph.Errors;
      IEnumerator<AReferencedFile> enumerator = graph.GetEnumerator();
      while (enumerator.MoveNext())
      {
        AReferencedFile current = enumerator.Current;
        if (current.UpgradeFailureType != UpgradeFailureType.UpgradeNotSelected && current.UpgradeFailureType != UpgradeFailureType.UpgradeSucceeded)
          this.m_upgradeErrorFiles.Add(current);
        switch (current.GetTransmissionStatus())
        {
          case FileTransmissionStatus.NotTried:
          case FileTransmissionStatus.ExcludedByType:
          case FileTransmissionStatus.ExcludedByParents:
            this.m_excludedFiles.Add(current);
            continue;
          case FileTransmissionStatus.Succeeded:
            this.m_includedFiles.Add(current);
            if (current.InspectionFailureType == InspectionFailureType.UnableToReadLinks || current.TransmissionFailureType == TransmissionFailureType.UnableToRepathLinks || current.TransmissionFailureType == TransmissionFailureType.FilePredatesR2012)
            {
              this.m_noInspectForLinksFiles.Add(current);
              continue;
            }
            continue;
          case FileTransmissionStatus.Failed:
            if (current.TransmissionFailureType == TransmissionFailureType.FilePredatesR2012 || current.TransmissionFailureType == TransmissionFailureType.UnableToRepathLinks)
            {
              this.m_noInspectForLinksFiles.Add(current);
              continue;
            }
            this.m_errorFiles.Add(current);
            continue;
          default:
            this.m_errorFiles.Add(current);
            continue;
        }
      }
    }

    public void AddIncludedFile(AReferencedFile aFile) => this.m_includedFiles.Add(aFile);

    public void AddExcludedFile(AReferencedFile aFile) => this.m_excludedFiles.Add(aFile);

    public void AddErrorFile(AReferencedFile aFile) => this.m_errorFiles.Add(aFile);

    public void AddFileWhichCouldNotBeCheckedForLinks(AReferencedFile aFile) => this.m_noInspectForLinksFiles.Add(aFile);

    public static string GetStringForUpgradeError(UpgradeFailureType failType)
    {
      switch (failType)
      {
        case UpgradeFailureType.UnableToOpenDocument:
          return eTransmitResources.UnableToOpenDocument;
        case UpgradeFailureType.UnableToSaveDocument:
          return eTransmitResources.UnableToSaveDocument;
        case UpgradeFailureType.UnableToPurgeUnused:
          return eTransmitResources.UnableToPurgeUnused;
        case UpgradeFailureType.UnableToDeleteBackups:
          return eTransmitResources.UnableToDeleteBackups;
        default:
          return eTransmitResources.UnableToOpenDocument;
      }
    }

    public static string GetStringForError(
      AReferencedFile aFile,
      string outputDirectory,
      bool shortNamesOnly)
    {
      if (aFile.InspectionFailureType == InspectionFailureType.UnableToReadLinks)
        return string.Format(eTransmitResources.UnableToReadLinks, (object) aFile.GetShortName());
      switch (aFile.TransmissionFailureType)
      {
        case TransmissionFailureType.NoFailure:
          return eTransmitResources.FailedWithNoErrorListed;
        case TransmissionFailureType.InsufficientDiskSpace:
          return eTransmitResources.InsufficientDiskSpace;
        case TransmissionFailureType.FileAlreadyExists:
          return string.Format(eTransmitResources.FileAlreadyExists, shortNamesOnly ? (object) aFile.GetShortName() : (object) aFile.GetLongName());
        case TransmissionFailureType.FileCouldNotBeCopied:
          return eTransmitResources.FileCouldNotBeCopied;
        case TransmissionFailureType.FilePredatesR2012:
          return string.Format(eTransmitResources.FilePredates2012, (object) aFile.GetShortName());
        case TransmissionFailureType.RevitServerError:
          return string.Format(eTransmitResources.RevitServerError, (object) aFile.GetShortName());
        case TransmissionFailureType.UnknownError:
          return eTransmitResources.UnknownError;
        case TransmissionFailureType.DirectoryNotWritable:
          return string.Format(eTransmitResources.DirectoryNotWritable, (object) outputDirectory);
        case TransmissionFailureType.FileNotFound:
          return eTransmitResources.FileNotFound;
        case TransmissionFailureType.UnableToRepathLinks:
          return string.Format(eTransmitResources.UnableToRepathLinks, (object) aFile.GetShortName());
        case TransmissionFailureType.RevitServerFileInUse:
          return eTransmitResources.RevitServerFileInUse;
        default:
          return eTransmitResources.FailedWithNoErrorListed;
      }
    }

    public void WriteReport(string outputDirectory, bool shortNamesOnly)
    {
      File.WriteAllLines(outputDirectory + "\\_TransmittalReport.txt", (IEnumerable<string>) this.GetReportText(outputDirectory, shortNamesOnly));
      foreach (string key in this.m_errors.Keys)
        this.WriteHTMLErrorReport(key, outputDirectory);
    }

    public void WriteHTMLErrorReport(string docName, string outputDirectory)
    {
      if (this.m_errors[docName].Count == 0)
        return;
      HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter) new StreamWriter(outputDirectory + "\\_" + eTransmitResources.ErrorReport + " - " + Path.GetFileNameWithoutExtension(docName) + ".html"));
      htmlTextWriter.WriteFullBeginTag("center");
      htmlTextWriter.WriteFullBeginTag("h1");
      htmlTextWriter.WriteLine(eTransmitResources.ErrorReport + " (" + (object) DateTime.Now + ")");
      htmlTextWriter.WriteBreak();
      htmlTextWriter.WriteLine(docName);
      htmlTextWriter.WriteEndTag("center");
      htmlTextWriter.WriteEndTag("h1");
      htmlTextWriter.WriteBreak();
      htmlTextWriter.WriteBeginTag("table");
      htmlTextWriter.WriteAttribute("border", "on");
      htmlTextWriter.Write('>');
      htmlTextWriter.Write("<th style=\"width:'40%'; vertical-align:top;\"> <center>");
      htmlTextWriter.Write(eTransmitResources.ErrorMessage);
      htmlTextWriter.Write("</center> </th>");
      htmlTextWriter.Write("<th style=\"width:'20%'; vertical-align:top;\"> <center>");
      htmlTextWriter.Write(eTransmitResources.ErrorResolution);
      htmlTextWriter.Write("</center> </th>");
      htmlTextWriter.Write("<th style=\"width:'40%'; vertical-align:top;\"> <center>");
      htmlTextWriter.Write(eTransmitResources.Elements);
      htmlTextWriter.Write("</center> </th>");
      foreach (FailureStorage failureStorage in this.m_errors[docName])
      {
        htmlTextWriter.WriteFullBeginTag("tr");
        htmlTextWriter.WriteFullBeginTag("td");
        htmlTextWriter.Write(failureStorage.Description);
        htmlTextWriter.WriteFullBeginTag("td");
        if (failureStorage.Severity != (Autodesk.Revit.DB.FailureSeverity)1)
          htmlTextWriter.Write(failureStorage.Resolution);
        else
          htmlTextWriter.Write("None");
        htmlTextWriter.WriteEndTag("td");
        htmlTextWriter.WriteFullBeginTag("td");
        foreach (string elem in failureStorage.Elems)
        {
          htmlTextWriter.WriteLine(elem);
          htmlTextWriter.WriteBreak();
        }
        htmlTextWriter.WriteEndTag("td");
      }
      htmlTextWriter.Flush();
      htmlTextWriter.Close();
    }

    public List<string> GetReportText(string outputDirectory, bool shortNamesOnly)
    {
      List<string> reportText = new List<string>();
      reportText.Add(string.Format(eTransmitResources.VersionInfo, (object) TransmissionOptions.ServiceVersion));
      reportText.Add(Environment.NewLine);
      reportText.Add(eTransmitResources.TransmittalReportList + Environment.NewLine);
      List<string> stringList = reportText;
      string createdBy = eTransmitResources.CreatedBy;
      DateTime now = DateTime.Now;
      string longDateString = now.ToLongDateString();
      now = DateTime.Now;
      string longTimeString = now.ToLongTimeString();
      string str1 = string.Format(createdBy, (object) longDateString, (object) longTimeString) + Environment.NewLine;
      stringList.Add(str1);
      reportText.Add(Environment.NewLine);
      reportText.Add(eTransmitResources.ModelList + Environment.NewLine);
      if (this.m_mainModels.Count<AReferencedFile>() == 1)
      {
        reportText.Add(string.Format(eTransmitResources.TransmittalBasedOn, (object) this.m_mainModels.First<AReferencedFile>().GetShortName()) + Environment.NewLine);
      }
      else
      {
        string directoryName = Path.GetDirectoryName(this.m_mainModels.First<AReferencedFile>().GetLongName());
        reportText.Add(string.Format(eTransmitResources.TransmittalBasedOnFolder, (object) directoryName) + Environment.NewLine);
      }
      reportText.Add(Environment.NewLine);
      if (this.m_includedFiles.Count > 0)
      {
        reportText.Add(eTransmitResources.FilesList + Environment.NewLine);
        foreach (AReferencedFile includedFile in this.m_includedFiles)
        {
          if (!includedFile.AdditionalFile)
            reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) includedFile.GetShortName()) + Environment.NewLine);
        }
        reportText.Add(Environment.NewLine);
        bool flag = false;
        foreach (AReferencedFile includedFile in this.m_includedFiles)
        {
          if (includedFile.AdditionalFile)
          {
            if (!flag)
            {
              reportText.Add(eTransmitResources.AdditionalFileReport + Environment.NewLine);
              flag = true;
            }
            reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) includedFile.GetShortName()) + Environment.NewLine);
          }
        }
        if (flag)
          reportText.Add(Environment.NewLine);
      }
      reportText.Add(eTransmitResources.RootModelList + Environment.NewLine);
      foreach (string str2 in this.m_mainModels.Select<AReferencedFile, string>((Func<AReferencedFile, string>) (x => x.GetShortName())))
        reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) str2) + Environment.NewLine);
      reportText.Add(Environment.NewLine);
      if (this.m_excludedFiles.Count > 0)
      {
        reportText.Add(eTransmitResources.ExcludedFilesList + Environment.NewLine);
        foreach (AReferencedFile excludedFile in this.m_excludedFiles)
          reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) excludedFile.GetShortName()) + Environment.NewLine);
        reportText.Add(Environment.NewLine);
      }
      if (this.m_errorFiles.Count > 0)
      {
        reportText.Add(eTransmitResources.ErrorFilesList + Environment.NewLine);
        foreach (AReferencedFile errorFile in this.m_errorFiles)
          reportText.Add(string.Format(eTransmitResources.IndentedFileNamePlusError, shortNamesOnly ? (object) errorFile.GetShortName() : (object) errorFile.GetLongName(), (object) eTransmitReport.GetStringForError(errorFile, outputDirectory, shortNamesOnly)) + Environment.NewLine);
        reportText.Add(Environment.NewLine);
      }
      if (this.m_noInspectForLinksFiles.Count > 0)
      {
        reportText.Add(eTransmitResources.CouldntCheckLinksList + Environment.NewLine);
        foreach (AReferencedFile inspectForLinksFile in this.m_noInspectForLinksFiles)
          reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) inspectForLinksFile.GetShortName()) + Environment.NewLine);
        reportText.Add(Environment.NewLine);
      }
      if (this.m_upgradeErrorFiles.Count > 0)
      {
        reportText.Add(eTransmitResources.UpgradeErrorsList + Environment.NewLine);
        foreach (AReferencedFile upgradeErrorFile in this.m_upgradeErrorFiles)
          reportText.Add(string.Format(eTransmitResources.IndentedFileName, (object) upgradeErrorFile.GetShortName()) + ": " + eTransmitReport.GetStringForUpgradeError(upgradeErrorFile.UpgradeFailureType) + Environment.NewLine);
        reportText.Add(Environment.NewLine);
      }
      return reportText;
    }
  }
}
