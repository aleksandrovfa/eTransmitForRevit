// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.OpenServerFileDialog
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using Autodesk.Revit.UI;
using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;

using ComboBox = System.Windows.Forms.ComboBox;

namespace eTransmitForRevitPirat
{
    public class OpenServerFileDialog : Form
    {
        private string m_fileName;
        private string m_currentServer;
        internal List<ServerTree> m_serverTrees;
        internal List<ServerTree> ServerTrees
        {
            get { return m_serverTrees; }
            set { m_serverTrees = value; }
        }
        private List<string> m_currentPath;
        private List<List<string>> m_lastPaths;
        private ExternalCommandData m_commandData;
        private IContainer components;
        private Button okButton;
        private Button cancelButton;
        private Button showFilesButton;
        private DataGridView serverGrid;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private Button upButton;
        private ComboBox serverComboBox1;
        private Button backButton;

        public OpenServerFileDialog(ExternalCommandData commandData)
        {
            this.InitializeComponent();
            new ToolTip().SetToolTip((Control)this.backButton, eTransmitResources.Back);
            new ToolTip().SetToolTip((Control)this.upButton, eTransmitResources.Up);
            new ToolTip().SetToolTip((Control)this.showFilesButton, eTransmitResources.Refresh);
            this.okButton.Text = eTransmitResources.OK;
            this.cancelButton.Text = eTransmitResources.Cancel;
            this.Text = eTransmitResources.eTransmitString;
            this.m_commandData = commandData;
            List<string> stringList = new List<string>();
            try
            {
                stringList = (List<string>)commandData.Application.Application.GetRevitServerNetworkHosts();
            }
            catch (Exception ex)
            {
                commandData.Application.Application.WriteJournalComment("eTransmit - Unable to get the central server name while opening the server dialog. Extended message: " + ex.Message, true);
            }
            this.m_currentServer = "";
            this.m_serverTrees = new List<ServerTree>();
            foreach (string serverName in stringList)
                this.m_serverTrees.Add(new ServerTree(serverName));
            this.m_fileName = "";
            this.m_currentPath = new List<string>();
            this.m_lastPaths = new List<List<string>>();
            this.serverGrid.CellDoubleClick += new DataGridViewCellEventHandler(this.serverGrid_DoubleClicked);
            this.HelpButtonClicked += new CancelEventHandler(this.openServerFileDialog_HelpButtonClicked);
            this.HelpRequested += new HelpEventHandler(this.textBox_HelpRequested);
            this.serverComboBox1.SelectionChangeCommitted += new EventHandler(this.comboBox_SelectionChangeCommitted);
            this.serverGrid.CellPainting += new DataGridViewCellPaintingEventHandler(this.serverGrid_CellPainting);
            this.serverGrid.Columns[0].HeaderText = eTransmitResources.Name;
            this.serverGrid.Columns[1].HeaderText = eTransmitResources.Type;
            this.serverGrid.Columns[2].HeaderText = eTransmitResources.DateModified;
            this.ShowFiles();
        }

        private void serverGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0)
                return;
            e.PaintBackground(e.CellBounds, true);
            Rectangle bounds = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
            if (bounds.Width > 16)
            {
                bounds.Width -= 17;
                bounds.Location = new Point(bounds.X + 16, bounds.Y);
            }
            new StringFormat().LineAlignment = StringAlignment.Center;
            Font font = this.serverGrid.DefaultCellStyle.Font;
            System.Windows.Forms.TextFormatFlags flags = System.Windows.Forms.TextFormatFlags.EndEllipsis | System.Windows.Forms.TextFormatFlags.SingleLine | System.Windows.Forms.TextFormatFlags.VerticalCenter;
            ServerNode serverNode = (ServerNode)e.Value;
            string name = serverNode.Name;
            TextRenderer.DrawText((IDeviceContext)e.Graphics, name, font, bounds, SystemColors.ControlText, flags);
            Rectangle rectangle = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
            Point location = rectangle.Location;
            if (rectangle.Height >= 16)
            {
                location.Y = rectangle.Top + rectangle.Height / 2 - 8;
                rectangle.Location = location;
            }
            rectangle.Size = new Size(16, 16);
            if (serverNode.NodeType == NodeType.Server)
                e.Graphics.DrawIconUnstretched(new Icon(eTransmitResources.CentralServer, new Size(16, 16)), rectangle);
            else if (serverNode.NodeType == NodeType.Folder)
                e.Graphics.DrawImageUnscaled((Image)eTransmitResources.Folder_transp_background1, rectangle);
            else
                e.Graphics.DrawIconUnstretched(new Icon(eTransmitResources.file_extension_icon_rvt, new Size(16, 16)), rectangle);
            e.Handled = true;
        }

        private void textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            eTransmitHelpLauncher.LaunchHelp();
            hlpevent.Handled = true;
        }

        private void openServerFileDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            eTransmitHelpLauncher.LaunchHelp();
            e.Cancel = true;
        }

        private void serverGrid_DoubleClicked(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string str = this.serverGrid.Rows[rowIndex].Cells[0].Value.ToString();
            ServerNode serverNode = (ServerNode)this.serverGrid.Rows[rowIndex].Cells[0].Value;
            if (serverNode.NodeType == NodeType.Server)
            {
                this.m_currentServer = str;
                this.m_lastPaths.Insert(0, new List<string>((IEnumerable<string>)this.m_currentPath));
                this.m_currentPath.Add(str);
                this.ShowFiles();
            }
            else if (serverNode.NodeType == NodeType.Folder)
            {
                this.m_lastPaths.Insert(0, new List<string>((IEnumerable<string>)this.m_currentPath));
                this.m_currentPath.Add(str);
                this.ModifyComboBoxForGridChange(this.m_currentPath);
                this.ModifyGridForComboBoxChange(this.m_currentPath);
            }
            else
                this.PossiblyExitDialog();
        }

        private void PossiblyExitDialog()
        {
            if (this.serverGrid.RowCount == 0)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                ServerNode serverNode = (ServerNode)this.serverGrid.SelectedRows[0].Cells[0].Value;
                //if (serverNode.NodeType == NodeType.Model)
                //{
                this.m_fileName = "RSN://" + this.m_currentPath[0] + "/";
                for (int index = 1; index < this.m_currentPath.Count; ++index)
                    this.m_fileName = this.m_fileName + this.m_currentPath[index] + "/";
                this.m_fileName += serverNode.Name;
                this.DialogResult = DialogResult.OK;
                this.Close();
                //}
                //else
                //{
                //    int num = (int)MessageBox.Show(eTransmitResources.PleaseSelectAValidModel);
                //}
            }
        }

        private void comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int selectedIndex = this.serverComboBox1.SelectedIndex;
            this.m_lastPaths.Insert(0, new List<string>((IEnumerable<string>)this.m_currentPath));
            if (selectedIndex != 0)
            {
                int count = this.m_currentPath.Count;
                for (int index = selectedIndex; index < count; ++index)
                    this.m_currentPath.RemoveAt(selectedIndex);
            }
            else
                this.m_currentPath.Clear();
            this.ModifyGridForComboBoxChange(this.m_currentPath);
            this.ModifyComboBoxForGridChange(this.m_currentPath);
        }

        private void button1_Click(object sender, EventArgs e) => this.PossiblyExitDialog();

        public string GetCentralServerName() => "RSN://" + this.m_currentServer;

        public string GetFileName() => this.m_fileName;

        private void ModifyGridForComboBoxChange(List<string> folderPath)
        {
            if (folderPath.Count == 0)
            {
                this.serverGrid.Rows.Clear();
                foreach (ServerTree serverTree in this.m_serverTrees)
                {
                    this.serverGrid.Rows.Add(new object[1]
                    {
            (object) serverTree.GetRootNode()
                    });
                    this.serverGrid.Rows[this.serverGrid.Rows.Count - 1].Cells[1].Value = (object)eTransmitResources.RevitServer;
                }
            }
            else
            {
                ServerTree serverTree = this.GetServerTree(folderPath[0]);
                if (serverTree == null)
                    return;
                ServerNode serverNode = serverTree.GetRootNode();
                for (int index = 1; index < folderPath.Count; ++index)
                {
                    foreach (ServerNode child in serverNode.GetChildren())
                    {
                        if (child.Name.CompareTo(folderPath[index]) == 0)
                        {
                            serverNode = child;
                            break;
                        }
                    }
                }
                this.serverGrid.Rows.Clear();
                foreach (ServerNode child in serverNode.GetChildren())
                {
                    this.serverGrid.Rows.Add(new object[1]
                    {
            (object) child
                    });
                    this.serverGrid.Rows[this.serverGrid.Rows.Count - 1].Cells[1].Value = (object)child.GetStringForNodeType();
                    this.serverGrid.Rows[this.serverGrid.Rows.Count - 1].Cells[2].Value = (object)child.Date;
                }
            }
        }

        private void ModifyComboBoxForGridChange(List<string> folderPath)
        {
            this.serverComboBox1.Items.Clear();
            this.serverComboBox1.Items.Add((object)eTransmitResources.RevitServerNetwork);
            if (folderPath.Count == 0)
                this.m_currentServer = "";
            for (int index1 = 0; index1 < folderPath.Count; ++index1)
            {
                string str = "";
                for (int index2 = 0; index2 < index1; ++index2)
                    str += " ";
                this.serverComboBox1.Items.Add((object)folderPath[index1]);
            }
            this.serverComboBox1.SelectedIndex = this.serverComboBox1.Items.Count - 1;
        }

        private string GetDateFromString(string dateString)
        {
            dateString = dateString.Replace("/Date(", "");
            dateString = dateString.Replace(")/", "");
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double)long.Parse(dateString));
            TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
            dateTime = dateTime.Add(utcOffset);
            return dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString();
        }

        private void GetDirectoryDate(ref ServerNode node, string path)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(new Uri(string.Format("http://{0}/RevitServerAdminRESTService{1}/AdminRESTService.svc/", (object)this.m_currentServer, (object)TransmissionOptions.ServiceVersion)), path + "/DirectoryInfo")) as HttpWebRequest;
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 180000;
            httpWebRequest.Headers.Add("User-Name", "eTransmit add-in");
            httpWebRequest.Headers.Add("User-Machine-Name", Environment.MachineName);
            httpWebRequest.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
            httpWebRequest.Headers.Add("API-Version", "1.0");
            try
            {
                HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                string end = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                FolderDateStructure folderDateStructure = (FolderDateStructure)new DataContractJsonSerializer(typeof(FolderDateStructure)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(end)));
                node.Date = this.GetDateFromString(folderDateStructure.DateModified);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(eTransmitResources.DialogServerError);
                this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Could not contact the server while trying to get a Revit Server file date. The directory was " + path + " Extended message: " + ex.Message, true);
            }
        }

        private void AddChildrenOfServerNode(ref ServerNode node, string dirPath)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(new Uri(string.Format("http://{0}/RevitServerAdminRESTService{1}/AdminRESTService.svc/", (object)this.m_currentServer, (object)TransmissionOptions.ServiceVersion)), dirPath + "/contents")) as HttpWebRequest;
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 180000;
            httpWebRequest.Headers.Add("User-Name", "eTransmit add-in");
            httpWebRequest.Headers.Add("User-Machine-Name", Environment.MachineName);
            httpWebRequest.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
            httpWebRequest.Headers.Add("API-Version", "1.0");
            try
            {
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
                    this.GetDirectoryDate(ref node1, dirPath + "|" + folder.Name);
                    if (folder.HasContents)
                        this.AddChildrenOfServerNode(ref node1, dirPath + "|" + folder.Name);
                    node.AddChild(node1);
                }
                foreach (FileOnServer model in folderOnServer.Models)
                {
                    ServerNode node2 = new ServerNode(NodeType.Model, model.Name);
                    this.GetDirectoryDate(ref node2, dirPath + "|" + model.Name);
                    node.AddChild(node2);
                }
            }
            catch (WebException ex)
            {
                int num = (int)MessageBox.Show(eTransmitResources.DialogServerError);
                this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Encountered a web exception trying to contact the server. The directory was " + dirPath + " Extended message: " + ex.Message, true);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(eTransmitResources.DialogServerError);
                this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Encountered an error trying to contact Revit Server. The directory was " + dirPath + " Extended message: " + ex.Message, true);
            }
        }

        private void showFilesButton_Click(object sender, EventArgs e) => this.ShowFiles();

        private void ShowFiles()
        {
            if (this.m_currentPath.Count < 0)
            {
                this.m_commandData.Application.Application.WriteJournalComment("Negative path length in m_currentPath in the server dialog.", true);
                this.m_currentPath.Clear();
            }
            if (this.m_currentPath.Count == 0)
            {
                List<string> stringList = new List<string>();
                try
                {
                    stringList = (List<string>)this.m_commandData.Application.Application.GetRevitServerNetworkHosts();
                }
                catch (Exception ex)
                {
                    this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Unable to get the central server name while opening the server dialog. Extended message: " + ex.Message, true);
                }
                this.serverComboBox1.Items.Clear();
                this.serverComboBox1.Items.Add((object)eTransmitResources.RevitServerNetwork);
                this.serverComboBox1.SelectedIndex = 0;
                this.serverGrid.Rows.Clear();
                this.m_serverTrees = new List<ServerTree>();
                foreach (string serverName in stringList)
                {
                    this.m_serverTrees.Add(new ServerTree(serverName));
                    this.serverGrid.Rows.Add(new object[1]
                    {
            (object) this.m_serverTrees[this.m_serverTrees.Count - 1].GetRootNode()
                    });
                    this.serverGrid.Rows[this.serverGrid.Rows.Count - 1].Cells[1].Value = (object)eTransmitResources.RevitServer;
                }
            }
            else
            {
                int indexOfServerTree = this.GetIndexOfServerTree(this.m_currentServer);
                if (indexOfServerTree != -1)
                    this.m_serverTrees[indexOfServerTree] = new ServerTree(this.m_currentServer);
                this.serverComboBox1.Items.Clear();
                this.serverGrid.Rows.Clear();
                if (string.IsNullOrWhiteSpace(this.m_currentServer))
                    return;
                HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(new Uri(string.Format("http://{0}/RevitServerAdminRESTService{1}/AdminRESTService.svc/", (object)this.m_currentServer, (object)TransmissionOptions.ServiceVersion)), "|/contents")) as HttpWebRequest;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 180000;
                httpWebRequest.Headers.Add("User-Name", "eTransmit add-in");
                httpWebRequest.Headers.Add("User-Machine-Name", Environment.MachineName);
                httpWebRequest.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
                httpWebRequest.Headers.Add("API-Version", "1.1");
                try
                {
                    HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                    HttpStatusCode statusCode = response.StatusCode;
                    string end = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    response.Close();
                    FolderOnServer folderOnServer = (FolderOnServer)new DataContractJsonSerializer(typeof(FolderOnServer)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(end)));
                    if (folderOnServer != null)
                    {
                        ServerTree serverTree = this.GetServerTree(this.m_currentServer);
                        if (serverTree == null)
                            return;
                        ServerNode rootNode = serverTree.GetRootNode();
                        string str = "";
                        foreach (InsideOfOneFolder folder in folderOnServer.Folders)
                        {
                            ServerNode node = new ServerNode(NodeType.Folder, folder.Name);
                            this.GetDirectoryDate(ref node, str + folder.Name);
                            if (folder.HasContents)
                                this.AddChildrenOfServerNode(ref node, str + folder.Name);
                            rootNode.AddChild(node);
                        }
                        foreach (FileOnServer model in folderOnServer.Models)
                        {
                            ServerNode node = new ServerNode(NodeType.Model, model.Name);
                            this.GetDirectoryDate(ref node, str + model.Name);
                            rootNode.AddChild(node);
                        }
                    }
                    this.ModifyGridForComboBoxChange(this.m_currentPath);
                    this.ModifyComboBoxForGridChange(this.m_currentPath);
                }
                catch (WebException ex)
                {
                    int num = (int)MessageBox.Show(eTransmitResources.DialogServerError);
                    this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Unable to contact the sever while showing files; encountered a web exception. Extended message: " + ex.Message, true);
                }
                catch (Exception ex)
                {
                    int num = (int)MessageBox.Show(eTransmitResources.DialogServerError);
                    this.m_commandData.Application.Application.WriteJournalComment("eTransmit - Unable to contact the server while showing files. Extended message: " + ex.Message, true);
                }
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (this.m_currentPath.Count <= 0)
                return;
            this.m_lastPaths.Insert(0, new List<string>((IEnumerable<string>)this.m_currentPath));
            this.m_currentPath.RemoveAt(this.m_currentPath.Count - 1);
            this.ModifyComboBoxForGridChange(this.m_currentPath);
            this.ModifyGridForComboBoxChange(this.m_currentPath);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (this.m_lastPaths.Count <= 0)
                return;
            List<string> lastPath = this.m_lastPaths[0];
            this.m_lastPaths.RemoveAt(0);
            this.m_currentPath = new List<string>((IEnumerable<string>)lastPath);
            this.ModifyComboBoxForGridChange(this.m_currentPath);
            this.ModifyGridForComboBoxChange(this.m_currentPath);
        }

        private ServerTree GetServerTree(string serverName)
        {
            foreach (ServerTree serverTree in this.m_serverTrees)
            {
                if (serverTree.GetRootNode().Name == serverName)
                    return serverTree;
            }
            return (ServerTree)null;
        }

        private int GetIndexOfServerTree(string serverName)
        {
            for (int index = 0; index < this.m_serverTrees.Count; ++index)
            {
                if (this.m_serverTrees[index].GetRootNode().Name == serverName)
                    return index;
            }
            return -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(OpenServerFileDialog));
            DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
            this.okButton = new Button();
            this.cancelButton = new Button();
            this.showFilesButton = new Button();
            this.serverGrid = new DataGridView();
            this.Column1 = new DataGridViewTextBoxColumn();
            this.Column2 = new DataGridViewTextBoxColumn();
            this.Column3 = new DataGridViewTextBoxColumn();
            this.upButton = new Button();
            this.serverComboBox1 = new ComboBox();
            this.backButton = new Button();
            ((ISupportInitialize)this.serverGrid).BeginInit();
            this.SuspendLayout();
            this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.okButton.Location = new Point(431, 393);
            this.okButton.Margin = new Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size((int)sbyte.MaxValue, 37);
            this.okButton.TabIndex = 0;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.button1_Click);
            this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(565, 393);
            this.cancelButton.Margin = new Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size((int)sbyte.MaxValue, 37);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.showFilesButton.Image = (Image)componentResourceManager.GetObject("showFilesButton.Image");
            this.showFilesButton.ImageAlign = ContentAlignment.BottomCenter;
            this.showFilesButton.Location = new Point(639, 15);
            this.showFilesButton.Margin = new Padding(4, 4, 4, 4);
            this.showFilesButton.Name = "showFilesButton";
            this.showFilesButton.Size = new Size(53, 26);
            this.showFilesButton.TabIndex = 4;
            this.showFilesButton.UseVisualStyleBackColor = true;
            this.showFilesButton.Click += new EventHandler(this.showFilesButton_Click);
            this.serverGrid.AllowUserToAddRows = false;
            this.serverGrid.AllowUserToDeleteRows = false;
            this.serverGrid.AllowUserToResizeRows = false;
            this.serverGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.serverGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.serverGrid.BackgroundColor = SystemColors.Window;
            this.serverGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridViewCellStyle1.BackColor = SystemColors.Control;
            gridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            gridViewCellStyle1.ForeColor = SystemColors.WindowText;
            gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.serverGrid.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
            this.serverGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.serverGrid.Columns.AddRange((DataGridViewColumn)this.Column1, (DataGridViewColumn)this.Column2, (DataGridViewColumn)this.Column3);
            gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridViewCellStyle2.BackColor = SystemColors.Window;
            gridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            gridViewCellStyle2.ForeColor = SystemColors.ControlText;
            gridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.serverGrid.DefaultCellStyle = gridViewCellStyle2;
            this.serverGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.serverGrid.GridColor = SystemColors.Control;
            this.serverGrid.Location = new Point(16, 48);
            this.serverGrid.Margin = new Padding(4, 4, 4, 4);
            this.serverGrid.MultiSelect = false;
            this.serverGrid.Name = "serverGrid";
            this.serverGrid.ReadOnly = true;
            gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridViewCellStyle3.BackColor = SystemColors.Control;
            gridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            gridViewCellStyle3.ForeColor = SystemColors.WindowText;
            gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            this.serverGrid.RowHeadersDefaultCellStyle = gridViewCellStyle3;
            this.serverGrid.RowHeadersVisible = false;
            this.serverGrid.RowTemplate.Height = 24;
            this.serverGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.serverGrid.Size = new Size(675, 337);
            this.serverGrid.TabIndex = 5;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.upButton.Image = (Image)componentResourceManager.GetObject("upButton.Image");
            this.upButton.Location = new Point(577, 15);
            this.upButton.Margin = new Padding(4, 4, 4, 4);
            this.upButton.Name = "upButton";
            this.upButton.Size = new Size(53, 26);
            this.upButton.TabIndex = 8;
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new EventHandler(this.upButton_Click);
            this.serverComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.serverComboBox1.FormattingEnabled = true;
            this.serverComboBox1.Location = new Point(16, 15);
            this.serverComboBox1.Margin = new Padding(4, 4, 4, 4);
            this.serverComboBox1.Name = "serverComboBox1";
            this.serverComboBox1.Size = new Size(487, 24);
            this.serverComboBox1.TabIndex = 5;
            this.backButton.Image = (Image)componentResourceManager.GetObject("backButton.Image");
            this.backButton.Location = new Point(516, 15);
            this.backButton.Margin = new Padding(4, 4, 4, 4);
            this.backButton.Name = "backButton";
            this.backButton.Size = new Size(53, 26);
            this.backButton.TabIndex = 9;
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new EventHandler(this.backButton_Click);
            this.AutoScaleDimensions = new SizeF(8f, 16f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(704, 439);
            this.Controls.Add((Control)this.backButton);
            this.Controls.Add((Control)this.serverComboBox1);
            this.Controls.Add((Control)this.showFilesButton);
            this.Controls.Add((Control)this.upButton);
            this.Controls.Add((Control)this.serverGrid);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.okButton);
            this.HelpButton = true;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Margin = new Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(719, 476);
            this.Name = nameof(OpenServerFileDialog);
            this.Name = "OpenServerFileDialog";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((ISupportInitialize)this.serverGrid).EndInit();
            this.ResumeLayout(false);
        }
    }
}
