using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jatan.Models;

namespace TestUI.Controls
{
    public partial class ResourceStackControl : UserControl, IParameterControl
    {
        public ResourceStackControl()
        {
            InitializeComponent();

            enumControl1.SetEnumType(typeof(ResourceTypes));
        }

        public Type GetParameterType()
        {
            return typeof(ResourceStack);
        }

        public object GetValue()
        {
            return new ResourceStack((ResourceTypes) enumControl1.GetValue(), (int) integerControl1.GetValue());
        }
    }
}
