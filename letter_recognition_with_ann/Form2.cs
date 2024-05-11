using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace letter_recognition_with_ann
{
    public partial class Form2 : Form
    {
        private ANeuralNetwork ann;
        public Form2(ANeuralNetwork ann)
        {
            InitializeComponent();
            this.ann = ann;
            this.CenterToScreen();
        }

        private void yes_Click(object sender, EventArgs e)
        {
            ann.WriteWeightsAndBiasesToFile("values.txt");
            this.Close();
        }

        private void no_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
