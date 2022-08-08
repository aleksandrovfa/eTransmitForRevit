// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.ServerNode
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

using System.Collections.Generic;

namespace eTransmitForRevitPirat
{
  internal class ServerNode
  {
    private NodeType m_nodeType;
    private string m_name;
    private HashSet<ServerNode> m_childNodes;
    private string m_date;

    public ServerNode(NodeType nodeType, string name)
    {
      this.m_nodeType = nodeType;
      this.m_name = name;
      this.m_childNodes = new HashSet<ServerNode>();
      this.m_date = "";
    }

    public void AddChild(ServerNode node) => this.m_childNodes.Add(node);

    public HashSet<ServerNode> GetChildren() => this.m_childNodes;

    public NodeType NodeType => this.m_nodeType;

    public string GetStringForNodeType()
    {
      if (this.m_nodeType == NodeType.Server)
        return eTransmitResources.RevitServer;
      if (this.m_nodeType == NodeType.Folder)
        return eTransmitResources.FileFolder;
      return this.m_nodeType == NodeType.Model ? eTransmitResources.RevitProject : "";
    }

    public string Name => this.m_name;

    public string Date
    {
      get => this.m_date;
      set => this.m_date = value;
    }

    public override string ToString() => this.m_name;
  }
}
