using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestUI.Controls
{
    public partial class BooleanControl : UserControl, IParameterControl
    {
        public BooleanControl()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        public Type GetParameterType()
        {
            return typeof(bool);
        }

        public object GetValue()
        {
            if (comboBox1.SelectedText == "True")
                return true;
            else return false;
        }
    }
}
