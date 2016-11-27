using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jatan.Core;

namespace TestUI.Controls
{
    public partial class HexPointControl : UserControl, IParameterControl
    {
        public HexPointControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(HexPoint);
        }

        public object GetValue()
        {
            return new HexPoint(
                (Hexagon)hexagonControl1.GetValue(),
                (Hexagon)hexagonControl2.GetValue(),
                (Hexagon)hexagonControl3.GetValue());
        }
    }
}
