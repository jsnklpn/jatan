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
    public partial class IntegerControl : UserControl, IParameterControl
    {
        public IntegerControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(int);
        }

        public object GetValue()
        {
            return (int) numericUpDown1.Value;
        }
    }
}
