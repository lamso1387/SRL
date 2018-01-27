using EasyTabs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// install Easy Tabs in manage nueget pakage.Add  AppContainer Form to your project. use project namespace.replce Program class. add some code to Form1
/// </summary>
namespace sahmiye
{
    //your application should have Form1
    /*just add this before Form1() constructor:
    protected TitleBarTabs ParentTabs
    {
        get
        {
            return (ParentForm as TitleBarTabs);
        }
    }
    */
    public partial class AppContainer : TitleBarTabs
    {
        public AppContainer()
        {
            InitializeComponent();

            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
        }

        // Handle the method CreateTab that allows the user to create a new Tab
        // on your app when clicking
        public override TitleBarTab CreateTab()
        {
            return new TitleBarTab(this)
            {
                // The content will be an instance of another Form
                // In our example, we will create a new instance of the Form1
                Content = new Form1
                {
                    Text = "New Tab"
                }
            };
        }
        

        // The rest of the events in your app here if you need to .....
    }

    //replce this with your Program class
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppContainer container = new AppContainer();

            // Add the initial Tab
            container.Tabs.Add(
                // Our First Tab created by default in the Application will have as content the Form1
                new TitleBarTab(container)
                {
                    Content = new Form1
                    {
                        Text = "New Tab"
                    }
                }
            );

            // Set initial tab the first one
            container.SelectedTabIndex = 0;

            // Create tabs and start application
            TitleBarTabsApplicationContext applicationContext = new TitleBarTabsApplicationContext();
            applicationContext.Start(container);
            Application.Run(applicationContext);
        }
    }
}
