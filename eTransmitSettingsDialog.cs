// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.eTransmitSettingsDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using eTransmitForRevit.Utils;
using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using Control = System.Windows.Forms.Control;
using Form = System.Windows.Forms.Form;
using Panel = System.Windows.Forms.Panel;
using Point = System.Drawing.Point;
using TextBox = System.Windows.Forms.TextBox;

namespace eTransmitForRevit
{
    public class eTransmitSettingsDialog : Form
    {
        public OpenServerFileDialog serverFileDialog;
        private string m_centralServerName;
        private ExternalCommandData m_commandData;
        private string m_outputDirectoryWithTimestamp;
        private TransmissionGraph m_graph;
        private bool m_failCreateDirectory;
        private bool m_failDiskSpace;
        private bool m_succeeded;
        private List<string> m_additionalFiles = new List<string>();
        private TransmissionViews m_transmissionView;
        private bool m_includeDatetime;
        private AdditionalFilesDialog aDialog;
        private SelectViewTypes cDialog;
        private string m_lastUsedDirectoryPath;
        private IContainer components;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private CheckBox saveSettingsCheckbox;
        private Button inspectModelButton;
        private Button cancelButton;
        private CheckBox addReportCheckbox;
        private TextBox fileSaveTextBox;
        private TextBox fileOpenTextBox;
        private Label label3;
        private Button fileSaveButton;
        private Button fileOpenButton;
        private Label label2;
        private CheckBox keynotesCheckbox;
        private CheckBox decalCheckbox;
        private CheckBox DWFMarkupsCheckbox;
        private CheckBox CADLinksCheckbox;
        private CheckBox revitLinksCheckbox;
        private Button serverFileOpenButton;
        private GroupBox groupBox3;
        private CheckBox purgeCheckbox;
        private CheckBox workSetCheckbox;
        private Button addFileButton;
        private Button removeSelectTypesButton;
        private RadioButton removeIncludeAllRadio;
        private RadioButton removeSelectRadio;
        private RadioButton includeSelectRadio;
        private RadioButton includeOnlyOnSheetsRadio;
        private RadioButton includeIncludeAllRadio;
        private RadioButton removeSheetsRadio;
        private RadioButton includeSheetsRadio;
        private Panel panel3;
        private Panel panel2;
        private Panel panel1;
        private Button includeSelectTypesButton;
        private Label label1;
        private LinkLabel HelpLinkLabel;
        private CheckBox upgradeCheckbox;
        private Button folderOpenButton;
        private CheckBox MultipleOutputFoldersCheckbox;

        public eTransmitSettingsDialog(
          ExternalCommandData commandData,
          TransmissionOptions options,
          bool includeReport)
        {
            this.InitializeComponent();
            this.Text = eTransmitResources.eTransmitString;
            this.groupBox1.Text = eTransmitResources.TransmitModelTitle;
            this.MultipleOutputFoldersCheckbox.Text = eTransmitResources.MultipleOutputFolders;
            this.fileOpenButton.Text = eTransmitResources.Browse;
            this.serverFileOpenButton.Text = eTransmitResources.BrowseRevitServer;
            this.label1.Text = eTransmitResources.IncludeFileTypes;
            this.label2.Text = eTransmitResources.ChooseModelToTransmit;
            this.fileSaveButton.Text = eTransmitResources.BrowseFolders;
            this.folderOpenButton.Text = eTransmitResources.BrowseFolders;
            this.addReportCheckbox.Text = eTransmitResources.AddTransmittalReport;
            this.groupBox2.Text = eTransmitResources.AddFilesTitle;
            this.label3.Text = eTransmitResources.SaveTransmittedModelTo;
            this.revitLinksCheckbox.Text = eTransmitResources.LinkedRevitModels;
            this.CADLinksCheckbox.Text = eTransmitResources.CADLinks;
            this.DWFMarkupsCheckbox.Text = eTransmitResources.DWFMarkups;
            this.decalCheckbox.Text = eTransmitResources.Decals;
            this.keynotesCheckbox.Text = eTransmitResources.Keynotes;
            this.saveSettingsCheckbox.Text = eTransmitResources.SaveSettings;
            this.inspectModelButton.Text = eTransmitResources.TransmitModel;
            this.cancelButton.Text = eTransmitResources.Cancel;
            this.workSetCheckbox.Text = eTransmitResources.RemovingAllWorksets;
            this.purgeCheckbox.Text = eTransmitResources.PurgeAllUnused;
            this.addFileButton.Text = eTransmitResources.AddOtherFiles;
            this.includeSelectTypesButton.Text = eTransmitResources.Select;
            this.removeSelectTypesButton.Text = eTransmitResources.Select;
            this.groupBox3.Text = eTransmitResources.UpgradeAndCleanupTitle;
            this.includeSheetsRadio.Text = eTransmitResources.IncludeAllSheetsAnd;
            this.removeSheetsRadio.Text = eTransmitResources.RemoveAllSheetsBut;
            this.includeIncludeAllRadio.Text = eTransmitResources.AllViews;
            this.includeOnlyOnSheetsRadio.Text = eTransmitResources.OnlyIncludeViewsOnSheets;
            this.includeSelectRadio.Text = eTransmitResources.ViewsOnSheetsAnd;
            this.removeIncludeAllRadio.Text = eTransmitResources.AllViews;
            this.removeSelectRadio.Text = eTransmitResources.SelectedViewTypes;
            this.upgradeCheckbox.Text = string.Format(eTransmitResources.Cleanup, (object)TransmissionOptions.ServiceVersion);
            this.HelpLinkLabel.Text = eTransmitResources.HowDoIUseETransmit;
            this.m_commandData = commandData;
            this.m_transmissionView = options.CustomizedViewTypes;
            this.m_includeDatetime = options.IncludeDateTime;
            this.m_outputDirectoryWithTimestamp = "";
            this.m_graph = new TransmissionGraph(new Autodesk.Revit.DB.FilePath(""), new TransmissionOptions());
            this.m_failCreateDirectory = false;
            this.m_failDiskSpace = false;
            this.m_succeeded = false;
            this.m_centralServerName = "";
            try
            {
                this.serverFileOpenButton.Enabled = this.m_commandData.Application.Application.GetRevitServerNetworkHosts().Count > 0;
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException ex)
            {
                this.serverFileOpenButton.Enabled = false;
            }
            catch (Exception ex)
            {
                this.serverFileOpenButton.Enabled = false;
                commandData.Application.Application.WriteJournalComment(" eTransmit - Unable to get the server name while opening the settings dialog. Extended message: " + ex.Message, true);
            }
            if (!this.serverFileOpenButton.Enabled)
                this.serverFileOpenButton.Text = eTransmitResources.RevitServerNotConnected;
            this.addReportCheckbox.Checked = includeReport;
            this.MultipleOutputFoldersCheckbox.Checked = options.MultipleOutputDirectories;
            if (options.OpenAndUpgrade)
            {
                this.upgradeCheckbox.Checked = true;
                this.purgeCheckbox.Enabled = true;
                this.workSetCheckbox.Enabled = true;
                this.includeSheetsRadio.Enabled = true;
                this.removeSheetsRadio.Enabled = true;
                if (!options.DeleteSheets)
                {
                    this.includeIncludeAllRadio.Enabled = true;
                    this.includeOnlyOnSheetsRadio.Enabled = true;
                    this.includeSelectRadio.Enabled = true;
                    this.removeIncludeAllRadio.Enabled = false;
                    this.removeSelectRadio.Enabled = false;
                }
                else
                {
                    this.removeIncludeAllRadio.Enabled = true;
                    this.removeSelectRadio.Enabled = true;
                    this.includeIncludeAllRadio.Enabled = false;
                    this.includeOnlyOnSheetsRadio.Enabled = false;
                    this.includeSelectRadio.Enabled = false;
                }
            }
            if (options.PurgeUnused)
                this.purgeCheckbox.Checked = true;
            if (options.RemoveWorksets)
                this.workSetCheckbox.Checked = true;
            if (options.DeleteSheets)
                this.removeSheetsRadio.Checked = true;
            else
                this.includeSheetsRadio.Checked = true;
            if (options.IncludeSheetsOptions == SheetOptions.IncludeAllViews)
                this.includeIncludeAllRadio.Checked = true;
            else if (options.IncludeSheetsOptions == SheetOptions.OnlyViewsOnSheets)
            {
                this.includeOnlyOnSheetsRadio.Checked = true;
            }
            else
            {
                this.includeSelectRadio.Checked = true;
                if (this.includeSelectRadio.Enabled)
                    this.includeSelectTypesButton.Enabled = true;
            }
            if (options.RemoveSheetOptions == SheetOptions.IncludeAllViews)
            {
                this.removeIncludeAllRadio.Checked = true;
            }
            else
            {
                this.removeSelectRadio.Checked = true;
                if (this.removeSelectRadio.Enabled)
                    this.removeSelectTypesButton.Enabled = true;
            }
            this.revitLinksCheckbox.Checked = options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)1);
            this.CADLinksCheckbox.Checked = options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)2);
            this.DWFMarkupsCheckbox.Checked = options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)3);
            this.decalCheckbox.Checked = options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)5);
            this.keynotesCheckbox.Checked = options.GetIncludedTypes().Contains((Autodesk.Revit.DB.ExternalFileReferenceType)4);
            IEnumerable<Autodesk.Revit.DB.ModelPath> mainModelNames = options.GetMainModelNames();
            Autodesk.Revit.DB.ModelPath modelPath = mainModelNames.First();
            string userVisiblePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
            if (mainModelNames.Count() == 1)
            {
                this.fileOpenTextBox.Text = userVisiblePath;
                this.m_centralServerName = modelPath.ServerPath ? modelPath.CentralServerPath : "";
            }
            else
            {
                this.fileOpenTextBox.Text = Path.GetDirectoryName(userVisiblePath);
                this.m_centralServerName = "";
            }
            this.fileSaveTextBox.Text = options.GetBaseOutputDirectory();
            new ToolTip().SetToolTip((Control)this.decalCheckbox, eTransmitResources.SelectDecal + Environment.NewLine + eTransmitResources.IfNoDecal);
            new ToolTip().SetToolTip((Control)this.keynotesCheckbox, eTransmitResources.SelectKeynote + Environment.NewLine + eTransmitResources.IfNoKeynote);
            new ToolTip().SetToolTip((Control)this.revitLinksCheckbox, eTransmitResources.SelectRevitLink + Environment.NewLine + eTransmitResources.IfNoRevitLink);
            new ToolTip().SetToolTip((Control)this.CADLinksCheckbox, eTransmitResources.SelectCAD + Environment.NewLine + eTransmitResources.IfNoCAD + Environment.NewLine + Environment.NewLine + eTransmitResources.AdditionalCADNotes);
            new ToolTip().SetToolTip((Control)this.DWFMarkupsCheckbox, eTransmitResources.SelectDWF + Environment.NewLine + eTransmitResources.IfNoDWF);
            this.HelpRequested += new HelpEventHandler(this.textBox_HelpRequested);
            this.includeSheetsRadio.CheckedChanged += new EventHandler(this.includeSheetsRadio_CheckedChanged);
            this.includeSelectRadio.CheckedChanged += new EventHandler(this.includeSelectRadio_CheckedChanged);
            this.removeSelectRadio.CheckedChanged += new EventHandler(this.removeSelectRadio_CheckedChanged);
            this.aDialog = new AdditionalFilesDialog(this.m_additionalFiles);
            this.cDialog = new SelectViewTypes(this.m_transmissionView);
            if (this.fileOpenTextBox.Text.CompareTo("") != 0 && Directory.Exists(Path.GetDirectoryName(this.fileOpenTextBox.Text)))
                this.m_lastUsedDirectoryPath = Path.GetDirectoryName(this.fileOpenTextBox.Text);
            else
                this.m_lastUsedDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        private void openRadio_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void includeSheetsRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.includeSheetsRadio.Checked)
            {
                this.includeIncludeAllRadio.Enabled = true;
                this.includeOnlyOnSheetsRadio.Enabled = true;
                this.includeSelectRadio.Enabled = true;
                if (this.includeSelectRadio.Checked)
                    this.includeSelectTypesButton.Enabled = true;
                this.removeIncludeAllRadio.Enabled = false;
                this.removeSelectRadio.Enabled = false;
                this.removeSelectTypesButton.Enabled = false;
            }
            else
            {
                this.includeIncludeAllRadio.Enabled = false;
                this.includeOnlyOnSheetsRadio.Enabled = false;
                this.includeSelectRadio.Enabled = false;
                this.includeSelectTypesButton.Enabled = false;
                this.removeIncludeAllRadio.Enabled = true;
                this.removeSelectRadio.Enabled = true;
                if (!this.removeSelectRadio.Checked)
                    return;
                this.removeSelectTypesButton.Enabled = true;
            }
        }

        private void includeSelectRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.includeSelectRadio.Enabled && this.includeSelectRadio.Checked)
                this.includeSelectTypesButton.Enabled = true;
            else
                this.includeSelectTypesButton.Enabled = false;
        }

        private void removeSelectRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.removeSelectRadio.Enabled && this.removeSelectRadio.Checked)
                this.removeSelectTypesButton.Enabled = true;
            else
                this.removeSelectTypesButton.Enabled = false;
        }

        private void textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            eTransmitHelpLauncher.LaunchHelp();
            hlpevent.Handled = true;
        }

        public string GetOutputDirectoryWithTimestamp() => this.m_outputDirectoryWithTimestamp;

        public TransmissionGraph GetTransmissionGraph() => this.m_graph;

        public bool GetFailCreateDirectory() => this.m_failCreateDirectory;

        public bool GetFailDiskSpace() => this.m_failDiskSpace;

        public bool Succeeded => this.m_succeeded;

        public bool MultipleOutputFolders => this.MultipleOutputFoldersCheckbox.Checked;

        public bool OpenAndUpgrade => this.upgradeCheckbox.Checked;

        public bool PurgeUnused => this.purgeCheckbox.Checked;

        public bool RemoveWorkSets => this.workSetCheckbox.Checked;

        public bool DeleteSheets => this.removeSheetsRadio.Checked;

        public SheetOptions IncludeSheetsOptions
        {
            get
            {
                if (this.includeIncludeAllRadio.Checked)
                    return SheetOptions.IncludeAllViews;
                return this.includeOnlyOnSheetsRadio.Checked ? SheetOptions.OnlyViewsOnSheets : SheetOptions.SelectTypes;
            }
        }

        public SheetOptions RemoveSheetsOptions => this.removeIncludeAllRadio.Checked ? SheetOptions.IncludeAllViews : SheetOptions.SelectTypes;

        public SheetOptions SheetOptions => this.removeSheetsRadio.Checked ? this.RemoveSheetsOptions : this.IncludeSheetsOptions;

        public bool GetRevitLinks() => this.revitLinksCheckbox.Checked;

        public bool GetCADLinks() => this.CADLinksCheckbox.Checked;

        public bool GetDWFMarkups() => this.DWFMarkupsCheckbox.Checked;

        public bool GetDecals() => this.decalCheckbox.Checked;

        public bool GetKeynotes() => this.keynotesCheckbox.Checked;

        public string GetInputName() => this.fileOpenTextBox.Text;

        public string GetSaveFolder() => this.fileSaveTextBox.Text;

        public bool GetSaveSettings() => this.saveSettingsCheckbox.Checked;

        public bool GetAddReport() => this.addReportCheckbox.Checked;

        public List<string> AdditionalFiles
        {
            get => this.m_additionalFiles;
            set => this.m_additionalFiles = value;
        }

        private void fileSaveButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (Directory.Exists(this.GetSaveFolder()))
                folderBrowserDialog.SelectedPath = this.GetSaveFolder();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            this.fileSaveTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = eTransmitResources.RevitFiles + "|*.rvt";
            openFileDialog.InitialDirectory = this.m_lastUsedDirectoryPath;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            this.fileOpenTextBox.Text = openFileDialog.FileName;
            this.m_lastUsedDirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private void folderOpenButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            try
            {
                string directoryName1 = Path.GetDirectoryName(this.GetInputName());
                if (Directory.Exists(directoryName1))
                    folderBrowserDialog.SelectedPath = directoryName1;
                else if (File.Exists(directoryName1))
                {
                    string directoryName2 = Path.GetDirectoryName(directoryName1);
                    folderBrowserDialog.SelectedPath = directoryName2;
                }
            }
            catch (System.ArgumentException ex)
            {
            }
            catch (PathTooLongException ex)
            {
            }
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            this.fileOpenTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void inspectModelButton_Click(object sender, EventArgs e)
        {
            IEnumerable<Autodesk.Revit.DB.ModelPath> fromFileOrFolder = (IEnumerable<Autodesk.Revit.DB.ModelPath>)TransmitModelSelectorUtils.GetModelPathsFromFileOrFolder(this.GetInputName(),GetServerList());
            if (!AReferencedFile.StringIsServerFile(this.GetInputName()) && (fromFileOrFolder == null || !fromFileOrFolder.Any()))
            {
                int num1 = (int)MessageBox.Show(eTransmitResources.PleaseSelectAValidModel);
            }
            else if (!Directory.Exists(this.GetSaveFolder()))
            {
                int num2 = (int)MessageBox.Show(eTransmitResources.PleaseSelectADirectory);
            }
            else
            {
                List<Autodesk.Revit.DB.ExternalFileReferenceType> typesToInclude = new List<Autodesk.Revit.DB.ExternalFileReferenceType>();
                if (this.GetRevitLinks())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)1);
                if (this.GetCADLinks())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)2);
                if (this.GetDWFMarkups())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)3);
                if (this.GetDecals())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)5);
                if (this.GetKeynotes())
                    typesToInclude.Add((Autodesk.Revit.DB.ExternalFileReferenceType)4);
                string saveFolder = this.GetSaveFolder();
                TransmissionOptions transmissionOptions = new TransmissionOptions(fromFileOrFolder, saveFolder, (ICollection<Autodesk.Revit.DB.ExternalFileReferenceType>)typesToInclude, this.OpenAndUpgrade, new UpgradeOptions()
                {
                    RemoveWorksets = this.RemoveWorkSets,
                    PurgeUnused = this.PurgeUnused,
                    DeleteSheets = this.DeleteSheets,
                    IncludeSheetsOptions = this.IncludeSheetsOptions,
                    RemoveSheetsOptions = this.RemoveSheetsOptions
                }, this.m_transmissionView, this.MultipleOutputFolders);
                if (!this.m_includeDatetime)
                    transmissionOptions.IncludeDateTime = false;
                if (this.GetSaveSettings())
                    eTransmitCommand.WriteSettingsFile(transmissionOptions, this.GetAddReport(), this.m_commandData.Application.Application);
                transmissionOptions.AdditionalFiles = this.m_additionalFiles;
                Result result = eTransmitCommand.uiTransmitFiles(this.m_commandData.Application, true, transmissionOptions, out this.m_graph, out this.m_outputDirectoryWithTimestamp, out this.m_failCreateDirectory, out this.m_failDiskSpace
                    );
                if (result == (Result)1)
                    return;
                this.m_succeeded = result == 0;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }


        private void fileOpenTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void serverFileOpenButton_Click(object sender, EventArgs e)
        {
            serverFileDialog = new OpenServerFileDialog(this.m_commandData);
            if (serverFileDialog.ShowDialog() != DialogResult.OK)
                return;
            this.fileOpenTextBox.Text = serverFileDialog.GetFileName();
            this.m_centralServerName = serverFileDialog.GetCentralServerName();
        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            int num = (int)this.aDialog.ShowDialog();
            this.aDialog.StartPosition = FormStartPosition.Manual;
        }

        private void customizeViewButton_Click(object sender, EventArgs e) => this.showSelectTypesDialog();

        private void includeSelectTypesButton_Click(object sender, EventArgs e) => this.showSelectTypesDialog();

        private void showSelectTypesDialog()
        {
            int num = (int)this.cDialog.ShowDialog();
            this.cDialog.StartPosition = FormStartPosition.Manual;
        }

        private void OnHelpLinkClicked(object sender, LinkLabelLinkClickedEventArgs eventArgs) => eTransmitHelpLauncher.LaunchHelp();

        private void upgradeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.upgradeCheckbox.Checked)
            {
                this.purgeCheckbox.Enabled = true;
                this.workSetCheckbox.Enabled = true;
                this.includeSheetsRadio.Enabled = true;
                this.removeSheetsRadio.Enabled = true;
                if (this.includeSheetsRadio.Checked)
                {
                    this.includeIncludeAllRadio.Enabled = true;
                    this.includeOnlyOnSheetsRadio.Enabled = true;
                    this.includeSelectRadio.Enabled = true;
                    if (!this.includeSelectRadio.Checked)
                        return;
                    this.includeSelectTypesButton.Enabled = true;
                }
                else
                {
                    this.removeIncludeAllRadio.Enabled = true;
                    this.removeSelectRadio.Enabled = true;
                    if (!this.removeSelectRadio.Checked)
                        return;
                    this.removeSelectTypesButton.Enabled = true;
                }
            }
            else
            {
                this.purgeCheckbox.Enabled = false;
                this.workSetCheckbox.Enabled = false;
                this.includeSelectTypesButton.Enabled = false;
                this.removeSelectTypesButton.Enabled = false;
                this.includeSheetsRadio.Enabled = false;
                this.removeSheetsRadio.Enabled = false;
                this.includeIncludeAllRadio.Enabled = false;
                this.includeOnlyOnSheetsRadio.Enabled = false;
                this.includeSelectRadio.Enabled = false;
                this.removeIncludeAllRadio.Enabled = false;
                this.removeSelectRadio.Enabled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(eTransmitSettingsDialog));
            this.groupBox1 = new GroupBox();
            this.MultipleOutputFoldersCheckbox = new CheckBox();
            this.folderOpenButton = new Button();
            this.serverFileOpenButton = new Button();
            this.addReportCheckbox = new CheckBox();
            this.fileSaveTextBox = new TextBox();
            this.fileOpenTextBox = new TextBox();
            this.label3 = new Label();
            this.fileSaveButton = new Button();
            this.fileOpenButton = new Button();
            this.label2 = new Label();
            this.addFileButton = new Button();
            this.groupBox2 = new GroupBox();
            this.label1 = new Label();
            this.keynotesCheckbox = new CheckBox();
            this.decalCheckbox = new CheckBox();
            this.DWFMarkupsCheckbox = new CheckBox();
            this.CADLinksCheckbox = new CheckBox();
            this.revitLinksCheckbox = new CheckBox();
            this.saveSettingsCheckbox = new CheckBox();
            this.inspectModelButton = new Button();
            this.cancelButton = new Button();
            this.groupBox3 = new GroupBox();
            this.upgradeCheckbox = new CheckBox();
            this.panel3 = new Panel();
            this.removeSheetsRadio = new RadioButton();
            this.panel2 = new Panel();
            this.removeIncludeAllRadio = new RadioButton();
            this.removeSelectTypesButton = new Button();
            this.removeSelectRadio = new RadioButton();
            this.includeSheetsRadio = new RadioButton();
            this.panel1 = new Panel();
            this.includeSelectTypesButton = new Button();
            this.includeIncludeAllRadio = new RadioButton();
            this.includeOnlyOnSheetsRadio = new RadioButton();
            this.includeSelectRadio = new RadioButton();
            this.workSetCheckbox = new CheckBox();
            this.purgeCheckbox = new CheckBox();
            this.HelpLinkLabel = new LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            this.groupBox1.Controls.Add((Control)this.MultipleOutputFoldersCheckbox);
            this.groupBox1.Controls.Add((Control)this.folderOpenButton);
            this.groupBox1.Controls.Add((Control)this.serverFileOpenButton);
            this.groupBox1.Controls.Add((Control)this.addReportCheckbox);
            this.groupBox1.Controls.Add((Control)this.fileSaveTextBox);
            this.groupBox1.Controls.Add((Control)this.fileOpenTextBox);
            this.groupBox1.Controls.Add((Control)this.label3);
            this.groupBox1.Controls.Add((Control)this.fileSaveButton);
            this.groupBox1.Controls.Add((Control)this.fileOpenButton);
            this.groupBox1.Controls.Add((Control)this.label2);
            this.groupBox1.Location = new Point(18, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(576, 265);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.MultipleOutputFoldersCheckbox.Location = new Point(9, 214);
            this.MultipleOutputFoldersCheckbox.MaximumSize = new Size(5000, 500);
            this.MultipleOutputFoldersCheckbox.Name = "MultipleOutputFoldersCheckbox";
            this.MultipleOutputFoldersCheckbox.Size = new Size(278, 39);
            this.MultipleOutputFoldersCheckbox.TabIndex = 8;
            this.MultipleOutputFoldersCheckbox.UseVisualStyleBackColor = true;
            this.folderOpenButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.folderOpenButton.Location = new Point(234, 63);
            this.folderOpenButton.Name = "folderOpenButton";
            this.folderOpenButton.Size = new Size(106, 30);
            this.folderOpenButton.TabIndex = 3;
            this.folderOpenButton.UseVisualStyleBackColor = true;
            this.folderOpenButton.Click += new EventHandler(this.folderOpenButton_Click);
            this.serverFileOpenButton.Location = new Point(346, 63);
            this.serverFileOpenButton.Name = "serverFileOpenButton";
            this.serverFileOpenButton.Size = new Size(221, 30);
            this.serverFileOpenButton.TabIndex = 4;
            this.serverFileOpenButton.UseVisualStyleBackColor = true;
            this.serverFileOpenButton.Click += new EventHandler(this.serverFileOpenButton_Click);
            this.addReportCheckbox.Checked = true;
            this.addReportCheckbox.CheckState = CheckState.Checked;
            this.addReportCheckbox.Location = new Point(9, 169);
            this.addReportCheckbox.MaximumSize = new Size(5000, 500);
            this.addReportCheckbox.Name = "addReportCheckbox";
            this.addReportCheckbox.Size = new Size(278, 39);
            this.addReportCheckbox.TabIndex = 7;
            this.addReportCheckbox.UseVisualStyleBackColor = true;
            this.fileSaveTextBox.Location = new Point(9, 113);
            this.fileSaveTextBox.Name = "fileSaveTextBox";
            this.fileSaveTextBox.Size = new Size(558, 20);
            this.fileSaveTextBox.TabIndex = 5;
            this.fileOpenTextBox.Location = new Point(9, 37);
            this.fileOpenTextBox.Name = "fileOpenTextBox";
            this.fileOpenTextBox.Size = new Size(558, 20);
            this.fileOpenTextBox.TabIndex = 1;
            this.fileOpenTextBox.TextChanged += new EventHandler(this.fileOpenTextBox_TextChanged);
            this.label3.Location = new Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new Size(561, 47);
            this.label3.TabIndex = 5;
            this.fileSaveButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.fileSaveButton.Location = new Point(366, 146);
            this.fileSaveButton.Name = "fileSaveButton";
            this.fileSaveButton.Size = new Size(201, 30);
            this.fileSaveButton.TabIndex = 6;
            this.fileSaveButton.UseVisualStyleBackColor = true;
            this.fileSaveButton.Click += new EventHandler(this.fileSaveButton_Click);
            this.fileOpenButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.fileOpenButton.Location = new Point(122, 63);
            this.fileOpenButton.Name = "fileOpenButton";
            this.fileOpenButton.Size = new Size(106, 30);
            this.fileOpenButton.TabIndex = 2;
            this.fileOpenButton.UseVisualStyleBackColor = true;
            this.fileOpenButton.Click += new EventHandler(this.fileOpenButton_Click);
            this.label2.Location = new Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new Size(564, 33);
            this.label2.TabIndex = 1;
            this.addFileButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.addFileButton.Location = new Point(369, 86);
            this.addFileButton.Name = "addFileButton";
            this.addFileButton.Size = new Size(201, 30);
            this.addFileButton.TabIndex = 9;
            this.addFileButton.UseVisualStyleBackColor = true;
            this.addFileButton.Click += new EventHandler(this.addFileButton_Click);
            this.groupBox2.Controls.Add((Control)this.label1);
            this.groupBox2.Controls.Add((Control)this.keynotesCheckbox);
            this.groupBox2.Controls.Add((Control)this.addFileButton);
            this.groupBox2.Controls.Add((Control)this.decalCheckbox);
            this.groupBox2.Controls.Add((Control)this.DWFMarkupsCheckbox);
            this.groupBox2.Controls.Add((Control)this.CADLinksCheckbox);
            this.groupBox2.Controls.Add((Control)this.revitLinksCheckbox);
            this.groupBox2.Location = new Point(18, 283);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(576, 122);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(30, 23);
            this.label1.Name = "label1";
            this.label1.Size = new Size(124, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Include related file types:";
            this.keynotesCheckbox.Location = new Point(225, 43);
            this.keynotesCheckbox.Name = "keynotesCheckbox";
            this.keynotesCheckbox.Size = new Size(150, 25);
            this.keynotesCheckbox.TabIndex = 13;
            this.keynotesCheckbox.UseVisualStyleBackColor = true;
            this.decalCheckbox.Location = new Point(225, 67);
            this.decalCheckbox.Name = "decalCheckbox";
            this.decalCheckbox.Size = new Size(150, 25);
            this.decalCheckbox.TabIndex = 14;
            this.decalCheckbox.UseVisualStyleBackColor = true;
            this.DWFMarkupsCheckbox.Checked = true;
            this.DWFMarkupsCheckbox.CheckState = CheckState.Checked;
            this.DWFMarkupsCheckbox.Location = new Point(40, 92);
            this.DWFMarkupsCheckbox.Name = "DWFMarkupsCheckbox";
            this.DWFMarkupsCheckbox.Size = new Size(150, 25);
            this.DWFMarkupsCheckbox.TabIndex = 12;
            this.DWFMarkupsCheckbox.UseVisualStyleBackColor = true;
            this.CADLinksCheckbox.Checked = true;
            this.CADLinksCheckbox.CheckState = CheckState.Checked;
            this.CADLinksCheckbox.Location = new Point(40, 67);
            this.CADLinksCheckbox.Name = "CADLinksCheckbox";
            this.CADLinksCheckbox.Size = new Size(150, 25);
            this.CADLinksCheckbox.TabIndex = 11;
            this.CADLinksCheckbox.UseVisualStyleBackColor = true;
            this.revitLinksCheckbox.Checked = true;
            this.revitLinksCheckbox.CheckState = CheckState.Checked;
            this.revitLinksCheckbox.Location = new Point(40, 43);
            this.revitLinksCheckbox.Name = "revitLinksCheckbox";
            this.revitLinksCheckbox.Size = new Size(150, 25);
            this.revitLinksCheckbox.TabIndex = 10;
            this.revitLinksCheckbox.UseVisualStyleBackColor = true;
            this.saveSettingsCheckbox.Checked = true;
            this.saveSettingsCheckbox.CheckState = CheckState.Checked;
            this.saveSettingsCheckbox.Location = new Point(18, 742);
            this.saveSettingsCheckbox.Name = "saveSettingsCheckbox";
            this.saveSettingsCheckbox.Size = new Size(570, 22);
            this.saveSettingsCheckbox.TabIndex = 30;
            this.saveSettingsCheckbox.UseVisualStyleBackColor = true;
            this.inspectModelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.inspectModelButton.Location = new Point(364, 770);
            this.inspectModelButton.Name = "inspectModelButton";
            this.inspectModelButton.Size = new Size(125, 30);
            this.inspectModelButton.TabIndex = 31;
            this.inspectModelButton.UseVisualStyleBackColor = true;
            this.inspectModelButton.Click += new EventHandler(this.inspectModelButton_Click);
            this.cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(499, 770);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(95, 30);
            this.cancelButton.TabIndex = 32;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.groupBox3.Controls.Add((Control)this.upgradeCheckbox);
            this.groupBox3.Controls.Add((Control)this.panel3);
            this.groupBox3.Controls.Add((Control)this.workSetCheckbox);
            this.groupBox3.Controls.Add((Control)this.purgeCheckbox);
            this.groupBox3.Location = new Point(18, 411);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(576, 325);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.upgradeCheckbox.AutoSize = true;
            this.upgradeCheckbox.Location = new Point(9, 19);
            this.upgradeCheckbox.Name = "upgradeCheckbox";
            this.upgradeCheckbox.Size = new Size(64, 17);
            this.upgradeCheckbox.TabIndex = 15;
            this.upgradeCheckbox.Text = "cleanup";
            this.upgradeCheckbox.UseVisualStyleBackColor = true;
            this.upgradeCheckbox.CheckedChanged += new EventHandler(this.upgradeCheckbox_CheckedChanged);
            this.panel3.Controls.Add((Control)this.removeSheetsRadio);
            this.panel3.Controls.Add((Control)this.panel2);
            this.panel3.Controls.Add((Control)this.includeSheetsRadio);
            this.panel3.Controls.Add((Control)this.panel1);
            this.panel3.Location = new Point(28, 102);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(542, 217);
            this.panel3.TabIndex = 31;
            this.removeSheetsRadio.AutoSize = true;
            this.removeSheetsRadio.Enabled = false;
            this.removeSheetsRadio.Location = new Point(3, 140);
            this.removeSheetsRadio.Name = "removeSheetsRadio";
            this.removeSheetsRadio.Size = new Size(112, 17);
            this.removeSheetsRadio.TabIndex = 25;
            this.removeSheetsRadio.Text = "Remove all sheets";
            this.removeSheetsRadio.UseVisualStyleBackColor = true;
            this.panel2.Controls.Add((Control)this.removeIncludeAllRadio);
            this.panel2.Controls.Add((Control)this.removeSelectTypesButton);
            this.panel2.Controls.Add((Control)this.removeSelectRadio);
            this.panel2.Location = new Point(28, 159);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(511, 50);
            this.panel2.TabIndex = 29;
            this.removeIncludeAllRadio.AutoSize = true;
            this.removeIncludeAllRadio.Checked = true;
            this.removeIncludeAllRadio.Enabled = false;
            this.removeIncludeAllRadio.Location = new Point(3, 3);
            this.removeIncludeAllRadio.Name = "removeIncludeAllRadio";
            this.removeIncludeAllRadio.Size = new Size(199, 17);
            this.removeIncludeAllRadio.TabIndex = 26;
            this.removeIncludeAllRadio.TabStop = true;
            this.removeIncludeAllRadio.Text = "Include all view types (not on sheets)";
            this.removeIncludeAllRadio.UseVisualStyleBackColor = true;
            this.removeSelectTypesButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.removeSelectTypesButton.Enabled = false;
            this.removeSelectTypesButton.Location = new Point(355, 17);
            this.removeSelectTypesButton.Name = "removeSelectTypesButton";
            this.removeSelectTypesButton.Size = new Size(146, 30);
            this.removeSelectTypesButton.TabIndex = 29;
            this.removeSelectTypesButton.UseVisualStyleBackColor = true;
            this.removeSelectTypesButton.Click += new EventHandler(this.customizeViewButton_Click);
            this.removeSelectRadio.AutoSize = true;
            this.removeSelectRadio.Enabled = false;
            this.removeSelectRadio.Location = new Point(3, 26);
            this.removeSelectRadio.Name = "removeSelectRadio";
            this.removeSelectRadio.Size = new Size(181, 17);
            this.removeSelectRadio.TabIndex = 27;
            this.removeSelectRadio.Text = "Select view types (not on sheets)";
            this.removeSelectRadio.UseVisualStyleBackColor = true;
            this.includeSheetsRadio.AutoSize = true;
            this.includeSheetsRadio.Checked = true;
            this.includeSheetsRadio.Enabled = false;
            this.includeSheetsRadio.Location = new Point(3, 3);
            this.includeSheetsRadio.Name = "includeSheetsRadio";
            this.includeSheetsRadio.Size = new Size(107, 17);
            this.includeSheetsRadio.TabIndex = 21;
            this.includeSheetsRadio.TabStop = true;
            this.includeSheetsRadio.Text = "Include all sheets";
            this.includeSheetsRadio.UseVisualStyleBackColor = true;
            this.panel1.Controls.Add((Control)this.includeSelectTypesButton);
            this.panel1.Controls.Add((Control)this.includeIncludeAllRadio);
            this.panel1.Controls.Add((Control)this.includeOnlyOnSheetsRadio);
            this.panel1.Controls.Add((Control)this.includeSelectRadio);
            this.panel1.Location = new Point(28, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(511, 108);
            this.panel1.TabIndex = 23;
            this.includeSelectTypesButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.includeSelectTypesButton.Enabled = false;
            this.includeSelectTypesButton.Location = new Point(355, 75);
            this.includeSelectTypesButton.Name = "includeSelectTypesButton";
            this.includeSelectTypesButton.Size = new Size(146, 30);
            this.includeSelectTypesButton.TabIndex = 28;
            this.includeSelectTypesButton.UseVisualStyleBackColor = true;
            this.includeSelectTypesButton.Click += new EventHandler(this.includeSelectTypesButton_Click);
            this.includeIncludeAllRadio.AutoSize = true;
            this.includeIncludeAllRadio.Checked = true;
            this.includeIncludeAllRadio.Enabled = false;
            this.includeIncludeAllRadio.Location = new Point(3, 3);
            this.includeIncludeAllRadio.Name = "includeIncludeAllRadio";
            this.includeIncludeAllRadio.Size = new Size(103, 17);
            this.includeIncludeAllRadio.TabIndex = 22;
            this.includeIncludeAllRadio.TabStop = true;
            this.includeIncludeAllRadio.Text = "Include all views";
            this.includeIncludeAllRadio.UseVisualStyleBackColor = true;
            this.includeOnlyOnSheetsRadio.AutoSize = true;
            this.includeOnlyOnSheetsRadio.Enabled = false;
            this.includeOnlyOnSheetsRadio.Location = new Point(3, 26);
            this.includeOnlyOnSheetsRadio.Name = "includeOnlyOnSheetsRadio";
            this.includeOnlyOnSheetsRadio.Size = new Size(162, 17);
            this.includeOnlyOnSheetsRadio.TabIndex = 23;
            this.includeOnlyOnSheetsRadio.Text = "Only include views on sheets";
            this.includeOnlyOnSheetsRadio.UseVisualStyleBackColor = true;
            this.includeSelectRadio.AutoSize = true;
            this.includeSelectRadio.Enabled = false;
            this.includeSelectRadio.Location = new Point(3, 49);
            this.includeSelectRadio.Name = "includeSelectRadio";
            this.includeSelectRadio.Size = new Size(181, 17);
            this.includeSelectRadio.TabIndex = 24;
            this.includeSelectRadio.Text = "Select view types (not on sheets)";
            this.includeSelectRadio.UseVisualStyleBackColor = true;
            this.workSetCheckbox.Enabled = false;
            this.workSetCheckbox.Location = new Point(30, 42);
            this.workSetCheckbox.Name = "workSetCheckbox";
            this.workSetCheckbox.Size = new Size(225, 24);
            this.workSetCheckbox.TabIndex = 16;
            this.workSetCheckbox.UseVisualStyleBackColor = true;
            this.purgeCheckbox.Enabled = false;
            this.purgeCheckbox.Location = new Point(30, 72);
            this.purgeCheckbox.Name = "purgeCheckbox";
            this.purgeCheckbox.Size = new Size(225, 24);
            this.purgeCheckbox.TabIndex = 17;
            this.purgeCheckbox.UseVisualStyleBackColor = true;
            this.HelpLinkLabel.AutoSize = true;
            this.HelpLinkLabel.Location = new Point(15, 725);
            this.HelpLinkLabel.Name = "HelpLinkLabel";
            this.HelpLinkLabel.Size = new Size(0, 13);
            this.HelpLinkLabel.TabIndex = 23;
            this.HelpLinkLabel.TabStop = true;
            this.HelpLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.OnHelpLinkClicked);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(616, 822);
            this.Controls.Add((Control)this.HelpLinkLabel);
            this.Controls.Add((Control)this.groupBox3);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.inspectModelButton);
            this.Controls.Add((Control)this.saveSettingsCheckbox);
            this.Controls.Add((Control)this.groupBox2);
            this.Controls.Add((Control)this.groupBox1);
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(628, 693);
            this.Name = "eTransmitSettingsDialog";
            //this.Name = nameof(eTransmitSettingsDialog);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private List<ServerTree> GetServerList()
        {
            if (this.serverFileDialog != null)
            {
                return this.serverFileDialog.ServerTrees;
            }
            else
            {
                string serverName = this.GetInputName()
                                        .Replace("RSN://", "")
                                        .Split('/')
                                        [0];
                ServerTree serverTree = new ServerTree(serverName);
                HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(new Uri(string.Format("http://{0}/RevitServerAdminRESTService{1}/AdminRESTService.svc/", serverName, (object)TransmissionOptions.ServiceVersion)), "|/contents")) as HttpWebRequest;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 180000;
                httpWebRequest.Headers.Add("User-Name", "eTransmit add-in");
                httpWebRequest.Headers.Add("User-Machine-Name", Environment.MachineName);
                httpWebRequest.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
                httpWebRequest.Headers.Add("API-Version", "1.1");

                HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                HttpStatusCode statusCode = response.StatusCode;
                string end = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                FolderOnServer folderOnServer = (FolderOnServer)new DataContractJsonSerializer(typeof(FolderOnServer)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(end)));
                if (folderOnServer != null)
                {
                    ServerNode rootNode = serverTree.GetRootNode();
                    string str = "";
                    foreach (InsideOfOneFolder folder in folderOnServer.Folders)
                    {
                        ServerNode node = new ServerNode(NodeType.Folder, folder.Name);
                        if (folder.HasContents)
                            AddChildrenOfServerNode(ref node, str + folder.Name, serverName);
                        rootNode.AddChild(node);
                    }
                    foreach (FileOnServer model in folderOnServer.Models)
                    {
                        ServerNode node = new ServerNode(NodeType.Model, model.Name);
                        rootNode.AddChild(node);
                    }
                }
                List<ServerTree> listServerTree = new List<ServerTree>();
                listServerTree.Add(serverTree);
                return listServerTree;
            }
        }


        private void AddChildrenOfServerNode(ref ServerNode node, string dirPath, string serverName)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(new Uri(string.Format("http://{0}/RevitServerAdminRESTService{1}/AdminRESTService.svc/", serverName, (object)TransmissionOptions.ServiceVersion)), dirPath + "/contents")) as HttpWebRequest;
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 180000;
            httpWebRequest.Headers.Add("User-Name", "eTransmit add-in");
            httpWebRequest.Headers.Add("User-Machine-Name", Environment.MachineName);
            httpWebRequest.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
            httpWebRequest.Headers.Add("API-Version", "1.0");
            HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
            HttpStatusCode statusCode = response.StatusCode;
            string end = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();
            FolderOnServer folderOnServer = (FolderOnServer)new DataContractJsonSerializer(typeof(FolderOnServer)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(end)));
            if (folderOnServer == null)
                return;
            foreach (InsideOfOneFolder folder in folderOnServer.Folders)
            {
                ServerNode node1 = new ServerNode(NodeType.Folder, folder.Name);
                if (folder.HasContents)
                    AddChildrenOfServerNode(ref node1, dirPath + "|" + folder.Name, serverName);
                node.AddChild(node1);
            }
            foreach (FileOnServer model in folderOnServer.Models)
            {
                ServerNode node2 = new ServerNode(NodeType.Model, model.Name);
                node.AddChild(node2);
            }
        }
    }
}
