using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jatan.GameLogic;
using Jatan.Models;

namespace TestUI
{
    public partial class MainForm : Form
    {
        public GameBoard _gameBoard;
        public ClassController _gameBoardController;
        public GameManager _gameManager;
        public ClassController _gameManagerController;
        public ClassController _selectedController;

        public MainForm()
        {
            InitializeComponent();

            SetupJatanControls();

            cbParentObjectName.SelectedIndex = 0;
            wbObjectViewer.DocumentText = "<h3>(Return objects shown here)</h3>";

            gameBoardViewer1.GameBoard = _gameBoard;
        }

        private void SetupJatanControls()
        {
            _gameManager = new GameManager();
            _gameManagerController = new ClassController(_gameManager);
            _gameBoard = _gameManager.GameBoard;
            _gameBoardController = new ClassController(_gameBoard);

            cbParentObjectName.Items.Add(_gameManagerController);
            cbParentObjectName.Items.Add(_gameBoardController);
        }

        private void PopulateMethodControls()
        {
            panelMethodList.Controls.Clear();
            panelMethodList.AutoScroll = false;
            int top = 0;
            int btnWidth = panelMethodList.Width - 2;
            int btnHeight = 40;
            foreach (var method in _selectedController.Methods)
            {
                var btn = GetButtonFromMethod(method, btnWidth, btnHeight);
                btn.Top = top;
                top += btn.Height + 2;
                panelMethodList.Controls.Add(btn);
            }
            panelMethodList.AutoScroll = true;
        }

        private Button GetButtonFromMethod(MethodCallInfo method, int width, int height)
        {
            var btn = new Button();
            //btn.Text = method.DisplayName;
            btn.Text = string.Format("{0} : {1}", method.MethodSignatureShort, method.ReturnType.Name);
            btn.Left = 0;
            btn.Height = height;
            btn.Width = width;
            btn.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            btn.Click += (s, e) =>
            {
                object returnObject = null;
                List<object> parameters = null;
                if (method.HasParameters)
                {
                    using (var form = new MethodParamForm(method))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                method.Invoke(form.Parameters, out returnObject);
                            }
                            catch (Exception ex)
                            {
                                Dump(ex);
                                tbMethodLog.AppendText(string.Format("{0} ⇒ Exception: {1}\r\n", method.Name, ex.Message));
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    try
                    {
                        method.Invoke(null, out returnObject);
                    }
                    catch (Exception ex)
                    {
                        Dump(ex);
                        tbMethodLog.AppendText(string.Format("{0} ⇒ Exception: {1}\r\n", method.Name, ex.Message));
                        return;
                    }
                }

                Dump(returnObject);
                tbMethodLog.AppendText(string.Format("{0} ⇒ {1}\r\n", method.Name, returnObject ?? "null"));
                // Update board control
                gameBoardViewer1.Invalidate();
            };
            return btn;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbParentObjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedController = cbParentObjectName.SelectedItem as ClassController;
            if (_selectedController != null)
            {
                PopulateMethodControls();
            }
        }

        private void btnDumpSelectedClass_Click(object sender, EventArgs e)
        {
            Dump(_selectedController.ParentObject);
        }

        private void Dump(object obj)
        {
            try
            {
                var html = LINQPad.Util.ToHtmlString(obj);
                wbObjectViewer.DocumentText = html;
            }
            catch (Exception ex)
            {
                wbObjectViewer.DocumentText = ex.ToString();
            }
            
        }
    }

    public class ClassController
    {
        public string Name { get; private set; }
        public List<MethodCallInfo> Methods { get; private set; }
        public object ParentObject { get; private set; }

        public ClassController(object parentObject)
        {
            this.ParentObject = parentObject;
            this.Name = parentObject.GetType().Name;
            this.Methods = new List<MethodCallInfo>();
            foreach (var method in parentObject.GetType().GetMethods())
            {
                this.Methods.Add(new MethodCallInfo(parentObject, method));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class MethodCallInfo
    {
        private object _parentObject;

        public MethodInfo MethodInfo { get; private set; }
        public List<ParameterInfo> Parameters { get; private set; }
        public bool HasParameters { get { return this.Parameters.Any(); } }
        public string Name { get { return MethodInfo.Name; } }
        public string DisplayName { get { return MethodInfo.Name.CamelSpaces().Replace("_", ""); } }
        public Type ReturnType { get { return MethodInfo.ReturnType; } }

        public string MethodSignature
        {
            get
            {
                string parameters = "";
                foreach (var p in this.Parameters)
                {
                    parameters += string.Format("{0} {1}, ", p.ParameterType.Name, p.Name);
                }
                var result = string.Format("{0}({1})", this.Name, parameters.TrimEnd(',', ' '));
                return result;
            }
        }

        public string MethodSignatureShort
        {
            get
            {
                string parameters = "";
                foreach (var p in this.Parameters)
                {
                    parameters += string.Format("{0}, ", p.Name);
                }
                var result = string.Format("{0}({1})", this.Name, parameters.TrimEnd(',', ' '));
                return result;
            }
        }

        public MethodCallInfo(object parentObject, MethodInfo methodInfo)
        {
            _parentObject = parentObject;
            this.MethodInfo = methodInfo;
            this.Parameters = methodInfo.GetParameters().ToList();
        }

        public void Invoke(IEnumerable<object> parameters, out object returnObject)
        {
            object[] p = parameters != null ? parameters.ToArray() : null;
            returnObject = this.MethodInfo.Invoke(_parentObject, p);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
