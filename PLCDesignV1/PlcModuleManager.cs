using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PLCDesignV1
{
    public class PlcModuleManager
    {
        private Dictionary<(int, int), PLCModuleControl> _moduleDict = new Dictionary<(int, int), PLCModuleControl>();
        public PlcModuleManager() { }

        // 添加模块
        public void RemoveModule(PLCModuleControl module,int cloumn,int row)
        {
            var mudule= GetModulePosition(module);
            if(mudule!=null)
            _moduleDict.Remove(((int, int))mudule);

        }

        public Dictionary<(int, int), PLCModuleControl> GetModule()
        { 
            return _moduleDict;
        }
        public bool IsTargetAreaOccupied(int targetColumn, int targetRow, int columnSpan, int rowSpan)
        {
            Rect targetRect = new Rect(targetColumn * MainWindow.GridCellSize, targetRow * MainWindow.GridCellSize, columnSpan * MainWindow.GridCellSize, rowSpan * MainWindow.GridCellSize);

            foreach (var c in _moduleDict)
            {
                var point = c.Key;
                var module = c.Value;
                Rect moduleRect = new Rect(point.Item1 * MainWindow.GridCellSize, point.Item2 * MainWindow.GridCellSize, module.CalculatedColumnSpan * MainWindow.GridCellSize, module.CalculatedRowSpan * MainWindow.GridCellSize);

                if (moduleRect.IntersectsWith(targetRect))
                {
                    return true; // 目标区域被占用
                }
            }
            return false; // 目标区域未被占用
        }

        public bool AddModule(PLCModuleControl module, int column, int row)
        {
            module.UpdateParameterUI();
            if (IsPositionOccupied(column, row, module.CalculatedColumnSpan, module.CalculatedRowSpan,null))
            {
                return false; // 位置被占用
            }

            _moduleDict[(column, row)] = module;
            //Grid.SetColumn(module, column);
            //Grid.SetRow(module, row);
            return true;
        }

        // 移动模块
        public bool MoveModule(PLCModuleControl module, int newColumn, int newRow)
        {
            var oldPosition = GetModulePosition(module);

        
            if (oldPosition == null || IsPositionOccupied(newColumn, newRow, module.CalculatedColumnSpan, module.CalculatedRowSpan,oldPosition))
            {
                return false; // 位置被占用或模块不存在
            }
            (int, int) old = ((int, int))oldPosition;
            _moduleDict.Remove(old); 
            _moduleDict[(newColumn, newRow)] = module;
            //Grid.SetColumn(module, newColumn);
            //Grid.SetRow(module, newRow);
            return true;
        }

        // 检查位置是否被占用
        public bool IsPositionOccupied(int column, int row, int columnSpan, int rowSpan,(int,int)?oldpoint)
        {
            foreach (var c in _moduleDict)
            {
         
                var point = c.Key;
                Debug.WriteLine($"{point}  {oldpoint}" );
                if (oldpoint == point)
                    continue;
                var module = c.Value;
                // 获取模块的矩形区域
                Rect moduleRect = new Rect(point.Item1 * MainWindow.GridCellSize, point.Item2 * MainWindow.GridCellSize, module.CalculatedColumnSpan * MainWindow.GridCellSize, module.CalculatedRowSpan * MainWindow.GridCellSize);

                // 创建待检查的矩形区域
                Rect checkRect = new Rect(column * MainWindow.GridCellSize, row * MainWindow.GridCellSize, columnSpan * MainWindow.GridCellSize, rowSpan * MainWindow.GridCellSize);

                Debug.WriteLine($" grid {point.Item1},{point.Item2} moduleRect {moduleRect}   {column},{row} checkRect {checkRect}");

                // 判断两个矩形是否相交
                if (moduleRect.IntersectsWith(checkRect))
                {
                    return true; // 位置被占用
                }
             


            }
            return false;
        }

        // 获取模块的位置
        private (int, int)? GetModulePosition(PLCModuleControl module)
        {
            foreach (var kvp in _moduleDict)
            {
                if (kvp.Value == module)
                {
                    return kvp.Key;
                }
            }
            return null;
        }
    }
}
