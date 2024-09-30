using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCDesignV1.CommandManager
{

    public class ChangePropertyCommand<T> : ICommand
    {
        private readonly T _target;
        private readonly string _propertyName;
        private readonly object _newValue;
        private readonly object _oldValue;
        private readonly PLCModuleControl _plcModuleControl;
        private readonly ObservableCollection<CustomEllipsControlDataModel> _originalContainer;

        public ChangePropertyCommand(T target, string propertyName, object newValue,object oldvalue, PLCModuleControl plcModuleControl)
        {
            _target = target;
            _propertyName = propertyName;
            _newValue = newValue;
            _oldValue = oldvalue;
            _plcModuleControl = plcModuleControl;

            var property = typeof(T).GetProperty(_propertyName);
            var convertedValue = Convert.ChangeType(oldvalue, property.PropertyType);
            property.SetValue(_target, convertedValue);
            _originalContainer = _plcModuleControl.GetParameterContainer(_target as CustomEllipsControlDataModel);
            convertedValue = Convert.ChangeType(_newValue, property.PropertyType);
            property.SetValue(_target, convertedValue);
        }

        public void Execute()
        {
            var property = typeof(T).GetProperty(_propertyName);
            var convertedValue = Convert.ChangeType(_newValue, property.PropertyType);
            property.SetValue(_target, convertedValue);

            // 获取新的容器
            var newContainer = _plcModuleControl.GetParameterContainer(_target as CustomEllipsControlDataModel);

            // 如果原先容器和新的容器不一样，删除原先容器中的数据
            if (_originalContainer != null && newContainer != _originalContainer)
            {
                _originalContainer.Remove(_target as CustomEllipsControlDataModel);
            }

            // 在新的容器中进行相应的操作，例如添加或更新参数
            if (newContainer != null && !newContainer.Contains(_target as CustomEllipsControlDataModel))
            {
                newContainer.Add(_target as CustomEllipsControlDataModel);
            }
        }

        public void Unexecute()
        {
            // 获取当前容器
            var currentContainer = _plcModuleControl.GetParameterContainer(_target as CustomEllipsControlDataModel);

            // 如果当前容器和原先容器不一样，删除当前容器中的数据
            if (currentContainer != null && currentContainer != _originalContainer)
            {
                currentContainer.Remove(_target as CustomEllipsControlDataModel);
            }
            var property = typeof(T).GetProperty(_propertyName);
            var convertedValue = Convert.ChangeType(_oldValue, property.PropertyType);
            property.SetValue(_target, convertedValue);
            // 在原先容器中进行相应的操作，例如恢复参数
            if (_originalContainer != null && !_originalContainer.Contains(_target as CustomEllipsControlDataModel))
            {
                _originalContainer.Add(_target as CustomEllipsControlDataModel);
                _plcModuleControl.UpdateParameterUI();
            }
        }
    }

}
