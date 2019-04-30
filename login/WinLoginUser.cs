using System;
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
    public partial class WinLoginUser : UserControl
    {
        DbContext db;
        string personnel_entity;
        WinLoginProfile profile; 

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name, family, role(admin, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="personnel_entity_"></param>
        /// <param name="btn_color"></param>
        /// <param name="permission_entity"></param>
        /// <param name="menu"></param>
        /// <param name="password_type"></param>
        /// <param name="enable_child_parent_check"></param>
        public WinLoginUser(DbContext db_, string personnel_entity_, Color btn_color, string permission_entity, MenuStrip menu, Security.HashAlgoritmType password_type, bool enable_child_parent_check, bool unique_user_name_)
        {
            InitializeComponent();
            db = db_;
            personnel_entity = personnel_entity_; 
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color, Color.Black, Color.FromKnownColor(KnownColor.Control));
            }

            if (permission_entity != null) SRL.WinTools.AddChildToParentControls(pnlRoles, new WinRolePermissions(db, permission_entity, btn_color, menu, enable_child_parent_check));

            profile = new WinLoginProfile(db, personnel_entity, btn_color, this, WinLoginProfile.ProfileMode.New, password_type, null, permission_entity, unique_user_name_);
            SRL.WinTools.AddChildToParentControls(pnlProfile, profile);

        }

        public void LoadUsersInDgv()
        {
            dgvUsers.Rows.Clear();
            string sql = "select id,password, name, family, username,role  from " + personnel_entity;
            var users = SRL.Database.SqlQuery<UserClass>(db, sql);
            foreach (var item in users)
            {
                dgvUsers.Rows.Add(item.id, item.password, item.name, item.family, item.username, item.role);

            }
            dgvUsers.ClearSelection();
        }

        private void DeleteUser(long id_del)
        {
            string err = SRL.Database.ExecuteQuery(db, "delete from " + personnel_entity + " where id=" + id_del.ToString() + "");
            if (err != "") MessageBox.Show(err);
        }
        private void WinLoginUser_Load(object sender, EventArgs e)
        {
            LoadUsersInDgv(); 

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count < 1) return;
            if (dgvUsers.SelectedRows[0].Cells["role"].Value.ToString() == UserRoles.admin.ToString()) return;
            long id_del = (long)dgvUsers.SelectedRows[0].Cells["id"].Value;
            DeleteUser(id_del);
            LoadUsersInDgv();
        }



        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            profile.ClearFields();
            int row_selected_count = dgvUsers.SelectedRows.Count;
            btnDel.Enabled = true;
            long? edit_id = null;
            if (row_selected_count > 0)
            {
                btnDel.Enabled = dgvUsers.SelectedRows[0].Cells["role"].Value.ToString() == UserRoles.admin.ToString() ? false : true;
                edit_id = (long)dgvUsers.SelectedRows[0].Cells["id"].Value;
            }

            profile.ChangeMode(dgvUsers.SelectedRows.Count < 1 ? WinLoginProfile.ProfileMode.New : WinLoginProfile.ProfileMode.EditUser, edit_id);

        }





        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvUsers.ClearSelection();
            profile.ChangeMode(WinLoginProfile.ProfileMode.New, null);
        }
    }
}
