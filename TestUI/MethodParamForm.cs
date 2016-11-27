using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestUI.Controls;

namespace TestUI
{
    public partial class MethodParamForm : Form
    {
        public List<Object> Parameters { get; private set; }

        private List<Control> _parameterControls;
        private MethodCallInfo _methodCallInfo;

        public MethodParamForm(MethodCallInfo methodInfo)
        {
            _methodCallInfo = methodInfo;
            
            InitializeComponent();

            lblMethodTitle.Text = string.Format("{0} : {1}", methodInfo.MethodSignatureShort, methodInfo.ReturnType.Name);
            this.Parameters = new List<object>();
            _parameterControls = new List<Control>();

            int rowIndex = 0;
            foreach (var parameter in methodInfo.Parameters)
            {
                var lbl = new Label();
                tableLayoutPanel1.Controls.Add(lbl, 0, rowIndex);
                lbl.Dock = DockStyle.Fill;
                lbl.Text = string.Format("{0} : {1}", parameter.Name, parameter.ParameterType.Name);

                var ctrl = ParameterControlFactory.GetControl(parameter);
                tableLayoutPanel1.Controls.Add(ctrl, 1, rowIndex);
                ctrl.Dock = DockStyle.Fill;
                _parameterControls.Add(ctrl);
                rowIndex++;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Parameters.Clear();
                for (int i = 0; i < _parameterControls.Count; i++)
                {
                    var ctrl = _parameterControls[i];
                    if (ctrl is IParameterControl)
                    {
                        Parameters.Add(((IParameterControl)ctrl).GetValue());
                    }
                    else
                    {
                        var type = _methodCallInfo.Parameters[i].ParameterType;
                        if (type.IsValueType)
                            Parameters.Add(Activator.CreateInstance(type));
                        else
                            Parameters.Add(null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception thrown while constructing parameters:\r\n\r\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
