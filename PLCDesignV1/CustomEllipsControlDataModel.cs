using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PLCDesignV1
{
    [Serializable]
    public class CustomEllipsControlDataModel : INotifyPropertyChanged
    {
        private string _name;
        private bool isInput = false;
        private string address;
        private string symbol;
        private string variableType;
        private string dataType;
        private string comment;

        public string Name
        {
            get => _name;
            set
            {
                this.RaisePropertyChanging("Name");
                _name = value;
                this.RaisePropertyChanged("Name");
            }
        }
        public bool IsInput
        {
            get => isInput;
            set
            {
                if (value != true && value != false)
                {
                    throw new ArgumentException("IsInput 属性只能接受 true 或 false。");
                }
                isInput = value;
                RaisePropertyChanged();
            }
        }

        public string Address { get => address; set { this.RaisePropertyChanging("Address"); address = value; this.RaisePropertyChanged("Address"); } }
        public string Symbol { get => symbol; set { this.RaisePropertyChanging("Symbol"); symbol = value; this.RaisePropertyChanged("Symbol"); } }
        public string VariableType { get => variableType; set { this.RaisePropertyChanging("VariableType"); variableType = value; this.RaisePropertyChanged("VariableType"); } }
        public string DataType { get => dataType; set { this.RaisePropertyChanging("DataType"); dataType = value; this.RaisePropertyChanged("DataType"); } }
        public string Comment { get => comment; set { this.RaisePropertyChanging("Comment"); comment = value; this.RaisePropertyChanged("Comment"); } }

        // 与 CustomEllipseControl 绑定的属性

        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //判断两个实例的参数是否都是一样的

        public  bool Equal( CustomEllipsControlDataModel value2)
        {
            if (this == null && value2 == null) return true;
            if (this.isInput != value2.isInput) return false;
            if (this.Name != value2.Name) return false;
            if (this.Address != value2.Address) return false;
            if (this.Symbol != value2.Symbol) return false;
            if (this.VariableType != value2.VariableType) return false;
            if (this.Comment != value2.Comment) return false;
            if (this.DataType != value2.DataType) return false;
            return true;
        }

        public CustomEllipsControlDataModel Clone()
        {
            return new CustomEllipsControlDataModel
            {
                Name = this.Name,
                IsInput = this.IsInput,
                Address = this.Address,
                Symbol = this.Symbol,
                VariableType = this.VariableType,
                DataType = this.DataType,
                Comment = this.Comment
            };
        }

    }

}
