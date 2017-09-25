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
    public partial class WinLoginProfile : UserControl
    {
        public enum ProfileMode
        {
            EditUser,
            New,
            EditProfile
        }
        DbContext db;
        string entity_name;
        WinSessionId session;
        SRL.Database srl_db = new SRL.Database();
        SRL.WinTools.UserControlValidation srl_valid;
        ProfileMode profile_mode;
        WinLoginUser win_login_user;
        long? edit_id;

        public WinLoginProfile(DbContext db_, string entity_name_, WinSessionId session_, Color btn_color, WinLoginUser win_login_user_, ProfileMode profile_mode_, long? edit_id_)
        {
            /* use:
            SRL.WinLoginProfile profile = new SRL.WinLoginProfile(Publics.dbGlobal, typeof(Personnel).Name, Publics.srl_session, Color.Blue,
            null, SRL.WinLoginProfile.ProfileMode.EditProfile, Publics.srl_session.user_id);
            SRL.WinTools.Modal modal = new SRL.WinTools.Modal(profile, "ویرایش پروفایل", profile.Width, profile.Height);
            modal.ShowDialog();
            */

            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;
            profile_mode = profile_mode_;
            win_login_user = win_login_user_;
            edit_id = edit_id_;

            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color, Color.Black, Color.FromKnownColor(KnownColor.Control));
            }

        }

        private void LoadUsersInDgv(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.Rows.Clear();
            string sql = "select id,password, name, family, username,role  from " + entity_name;
            var users = SRL.Database.SqlQuery<UserClass>(db, sql);
            foreach (var item in users)
            {
                dgv.Rows.Add(item.id, item.password, item.name, item.family, item.username, item.role);

            }
            dgv.ClearSelection();
        }

        public void ChangeMode(ProfileMode profile_mode_, long? edit_id_)
        {
            profile_mode = profile_mode_;
            edit_id = edit_id_;

            if (profile_mode == ProfileMode.EditUser || profile_mode == ProfileMode.EditProfile)
            {
                btnEdit.Enabled = true;
                ShowUserDataEdit();
            }
            else btnEdit.Enabled = false;
            btnAdd.Enabled = !btnEdit.Enabled;
            if (btnAdd.Enabled) ClearFields();

        }
        private void DeleteUser(long id_del)
        {
            string err = SRL.Database.ExecuteQuery(db, "delete from " + entity_name + " where id=" + id_del.ToString() + "");
            if (err != "") MessageBox.Show(err);
        }

        public void AddNewUser()
        {
            long? user_id_dup = null;
            if (!CheckUsernameUnique(out user_id_dup)) return;

            string sql = "insert into " + entity_name + "(name,family,username,password, role)" +
                     " values ('" + tbname.Text + "','" + tbFamily.Text + "','" + tbUsername.Text + "','" + tbPass.Text + "','" + tbRole.Text + "')";
            string err = SRL.Database.ExecuteQuery(db, sql);
            if (err != "") MessageBox.Show(err);
            win_login_user.LoadUsersInDgv();
        }

        private bool CheckUsernameUnique(out long? user_id_duplicate, long? id_to_edit = null)
        {
            user_id_duplicate = null;
            string sql = "select *  from " + entity_name + " where username='" + tbUsername.Text + "'";
            var user = SRL.Database.SqlQuery<UserClass>(db, sql);
            if (user.Any())
            {
                user_id_duplicate = user.First().id;
                if (id_to_edit == user_id_duplicate) return true;
                else
                    MessageBox.Show("نام کاربری تکراری است");
                return false;
            }
            else return true;
        }

        public void ShowUserDataEdit()
        {
            string sql = "select *  from " + entity_name + " where id=" + edit_id + "";
            var userL = SRL.Database.SqlQuery<UserClass>(db, sql);
            var user = userL.First();

            //var row = dgv.SelectedRows[0];
            //tbname.Text = row.Cells["name"].Value.ToString();
            //tbFamily.Text = row.Cells["family"].Value.ToString();
            //tbUsername.Text = row.Cells["username"].Value.ToString();
            //tbPass.Text = tbPassRep.Text = row.Cells["password"].Value.ToString();
            //tbRole.Text = row.Cells["role"].Value.ToString();

            tbname.Text = user.name;
            tbFamily.Text = user.family;
            tbUsername.Text = user.username;
            tbPass.Text = tbPassRep.Text = user.password;
            tbRole.Text = user.role;

        }

        private void EditUser()
        {
            long? user_id_dup = null;
            if (!CheckUsernameUnique(out user_id_dup, edit_id)) return;

            string err = SRL.Database.ExecuteQuery(db, "update " + entity_name + " set name='" + tbname.Text +
            "' , family='" + tbFamily.Text + "' , username='" + tbUsername.Text + "' , password='" + tbPass.Text + "' , role='" + tbRole.Text + "' " +
                " where id=" + edit_id.ToString());
            if (err != "") MessageBox.Show(err);
            if (profile_mode == ProfileMode.EditUser) win_login_user.LoadUsersInDgv();
        }
        public void ClearFields()
        {
            SRL.ChildParent.RefreshFormControls(this, new List<Type> { typeof(TextBox), typeof(ComboBox) });
            tbRole.Text = UserRoles.user.ToString();
        }


        private void WinLoginUser_Load(object sender, EventArgs e)
        {
            srl_valid = new WinTools.UserControlValidation(this, errorProvider1, false);
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<TextBox>())
            {

                srl_valid.ControlValidation(item, WinTools.UserControlValidation.ErrorTypes.NotNull);
            }
            srl_valid.ControlValidation(tbRole, WinTools.UserControlValidation.ErrorTypes.NotNull);

            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                //  new SRL.WinUI.StyleButton(item, Color.Blue, Color.Black);
            }


            ChangeMode(profile_mode, edit_id);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbPass.Text != tbPassRep.Text) return;
            bool res = false;
            res = srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, tbPass, tbRole });
            if (res)
            {
                AddNewUser();

            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (profile_mode == ProfileMode.EditUser || profile_mode == ProfileMode.EditProfile)
            {
                if (tbPass.Text != tbPassRep.Text) return;

                bool res = false;
                res = srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, tbPass, tbRole });
                if (res)
                {
                    EditUser();

                }
            }

        }


    }
}