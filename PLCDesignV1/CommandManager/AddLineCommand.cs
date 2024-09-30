using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PLCDesignV1.CommandManager
{
    public class AddLineCommand <T>: ICommand where T : Shape
    {
        private readonly T _line;
        private readonly LineManager<T,Ellipse> _lineManager;

        public AddLineCommand( T line,LineManager<T, Ellipse> lineManager)
        {
            _line = line;
            _lineManager = lineManager;
        }

        public void Execute()
        {
            _lineManager.AddLine(_line);
        }

        public void Unexecute()
        {
            _lineManager.RemoveLine(_line);
        }
    }

}
