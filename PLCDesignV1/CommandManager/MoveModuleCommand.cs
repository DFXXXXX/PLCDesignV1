using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLCDesignV1.CommandManager
{
    public class MoveModuleCommand : ICommand
    {
        private readonly Grid _designCanvas;
        private readonly PLCModuleControl _module;
        private readonly int _newColumn;
        private readonly int _newRow;
        private readonly int _oldColumn;
        private readonly int _oldRow;
        private readonly PLCCanvas _plcCanvas;
        private readonly double _newLeft;
        private readonly double _newTop;
        private readonly double _oldLeft;
        private readonly double _oldTop;
        public MoveModuleCommand(Grid designCanvas, PLCModuleControl module, PLCCanvas plcCanvas, int newColumn, int newRow)
        {
            _designCanvas = designCanvas;
            _module = module;
            _newColumn = newColumn;
            _newRow = newRow;
            _oldColumn = Grid.GetColumn(module);
            _oldRow = Grid.GetRow(module);
            _plcCanvas = plcCanvas;
        }


        public void Execute()
        {

            if (!_plcCanvas.PlcModuleManager.MoveModule(_module, _newColumn, _newRow))
            {
                // 位置被占用，不执行命令
                return;
            }
            Grid.SetColumn(_module, _newColumn);
            Grid.SetRow(_module, _newRow);
        }

        public void Unexecute()
        {
            if (!_plcCanvas.PlcModuleManager.MoveModule(_module, _newColumn, _newRow))
            {
                // 位置被占用，不执行命令
                return;
            }
            Grid.SetColumn(_module, _oldColumn);
            Grid.SetRow(_module, _oldRow);
        }
    }

}
