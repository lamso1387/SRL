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
        string entity_name;
        WinSessionId session;
        SRL.Database srl_db = new SRL.Database(); 
        Color btn_color;
        WinLoginProfile profile;

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name, family, role(master, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLoginUser(DbContext db_, string entity_name_, WinSessionId session_, Color btn_color_)
        {
            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;
            btn_color = btn_color_;
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color_, Color.Black,Color.FromKnownColor(KnownColor.Control));
            }

        }

        public void LoadUsersInDgv()
        {
            dgvUsers.Rows.Clear();
            string sql = "select id,password, name, family, username,role  from " + entity_name;
            var users = SRL.Database.SqlQuery<UserClass>(db, sql);
            foreach (var item in users)
            {
                dgvUsers.Rows.Add(item.id, item.password, item.name, item.family, item.username, item.role);

            }
            dgvUsers.ClearSelection();
        }

        private void DeleteUser(long id_del)
        {
            string err = SRL.Database .ExecuteQuery(db, "delete from " + entity_name + " where id=" + id_del.ToString() + "");
            if (err != "") MessageBox.Show(err);
        }
        private void WinLoginUser_Load(object sender, EventArgs e)
        {
            profile = new WinLoginProfile(db, entity_name, session, btn_color, this, WinLoginProfile.ProfileMode.New, null);

            LoadUsersInDgv(); 
            
           

            SRL.WinTools.AddChildToParentControls(pnlProfile, profile);

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
            int row_selected_count=dgvUsers.SelectedRows.Count;   
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
