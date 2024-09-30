using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection<PLCModuleControl> PLCModules { get; set; }


        public static readonly DependencyProperty PLCModulesProperty =
     DependencyProperty.Register(
         "PLCModules",
         typeof(ObservableCollection<PLCModuleControl>),
         typeof(PLCModuleControl),
         new PropertyMetadata(new ObservableCollection<PLCModuleControl>())
     );

        public ObservableCollection<PLCModuleControl> PLCModules
        {
            get { return (ObservableCollection<PLCModuleControl>)GetValue(PLCModulesProperty); }
            set { SetValue(PLCModulesProperty, value); }
        }



        public const int GridCellSize = 32;

        public const int GridCellHeightRate = 3;
        public const int GridCellWidthRate = 4;

        public const int GridCellRectangleHeight = GridCellSize * GridCellHeightRate;

        public const int GridCellRectanglWidth = GridCellSize * GridCellWidthRate;
        public MainWindow()
        {
            //InitializeComponent();

            PLCModules = new ObservableCollection<PLCModuleControl>();
            PLCModuleControl pLCModuleControl = new PLCModuleControl();
            pLCModuleControl.PLCName = "PLC A";
            pLCModuleControl.Width = 100;
            pLCModuleControl.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input1", Address = "A1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl.OutputParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "output", Address = "B1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            PLCModules.Add(pLCModuleControl);
            pLCModuleControl.UpdateParameterUI();
            PLCModuleControl pLCModuleControl2 = new PLCModuleControl();
            pLCModuleControl2.PLCName = "PLC B";
            pLCModuleControl2.Width = 100;
            pLCModuleControl2.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input1", Address = "A1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl2.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input1", Address = "A2", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl2.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input3", Address = "A2", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });

            pLCModuleControl2.OutputParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "output1", Address = "B1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl2.OutputParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "output1", Address = "B2", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });


            pLCModuleControl2.InputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "int1", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });

            pLCModuleControl2.OutputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "int2", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });

            pLCModuleControl2.OutputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "int3", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });
            pLCModuleControl2.OutputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "int4", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });

            pLCModuleControl2.OutputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "int5", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });

            pLCModuleControl2.OutputOperandParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "int6", Address = "B3", Symbol = "Sym1", VariableType = "Var1", DataType = "int", Comment = "First" });

            pLCModuleControl2.UpdateParameterUI();
            PLCModules.Add(pLCModuleControl2);
     ;

            PLCModuleControl pLCModuleControl3 = new PLCModuleControl();
            pLCModuleControl3.PLCName = "PLC B";
            pLCModuleControl3.Width = 100;
            pLCModuleControl3.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input1", Address = "A1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl3.InputParameters.Add(new CustomEllipsControlDataModel { IsInput = true, Name = "input1", Address = "A2", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });


            pLCModuleControl3.OutputParameters.Add(new CustomEllipsControlDataModel { IsInput = false, Name = "output1", Address = "B1", Symbol = "Sym1", VariableType = "Var1", DataType = "Bool", Comment = "First" });
            pLCModuleControl3.UpdateParameterUI();
            PLCModules.Add(pLCModuleControl3);

            
            // 设置 DataContext
            DataContext = this;


        }
   

     
    }
}