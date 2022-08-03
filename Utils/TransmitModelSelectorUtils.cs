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
        public static IEnumerable<Autodesk.Revit.DB.ModelPath> GetModelPathsFromFileOrFolder(string input, List<ServerTree> serverTrees = null)
        {
            if (Directory.Exists(input))
                return ((IEnumerable<string>)Directory.GetFiles(input))
                            .Where<string>((Func<string, bool>)(x => Path.GetExtension(x).Equals(".rvt")))
                            .Select<string, Autodesk.Revit.DB.ModelPath>(new Func<string, Autodesk.Revit.DB.ModelPath>(ModelPathUtils.ConvertUserVisiblePathToModelPath));


            if (File.Exists(input))
            {
                List<Autodesk.Revit.DB.ModelPath> fromFileOrFolder = new List<Autodesk.Revit.DB.ModelPath>();
                fromFileOrFolder.Add(ModelPathUtils.ConvertUserVisiblePathToModelPath(input));
                return (IEnumerable<Autodesk.Revit.DB.ModelPath>)fromFileOrFolder;
            }

            Autodesk.Revit.DB.ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(input);
            List<Autodesk.Revit.DB.ModelPath> fromFileOrFolder1 = new List<Autodesk.Revit.DB.ModelPath>();
            if (serverTrees == null)
            {
                if (!modelPath.ServerPath)
                    return (IEnumerable<Autodesk.Revit.DB.ModelPath>)null;
                fromFileOrFolder1.Add((Autodesk.Revit.DB.ModelPath)modelPath);
                return (IEnumerable<Autodesk.Revit.DB.ModelPath>)fromFileOrFolder1;
            }

            else
            {
                ServerTree tree = serverTrees
                    .Where(x => x.m_rootNode.Name == modelPath.CentralServerPath)
                    .FirstOrDefault();

                string newInput = input.Replace("RSN://","" );
                string[] newInputArr = newInput.Split('/');
                //string folderPath = "";
                ServerNode folder = GetServerNode(tree, newInputArr);

                List<string> inputNodes = new List<string>();
                string inputNode = "";
                GetRvtFilesInFolder(ref inputNodes, ref inputNode, folder);

                inputNodes.ForEach(x => fromFileOrFolder1.Add(ModelPathUtils.ConvertUserVisiblePathToModelPath(input + x)));

                return fromFileOrFolder1;
            }

        }

        private static void GetRvtFilesInFolder(ref List<string> inputNodes, ref string inputNode, ServerNode folder)
        {
            foreach (ServerNode node in folder.GetChildren())
            {
                if (node.NodeType == NodeType.Folder)
                {
                    
                    foreach (ServerNode nodeChildren in node.GetChildren())
                    {
                        inputNode = inputNode + "/" + node.Name;
                        GetRvtFilesInFolder(ref inputNodes, ref inputNode, nodeChildren);
                    }
                    //GetRvtFilesInFolder(ref inputNodes, ref inputNode, folder);
                }
                else if (node.NodeType == NodeType.Model)
                {
                    inputNodes.Add(inputNode + "/" + node.Name);
                    inputNode = "";
                }
            }
        }

        public static ServerNode GetServerNode (ServerTree tree, string[] newinputArr)
        {
            ServerNode nodeCurrent = tree.GetRootNode();
            for (int i = 1; i < newinputArr.Length; i++)
            {
                ServerNode node = nodeCurrent
                   .GetChildren()
                   .Where(x => x.Name == newinputArr[i])
                   .FirstOrDefault();
                nodeCurrent = node;
            }
            return nodeCurrent;
        }
    }
}
