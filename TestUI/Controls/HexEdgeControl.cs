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
    public partial class HexEdgeControl : UserControl, IParameterControl
    {
        public HexEdgeControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(HexEdge);
        }

        public object GetValue()
        {
            return new HexEdge((Hexagon)hexagonControl1.GetValue(), (Hexagon)hexagonControl2.GetValue());
        }
    }
}
