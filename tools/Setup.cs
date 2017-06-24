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
        SRL.FileManagement srl_file = new SRL.FileManagement();
        string dir_install_name = "";
        string app_name = "";
        string db_name = "";
        public Setup(string directory_install_name="", string application_name="", string database_name="")
        {
            InitializeComponent();
            dir_install_name =GetDirName(directory_install_name);
            app_name=GetAppName(application_name);
            db_name = GetDbName(database_name);
        }

        
        private string SearchForAppName()
        {
            string folder_install_name = "AppDefault";
            var dirs = System.IO.Directory.GetFiles(new SRL.FileManagement().GetCurrentDirectory(), "*.exe")
                .Where(name => !name.EndsWith("Setup.exe") && !name.EndsWith(".vshost.exe"));
            folder_install_name = System.IO.Path.GetFileNameWithoutExtension(dirs.DefaultIfEmpty(folder_install_name).FirstOrDefault()).ToString();
            return folder_install_name;
        }
        private string SearchForDbName()
        {
            string name = "MyDatabase.sqlite";
            var dirs = System.IO.Directory.GetFiles(new SRL.FileManagement().GetCurrentDirectory(), "*.sqlite");

            name = System.IO.Path.GetFileName(dirs.DefaultIfEmpty(name).FirstOrDefault()).ToString();
            return name;
        }
        private string GetDbName(string database_name)
        {
            if (database_name != "")
                return database_name;
            else
            {
                return SearchForDbName();
            }
        }
        private string GetAppName(string application_name)
        {
            if (application_name != "")
                return application_name;
            else
            {
                return SearchForAppName();
            }
        }

        private string GetDirName(string directory_install_name)
        {
            if (directory_install_name != "")
                return directory_install_name;
            else
            {
                return SearchForAppName();
            }
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            tbSource.Text = srl_file.GetCurrentDirectory();
            tbDestination.Text = @"C:\Program Files\"+dir_install_name+@"\";
            tbappname.Text = app_name;
            tbDb.Text = db_name;
        }

       

        private void btnCopy_Click(object sender, EventArgs e)
        {
            InstallFiles();

        }

        private void InstallFiles()
        {
            srl_file.ReplaceAllFilesFromDirToDir(tbSource.Text, tbDestination.Text);
            srl_file.MakeShortcut(tbappname.Text, srl_file.GetDesktopDirectory(), tbDestination.Text + app_name + ".exe");
            this.Close();
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
            string old_db = tbDestination.Text + tbDb.Text;
            File.Copy(old_db, tbSource.Text + tbDb.Text, true);

            InstallFiles();
        }

        private void btnDb_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            tbDb.Text = System.IO.Path.GetFileName(openFileDialog1.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
