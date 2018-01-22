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
        string permission_entity;
        WinSessionId session;
        SRL.Database srl_db = new SRL.Database();
        Color btn_color;
        WinLoginProfile profile;
        bool multi_admin;
        MenuStrip menu;
        Security.HashAlgoritmType password_type;

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name, family, role(master, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLoginUser(DbContext db_, string personnel_entity_, WinSessionId session_, Color btn_color_, string permission_entity_, MenuStrip menu_, bool multi_admin_, Security.HashAlgoritmType password_type_)
        {
            InitializeComponent();
            db = db_;
            personnel_entity = personnel_entity_;
            permission_entity = permission_entity_;
            session = session_;
            btn_color = btn_color_;
            multi_admin = multi_admin_;
            menu = menu_;
            password_type = password_type_;
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color_, Color.Black, Color.FromKnownColor(KnownColor.Control));
            }

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
            profile = new WinLoginProfile(db, personnel_entity, session, btn_color, this, WinLoginProfile.ProfileMode.New,password_type, null,multi_admin,permission_entity);

            LoadUsersInDgv();

            SRL.WinTools.AddChildToParentControls(pnlProfile, profile);

            if (permission_entity != null) SRL.WinTools.AddChildToParentControls(pnlRoles, new WinRolePermissions(db, permission_entity, session, btn_color, menu));

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
