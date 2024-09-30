using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCDesignV1.CommandManager
{
    public class ModifyParameterCommand : ICommand
    {
        private readonly PLCModuleControl _module;
        private readonly CustomEllipsControlDataModel _parameter;
        private readonly CustomEllipsControlDataModel _oldParameter;
        private readonly ObservableCollection<CustomEllipsControlDataModel> _container;
        private readonly bool _isAddOperation;
        private readonly bool _isDeleteOperation;

        public ModifyParameterCommand(PLCModuleControl module, ObservableCollection<CustomEllipsControlDataModel> container, CustomEllipsControlDataModel parameter, CustomEllipsControlDataModel oldParameter = null, bool isAddOperation = false, bool isDeleteOperation = false)
        {
            _module = module;
            _parameter = parameter;
            _container = container;
            _oldParameter = oldParameter?.Clone();
            if (_oldParameter == null)
            {
                _oldParameter = parameter.Clone();
            }
            _isAddOperation = isAddOperation;
            _isDeleteOperation = isDeleteOperation;
        }

        public void Execute()
        {
            if (_isAddOperation)
            {
                if (_parameter.IsInput)
                    _container.Add(_parameter);
                else
                    _container.Add(_parameter);
            }
            else if (_isDeleteOperation)
            {
                if (_parameter.IsInput)
                    _container.Remove(_parameter);
                else
                    _container.Remove(_parameter);
            }
            else
            {
                var collection = _container;
                var index = collection.IndexOf(_oldParameter);
                if (index >= 0)
                    collection[index] = _parameter;
            }
            _module.UpdateParameterUI();
        }

        public void Unexecute()
        {
            if (_isAddOperation)
            {
                if (_parameter.IsInput)
                    _container.Remove(_parameter);
                else
                    _container.Remove(_parameter);
            }
            else if (_isDeleteOperation)
            {
                if (_parameter.IsInput)
                    _container.Add(_oldParameter);
                else
                    _container.Add(_oldParameter);
            }
            else
            {
                var collection = _container;
                var index = collection.IndexOf(_parameter);
                if (index >= 0)
                    collection[index] = _oldParameter;
            }
            _module.UpdateParameterUI();
        }
    }

}
