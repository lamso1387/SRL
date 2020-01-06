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
        string personnel_entity;
        SRL.WinTools.UserControlValidation srl_valid;
        private ProfileMode profile_mode { get; set; }
        WinLoginUser win_login_user { get; set; }
        long? edit_id;
        string permission_entity;
        bool unique_user_name;
        Security.HashAlgoritmType password_type;

        public WinLoginProfile(DbContext db_, string personnel_entity_, Color btn_color, WinLoginUser win_login_user_, ProfileMode profile_mode_, Security.HashAlgoritmType password_type_, long? edit_id_, string permission_entity_, bool uniqe_user_name_)
        {
            /* use:
            SRL.WinLoginProfile profile = new SRL.WinLoginProfile(Publics.dbGlobal, typeof(Personnel).Name, Publics.srl_session, Color.Blue,
            null, SRL.WinLoginProfile.ProfileMode.EditProfile, Publics.srl_session.user_id);
            SRL.WinTools.Modal modal = new SRL.WinTools.Modal(profile, "ویرایش پروفایل", profile.Width, profile.Height);
            modal.ShowDialog();
            */

            InitializeComponent();
            db = db_;
            personnel_entity = personnel_entity_;
            permission_entity = permission_entity_;
            profile_mode = profile_mode_;
            win_login_user = win_login_user_;
            edit_id = edit_id_;
            password_type = password_type_;
            unique_user_name = uniqe_user_name_;

            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<Button>())
            {
                new SRL.WinUI.ButtonClass.StyleButton(item, btn_color, Color.Black, Color.FromKnownColor(KnownColor.Control));
            }

        }

        private void LoadUsersInDgv(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.Rows.Clear();
            string sql = "select id,password, name, family, username,role  from " + personnel_entity;
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

            if (profile_mode == ProfileMode.EditUser)
            {
                btnEdit.Enabled = true;
                if (permission_entity != null) cbRole.Enabled = true;
                ShowUserDataEdit();
            }
            else if (profile_mode == ProfileMode.EditProfile)
            {
                btnEdit.Enabled = true;
                cbRole.Enabled = false;
                ShowUserDataEdit();
            }
            else
            {
                btnEdit.Enabled = false;
                cbRole.Enabled = true;
            }
            btnAdd.Enabled = !btnEdit.Enabled;
            if (btnAdd.Enabled) ClearFields();

        }
        private void DeleteUser(long id_del)
        {
            string err = SRL.Database.ExecuteQuery(db, "delete from " + personnel_entity + " where id=" + id_del.ToString() + "");
            if (err != "") MessageBox.Show(err);
        }

        public void AddNewUser(bool unique)
        {
            long? user_id_dup = null;
            if (unique && !CheckUsernameUnique(out user_id_dup)) return;

            string sql = "insert into " + personnel_entity + "(name,family,username,password, role)" +
                     " values ('" + tbname.Text + "','" + tbFamily.Text + "','" + tbUsername.Text + "','" + SRL.Security.GetHashString(tbPass.Text, password_type) + "','" + cbRole.Text + "')";
            string err = SRL.Database.ExecuteQuery(db, sql);
            if (err != "") MessageBox.Show(err);
            win_login_user.LoadUsersInDgv();
        }

        private bool CheckUsernameUnique(out long? user_id_duplicate, long? id_to_edit = null)
        {
            user_id_duplicate = null;
            string sql = "select *  from " + personnel_entity + " where username='" + tbUsername.Text + "'";
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
            string sql = "select *  from " + personnel_entity + " where id=" + edit_id + "";
            var userL = SRL.Database.SqlQuery<UserClass>(db, sql);
            var user = userL.First();

            tbname.Text = user.name;
            tbFamily.Text = user.family;
            tbUsername.Text = user.username;
            cbRole.Text = user.role;

        }

        private void EditUser()
        {
            long? user_id_dup = null;
            if (!CheckUsernameUnique(out user_id_dup, edit_id)) return;

            string update_sql = "update " + personnel_entity + " set name='" + tbname.Text +
            "' , family='" + tbFamily.Text + "' , username='" + tbUsername.Text + "' , role='" + cbRole.Text + "' ";
            if (!string.IsNullOrWhiteSpace(tbPass.Text)) update_sql += " , password = '" + SRL.Security.GetHashString(tbPass.Text, password_type) + "'";
            update_sql +=" where id=" + edit_id.ToString();

            string err = SRL.Database.ExecuteQuery(db, update_sql);
            if (err != "") MessageBox.Show(err);
            if (profile_mode == ProfileMode.EditUser) win_login_user.LoadUsersInDgv();
        }
        public void ClearFields()
        {
            SRL.ChildParent.RefreshFormControls(this, new List<Type> { typeof(TextBox), typeof(ComboBox) });
            if (permission_entity != null)
            {
                cbRole.Enabled = true;
                cbRole.DataSource = SRL.Security.RolePermissionManagement.GetAllRoles(permission_entity, db);

            }
            else
            {
                cbRole.Enabled = false;
                cbRole.Text = UserRoles.user.ToString();
            }
        }


        private void WinLoginUser_Load(object sender, EventArgs e)
        {
            srl_valid = new WinTools.UserControlValidation(this, errorProvider1, false);
            foreach (var item in SRL.ChildParent.GetAllChildrenControls(this).OfType<TextBox>())
            {
                if (item==tbPass || item==tbPassRep) continue;
                srl_valid.ControlValidation(item, WinTools.UserControlValidation.ErrorTypes.NotNull);
            }
            srl_valid.ControlValidation(cbRole, WinTools.UserControlValidation.ErrorTypes.NotNull);

            ChangeMode(profile_mode, edit_id);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbPass.Text != tbPassRep.Text) return;
            bool res = false;
            res = srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, tbPass, cbRole });
            if (res)
            {
                AddNewUser(unique_user_name); 
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (profile_mode == ProfileMode.EditUser || profile_mode == ProfileMode.EditProfile)
            {
                if (tbPass.Text != tbPassRep.Text) return;

                bool res = false;
                res = srl_valid.CheckAllField(new List<Control> { tbname, tbFamily, tbUsername, cbRole });
                if (res)
                {
                    EditUser();

                }
            }

        }


    }
}