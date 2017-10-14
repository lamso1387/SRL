﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace SRL
{
    public partial class WinRolePermissions : UserControl
    {

        DbContext db;
        string permission_entity;
        WinSessionId session;
        SRL.Database srl_db = new SRL.Database();
        Color btn_color;
        MenuStrip menu;
        public WinRolePermissions(DbContext db_, string permission_entity_, WinSessionId session_, Color btn_color_, MenuStrip menu_)
        {
            InitializeComponent();
            db = db_;
            permission_entity = permission_entity_;
            session = session_;
            menu = menu_;
            btn_color = btn_color_;
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color_, Color.Black, Color.FromKnownColor(KnownColor.Control));
            }
        }

        public void LoadRoles()
        {
            string sql = "select *  from " + permission_entity;
            var users = SRL.Database.SqlQuery<RoleClass>(db, sql);
            if (users == null ? false : users.Any())
            {
                var dt = SRL.Convertor.IEnumerableToDatatable.CopyToDataTable(users.Select(x => new { x.id, x.role, x.permission }));
                dgvRoles.DataSource = dt;
            }
 
        }

        private void WinRolePermissions_Load(object sender, EventArgs e)
        {
            SRL.Convertor.ConvertMenuToTreeView(menu, tvPermissions);
            dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LoadRoles();
            dgvRoles.CellEndEdit += DgvRoles_CellEndEdit;
            dgvRoles.UserDeletingRow += DgvRoles_UserDeletingRow;
            dgvRoles.UserDeletedRow += DgvRoles_UserDeletedRow;
            SRL.ChildParent.CompatibleTreeChildAndParentCheck(tvPermissions);
        }

        private void DgvRoles_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            SRL.Security.RolePermissionManagement.DeleteRole(permission_entity, dgvRoles.Rows[e.Row.Index].Cells["id"].Value, db);
        }

        private void DgvRoles_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            LoadRoles();
        }


        private void DgvRoles_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SRL.ActionManagement.MethodCall.MethodDynamicInvoker(() => AddOrEditNewRole(e.ColumnIndex, e.RowIndex),this);
        }
        public void AddOrEditNewRole(int col, int row)
        {
            if (SRL.Security.RolePermissionManagement.AddOrEditNewRole(permission_entity, dgvRoles[col, row].Value, db, dgvRoles.Rows[row].Cells["id"].Value))
                LoadRoles();
        }
        private void dgvRoles_SelectionChanged(object sender, EventArgs e)
        {
            var rows = dgvRoles.SelectedRows;
            if (rows.Count > 0)
            {
                SRL.TreeMenuAccess.LoadPermissionsInTree(rows[0].Cells["permission"].Value, tvPermissions);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var rows = dgvRoles.SelectedRows;
            if (rows.Count > 0)
            {
                SRL.TreeMenuAccess.SavePermissionFromTree(rows[0].Cells["role"].Value, tvPermissions, permission_entity, db);
                LoadRoles();
            }
        }
    }
}