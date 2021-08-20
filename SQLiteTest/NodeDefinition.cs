using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static SQLiteTest.NodeDefinition;
using System.Runtime.InteropServices.ComTypes;

namespace SQLiteTest
{
    class NodeDefinition
    {
        public enum NodeType
        {
            ntAppHeadline,
            ntApp,
            ntCountTotalEntriesHeadline,
            ntCountTotalEntries,
            ntCountTotalThreadsActiveHeadline,
            ntCountTotalThreadsActive,
            ntThread,
            ntThreadHeadline,
            ntCountThreadHeadline,
            ntTotalThreadCount,
            ntEntryHeadline,
            ntEntry,
            ntCountAppEntriesHeadline,
            ntCountAppEntries,
            ntCountThreadEntriesHeadline,
            ntCountThreadEntries,
            ntThreadThroughputHeadline,
            ntThreadThroughput,
            ntAppThroughputHeadline,
            ntAppThroughput,
            ntTotalThroughputHeadline,
            ntTotalThroughput,
            ntTotalStatusHeadline,
            ntTotalStatus,
            ntStatusHeadline,
            ntStatusItem,

            ntStatusFileSize

        };

        public enum NodeAction { LiveView, Update };

        public NodeType nodeType;
        public string strAppID = "";
        public string strThreadID = "";
        public string strID = "";
        public Color parentColor = Color.Black;

        static public bool isLiveable(NodeType type)
        {
            switch (type)
            {
                case NodeType.ntThreadThroughputHeadline:
                case NodeType.ntAppThroughputHeadline:
                case NodeType.ntTotalThroughputHeadline:
                case NodeType.ntStatusHeadline:
                case NodeType.ntCountTotalThreadsActiveHeadline:
                case NodeType.ntCountTotalEntriesHeadline:
                case NodeType.ntCountAppEntriesHeadline:
                case NodeType.ntCountThreadHeadline:
                case NodeType.ntCountThreadEntriesHeadline:

                    //case NodeType.ntThreadThroughput:
                    return true;
            }
            return false;
        }

        static public void removeInactives(TreeNodeCollection nodes, ref List<TreeNode> activeNodes, NodeType type)
        {
            bool bFound = false;

            switch (type)
            {
                case NodeDefinition.NodeType.ntApp:
                    {
                        for (int x = nodes.Count - 1; x >= 0; x--)
                        {
                            bFound = false;

                            if (nodes[x].Nodes.Count == 0)
                                nodes[x].Nodes.Add("");

                            for (int y = 0; y < activeNodes.Count; y++)
                            {
                                if (activeNodes[y].Text.Equals(nodes[x].Text))
                                    bFound = true;
                            }
                            if (!bFound)
                                nodes.RemoveAt(x);
                        }
                        break;
                    }
            }
        }

        static public TreeNode Add(NodeType nodeType, String name, bool expandable, TreeNodeCollection nodes, ref List<TreeNode> activeNodes, Color color, String strAppID = "", String strThreadID = "", String strID = "")
        {
            TreeNode node = new TreeNode(name);
            if (expandable)
            {
                node.Nodes.Add("");
            }
            NodeDefinition nd = new NodeDefinition();
            nd.strAppID = strAppID;
            nd.strThreadID = strThreadID;
            nd.strID = strID;
            nd.nodeType = nodeType;
            nd.parentColor = color;
            node.Tag = nd;
            node.ForeColor = color;
            String strNodeText = node.Text;
            Color nodeColor = node.ForeColor;
            for (int i = nodes.Count - 1; i >= 0; i--)
                if (nodes[i].Text.Length == 0)
                    nodes.RemoveAt(i);
            Add(ref node, nodes, ref activeNodes);
            node.Text = strNodeText;
            node.ForeColor = nodeColor;
            return node;
        }
        static public void Add(ref TreeNode newNode, TreeNodeCollection nodes, ref List<TreeNode> activeNodes)
        {
            bool bFound = false;

            foreach (TreeNode node in nodes)
            {
                NodeDefinition nd = (NodeDefinition)node.Tag;
                switch (((NodeDefinition)newNode.Tag).nodeType)
                {
                    case NodeType.ntAppHeadline:
                    case NodeType.ntCountTotalEntriesHeadline:
                    case NodeType.ntCountTotalThreadsActiveHeadline:
                    case NodeType.ntCountTotalThreadsActive:
                    case NodeType.ntTotalThroughputHeadline:
                    case NodeType.ntTotalThreadCount:
                    case NodeType.ntThreadHeadline:

                    case NodeType.ntEntryHeadline:
                    case NodeType.ntCountThreadEntriesHeadline:
                    case NodeType.ntThreadThroughputHeadline:

                    case NodeType.ntCountThreadHeadline:
                    case NodeType.ntCountAppEntriesHeadline:

                    case NodeType.ntCountAppEntries:
                    case NodeType.ntCountThreadEntries:
                    case NodeType.ntCountTotalEntries:

                    //case NodeType.ntCountTotalThreadsActiveHeadline:                                       
                    case NodeType.ntAppThroughputHeadline:
                    case NodeType.ntTotalThroughput:

                    case NodeType.ntTotalStatusHeadline:
                    case NodeType.ntStatusHeadline:

                        if (((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType)
                        {
                            bFound = true;
                            newNode = node;
                        }
                        break;
                    case NodeDefinition.NodeType.ntApp:
                    case NodeType.ntAppThroughput:

                        if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                           (((NodeDefinition)(newNode.Tag)).strAppID == nd.strAppID))
                        {
                            bFound = true;
                            newNode = node;
                        }
                        break;
                    case NodeType.ntThread:
                    case NodeType.ntThreadThroughput:
                        if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                           (((NodeDefinition)(newNode.Tag)).strAppID == nd.strAppID) &&
                            (((NodeDefinition)(newNode.Tag)).strThreadID == nd.strThreadID))
                        {
                            bFound = true;
                            newNode = node;
                        }
                        break;
                    //( 
                    case NodeType.ntEntry:
                        if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                            (((NodeDefinition)(newNode.Tag)).strID == nd.strID))
                        {
                            bFound = false;
                        }
                        break;
                    case NodeType.ntStatusItem:
                    case NodeType.ntStatusFileSize:
                        if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                            (((NodeDefinition)(newNode.Tag)).strID == nd.strID))
                        {
                            bFound = true;
                            newNode = node;
                        }
                        break;
                }
                if (bFound == true)
                    break;
            }
            if (!bFound)
            {
                nodes.Add(newNode);
            }
            if (activeNodes != null)
                activeNodes.Add(newNode);

        }
    }
}
