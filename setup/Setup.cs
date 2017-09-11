using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SRL
{
    public partial class Setup : Form
    {
        string end_string = "پایان";
        SRL.FileManagement srl_file = new SRL.FileManagement();
        string dir_install_name = "";
        string dir_installations_files_name = "";
        public string app_exe_name = "";
        List<string> update_file_list;
        string app_display_name = "";
        string icon_full_path_from_source_directory;

        /// <summary>
        /// for update or you can use is_update_or_null_if_all=true or you can use is_update_or_null_if_all=false and remove db and license and .. files from sourec
        /// </summary>
        /// <param name="application_name_without_extention"></param>
        /// <param name="application_to_display_name_"></param>
        /// <param name="directory_install_path_">e.i. @"C:\Program Files\hami\" </param>
        /// <param name="is_update_or_null_if_all"></param>
        /// <param name="update_file_list_"></param>

        public Setup(string application_name_without_extention, string application_to_display_name_, string directory_install_path_, string directory_installations_files_path_
            , bool? is_update_or_null_if_all, List<string> update_file_list_ = null, string icon_full_path_from_source_directory_ = null)

        {
            //var directory_install_path_ =Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),@"PmLite");
            // var directory_installations_files_path_ = Path.Combine(SRL.FileManagement.GetCurrentDirectory(), "Release");
            InitializeComponent();
            icon_full_path_from_source_directory = icon_full_path_from_source_directory_;
            dir_install_name = directory_install_path_;
            dir_installations_files_name = directory_installations_files_path_;
            app_exe_name = application_name_without_extention;
            update_file_list = update_file_list_;
            app_display_name = application_to_display_name_;

            if (is_update_or_null_if_all != null)
            {
                if ((bool)is_update_or_null_if_all) btnCopy.Enabled = false;
                else btnUpdate.Enabled = false;
            }
        }

        private void InstallFiles(Control clicker, List<string> update_file_list = null)
        {
            SRL.FileManagement.ReplaceAllFilesFromDirToDir(tbSource.Text, tbDestination.Text, update_file_list);
            progressBar1.Value = 60;
            SRL.FileManagement.MakeShortcut(app_display_name, SRL.FileManagement.GetDesktopDirectory(), tbDestination.Text, app_exe_name + ".exe", icon_full_path_from_source_directory);
            progressBar1.Value = 100;
            clicker.Text = end_string;
            button1.Enabled = false;



        }

        private void KeepUpdateFiles()
        {
            foreach (var item in update_file_list)
            {
                File.Copy(tbDestination.Text + item, tbSource.Text + item, true);
            }

        }

        private void Setup_Load(object sender, EventArgs e)
        {
            tbSource.Text = dir_installations_files_name;
            tbDestination.Text = dir_install_name;
            this.Text = app_exe_name;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {

            Control c = sender as Control;
            if (c.Text == end_string) this.Close();
            else
            {
                if (SRL.MessageBoxForm2.Show("با نصب برنامه اگر قبلا برنامه نصب بوده باشد اطلاعات از بین می رود. آیا برای نصب مطئن هستید؟", "تایید نصب", MessageBoxForm2.Buttons.YesNo) == DialogResult.No) return;
                else InstallFiles(c);
            }

        }

        private void btnBrowsSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            tbSource.Text = folderBrowserDialog1.SelectedPath + @"\";

        }

        private void btnBrowsDes_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            tbDestination.Text = folderBrowserDialog1.SelectedPath + @"\";

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c.Text == end_string) this.Close();
            else
            {

                //   if (update_file_list != null) KeepUpdateFiles();
                SRL.MessageBoxForm2.Show("برای بروزرسانی لازم است محل نصب نرم افزار بدرستی انتخاب شود");
                InstallFiles(c, update_file_list);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
