using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCDesignV1.CommandManager
{
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }
}
