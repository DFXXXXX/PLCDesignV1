using PLCDesignV1.CommandManager;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    /// <summary>
    /// PLCModuleControl.xaml 的交互逻辑
    /// </summary>
    /// 
    [Serializable]
    public partial class PLCModuleControl : UserControl, ISerializable,INotifyPropertyChanged
    {
        public List<Connection> Connections { get; private set; } = new List<Connection>();
     

        public UndoManager _undoManager;
        private ObservableCollection<CustomEllipsControlDataModel> _InputParameters { get; set; }
        private ObservableCollection<CustomEllipsControlDataModel> _OutputParameters { get; set; }

        private ObservableCollection<CustomEllipsControlDataModel> _InputOperandParameters { get; set; }
        private ObservableCollection<CustomEllipsControlDataModel> _OutputOperandParameters { get; set; }

        public event Action<Ellipse> EllipseClicked;

        public ObservableCollection<CustomEllipsControlDataModel> InputParameters
        {
            get { return _InputParameters; }
            set
            {
                if (_InputParameters != value)
                {
                    _InputParameters = value;
                    OnPropertyChanged(nameof(InputParameters));
                }
            }
        }

        public ObservableCollection<CustomEllipsControlDataModel> OutputParameters
        {
            get { return _OutputParameters; }
            set
            {
                if (_OutputParameters != value)
                {
                    _OutputParameters = value;
                    OnPropertyChanged(nameof(OutputParameters));
                }
            }
        }

        public ObservableCollection<CustomEllipsControlDataModel> InputOperandParameters
        {
            get { return _InputOperandParameters; }
            set
            {
                if (_InputOperandParameters != value)
                {
                    _InputOperandParameters = value;
                    OnPropertyChanged(nameof(InputOperandParameters));
                }
            }
        }


        public ObservableCollection<CustomEllipsControlDataModel> OutputOperandParameters
        {
            get { return _OutputOperandParameters; }
            set
            {
                if (_OutputOperandParameters != value)
                {
                    _OutputOperandParameters = value;
                     OnPropertyChanged(nameof(OutputOperandParameters));
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Point LinePoint { get; set; }

        public PLCModuleControl()
        {
            InitializeComponent();
            InputParameters = new ObservableCollection<CustomEllipsControlDataModel>();
            OutputParameters = new ObservableCollection<CustomEllipsControlDataModel>();
            InputOperandParameters = new ObservableCollection<CustomEllipsControlDataModel>();
            OutputOperandParameters = new ObservableCollection<CustomEllipsControlDataModel>();
            

            (this.Content as FrameworkElement).DataContext = this;
            this.Loaded += PLCModuleControl_Loaded;
            this.MouseLeftButtonDown += PLCModuleControl_MouseLeftButtonDown;
        }


        private void PLCModuleControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }
        private void PLCModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focusable = true;
            this.IsTabStop = true;
            this.Focus();
            this.KeyDown += PLCModuleControl_KeyDown;
            Debug.WriteLine($"message :aaaaaaaaaaaaaaaaaa {this.Name}");
            UpdateParameterUI();
        }
        public int PLCTextBlockHeight
        {
            get { return (int)GetValue(PLCTextBlockHeightProperty); }
            set { SetValue(PLCTextBlockHeightProperty, value); }
        }

        public static readonly DependencyProperty PLCTextBlockHeightProperty =
           DependencyProperty.Register(
               "PLCTextBlockHeight",
               typeof(int),
               typeof(PLCModuleControl),
               new PropertyMetadata(MainWindow.GridCellSize)
           );

        public string PLCName
        {
            get { return (string)GetValue(PLCNameProperty); }
            set { SetValue(PLCNameProperty, value); }
        }

        public static readonly DependencyProperty PLCNameProperty =
           DependencyProperty.Register(
               "PLCName",
               typeof(string),
               typeof(PLCModuleControl),
               new PropertyMetadata("PLC Module Default")
           );

        public int CalculatedColumnSpan
        {
            get { return (int)GetValue(CalculatedColumnSpanProperty); }
            set { SetValue(CalculatedColumnSpanProperty, value); }
        }

        public static readonly DependencyProperty CalculatedColumnSpanProperty =
            DependencyProperty.Register(
                "CalculatedColumnSpan",
                typeof(int),
                typeof(PLCModuleControl),
                new PropertyMetadata(0)
            );

        public int CalculatedRowSpan
        {
            get { return (int)GetValue(CalculatedRowSpanProperty); }
            set { SetValue(CalculatedRowSpanProperty, value); }
        }

        public static readonly DependencyProperty CalculatedRowSpanProperty =
            DependencyProperty.Register("CalculatedRowSpan", typeof(int), typeof(PLCModuleControl), new PropertyMetadata(0));

        public int CalculatedRowHight
        {
            get { return (int)GetValue(CalculatedRowHightProperty); }
            set { SetValue(CalculatedRowHightProperty, value); }
        }



        public static readonly DependencyProperty CalculatedRowHightProperty =
            DependencyProperty.Register(
                "CalculatedRowHight",
                typeof(int),
                typeof(PLCModuleControl),
                new PropertyMetadata(0)
            );

        public int CalculatedOperandRowHight
        {
            get { return (int)GetValue(CalculatedOperandRowHightProperty); }
            set { SetValue(CalculatedOperandRowHightProperty, value); }
        }



        public static readonly DependencyProperty CalculatedOperandRowHightProperty =
            DependencyProperty.Register(
                "CalculatedOperandRowHight",
                typeof(int),
                typeof(PLCModuleControl),
                new PropertyMetadata(0)
            );
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Debug.WriteLine(e.OriginalSource.GetType());

            if (e.OriginalSource.GetType() != typeof(Ellipse))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DataObject data = new DataObject(typeof(PLCModuleControl), this);
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                }
                base.OnMouseMove(e);
            }
            
        }

  
        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            UpdateParameterUI();
            // 处理删除点击事件
            MessageBox.Show("你点击了删除");
        }

        private void Editor_Click(object sender, RoutedEventArgs e)
        {
            // 处理编辑点击事件
            CustomEllipsControlDataModel customEllipsControlDataModel = new CustomEllipsControlDataModel();
            customEllipsControlDataModel.Name = "output4";
            customEllipsControlDataModel.Address = "B4";
            customEllipsControlDataModel.Symbol = "Sym4";
            customEllipsControlDataModel.VariableType = "Var4";
            customEllipsControlDataModel.DataType = "Int";
            customEllipsControlDataModel.Comment = "Fourth";
            customEllipsControlDataModel.IsInput = false;

            OutputParameters.Add(customEllipsControlDataModel);

            UpdateParameterUI();
            //GetBounds();
            MessageBox.Show($"你点击了编辑{ GetBounds()}");
        }
        public Rect GetBounds()
        {
            PLCCanvas parentCanvas = CommonHelper.FindParentCanvas<PLCCanvas>(this);
            var pointx = Grid.GetColumn(this);
            var pointy = Grid.GetRow(this);
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            int x = (int)pointx * MainWindow.GridCellSize;
            int y = (int)pointy * MainWindow.GridCellSize;

            //Debug.WriteLine($"GetBounds {x},{y},{width},{height}");
            return new Rect(x, y, width, height);

        }

    
        private void AddParam_Click(object sender, RoutedEventArgs e)
        {
            InputParam inputParam = new InputParam();
            inputParam.ShowDialog();
            var container = GetParameterContainer(inputParam.DataModel);
            container.Add(inputParam.DataModel);
            UpdateParameterUI();
        }


        public void UpdateParameterUI()
        {
            // 设置控件的高度
            int inputCount = InputParameters.Count;
            int outputCount = OutputParameters.Count;
            int maxCount = Math.Max(inputCount, outputCount);

            int inputOperandCount = InputOperandParameters.Count;
            int outputOperandCount = OutputOperandParameters.Count;
            int maxOperandCount = Math.Max(inputOperandCount, outputOperandCount);

           
            if (maxCount == 1 && maxOperandCount == 0)
            {
                CalculatedRowHight = MainWindow.GridCellSize;
            }
            else
            {

                CalculatedRowHight = MainWindow.GridCellRectangleHeight;
            } 

            //每MainWindow.GridCellSize 存2个Operand的参数,如果不够2个，也要占用一个MainWindow.GridCellSize
            int operandRowCount = (int)Math.Ceiling((double)maxOperandCount / MainWindow.GridCellWidthRate);
            CalculatedOperandRowHight = MainWindow.GridCellSize;

            this.Height= maxCount * MainWindow.GridCellRectangleHeight;

            if (this.Height == MainWindow.GridCellRectangleHeight)
            {
                this.Borderpcl.BorderThickness = new Thickness(0);
            }else
            {
                this.Borderpcl.BorderThickness = new Thickness(2);
            }
            ParamLineCanvas.Children.Clear();  // 清除之前的线条
            GenerateGridRows(inputParamContinerGrid, InputParameters);
            GenerateGridRows(outputParamContinerGrid, OutputParameters);

            this.Width = MainWindow.GridCellRectanglWidth ;
            MainGrid.Height = this.Height;
            ParamLineCanvas.Height = this.Height;
            ParamLineCanvas.Width = this.Width;
            PLCTextBlockHeight = MainWindow.GridCellSize;
            CalculatedRowSpan = (int)Math.Ceiling(this.Height / (MainWindow.GridCellSize));
            CalculatedColumnSpan = (int)Math.Ceiling(this.Width / (MainWindow.GridCellSize));
            //Grid.SetColumnSpan(this, columnSpan);
            Borderpcl.Width = MainWindow.GridCellRectanglWidth - 8;
            MainGrid.RowDefinitions[0].Height = new GridLength(PLCTextBlockHeight);
            //MainGrid.RowDefinitions[2].Height = new GridLength(operandRowCount * MainWindow.GridCellSize);

            Grid.SetColumnSpan(this, CalculatedColumnSpan);
            Grid.SetRowSpan(this, CalculatedRowSpan);

            //InputParameterContainer.Height = CalculatedRowHight * maxCount;
            //OutputParameterContainer.Height = CalculatedRowHight * maxCount;
            // 更新输入StackPanel的位置
            // 更新输入ItemsControl的位置
            //Canvas.SetLeft(InputParameterContainer, 0); // 设置 X 坐标
            //Canvas.SetTop(InputParameterContainer, PLCTextBlockHeight); // 设置 Y 坐标

            // 更新输出ItemsControl的位置
            //Canvas.SetLeft(OutputParameterContainer, this.Width - OutputParameterContainer.Width); // 设置 X 坐标
            //Canvas.SetTop(OutputParameterContainer, PLCTextBlockHeight); // 设置 Y 坐标
            //InputParameterUniformGrid.Height = CalculatedRowHight * maxCount;
            //InputParameterUniformGrid.DataContext = InputParameters;
            //OutputParameterUniformGrid.Height = CalculatedRowHight * maxCount;
            //InputParameterUniformGrid.DataContext = OutputParameters;
            //// 打印调试信息
            //DebugParameterPositions(InputParameterUniformGrid, "Input");
            //DebugParameterPositions(OutputParameterUniformGrid, "Output");
        }


        private void GenerateGridRowsAddParam(Grid grid, CustomEllipsControlDataModel parameter, int rowIndex,int rate)
        {
            var textBlock = new TextBlock
            {
                Text = parameter.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(parameter.IsInput ? 10 : 0, 0, parameter.IsInput ? 0 : 10, 0) // 为了给横线留出空间
            };
            Grid.SetRow(textBlock, rowIndex * rate);
            Grid.SetColumn(textBlock, 1); // 将 TextBlock 放在第二列
            grid.Children.Add(textBlock);

            // 创建横线并添加到 LineCanvas 中
            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,

            };
            line.X1 = parameter.IsInput ? 0 : ParamLineCanvas.ActualWidth - 4; // 输入从左边开始
            line.X2 = parameter.IsInput ? 4 : ParamLineCanvas.ActualWidth; // 线条长度 4
            line.Y1 = ((rowIndex * rate + 1) * MainWindow.GridCellSize) + (MainWindow.GridCellSize / 2); // 行中心对齐
            line.Y2 = ((rowIndex * rate + 1) * MainWindow.GridCellSize) + (MainWindow.GridCellSize / 2);
            ParamLineCanvas.Children.Add(line);

        }
        private void GenerateGridRows(Grid grid, ObservableCollection<CustomEllipsControlDataModel> parameters)
        {
            grid.RowDefinitions.Clear();
            grid.Children.Clear();
            int rowIndex = 0;

            //计算rows 有多少行
            var i = parameters.Count(x => x.DataType.ToLower() == "bool");
            var j = parameters.Count() - i;
            int rows = i * 3;
            //if (j > i * 2)


            foreach (var parameter in parameters)
            {
                // 添加参数行
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(MainWindow.GridCellSize) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(MainWindow.GridCellSize) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(MainWindow.GridCellSize) });

                int rate = 1;
                // 创建 TextBlock
                if (parameter.DataType.ToLower() == "bool")
                {
                    rate = MainWindow.GridCellHeightRate;
                }
                //GenerateGridRowsAddParam(grid,parameter,rowIndex, rate);
                NewMethod(grid, rowIndex, parameter, rate);
                rowIndex++;
            }
        }

        private void NewMethod(Grid grid, int rowIndex, CustomEllipsControlDataModel parameter, int rate)
        {
            var textBlock = new TextBlock
            {
                Text = parameter.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(parameter.IsInput ? 10 : 0, 0, parameter.IsInput ? 0 : 10, 0) // 为了给横线留出空间
            };
            Grid.SetRow(textBlock, rowIndex * rate);
            Grid.SetColumn(textBlock, 1); // 将 TextBlock 放在第二列
            grid.Children.Add(textBlock);

            // 创建横线并添加到 LineCanvas 中
            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,

            };
            line.X1 = parameter.IsInput ? 0 : ParamLineCanvas.ActualWidth - 4; // 输入从左边开始
            line.X2 = parameter.IsInput ? 4 : ParamLineCanvas.ActualWidth; // 线条长度 4
            line.Y1 = ((rowIndex * rate + 1) * MainWindow.GridCellSize) + (MainWindow.GridCellSize / 2); // 行中心对齐
            line.Y2 = ((rowIndex * rate + 1) * MainWindow.GridCellSize) + (MainWindow.GridCellSize / 2);
            ParamLineCanvas.Children.Add(line);
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            if (sender is Ellipse dot)
            {
                var data = dot.DataContext as CustomEllipsControlDataModel;
                if(data.DataType?.ToLower()=="bool")
                {
                    EllipseClicked?.Invoke(dot);
                    //var parentCanvas = CommonHelper.FindParentCanvas<PLCCanvas>(this);
                    //if (parentCanvas != null)
                    //{
                    //    //parentCanvas.DesignCanvas_MouseLeftButtonDown(sender, e);
                    //    parentCanvas.StartConnection(dot);
                    //}
                }
                e.Handled = true;
                //Debug.WriteLine($"Dot { dot.Name } clicked at: {position.X}, {position.Y}");
            }
        }

        public Dictionary<string, Point> GetPortPositions()
        {
            var portPositions = new Dictionary<string, Point>();

            foreach (var input in InputParameters)
            {
                var dot = FindDotByName(input.Name, true);
                if (dot != null)
                {
                    portPositions[input.Name] = GetPortPosition(dot);
                }
            }

            foreach (var output in OutputParameters)
            {
                var dot = FindDotByName(output.Name, false);
                if (dot != null)
                {
                    portPositions[output.Name] = GetPortPosition(dot);
                }
            }

            return portPositions;
        }

        private UIElement FindDotByName(string name, bool isInput)
        {
            //var grid = isInput ? InputParameterUniformGrid : OutputParameterUniformGrid;
            //var stackPanel = grid.Children.OfType<StackPanel>().FirstOrDefault(sp => sp.Name == name);

            //return stackPanel?.Children.OfType<Ellipse>().FirstOrDefault();
            return null;
        }

        public Point GetPortPosition(UIElement element)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            return new Point(left, top);
        }


        private void Dot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse dot)
            {
                CustomEllipsControlDataModel data= dot.DataContext as CustomEllipsControlDataModel;


                //if (data.DataType.ToLower() == "bool")
                    dot.Fill = Brushes.Yellow; // Set the fill color to yellow for highlighting
            }
        }

        private void Dot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse dot)
            {
                // 判断是输入还是输出
                CustomEllipsControlDataModel data = dot.DataContext as CustomEllipsControlDataModel;
                bool isInput = data.IsInput;
                if (data.DataType?.ToLower() == "bool") 
                         dot.Fill = isInput ? Brushes.Blue : Brushes.Red; // Restore the original fill color
                else
                    dot.Fill = Brushes.Green; // Restore the original fill color
            }
        }
        public void ModifyParameter(ObservableCollection<CustomEllipsControlDataModel> container, CustomEllipsControlDataModel parameter)
        {
            var oldParameter = parameter.Clone();
            var modifyParameterCommand = new ModifyParameterCommand(this, container, parameter, oldParameter);
            _undoManager.Execute(modifyParameterCommand);
        }

        public void AddParameter(ObservableCollection<CustomEllipsControlDataModel> container, CustomEllipsControlDataModel parameter)
        {
            var addParameterCommand = new ModifyParameterCommand(this, container, parameter, isAddOperation: true);
            _undoManager.Execute(addParameterCommand);
            //UpdateParameterUI();
        }

        public void DeleteParameter(ObservableCollection<CustomEllipsControlDataModel> container, CustomEllipsControlDataModel parameter)
        {
            var deleteParameterCommand = new ModifyParameterCommand(this, container, parameter, isDeleteOperation: true);
            _undoManager.Execute(deleteParameterCommand);
            //UpdateParameterUI();
        }

        public ObservableCollection<CustomEllipsControlDataModel> GetParameterContainer(CustomEllipsControlDataModel parameter)
        {    
            if (parameter.IsInput && parameter.DataType?.ToLower() != "bool")
            { 
                return InputOperandParameters;
            }
            if (!parameter.IsInput && parameter.DataType?.ToLower() != "bool")
            {
                return OutputOperandParameters;
            }
            if (parameter.IsInput && parameter.DataType?.ToLower() == "bool")
            {
                return InputParameters;
            }
            if (!parameter.IsInput && parameter.DataType?.ToLower() == "bool")
            {
                return OutputParameters;
            }
            return null;
        }
        private void PLCModuleControl_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine($"KeyDown event triggered: {e.Key}");
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                CommonHelper.CopyToClipboard(this);
            }
            if (e.Key == Key.Back)
            {
                var parentCanvas = CommonHelper.FindParentCanvas<PLCCanvas>(this);
                if (parentCanvas == null)
                {
                    return;
                }
                int CurrentRectangeRow = Grid.GetRow(this);
                int CurrentRectangeCloumn = Grid.GetColumn(this);
               
                RemoveModuleCommand  remove = new RemoveModuleCommand(parentCanvas, this,CurrentRectangeCloumn, CurrentRectangeRow);
                _undoManager.Execute(remove);
                Debug.WriteLine("moduleControl Point  {0} {1}", CurrentRectangeRow, CurrentRectangeCloumn);

            }
        }
        public PLCModuleControl Copy()
        {
            var newModule = new PLCModuleControl
            {
                PLCName = this.PLCName,
                InputParameters = new ObservableCollection<CustomEllipsControlDataModel>(this.InputParameters.Select(p => p.Clone())),
                OutputParameters = new ObservableCollection<CustomEllipsControlDataModel>(this.OutputParameters.Select(p => p.Clone())),

                InputOperandParameters = new ObservableCollection<CustomEllipsControlDataModel>(this.InputOperandParameters.Select(p => p.Clone())),
                OutputOperandParameters = new ObservableCollection<CustomEllipsControlDataModel>(this.OutputOperandParameters.Select(p => p.Clone()))

            };


            return newModule;
        }

        #region 自定义序列化
        protected PLCModuleControl(SerializationInfo info, StreamingContext context)
        {
            Connections = (List<Connection>)info.GetValue("Connections", typeof(List<Connection>));
            InputParameters = (ObservableCollection<CustomEllipsControlDataModel>)info.GetValue("InputParameters", typeof(ObservableCollection<CustomEllipsControlDataModel>));
            OutputParameters = (ObservableCollection<CustomEllipsControlDataModel>)info.GetValue("OutputParameters", typeof(ObservableCollection<CustomEllipsControlDataModel>));
            InputOperandParameters = (ObservableCollection<CustomEllipsControlDataModel>)info.GetValue("InputOperandParameters", typeof(ObservableCollection<CustomEllipsControlDataModel>));
            OutputOperandParameters = (ObservableCollection<CustomEllipsControlDataModel>)info.GetValue("OutputOperandParameters", typeof(ObservableCollection<CustomEllipsControlDataModel>));
            LinePoint = (Point)info.GetValue("LinePoint", typeof(Point));
            PLCTextBlockHeight = info.GetInt32("PLCTextBlockHeight");
            PLCName = info.GetString("PLCName");
            CalculatedColumnSpan = info.GetInt32("CalculatedColumnSpan");
            CalculatedRowSpan = info.GetInt32("CalculatedRowSpan");
            CalculatedRowHight = info.GetInt32("CalculatedRowHight");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Connections", Connections);
            info.AddValue("InputParameters", InputParameters);
            info.AddValue("OutputParameters", OutputParameters);
            info.AddValue("InputOperandParameters", InputOperandParameters);
            info.AddValue("OutputOperandParameters", OutputOperandParameters);
            info.AddValue("LinePoint", LinePoint);
            info.AddValue("PLCTextBlockHeight", PLCTextBlockHeight);
            info.AddValue("PLCName", PLCName);
            info.AddValue("CalculatedColumnSpan", CalculatedColumnSpan);
            info.AddValue("CalculatedRowSpan", CalculatedRowSpan);
            info.AddValue("CalculatedRowHight", CalculatedRowHight);
        }
        #endregion


    }


    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items) where T : CustomEllipsControlDataModel
        {

            // 将所有元素copy到一个新的集合中，然后再添加到ObservableCollection中

            foreach (var item in items)
            {
                // 使用 Clone 方法创建元素的副本
                var clonedItem = (T)item.Clone();
                collection.Add(clonedItem);
            }
        }
    }
}
