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
    public partial class EnumControl : UserControl, IParameterControl
    {
        private Type _paramType;

        /// <summary>
        /// Default constructor. Do not use. Required for the designer.
        /// </summary>
        public EnumControl()
            : this(typeof(DayOfWeek))
        {
        }

        public EnumControl(Type enumType)
        {
            InitializeComponent();
            SetEnumType(enumType);
        }

        public void SetEnumType(Type enumType)
        {
            _paramType = enumType;

             this.comboBox1.Items.Clear();
            foreach (var value in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, value);
                this.comboBox1.Items.Add(new EnumValue() { Name = name, Value = value });
            }
            this.comboBox1.SelectedIndex = 0;
        }

        public Type GetParameterType()
        {
            return _paramType;
        }

        public object GetValue()
        {
            var selectedItem = (EnumValue)comboBox1.SelectedItem;
            return selectedItem.Value;
        }
    }

    public class EnumValue
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
