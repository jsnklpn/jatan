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
    public partial class StringControl : UserControl, IParameterControl
    {
        public StringControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(string);
        }

        public object GetValue()
        {
            return textBox1.Text;
        }
    }
}
