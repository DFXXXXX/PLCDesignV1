using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PLCDesignV1.CommandManager
{
    public class RemoveLineCommand <T>: ICommand where T : Shape
    {

        private readonly T _line;
        private readonly LineManager<T,Ellipse> _lineManager;

        public RemoveLineCommand(T line,LineManager<T, Ellipse> lineManager)
        {

            _line = line;
            _lineManager = lineManager;
  
        }

        public void Execute()
        {
            _lineManager.RemoveLine(_line);
        }

        public void Unexecute()
        {
            _lineManager.AddLine(_line);
        }
    }

}
