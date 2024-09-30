using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLCDesignV1.CommandManager
{
    public class AddModuleCommand : ICommand
    {
        private readonly Grid _designCanvas;
        private readonly PLCModuleControl _module;
        private readonly int _column;
        private readonly int _row;
        private readonly PLCCanvas _plcCanvas;

        public AddModuleCommand(Grid designCanvas, PLCModuleControl module, PLCCanvas plcCanvas, int column, int row)
        {
            _designCanvas = designCanvas;
            _module = module;
            _column = column;
            _row = row;
            _plcCanvas = plcCanvas;
        }
       
        public void Execute()
        {
            if (_module.Parent != null)
            {
                var parent = _module.Parent as Panel;
                parent?.Children.Remove(_module);
            }
            if (!_plcCanvas.PlcModuleManager.AddModule(_module, _column, _row))
            {
                MessageBox.Show("模块不能重叠");
                return;
            }
            Grid.SetColumn(_module, _column);
            Grid.SetRow(_module, _row);
            _designCanvas.Children.Add(_module);
        }

        public void Unexecute()
        {
            _plcCanvas.PlcModuleManager.RemoveModule(_module, _column, _row);
            _designCanvas.Children.Remove(_module);
            _designCanvas.Focus();
        }
    }
}
