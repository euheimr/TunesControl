using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tunesControl
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //TODO
            //bring up login first (asks for perms for the account)
            //then pass the login API key and process the info using tunesControlLib

        }

        private void tsSource_Click(object sender, EventArgs e)
        {
            //TODO
            //Open web browser that links to the project source
        }

        private void tsCheckForUpdates_Click(object sender, EventArgs e)
        {
            //TODO
            //run a check for updates, use an AWS for version checking, then download the binary if nescessary
        }

        private void tsOptions_Click(object sender, EventArgs e)
        {

        }

        private void btnConsole_Click(object sender, EventArgs e)
        {
            
        }
    }
}
