// Decompiled with JetBrains decompiler
// Type: eTransmitForRevit.ServerTree
// Assembly: eTransmitForRevit, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 464563F1-96AD-4B9F-A23A-FA49D8EE3FD8
// Assembly location: C:\Program Files\Autodesk\eTransmit for Revit 2019\eTransmitForRevit.dll

namespace eTransmitForRevit
{
  internal class ServerTree
  {
    private ServerNode m_rootNode;

    public ServerTree(string serverName) => this.m_rootNode = new ServerNode(NodeType.Server, serverName);

    public ServerNode GetRootNode() => this.m_rootNode;
  }
}
