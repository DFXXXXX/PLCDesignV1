using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    public class Connection
    {
        private Line _connectionLine;
        private PLCModuleControl _startModule;
        private PLCModuleControl _endModule;

        public Connection(PLCModuleControl startModul, PLCModuleControl endModule)
        {
            _startModule = startModul;
            _endModule = endModule;
        }

       


    }


}
