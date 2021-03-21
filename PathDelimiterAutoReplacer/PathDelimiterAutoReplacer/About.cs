using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathDelimiterAutoReplacer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            ResourceManager rm = Properties.Resources.ResourceManager;
            this.Icon = (Icon)rm.GetObject("icon");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel)sender).Text);
        }
    }
}
