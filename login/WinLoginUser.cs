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
        SRL.WinTools.UserControlValidation srl_valid;

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name, family, role(master, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLoginUser(DbContext db_, string entity_name_, WinSessionId session_, Color btn_color)
        {
            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;

            foreach (var item in new SRL.ChildParent().GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color, Color.Black,Color.FromKnownColor(KnownColor.Control));
            }

        }

        private void LoadUsersInDgv(DataGridView dgv)
        {
            dgv.Rows.Clear();
            string sql = "select ID,password, name, family, username,role  from " + entity_name;
            var users = srl_db.SqlQuery<UserClass>(db, sql);
            foreach (var item in users)
            {
                dgv.Rows.Add(item.ID, item.password, item.name, item.family, item.username, item.role);

            }
            dgv.ClearSelection();
        }

        private void DeleteUser(long id_del)
        {
            string err = srl_db.ExecuteQuery(db, "delete from " + entity_name + " where ID=" + id_del.ToString() + "");
            if (err != "") MessageBox.Show(err);
        }

        private void AddNewUser()
        {
            long? user_id_dup = null;
            if (!CheckUsernameUnique(out user_id_dup)) return;

            string sql = "insert into " + entity_name + "(name,family,username,password, role)" +
                     " values ('" + tbname.Text + "','" + tbFamily.Text + "','" + tbUsername.Text + "','" + tbPass.Text + "','" + tbRole.Text + "')";
            string err = srl_db.ExecuteQuery(db, sql);
            if (err != "") MessageBox.Show(err);
            LoadUsersInDgv(dgvUsers);
        }

        private bool CheckUsernameUnique(out long? user_id_duplicate, long? id_to_edit = null)
        {
            user_id_duplicate = null;
            string sql = "select *  from " + entity_name + " where username='" + tbUsername.Text + "'";
            var user = srl_db.SqlQuery<UserClass>(db, sql);
            if (user.Any())
            {
                user_id_duplicate = user.First().ID;
                if (id_to_edit == user_id_duplicate) return true;
                else
                    MessageBox.Show("نام کاربری تکراری است");
                return false;
            }
            else return true;
        }

        private void ShowUserDataEdit()
        {
            var row = dgvUsers.SelectedRows[0];
            tbname.Text = row.Cells["name"].Value.ToString();
            tbFamily.Text = row.Cells["family"].Value.ToString();
            tbUsername.Text = row.Cells["username"].Value.ToString();
            tbPass.Text = tbPassRep.Text = row.Cells["password"].Value.ToString();
            tbRole.Text = row.Cells["role"].Value.ToString();
        }

        private void EditUser()
        {
            long id_edit = (long)dgvUsers.SelectedRows[0].Cells["id"].Value;
            long? user_id_dup = null;
            if (!CheckUsernameUnique(out user_id_dup, id_edit)) return;

            string err = srl_db.ExecuteQuery(db, "update " + entity_name + " set name='" + tbname.Text +
            "' , family='" + tbFamily.Text + "' , username='" + tbUsername.Text + "' , password='" + tbPass.Text + "' , role='" + tbRole.Text + "' " +
                " where ID=" + id_edit.ToString());
            if (err != "") MessageBox.Show(err);
            LoadUsersInDgv(dgvUsers);
        }
        private void ClearFields()
        {
            new SRL.ChildParent().RefreshFormControls(this, new List<Type> { typeof(TextBox), typeof(ComboBox) });
            tbRole.Text = UserRoles.user.ToString();
        }


        private void WinLoginUser_Load(object sender, EventArgs e)
        {
            LoadUsersInDgv(dgvUsers);
            srl_valid = new WinTools.UserControlValidation(this, errorProvider1, false);
            foreach (var item in new SRL.ChildParent().GetAllChildrenControls(this).OfType<TextBox>())
            {

                srl_valid.ControlValidation(item, WinTools.UserControlValidation.ErrorTypes.NotNull);
            }
            srl_valid.ControlValidation(tbRole, WinTools.UserControlValidation.ErrorTypes.NotNull);

            foreach (var item in new SRL.ChildParent().GetAllChildrenControls(this).OfType<Button>())
            {
              //  new SRL.WinUI.StyleButton(item, Color.Blue, Color.Black);
            }

        }

        

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count < 1) return;
            if (dgvUsers.SelectedRows[0].Cells["role"].Value.ToString() == UserRoles.admin.ToString()) return;
            long id_del = (long)dgvUsers.SelectedRows[0].Cells["id"].Value;
            DeleteUser(id_del);
            LoadUsersInDgv(dgvUsers);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbPass.Text != tbPassRep.Text) return;
            bool res = false;
            srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, tbPass, tbRole }, out res);
            if (res)
            {
                AddNewUser();
                
            }
        }


        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            ClearFields();
            int row_selected_count=dgvUsers.SelectedRows.Count;
            btnEdit.Enabled = row_selected_count > 0;
            btnAdd.Enabled = !btnEdit.Enabled;            

            btnDel.Enabled = true;
            if (row_selected_count > 0)
                btnDel.Enabled = dgvUsers.SelectedRows[0].Cells["role"].Value.ToString() == UserRoles.admin.ToString() ? false : true;
            if (btnAdd.Enabled) ClearFields();
            if (dgvUsers.SelectedRows.Count < 1) return;

            ShowUserDataEdit();

        }

      



        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count < 1) return;
            if (tbPass.Text != tbPassRep.Text) return;

            bool res = false;
            srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, tbPass, tbRole }, out res);
            if (res)
            {
                EditUser();
                
            }

        }



        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvUsers.ClearSelection();
        }
    }
}
