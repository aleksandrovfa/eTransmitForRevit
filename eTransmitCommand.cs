// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitCommand
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using eTransmitForRevit.Utils;
using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Form = System.Windows.Forms.Form;



namespace eTransmitForRevit
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    internal class eTransmitCommand : IExternalCommand
    {
        public static eTransmitSettingsDialog transmitSettingsDialog = null;
        public Result Execute(ExternalCommandData commandData,ref string message,ElementSet elements)
        {
            IDictionary<string, string> journalData = commandData.JournalData;
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            bool isRecording = true;
            if (journalData.Count > 0)
                isRecording = false;
            if (commandData.Application.ActiveUIDocument != null)
            {
                new TaskDialog(eTransmitResources.eTransmitString)
                {
                    TitleAutoPrefix = false,
                    MainContent = eTransmitResources.AllModelsMustBeClosed
                }.Show();
                return Result.Cancelled;
            }
            if (!isRecording)
            {
                string str;
                if (!journalData.TryGetValue("UserCanceled", out str))
                    return this.JournalFailure(ref message);
                if (str.CompareTo("yes") == 0)
                    return Result.Cancelled;
            }
            bool includeReport = true;
            TransmissionOptions.ServiceVersion = commandData.Application.Application.VersionNumber;
            TransmissionOptions options = this.ReadSettingsFile(out includeReport, commandData.Application.Application);
            bool flag1 = false;
            TransmissionOptions transmissionOptions = new TransmissionOptions();
            bool generateReport = false;
            string baseOutputDirectory = "";
            TransmissionGraph graph = new TransmissionGraph(new Autodesk.Revit.DB.FilePath(""), new TransmissionOptions());
            bool failCreateDirectory = false;
            bool failDiskSpace = false;
            bool openAndUpgrade = false;
            bool flag2 = true;
            List<string> stringList = new List<string>();
            UpgradeOptions upgradeSettings = new UpgradeOptions();
            TransmissionViews excludedViewsTypes = new TransmissionViews();
            string input = "";
            string str1 = "";
            transmitSettingsDialog = new eTransmitSettingsDialog(commandData, options, includeReport);
            bool flag3;
            if (isRecording)
            {
                if (transmitSettingsDialog.ShowDialog() != DialogResult.OK)
                {
                    journalData.Add("UserCanceled", "yes");
                    return Result.Cancelled;
                }
                journalData.Add("UserCanceled", "no");
                journalData.Add("RevitLinksIncluded", transmitSettingsDialog.GetRevitLinks() ? "yes" : "no");
                journalData.Add("CADLinksIncluded", transmitSettingsDialog.GetCADLinks() ? "yes" : "no");
                journalData.Add("DWFMarkupsIncluded", transmitSettingsDialog.GetDWFMarkups() ? "yes" : "no");
                journalData.Add("KeynotesIncluded", transmitSettingsDialog.GetKeynotes() ? "yes" : "no");
                journalData.Add("DecalsIncluded", transmitSettingsDialog.GetDecals() ? "yes" : "no");
                List<Autodesk.Revit.DB.ExternalFileReferenceType> typesToInclude = new List<Autodesk.Revit.DB.ExternalFileReferenceType>();
                if (transmitSettingsDialog.GetRevitLinks())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)1);
                if (transmitSettingsDialog.GetCADLinks())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)2);
                if (transmitSettingsDialog.GetDWFMarkups())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)3);
                if (transmitSettingsDialog.GetDecals())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)5);
                if (transmitSettingsDialog.GetKeynotes())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)4);
                string saveFolder = transmitSettingsDialog.GetSaveFolder();
                string inputName = transmitSettingsDialog.GetInputName();
                journalData.Add("MainModelName", inputName);
                journalData.Add("CentralServerName", str1);
                journalData.Add("MultipleOutputFolders", transmitSettingsDialog.MultipleOutputFolders ? "yes" : "no");
                journalData.Add("OpenAndUpgrade", transmitSettingsDialog.OpenAndUpgrade ? "yes" : "no");
                journalData.Add("PurgeUnused", transmitSettingsDialog.PurgeUnused ? "yes" : "no");
                journalData.Add("RemoveWorksets", transmitSettingsDialog.RemoveWorkSets ? "yes" : "No");
                journalData.Add("DeleteSheets", transmitSettingsDialog.DeleteSheets ? "yes" : "No");
                string str2 = transmitSettingsDialog.IncludeSheetsOptions != SheetOptions.IncludeAllViews ? (transmitSettingsDialog.IncludeSheetsOptions != SheetOptions.OnlyViewsOnSheets ? "Select types" : "Only views on sheets") : "Include all views";
                journalData.Add("IncludeSheetsOptions", str2);
                string str3 = transmitSettingsDialog.RemoveSheetsOptions != SheetOptions.IncludeAllViews ? "Select types" : "Include all views";
                journalData.Add("RemoveSheetsOptions", str3);
                journalData.Add("IncludeDatetime", options.IncludeDateTime ? "yes" : "no");
                foreach (string includedTypeName in options.CustomizedViewTypes.getIncludedTypeNames())
                    journalData.Add(includedTypeName, "yes");
                foreach (string excludedTypeName in options.CustomizedViewTypes.getExcludedTypeNames())
                    journalData.Add(excludedTypeName, "no");
                journalData.Add("AddFiles", string.Join("|", (IEnumerable<string>)transmitSettingsDialog.AdditionalFiles));
                upgradeSettings.RemoveWorksets = transmitSettingsDialog.RemoveWorkSets;
                upgradeSettings.PurgeUnused = transmitSettingsDialog.PurgeUnused;
                upgradeSettings.DeleteSheets = transmitSettingsDialog.DeleteSheets;
                upgradeSettings.IncludeSheetsOptions = transmitSettingsDialog.IncludeSheetsOptions;
                upgradeSettings.RemoveSheetsOptions = transmitSettingsDialog.RemoveSheetsOptions;
                transmissionOptions = new TransmissionOptions((IEnumerable<Autodesk.Revit.DB.ModelPath>)TransmitModelSelectorUtils.GetModelPathsFromFileOrFolder(inputName), saveFolder, (ICollection<Autodesk.Revit.DB.ExternalFileReferenceType>)typesToInclude, transmitSettingsDialog.OpenAndUpgrade, upgradeSettings, options.CustomizedViewTypes, transmitSettingsDialog.MultipleOutputFolders);
                bool saveSettings = transmitSettingsDialog.GetSaveSettings();
                generateReport = transmitSettingsDialog.GetAddReport();
                journalData.Add("SaveSettings", saveSettings ? "yes" : "no");
                journalData.Add("GenerateReport", generateReport ? "yes" : "no");
                journalData.Add("OutputDirectory", saveFolder);
                baseOutputDirectory = transmitSettingsDialog.GetOutputDirectoryWithTimestamp();
                graph = transmitSettingsDialog.GetTransmissionGraph();
                journalData.Add("TransmissionGraph", graph.ToString());
                failCreateDirectory = transmitSettingsDialog.GetFailCreateDirectory();
                failDiskSpace = transmitSettingsDialog.GetFailDiskSpace();
                flag3 = transmitSettingsDialog.Succeeded;
            }
            else
            {
                List<Autodesk.Revit.DB.ExternalFileReferenceType> typesToInclude = new List<Autodesk.Revit.DB.ExternalFileReferenceType>();
                string str4;
                if (!journalData.TryGetValue("RevitLinksIncluded", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    typesToInclude.Add(Autodesk.Revit.DB.ExternalFileReferenceType.RevitLink);
                if (!journalData.TryGetValue("CADLinksIncluded", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    typesToInclude.Add(Autodesk.Revit.DB.ExternalFileReferenceType.CADLink);
                if (!journalData.TryGetValue("DWFMarkupsIncluded", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    typesToInclude.Add(Autodesk.Revit.DB.ExternalFileReferenceType.DWFMarkup);
                if (!journalData.TryGetValue("DecalsIncluded", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    typesToInclude.Add(Autodesk.Revit.DB.ExternalFileReferenceType.Decal);
                if (!journalData.TryGetValue("KeynotesIncluded", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    typesToInclude.Add(Autodesk.Revit.DB.ExternalFileReferenceType.KeynoteTable);
                if (!journalData.TryGetValue("SaveSettings", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    flag1 = true;
                if (!journalData.TryGetValue("GenerateReport", out str4))
                    return this.JournalFailure(ref message);
                if (str4.CompareTo("yes") == 0)
                    generateReport = true;
                string str5 = "";
                bool multipleOutputDirectories = journalData.TryGetValue("MultipleOutputFolders", out str5) && str5.CompareTo("yes") == 0;
                if (!journalData.TryGetValue("MainModelName", out input) || !journalData.TryGetValue("CentralServerName", out str1))
                    return this.JournalFailure(ref message);
                string baseBaseOutputDirectory = "";
                if (!journalData.TryGetValue("OutputDirectory", out baseBaseOutputDirectory))
                    return this.JournalFailure(ref message);
                if (!journalData.TryGetValue("OpenAndUpgrade", out str4))
                    str4 = "no";
                if (str4.CompareTo("yes") == 0)
                    openAndUpgrade = true;
                if (!journalData.TryGetValue("RemoveWorksets", out str4))
                    str4 = "no";
                if (str4.CompareTo("yes") == 0)
                    upgradeSettings.RemoveWorksets = true;
                if (!journalData.TryGetValue("PurgeUnused", out str4))
                    str4 = "no";
                if (str4.CompareTo("yes") == 0)
                    upgradeSettings.PurgeUnused = true;
                if (!journalData.TryGetValue("DeleteSheets", out str4))
                    str4 = "no";
                if (str4.CompareTo("yes") == 0)
                    upgradeSettings.DeleteSheets = true;
                bool flag4 = false;
                if (!journalData.TryGetValue("CustomizeViews", out str4))
                    str4 = "no";
                else
                    flag4 = true;
                if (str4.CompareTo("yes") == 0)
                {
                    if (upgradeSettings.DeleteSheets)
                        upgradeSettings.RemoveSheetsOptions = SheetOptions.SelectTypes;
                    else
                        upgradeSettings.IncludeSheetsOptions = SheetOptions.SelectTypes;
                }
                if (!journalData.TryGetValue("DeleteViewsNotOnSheets", out str4))
                    str4 = "no";
                else
                    flag4 = true;
                if (str4.CompareTo("yes") == 0)
                    upgradeSettings.IncludeSheetsOptions = SheetOptions.OnlyViewsOnSheets;
                if (!flag4)
                {
                    if (!journalData.TryGetValue("IncludeSheetsOptions", out str4))
                        str4 = "Include all views";
                    upgradeSettings.IncludeSheetsOptions = str4.CompareTo("Include all views") != 0 ? (str4.CompareTo("Only views on sheets") != 0 ? SheetOptions.SelectTypes : SheetOptions.OnlyViewsOnSheets) : SheetOptions.IncludeAllViews;
                    if (!journalData.TryGetValue("RemoveSheetsOptions", out str4))
                        str4 = "Include all views";
                    upgradeSettings.RemoveSheetsOptions = str4.CompareTo("Include all views") != 0 ? SheetOptions.SelectTypes : SheetOptions.IncludeAllViews;
                }
                if (journalData.TryGetValue("IncludeDatetime", out str4) && str4.CompareTo("no") == 0)
                    flag2 = false;
                if (journalData.TryGetValue("AddFiles", out str4) && str4.CompareTo("") != 0)
                    stringList = new List<string>((IEnumerable<string>)str4.Split('|'));
                excludedViewsTypes.setTransmissionViewsByJournal(journalData);
                TransmissionOptions optionsNoTimestamp = new TransmissionOptions((IEnumerable<Autodesk.Revit.DB.ModelPath>)TransmitModelSelectorUtils.GetModelPathsFromFileOrFolder(input), baseBaseOutputDirectory, (ICollection<Autodesk.Revit.DB.ExternalFileReferenceType>)(ICollection<ExternalFileReferenceType>)typesToInclude, openAndUpgrade, upgradeSettings, excludedViewsTypes, multipleOutputDirectories);
                if (!flag2)
                    optionsNoTimestamp.IncludeDateTime = false;
                optionsNoTimestamp.AdditionalFiles = stringList;
                flag3 = eTransmitCommand.uiTransmitFiles(commandData.Application, isRecording, optionsNoTimestamp, out graph, out baseOutputDirectory, out failCreateDirectory, out failDiskSpace) == 0;
            }
            if (!isRecording)
                journalData.Remove("FailedWritePermission");
            if (failCreateDirectory)
            {
                journalData.Add("FailedWritePermission", "yes");
                return Result.Failed;
            }
            journalData.Add("FailedWritePermission", "no");
            if (!isRecording)
                journalData.Remove("FailedAvailableDiskSpace");
            if (failDiskSpace)
            {
                journalData.Add("FailedAvailableDiskSpace", "yes");
                return Result.Failed;
            }
            journalData.Add("FailedAvailableDiskSpace", "no");
            if (generateReport)
            {
                eTransmitReport eTransmitReport = new eTransmitReport(graph);
                try
                {
                    eTransmitReport.WriteReport(baseOutputDirectory, !isRecording);
                    File.WriteAllLines(baseOutputDirectory + "\\_TransmittalReport.txt", (IEnumerable<string>)eTransmitReport.GetReportText(baseOutputDirectory, !isRecording));
                }
                catch (Exception ex)
                {
                    commandData.Application.Application.WriteJournalComment("eTransmit - Unable to write the transmittal report to directory " + baseOutputDirectory + ". Extended message: " + ex.Message, true);
                }
            }
            if (isRecording)
            {
                journalData.Add("TransmissionSucceeded", flag3 ? "yes" : "no");
            }
            else
            {
                journalData.Remove("TransmissionSucceeded");
                journalData.Add("TransmissionSucceeded", flag3 ? "yes" : "no");
            }
            IEnumerable<string> strings = graph.GetMainModelNames().Select(x => AReferencedFile.GetShortName(x));
            if (flag3)
            {
                string allErrors = graph.GetAllErrors();
                if (isRecording)
                    journalData.Add("TransmissionErrors", allErrors);
                if (isRecording)
                {
                    
                    SuccessDialog successDialog = new SuccessDialog(strings, baseOutputDirectory, allErrors.CompareTo("") == 0, flag3, commandData.Application.Application);
                    ((Form)successDialog).ShowDialog();
                }
            }
            else if (isRecording)
            {
                if (graph.GetMainModels().Any<AReferencedFile>((Func<AReferencedFile, bool>)(x => x.TransmissionFailureType == TransmissionFailureType.DirectoryNotWritable)))
                {
                    int num1 = (int)MessageBox.Show(eTransmitReport.GetStringForError(graph.GetMainModels().First<AReferencedFile>((Func<AReferencedFile, bool>)(x => x.TransmissionFailureType == TransmissionFailureType.DirectoryNotWritable)), baseOutputDirectory, false));
                }
                else if (strings.Count<string>() == 1)
                {
                    int num2 = (int)MessageBox.Show(string.Format(eTransmitResources.UnableToTransmit, (object)strings.First<string>()));
                }
                else
                {
                    int num3 = (int)MessageBox.Show(string.Format(eTransmitResources.UnableToTransmitFiles, (object)strings.Count<string>()));
                }
            }
            return Result.Succeeded;
        }

        public static Result uiTransmitFiles(UIApplication uiApp,bool isRecording,TransmissionOptions optionsNoTimestamp, out TransmissionGraph graph,out string baseOutputDirectory, out bool failCreateDirectory,out bool failDiskSpace)
        {
            Application application = uiApp.Application;
            failCreateDirectory = false;
            failDiskSpace = false;
            baseOutputDirectory = optionsNoTimestamp.GetBaseOutputDirectory();
            //string baseInputDirectory = ;
            IEnumerable<Autodesk.Revit.DB.ModelPath> mainModelNames = optionsNoTimestamp.GetMainModelNames();
            string userVisiblePath1 = ModelPathUtils.ConvertModelPathToUserVisiblePath(mainModelNames.First());
            //if (mainModelNames.Count() == 1)
            if (mainModelNames.Count() == 1)
            {
                baseOutputDirectory = Path.Combine(baseOutputDirectory, Path.GetFileNameWithoutExtension(userVisiblePath1));
            }
            else
            {
                
                //string directoryName = Path.GetDirectoryName(Path.GetFullPath(userVisiblePath1));
                //if (!string.IsNullOrWhiteSpace(directoryName))
                //{
                //    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                //    baseOutputDirectory = Path.Combine(baseOutputDirectory, directoryInfo.Name);
                //}
                //else
                //    baseOutputDirectory = Path.Combine(baseOutputDirectory, Path.GetFileNameWithoutExtension(userVisiblePath1));

                string InputName = transmitSettingsDialog.GetInputName();
                string nameFolder = InputName.Split('/').Last();

                string[] nameFolder1 = InputName.Split('/');
                

                baseOutputDirectory = Path.Combine(baseOutputDirectory, nameFolder);
            }
            DateTime now = DateTime.Now;
            if (optionsNoTimestamp.IncludeDateTime)
                baseOutputDirectory = baseOutputDirectory + "_" + (object)now.Year + "-" + (object)now.Month + "-" + (object)now.Day + "_" + (object)now.Hour + "." + (object)now.Minute + "." + (object)now.Second;
            TransmissionOptions options = new TransmissionOptions(mainModelNames, baseOutputDirectory, optionsNoTimestamp.GetIncludedTypes(), optionsNoTimestamp.OpenAndUpgrade, new UpgradeOptions()
            {
                RemoveWorksets = optionsNoTimestamp.RemoveWorksets,
                PurgeUnused = optionsNoTimestamp.PurgeUnused,
                DeleteSheets = optionsNoTimestamp.DeleteSheets,
                IncludeSheetsOptions = optionsNoTimestamp.IncludeSheetsOptions,
                RemoveSheetsOptions = optionsNoTimestamp.RemoveSheetOptions
            }, optionsNoTimestamp.CustomizedViewTypes, optionsNoTimestamp.MultipleOutputDirectories);
            options.AdditionalFiles = optionsNoTimestamp.AdditionalFiles;
            if (isRecording)
            {
                foreach (Document document in application.Documents)
                {
                    if (document.IsModified)
                    {
                        string userVisiblePath2 = ModelPathUtils.ConvertModelPathToUserVisiblePath(mainModelNames.First());
                        TaskDialog taskDialog = new TaskDialog(eTransmitResources.eTransmitString);
                        taskDialog.TitleAutoPrefix = false;
                        taskDialog.MainInstruction = eTransmitResources.ModelHasUnsavedChanges;
                        taskDialog.MainContent = string.Format(eTransmitResources.UnsavedChangesDescription, (object)Path.GetFileName(userVisiblePath2));
                        taskDialog.AddCommandLink((TaskDialogCommandLinkId)1001, eTransmitResources.UnsavedChangesContinue);
                        taskDialog.AddCommandLink((TaskDialogCommandLinkId)1002, eTransmitResources.UnsavedChangesCancel);
                        TaskDialogResult taskDialogResult = taskDialog.Show();
                        if (taskDialogResult == Autodesk.Revit.UI.TaskDialogResult.CommandLink2 || taskDialogResult == Autodesk.Revit.UI.TaskDialogResult.Cancel)
                        {
                            graph = new TransmissionGraph(mainModelNames, new TransmissionOptions());
                            return Result.Cancelled;
                        }
                    }
                }
            }
            EventHandler<DialogBoxShowingEventArgs> eventHandler = eTransmit_DialogBoxShowing;
            uiApp.DialogBoxShowing += eventHandler;
            eTransmitCallbackHandler handler = new eTransmitCallbackHandler();
            InspectingModelDialog inspectingModelDialog = new InspectingModelDialog(options, handler, application);
            inspectingModelDialog.DisplayProgressWhileBuildingGraph(mainModelNames, isRecording);
            graph = inspectingModelDialog.GetTransmissionGraph();
            bool canceled1 = inspectingModelDialog.Canceled;
            if (isRecording & canceled1)
                return Result.Cancelled;
            try
            {
                Directory.CreateDirectory(baseOutputDirectory);
                if (options.MultipleOutputDirectories)
                {
                    foreach (string path in mainModelNames.Select(ModelPathUtils.ConvertModelPathToUserVisiblePath))
                        Directory.CreateDirectory(Path.Combine(baseOutputDirectory, Path.GetFileNameWithoutExtension(path)));
                }
            }
            catch (Exception ex)
            {
                if (isRecording)
                {
                    int num = (int)MessageBox.Show(string.Format(eTransmitResources.DirectoryNotWritable, (object)optionsNoTimestamp.GetBaseOutputDirectory()));
                }
                failCreateDirectory = true;
                return Result.Failed;
            }
            failCreateDirectory = false;
            try
            {
                if (new DriveInfo(Directory.GetDirectoryRoot(baseOutputDirectory)).AvailableFreeSpace < (options.MultipleOutputDirectories ? graph.GetTotalIncludedFileSizeWithMultiplicity() : graph.GetTotalIncludedFileSize()))
                {
                    if (isRecording)
                    {
                        int num = (int)MessageBox.Show(eTransmitResources.InsufficientDiskSpace);
                    }
                    failDiskSpace = true;
                    try
                    {
                        Directory.Delete(baseOutputDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        application.WriteJournalComment("eTransmit - Unable to delete the output directory after noticing that you don't have enough disk space to transmit everything. Extended message: " + ex.Message, true);
                    }
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                application.WriteJournalComment("eTransmit - Unable to check whether you have enough disk space. Extended message: " + ex.Message, true);
            }
            failDiskSpace = false;
            TransmittingFilesDialog transmittingFilesDialog = new TransmittingFilesDialog(graph, options, handler);
            transmittingFilesDialog.DisplayProgressWhileTransmittingFiles(uiApp, isRecording);
            graph = transmittingFilesDialog.GetTransmissionGraph();
            bool canceled2 = transmittingFilesDialog.Canceled;
            bool succeeded = transmittingFilesDialog.Succeeded;
            uiApp.DialogBoxShowing -= eventHandler;
            if (isRecording & canceled2)
            {
                try
                {
                    Directory.Delete(baseOutputDirectory, true);
                }
                catch (Exception ex)
                {
                    application.WriteJournalComment("eTransmit - Unable to delete transmission directory after canceling transmission. Extended message: " + ex.Message, true);
                }
                return Result.Cancelled;
            }
            return !succeeded ? Result.Failed : Result.Succeeded;
        }

        public static void eTransmit_DialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            if (!(((object)e).GetType() == typeof(TaskDialogShowingEventArgs)))
                return;
            TaskDialogShowingEventArgs showingEventArgs = (TaskDialogShowingEventArgs)e;
            if (((DialogBoxShowingEventArgs)showingEventArgs).DialogId == "TaskDialog_Notable_Changes_To_Room_Logic")
                ((DialogBoxShowingEventArgs)showingEventArgs).OverrideResult(8);
            else if (((DialogBoxShowingEventArgs)showingEventArgs).DialogId == "TaskDialog_Save_File_As_Central_Model")
                ((DialogBoxShowingEventArgs)showingEventArgs).OverrideResult(6);
            else if (((DialogBoxShowingEventArgs)showingEventArgs).DialogId == "TaskDialog_CSVMsg_FileMissing")
                ((DialogBoxShowingEventArgs)showingEventArgs).OverrideResult(1002);
            else if (((DialogBoxShowingEventArgs)showingEventArgs).DialogId == "TaskDialog_Detach_From_Central_Ignored")
            {
                ((DialogBoxShowingEventArgs)showingEventArgs).OverrideResult(8);
            }
            else
            {
                if (!(((DialogBoxShowingEventArgs)showingEventArgs).DialogId == ""))
                    return;
                ((DialogBoxShowingEventArgs)showingEventArgs).OverrideResult(8);
            }
        }

        public Result JournalFailure(ref string message)
        {
            message = eTransmitResources.UnableToReadJournal;
            return Result.Failed;
        }

        public TransmissionOptions ReadSettingsFile(  out bool includeReport, Autodesk.Revit.ApplicationServices.Application revitApp)
        {
            string input = "";
            string str1 = "";
            string baseBaseOutputDirectory = "";
            includeReport = true;
            bool openAndUpgrade = false;
            UpgradeOptions upgradeSettings = new UpgradeOptions();
            bool multipleOutputDirectories = false;
            HashSet<Autodesk.Revit.DB.ExternalFileReferenceType> hashSet = new HashSet<Autodesk.Revit.DB.ExternalFileReferenceType>();
            string path1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Autodesk\\eTransmit";
            TransmissionViews excludedViewsTypes = new TransmissionViews();
            bool includeDateTime = true;
            if (!Directory.Exists(path1))
                return new TransmissionOptions();
            string path2 = path1 + "\\eTransmit.ini";
            if (!File.Exists(path2))
                return new TransmissionOptions();
            try
            {
                foreach (string readAllLine in File.ReadAllLines(path2))
                {
                    if (!readAllLine.StartsWith("[") && !string.IsNullOrWhiteSpace(readAllLine))
                    {
                        if (readAllLine.StartsWith("Main Model Name:"))
                            input = readAllLine.Replace("Main Model Name:", "").Trim();
                        else if (readAllLine.StartsWith("Central Server Name:"))
                            str1 = readAllLine.Replace("Central Server Name:", "").Trim();
                        else if (readAllLine.StartsWith("Output Directory:"))
                            baseBaseOutputDirectory = readAllLine.Replace("Output Directory:", "").Trim();
                        else if (readAllLine.StartsWith("Multiple Output Folders:"))
                        {
                            if (readAllLine.Replace("Multiple Output Folders:", "").Trim().CompareTo("true") == 0)
                                multipleOutputDirectories = true;
                        }
                        else if (readAllLine.StartsWith("Include Revit Links:"))
                        {
                            if (readAllLine.Replace("Include Revit Links:", "").Trim().CompareTo("true") == 0)
                                hashSet.Add((Autodesk.Revit.DB.ExternalFileReferenceType) 1);
                        }
                        else if (readAllLine.StartsWith("Include CAD Links:"))
                        {
                            if (readAllLine.Replace("Include CAD Links:", "").Trim().CompareTo("true") == 0)
                                hashSet.Add((Autodesk.Revit.DB.ExternalFileReferenceType)2);
                        }
                        else if (readAllLine.StartsWith("Include DWF Markups:"))
                        {
                            if (readAllLine.Replace("Include DWF Markups:", "").Trim().CompareTo("true") == 0)
                                hashSet.Add((Autodesk.Revit.DB.ExternalFileReferenceType)3);
                        }
                        else if (readAllLine.StartsWith("Include Decals:"))
                        {
                            if (readAllLine.Replace("Include Decals:", "").Trim().CompareTo("true") == 0)
                                hashSet.Add((Autodesk.Revit.DB.ExternalFileReferenceType)5);
                        }
                        else if (readAllLine.StartsWith("Include the Keynote file:"))
                        {
                            if (readAllLine.Replace("Include the Keynote file:", "").Trim().CompareTo("true") == 0)
                                hashSet.Add((Autodesk.Revit.DB.ExternalFileReferenceType)4);
                        }
                        else if (readAllLine.StartsWith("Include transmittal report:"))
                        {
                            string str2 = readAllLine.Replace("Include transmittal report:", "").Trim();
                            includeReport = str2.CompareTo("true") == 0;
                        }
                        else if (readAllLine.StartsWith("Open and upgrade models:"))
                            openAndUpgrade = readAllLine.Replace("Open and upgrade models:", "").Trim().CompareTo("true") == 0;
                        else if (readAllLine.StartsWith("Remove worksets:"))
                        {
                            string str3 = readAllLine.Replace("Remove worksets:", "").Trim();
                            upgradeSettings.RemoveWorksets = str3.CompareTo("true") == 0;
                        }
                        else if (readAllLine.StartsWith("Purge unused:"))
                        {
                            string str4 = readAllLine.Replace("Purge unused:", "").Trim();
                            upgradeSettings.PurgeUnused = str4.CompareTo("true") == 0;
                        }
                        else if (readAllLine.StartsWith("Delete sheets:"))
                        {
                            string str5 = readAllLine.Replace("Delete sheets:", "").Trim();
                            upgradeSettings.DeleteSheets = str5.CompareTo("true") == 0;
                        }
                        else if (readAllLine.StartsWith("Customize views:"))
                        {
                            if (readAllLine.Replace("Customize views:", "").Trim().CompareTo("true") == 0)
                            {
                                upgradeSettings.IncludeSheetsOptions = SheetOptions.SelectTypes;
                                upgradeSettings.RemoveSheetsOptions = SheetOptions.SelectTypes;
                            }
                        }
                        else if (readAllLine.StartsWith("Delete views not on sheets:"))
                        {
                            if (readAllLine.Replace("Delete views not on sheets:", "").Trim().CompareTo("true") == 0)
                                upgradeSettings.IncludeSheetsOptions = SheetOptions.OnlyViewsOnSheets;
                        }
                        else if (readAllLine.StartsWith("Include sheets options:"))
                        {
                            string str6 = readAllLine.Replace("Include sheets options:", "").Trim();
                            upgradeSettings.IncludeSheetsOptions = str6.CompareTo("Include all views") != 0 ? (str6.CompareTo("Only views on sheets") != 0 ? SheetOptions.SelectTypes : SheetOptions.OnlyViewsOnSheets) : SheetOptions.IncludeAllViews;
                        }
                        else if (readAllLine.StartsWith("Remove sheets options:"))
                            upgradeSettings.RemoveSheetsOptions = readAllLine.Replace("Remove sheets options:", "").Trim().CompareTo("Include all views") != 0 ? SheetOptions.SelectTypes : SheetOptions.IncludeAllViews;
                        else if (readAllLine.StartsWith("Include Datetime:"))
                        {
                            includeDateTime = readAllLine.Replace("Include Datetime:", "").Trim().CompareTo("true") == 0;
                        }
                        else
                        {
                            string[] strArray = excludedViewsTypes.checkViewTypeName(readAllLine) ? readAllLine.Split(':') : throw new Exception(string.Format("Could not parse line \"{0}\"", (object)readAllLine));
                            excludedViewsTypes.setOneViewType(strArray[0], strArray[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(eTransmitResources.UnableToReadSettings);
                revitApp.WriteJournalComment("eTransmit - Unable to read the settings file. Extended message: " + ex.Message, true);
                return new TransmissionOptions();
            }
            IEnumerable<Autodesk.Revit.DB.ModelPath> modelPathsFromFileOrFolder = (IEnumerable<Autodesk.Revit.DB.ModelPath>)TransmitModelSelectorUtils.GetModelPathsFromFileOrFolder(input);
            return new TransmissionOptions(modelPathsFromFileOrFolder, baseBaseOutputDirectory, hashSet, openAndUpgrade, upgradeSettings, excludedViewsTypes, multipleOutputDirectories)
            {
                IncludeDateTime = includeDateTime
            };
        }

        public static void WriteSettingsFile( TransmissionOptions options,  bool includeReport,  Application revitApp)
        {
            string path1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Autodesk\\eTransmit";
            if (!Directory.Exists(path1))
            {
                try
                {
                    Directory.CreateDirectory(path1);
                }
                catch (Exception ex)
                {
                    int num = (int)MessageBox.Show(eTransmitResources.UnableToCreateSettingsDirectory);
                    revitApp.WriteJournalComment("eTransmit - Unable to create the directory for the settings file. Extended message: " + ex.Message, true);
                }
            }
            string path2 = path1 + "\\eTransmit.ini";
            try
            {
                List<string> contents = new List<string>();
                string userVisiblePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(options.GetMainModelNames().First());
                if (options.GetMainModelNames().Count() == 1)
                {
                    contents.Add("Main Model Name: " + userVisiblePath);
                    contents.Add("Central Server Name: " + options.GetMainModelNames().First().CentralServerPath);
                }
                else
                    contents.Add("Main Model Name: " + Path.GetDirectoryName(userVisiblePath));
                contents.Add("Multiple Output Folders: " + (options.MultipleOutputDirectories ? "true" : "false"));
                contents.Add("Output Directory: " + options.GetBaseOutputDirectory());
                contents.Add("Include Revit Links: " + (options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)1) ? "true" : "false"));
                contents.Add("Include CAD Links: " + (options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)2) ? "true" : "false"));
                contents.Add("Include DWF Markups: " + (options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)3) ? "true" : "false"));
                contents.Add("Include Decals: " + (options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)5) ? "true" : "false"));
                contents.Add("Include the Keynote file: " + (options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)4) ? "true" : "false"));
                contents.Add("Open and upgrade models: " + (options.OpenAndUpgrade ? "true" : "false"));
                contents.Add("Remove worksets: " + (options.RemoveWorksets ? "true" : "false"));
                contents.Add("Purge unused: " + (options.PurgeUnused ? "true" : "false"));
                contents.Add("Delete sheets: " + (options.DeleteSheets ? "true" : "false"));
                contents.Add("Include Datetime: " + (options.IncludeDateTime ? "true" : "false"));
                string str1 = options.IncludeSheetsOptions != SheetOptions.IncludeAllViews ? (options.IncludeSheetsOptions != SheetOptions.OnlyViewsOnSheets ? "Select types" : "Only views on sheets") : "Include all views";
                contents.Add("Include sheets options: " + str1);
                string str2 = options.RemoveSheetOptions != SheetOptions.IncludeAllViews ? "Select types" : "Include all views";
                contents.Add("Remove sheets options: " + str2);
                foreach (string viewTypeName in options.CustomizedViewTypes.getViewTypeNameList())
                    contents.Add(viewTypeName);
                contents.Add("Include transmittal report: " + (includeReport ? "true" : "false") + "\n");
                File.WriteAllLines(path2, (IEnumerable<string>)contents);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(eTransmitResources.UnableToCreateSettings);
                revitApp.WriteJournalComment("eTransmit - Unable to write the settings file. Extended message: " + ex.Message, true);
            }
        }
    }
}
