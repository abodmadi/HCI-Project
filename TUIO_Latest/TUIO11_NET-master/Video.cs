using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuioDemo1
{
    public partial class Video : Form
    {
        public Video(string path)
        {
            InitializeComponent();
            //MessageBox.Show( TuioDemo.vvv.ToString());
            MediaPlayer.URL = path;
            MediaPlayer.Ctlcontrols.play();
        }
    }
}
