﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseInterpreter.Model;
using DatabaseInterpreter.Core;
using DatabaseManager.Core;
using DatabaseManager.Helper;

namespace DatabaseManager.Controls
{
    public partial class UC_DbObjectsSimpleTree : UserControl
    {
        public UC_DbObjectsSimpleTree()
        {
            InitializeComponent();
            TreeView.CheckForIllegalCrossThreadCalls = false;
        }

        public async Task LoadTree(DatabaseType dbType, ConnectionInfo connectionInfo)
        {
            this.tvDbObjects.Nodes.Clear();

            DbInterpreterOption option = new DbInterpreterOption() { ObjectFetchMode = DatabaseObjectFetchMode.Simple };
            DbInterpreter dbInterpreter = DbInterpreterHelper.GetDbInterpreter(dbType, connectionInfo, option);

            SchemaInfo schemaInfo = await dbInterpreter.GetSchemaInfoAsync(new SelectionInfo(), DbObjectsTreeHelper.DefaultObjectType);

            this.tvDbObjects.Nodes.AddDbObjectFolderNode(nameof(UserDefinedType), "User Defined Types", schemaInfo.UserDefinedTypes);
            this.tvDbObjects.Nodes.AddDbObjectFolderNode(nameof(Table), "Tables", schemaInfo.Tables);
            this.tvDbObjects.Nodes.AddDbObjectFolderNode(nameof(DatabaseInterpreter.Model.View), "Views", schemaInfo.Views);

            if (this.tvDbObjects.Nodes.Count == 1)
            {
                this.tvDbObjects.ExpandAll();
            }
        }

        public void ClearNodes()
        {
            this.tvDbObjects.Nodes.Clear();
        }

        private void tvDbObjects_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                }
            }
        }

        public SchemaInfo GetSchemaInfo()
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            foreach (TreeNode node in this.tvDbObjects.Nodes)
            {
                foreach (TreeNode item in node.Nodes)
                {
                    if (item.Checked)
                    {
                        switch (node.Name)
                        {
                            case nameof(UserDefinedType):
                                schemaInfo.UserDefinedTypes.Add(item.Tag as UserDefinedType);
                                break;
                            case nameof(Table):
                                schemaInfo.Tables.Add(item.Tag as Table);
                                break;
                            case nameof(DatabaseInterpreter.Model.View):
                                schemaInfo.Views.Add(item.Tag as DatabaseInterpreter.Model.View);
                                break;
                        }
                    }
                }
            }
            return schemaInfo;
        }
    }
}