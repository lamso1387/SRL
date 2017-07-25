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
        public string app_exe_name = "";
        List<string> update_file_list;
        string app_display_name = "";
        string icon_full_path_from_source_directory;

        /// <summary>
        /// for update or you can use is_update=true or you can use is_update=false and remove db and license and .. files from sourec
        /// </summary>
        /// <param name="application_name_without_extention"></param>
        /// <param name="application_to_display_name_"></param>
        /// <param name="directory_install_path_">e.i. @"C:\Program Files\hami\" </param>
        /// <param name="is_update"></param>
        /// <param name="update_file_list_"></param>

        public Setup(string application_name_without_extention, string application_to_display_name_, string directory_install_path_, bool is_update, List<string> update_file_list_ = null, string icon_full_path_from_source_directory_ = null)

        {
            InitializeComponent();
            icon_full_path_from_source_directory = icon_full_path_from_source_directory_;
            dir_install_name = directory_install_path_;
            app_exe_name = application_name_without_extention;
            update_file_list = update_file_list_;
            app_display_name= application_to_display_name_;

            if (is_update) btnCopy.Enabled = false;
            else btnUpdate.Enabled = false;
        }

        private void InstallFiles(Control clicker)
        {
            srl_file.ReplaceAllFilesFromDirToDir(tbSource.Text, tbDestination.Text);
            progressBar1.Value = 60;
            srl_file.MakeShortcut(app_display_name, srl_file.GetDesktopDirectory(), tbDestination.Text, app_exe_name + ".exe", icon_full_path_from_source_directory);
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
            tbSource.Text = srl_file.GetCurrentDirectory();
            tbDestination.Text = dir_install_name;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Control c=sender as Control;
            if(c.Text==end_string) this.Close();
            else InstallFiles(c);

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

                if (update_file_list != null) KeepUpdateFiles();

                InstallFiles(c);
            }
        }     

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
