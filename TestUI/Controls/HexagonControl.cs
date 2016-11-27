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
    public partial class HexagonControl : UserControl, IParameterControl
    {
        public HexagonControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(Jatan.Core.Hexagon);
        }

        public object GetValue()
        {
            return new Jatan.Core.Hexagon((int) numericUpDown1.Value, (int) numericUpDown2.Value);
        }
    }
}
