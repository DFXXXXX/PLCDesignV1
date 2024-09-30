using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLCDesignV1.CommandManager
{
    public class RemoveModuleCommand : ICommand
    {
        private readonly Grid _designCanvas;
        private readonly PLCModuleControl _module;
        private readonly int _column;
        private readonly int _row;
        private readonly PLCCanvas _pLCCanvas;

        public RemoveModuleCommand(PLCCanvas pLCCanvas, PLCModuleControl module, int column, int row)
        {
            _pLCCanvas = pLCCanvas;
            _designCanvas = pLCCanvas.DesignCanvas;
            _module = module;
            _column = column;
            _row = row;
        }



        public void Execute()
        {
            _pLCCanvas.PlcModuleManager.RemoveModule(_module, _column, _row);
            _designCanvas.Children.Remove(_module);
            _designCanvas.Focus();
        }

        public void Unexecute()
        {
            if (_module.Parent != null)
            {
                var parent = _module.Parent as Panel;
                parent?.Children.Remove(_module);
            }
            _pLCCanvas.PlcModuleManager.AddModule(_module, _column, _row);
            Grid.SetColumn(_module, _column);
            Grid.SetRow(_module, _row);
            _designCanvas.Children.Add(_module);
        }
    }
}
