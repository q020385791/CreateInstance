using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateInstance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ServerCenter server = new ServerCenter(this);
                Thread ThreadServer = new Thread(new ThreadStart(server.Run));
                ThreadServer.Start();
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
        }
    }
}
