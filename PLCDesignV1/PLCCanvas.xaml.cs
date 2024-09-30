using Newtonsoft.Json;
using PLCDesignV1.CommandManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    /// <summary>
    /// PLCCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class PLCCanvas : UserControl
    {
        private Point _startPoint;  // 保存鼠标点击时的位置
        private Point currentPosition;  //当前鼠标的位置
        private bool _isDragging;  // 标志是否正在拖动
        private UIElement _draggedElement;  // 当前被拖动的元素
        private const double DragThreshold = 20;  // 定义鼠标移动多少像素后开始拖动
        private PLCModuleControl moduleControl;  //当前选定的plcmodule
        private List<PLCModuleControl> _modules = new List<PLCModuleControl>();
        private Line _currentLine;
        private int GridRowSize = 72;  // 定义网格的大小
        private int GridColumnSize = 72;  // 定义网格的大小
        private bool _isConnecting;
        private Dictionary<(int, int), Point> _gridIndex;
        private HashSet<Polyline> _lines { get; set; }
        private readonly UndoManager _undoManager = new UndoManager();
        private Point _startLinePoint;  // 保存鼠标点击时的位置
        private PLCDesignGridServices _PLCDesignGridServices { get; set; }
        public ObservableCollection<CustomEllipsControlDataModel> SelectedModulePoints { get; set; }
        public PlcModuleManager PlcModuleManager { get; set; }
        public ObservableCollection<CustomEllipsControlDataModel> SelectedOperandModulePoints { get; set; }
        public LineManager<Polyline, Ellipse> _lineManager { get; set; }
        public PLCCanvas()
        {
            InitializeComponent();
            DesignCanvas.AllowDrop = true;
            DesignCanvas.Drop += DesignCanvas_Drop;
            DesignCanvas.MouseLeftButtonDown += DesignCanvas_MouseLeftButtonDown;

            DesignCanvas.MouseLeftButtonUp += DesignCanvas_MouseLeftButtonUp;
            DesignCanvas.MouseMove += DesignCanvas_MouseMove;
            DesignCanvas.MouseRightButtonDown += DesignCanvas_MouseRightButtonDown;
            this.PreviewKeyDown += PLCCanvas_PreviewKeyDown;


            this.DataContext = this;
            this.Focusable = true;
            this.IsTabStop = true;
            this.KeyDown += PLCCanvas_KeyDown;
            PlcModuleManager = new PlcModuleManager();
            SelectedModulePoints = new ObservableCollection<CustomEllipsControlDataModel>();
            SelectedOperandModulePoints = new ObservableCollection<CustomEllipsControlDataModel>();
            _PLCDesignGridServices = new PLCDesignGridServices(GridRowSize, GridColumnSize, DesignCanvas, PlcModuleManager);
            _lineManager = new LineManager<Polyline, Ellipse>(LineCanvas, DesignCanvas);
            _lineManager.EllipseClicked += OnEllipseClicked;
            _lines = _lineManager.GetLines();
            LineCanvas.MouseLeftButtonDown += _lineManager.Ellipse_MouseLeftButtonDown;
            LineCanvas.MouseLeftButtonDown += _lineManager.Dot_MouseEnter;
            LineCanvas.MouseLeave += _lineManager.Dot_MouseLeave;
        }

        private void OnEllipseClicked(Point point)
        {
            StartConnection(point);
     
        }
        private void DeleteSelectedRow_Click(object sender, RoutedEventArgs e)
        {
            // 获取 DataGrid 中选中的行
            var selectedItems = PointDataGrid.SelectedItems;
            if (selectedItems.Count <= 0)
            {
                selectedItems = PointOperandDataGrid.SelectedItems;
            }


            if (selectedItems != null && selectedItems.Count > 0)
            {
                var pointsToDelete = new List<CustomEllipsControlDataModel>();

                foreach (var item in selectedItems)
                {
                    if (item is CustomEllipsControlDataModel point)
                    {
                        pointsToDelete.Add(point);
                    }
                }

                foreach (var point in pointsToDelete)
                {
                    if (moduleControl != null)
                    {
                        moduleControl.DeleteParameter(moduleControl.GetParameterContainer(point), point);
                        SelectedModulePoints.Remove(point);  // 从绑定的集合中删除
                    }
                }
            }
        }

        private void AddSelectedRow_Click(object sender, RoutedEventArgs e)
        {
            // 获取 DataGrid 中选中的行
            var selectedItems = PointDataGrid.SelectedItems;


            if (selectedItems != null && selectedItems.Count > 0)
            {
                var pointsToAdd = selectedItems.Cast<CustomEllipsControlDataModel>().ToList();

                foreach (var point in pointsToAdd)
                {
                    if (point.Name == null)
                    {
                        return;
                    }
                    moduleControl?.AddParameter(moduleControl.GetParameterContainer(point), point);
                    //SelectedModulePoints.Add(point);  // 从绑定的集合中删除

                }
            }
        }
        private bool? _oldValue;
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var cloum = e.Column;
            if (cloum is DataGridComboBoxColumn)
            {
                if (e.Row.Item is CustomEllipsControlDataModel editedParameter)
                {
                    var bindingPath = e.Column.SortMemberPath;
                    _oldValue = (bool?)typeof(CustomEllipsControlDataModel).GetProperty(bindingPath).GetValue(editedParameter);
                }
            }
        }
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // 在这里可以处理单元格编辑结束后的逻辑
            // 例如，验证数据或更新其他UI元素
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var dataGrid = sender as DataGrid;
                var editedParameter = e.Row.Item as CustomEllipsControlDataModel;

                if (editedParameter != null && moduleControl != null)
                {
                    var column = e.Column;
                    var bindingPath = string.Empty;

                    if (column is DataGridBoundColumn boundColumn)
                    {
                        bindingPath = (boundColumn.Binding as Binding)?.Path?.Path;
                    }
                    else if (column is DataGridComboBoxColumn comboBoxColumn)
                    {
                        bindingPath = (comboBoxColumn.SelectedValueBinding as Binding)?.Path?.Path;
                    }

                    if (!string.IsNullOrEmpty(bindingPath))
                    {
                        if (bindingPath == nameof(CustomEllipsControlDataModel.IsInput))
                        {
                            var comboBox = e.EditingElement as ComboBox;
                            if (comboBox != null)
                            {
                                var newValue = (bool)comboBox.SelectedValue;

                                if (!Equals(newValue, _oldValue))
                                {
                                    var command = new ChangePropertyCommand<CustomEllipsControlDataModel>(editedParameter, bindingPath, newValue, _oldValue, moduleControl);
                                    _undoManager.Execute(command);
                                }
                            }
                        }
                        else
                        {
                            var textBox = e.EditingElement as TextBox;
                            if (textBox != null)
                            {
                                var newValue = textBox.Text;
                                var oldValue = typeof(CustomEllipsControlDataModel).GetProperty(bindingPath).GetValue(editedParameter);

                                if (!Equals(newValue, oldValue))
                                {
                                    var command = new ChangePropertyCommand<CustomEllipsControlDataModel>(editedParameter, bindingPath, newValue, oldValue, moduleControl);
                                    _undoManager.Execute(command);
                                }
                            }
                        }
                        UpdateSourceData();
                    }
                }
            }
        }
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // 获取编辑结束后的行数据
            var editedParameter = e.Row.Item as CustomEllipsControlDataModel; // 假设行对应的数据类型是 ModulePoint

            if (editedParameter != null && moduleControl != null && editedParameter.Name != null && e.EditAction == DataGridEditAction.Commit)
            {
                if (editedParameter != null)
                {

                    if (moduleControl != null)
                    {
                        if (e.Row.IsNewItem)
                        {
                            // 添加新参数
                            moduleControl.AddParameter(moduleControl.GetParameterContainer(editedParameter), editedParameter);
                        }
                        else
                        {
                            //    var old = editedParameter;
                            //    // 修改现有参数
                            //    var container = moduleControl.GetParameterContainer(editedParameter);
                            //    moduleControl.ModifyParameter(container, editedParameter);
                        }
                    }
                }
            }
        }

        //移动矩形光标
        private void Rectange_KeyDown(Key e)
        {
            if (_PLCDesignGridServices._selectedRectangle == null) return;

            int row = Grid.GetRow(_PLCDesignGridServices._selectedRectangle);
            int column = Grid.GetColumn(_PLCDesignGridServices._selectedRectangle);
            double GridWidthOffset = _PLCDesignGridServices.DesignCanvas.ActualWidth / _PLCDesignGridServices.Columns;
            double GridHeightOffset = _PLCDesignGridServices.DesignCanvas.ActualHeight / _PLCDesignGridServices.Rows;



            switch (e)
            {
                case Key.Up:
                    if (row > 0) row -= MainWindow.GridCellHeightRate;


                    break;
                case Key.Down:
                    if (row < _PLCDesignGridServices.Rows - 1) row += MainWindow.GridCellHeightRate;


                    //DesignCanvasScrollViewer.ScrollToVerticalOffset(25 + DesignCanvasScrollViewer.VerticalOffset);
                    break;
                case Key.Left:
                    if (column > 0) column -= MainWindow.GridCellWidthRate;

                    break;
                case Key.Right:
                    if (column < _PLCDesignGridServices.Columns - 1) column += MainWindow.GridCellWidthRate;
                    break;
                default:
                    return;
            }
            if (e == Key.Left && DesignCanvasScrollViewer.HorizontalOffset > column * GridWidthOffset)
                DesignCanvasScrollViewer?.ScrollToHorizontalOffset(DesignCanvasScrollViewer.HorizontalOffset - GridWidthOffset);
            if (e == Key.Right && DesignCanvasScrollViewer.HorizontalOffset < column * GridWidthOffset)
                DesignCanvasScrollViewer?.ScrollToHorizontalOffset(DesignCanvasScrollViewer.HorizontalOffset + GridWidthOffset);



            if (e == Key.Down && DesignCanvasScrollViewer.VerticalOffset < row * GridHeightOffset)
                DesignCanvasScrollViewer?.ScrollToVerticalOffset(DesignCanvasScrollViewer.VerticalOffset + GridHeightOffset);

            if (e == Key.Up && DesignCanvasScrollViewer.VerticalOffset > row * GridHeightOffset)
                DesignCanvasScrollViewer?.ScrollToVerticalOffset(DesignCanvasScrollViewer.VerticalOffset - GridWidthOffset);


            // 获取指定行列位置的矩形
            var targetRectangle = _PLCDesignGridServices.DesignCanvas.Children
                .OfType<Rectangle>()
                .FirstOrDefault(r => Grid.GetRow(r) == row && Grid.GetColumn(r) == column);
            var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                Source = targetRectangle
            };
            targetRectangle?.RaiseEvent(args);
        }

        private void Rectange_DwringLine(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                Debug.WriteLine($"Keyboard  {e.Key} {e.SystemKey}");
                Point StartPoint;
                Point EndPoint;

                if (e.SystemKey == Key.Left || e.SystemKey == Key.Right)
                {
                    Debug.WriteLine("Alt + Left Arrow key is pressed.");
                    // 在这里添加你的处理逻辑
                    (StartPoint, EndPoint) = _PLCDesignGridServices.GetSelectRectangeHorizontalLine();
                }
                if (e.SystemKey == Key.Up)
                {
                    (StartPoint, EndPoint) = _PLCDesignGridServices.GetSelectRectangeTopVerticaLine();
                }
                if (e.SystemKey == Key.Down)
                {
                    (StartPoint, EndPoint) = _PLCDesignGridServices.GetSelectRectangeBottomVerticaLine();
                }

                if (StartPoint != EndPoint)
                {

                    Polyline line = new Polyline()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                    };
                    line.Points.Add(StartPoint);
                    line.Points.Add(EndPoint);

                    var addLineCommand = new AddLineCommand<Polyline>(line, _lineManager);
                    _undoManager.Execute(addLineCommand);
                    Rectange_KeyDown(e.SystemKey);
                }
            }
        }

        private void Rectange_RemoveLine(KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (_PLCDesignGridServices._selectedRectangle != null)
                {
                    Point begin, end;
                    (begin, end) = _PLCDesignGridServices.GetSelectRectangeTopHorizontalLine();
                    RemoveLine(begin, end);
                    (begin, end) = _PLCDesignGridServices.GetSelectRectangeHorizontalLine();
                    RemoveLine(begin, end);
                    (begin, end) = _PLCDesignGridServices.GetSelectRectangeBottomHorizontalLine();
                    RemoveLine(begin, end);
                    (begin, end) = _PLCDesignGridServices.GetSelectRectangeTopVerticaLine();
                    RemoveLine(begin, end);
                    (begin, end) = _PLCDesignGridServices.GetSelectRectangeBottomVerticaLine();
                    RemoveLine(begin, end);
                }
                moduleControl?.RaiseEvent(new KeyEventArgs(
                   e.KeyboardDevice,
                   e.InputSource,
                   e.Timestamp,
                   e.Key)
                { RoutedEvent = Keyboard.KeyDownEvent });
            }

        }

        private void RemoveLine(Point begin, Point end)
        {
            if (begin.X != 0 && begin.Y != 0 && end.X != 0 && end.Y != 0)
            {
                var line = _lines.FirstOrDefault(x => x.Points.Contains(begin) && x.Points.Contains(end));
                if (line != null)
                {
                    var removeLineCommand = new RemoveLineCommand<Polyline>(line, _lineManager);
                    _undoManager.Execute(removeLineCommand);
                }
            }

        }

        private void PLCCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                PLCCanvas_KeyDown(sender, e);
                //e.Handled = true; // 阻止事件进一步传播
            }
        }


        private bool isHandlingKeyDown = false;

        private void PLCCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (isHandlingKeyDown) return;

            Debug.WriteLine("PLCCanvas_KeyDown");

            isHandlingKeyDown = true;
            try

            {

                Rectange_KeyDown(e.Key);

                Rectange_DwringLine(e);

                Rectange_RemoveLine(e);

                if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    moduleControl?.RaiseEvent(new KeyEventArgs(
                            e.KeyboardDevice,
                            e.InputSource,
                            e.Timestamp,
                            e.Key)
                    { RoutedEvent = Keyboard.KeyDownEvent });
                }
                // 将事件传递给子控件


                if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    PasteFromClipboard();
                }
                else if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    _undoManager.Undo();
                }
                else if (e.Key == Key.R && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    _undoManager.Redo();
                }
                //e.Handled = true;
            }
            finally
            {
                isHandlingKeyDown = false;
            }
        }
        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsText())
            {

                var clipboardText = Clipboard.GetText();
                PLCModuleControl data = null;
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    var temp = clipboardText;
                    data = JsonConvert.DeserializeObject<PLCModuleControl>(temp);
                }


                if (data != null)
                {
                    // 创建新的 PLCModuleControl 实例并复制属性
                    if (_PLCDesignGridServices._selectedRectangle == null)
                        return;
                    var newModule = data.Copy();

                    //将Clipboard 中清空
                    var point = GetPlcDesignGridPoint(currentPosition);
                    //DesignCanvas.Children.Add(newModule);
                    var commod = new AddModuleCommand(DesignCanvas, newModule, this, (int)_PLCDesignGridServices.CurrentRectangeCloumn, _PLCDesignGridServices.CurrentRectangeRow);
                    _undoManager.Execute(commod);
                    // 将新模块添加到设计画布或其他容器中
                    //DesignCanvas.Children.Add(newModule);
                }
                else
                {
                    MessageBox.Show("Failed to retrieve PLCModuleControl data from clipboard.");
                }
            }
            else
            {
                MessageBox.Show("Clipboard does not contain PLCModuleControl data.");
            }
        }

        private void DesignCanvas_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(typeof(PLCModuleControl)))
            {
                var module = e.Data.GetData(typeof(PLCModuleControl)) as PLCModuleControl;
                var position = e.GetPosition(DesignCanvas);
                PlaceModuleInGrid(module, position);
            }
        }

        public Point GetPlcDesignGridPoint(Point position)
        {
            int targetColumn = (int)(position.X / (MainWindow.GridCellSize));
            int targetRow = (int)(position.Y / (MainWindow.GridCellSize));
            targetColumn = Math.Max(0, Math.Min(targetColumn, _PLCDesignGridServices.Columns - (int)(MainWindow.GridCellRectanglWidth / MainWindow.GridCellSize)));
            targetRow = Math.Max(0, Math.Min(targetRow, _PLCDesignGridServices.Rows - (int)(MainWindow.GridCellRectangleHeight / MainWindow.GridCellSize)));
            // 将模块对齐到最近的矩形位置
            targetColumn = (targetColumn / MainWindow.GridCellWidthRate) * MainWindow.GridCellWidthRate;
            targetRow = (targetRow / MainWindow.GridCellHeightRate) * MainWindow.GridCellHeightRate;
            Point result;
            result.X = targetColumn;
            result.Y = targetRow;
            return result;

        }
        private void PlaceModuleInGrid(PLCModuleControl module, Point position)
        {

            var target = GetPlcDesignGridPoint(position);

            if (module != null)
            {
                if (!DesignCanvas.Children.Contains(module))
                {
                    AddNewModuleToGrid(module, (int)target.X, (int)target.Y);

                }
                else
                {
                    MoveModule(module, (int)target.X, (int)target.Y);
                    //UpdateExistingModuleInGrid(module, targetColumn, targetRow);
                }
            }
        }

        private void AddNewModuleToGrid(PLCModuleControl module, int targetColumn, int targetRow)
        {
            var newModule = module.Copy();
            newModule._undoManager = _undoManager;
            newModule.EllipseClicked += StartConnection;
            Debug.WriteLine($"Create PLCName {module.PLCName} X:{targetColumn} Y:{targetRow}");
            Grid.SetColumn(newModule, targetColumn);
            Grid.SetRow(newModule, targetRow);
            var command = new AddModuleCommand(DesignCanvas, newModule, this, targetColumn, targetRow);
            _undoManager.Execute(command);

            //DesignCanvas.Children.Add(newModule);
        }
        private void UpdateExistingModuleInGrid(PLCModuleControl module, int targetColumn, int targetRow)
        {
            //module.AdjustPortPositions(GridCellSize);
            Grid.SetColumn(module, targetColumn);
            Grid.SetRow(module, targetRow);
        }

        private void MoveModule(PLCModuleControl module, int newColumn, int newRow)
        {

            var moveModuleCommand = new MoveModuleCommand(DesignCanvas, module, this, newColumn, newRow);
            _undoManager.Execute(moveModuleCommand);
        }

        public void UpdateSourceData()
        {
            moduleControl.Focus();
            SelectedModulePoints.Clear();
            SelectedOperandModulePoints.Clear();
            var inputPoints = moduleControl.InputParameters.ToList();
            var outputPoints = moduleControl.OutputParameters.ToList();
            var inputOperandPoints = moduleControl.InputOperandParameters.ToList();
            var outputOperandPoints = moduleControl.OutputOperandParameters.ToList();
            //SelectedModulePoints.AddRange(inputPoints);
            //SelectedModulePoints.AddRange(outputPoints);
            foreach (var point in inputPoints)
            {
                SelectedModulePoints.Add(point);
            }
            foreach (var point in outputPoints)
            {
                SelectedModulePoints.Add(point);
            }


            foreach (var point in inputOperandPoints)
            {
                SelectedOperandModulePoints.Add(point);
            }
            foreach (var point in outputOperandPoints)
            {
                SelectedOperandModulePoints.Add(point);
            }


        }

        public void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as FrameworkElement; ;
            if (clickedElement != null)
            {
                moduleControl = clickedElement.DataContext as PLCModuleControl;

                if (moduleControl != null)
                {
                    // 更新 SelectedModulePoints
                    int CurrentRectangeRow = Grid.GetRow(moduleControl);
                    int CurrentRectangeCloumn = Grid.GetColumn(moduleControl);

                    Debug.WriteLine($"module Name {moduleControl.PLCTextBlock.Text} Row: {CurrentRectangeRow} cloumn {CurrentRectangeCloumn}");

                    UpdateSourceData();

                }
            }

            if (_isConnecting && _highlightedPoint==null)
            {
                _isConnecting = false;
                var point = GetPointInDesignCanvas(_startPoint, false);
                if (point == _startLinePoint)
                    return;
 
                LineCanvas.Children.Remove(_currentLine);

                DrawLine(_startLinePoint, point);

            }


            if (_draggedElement != null && DesignCanvas.Children.Contains(_draggedElement) && _isConnecting == false)
            {
                _isDragging = true;
                _draggedElement.CaptureMouse();

            }
        }

        private void SelectedModulePoints_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (moduleControl != null)
            {
                // 手动触发绘制
                moduleControl.UpdateParameterUI();  // 通知控件重新绘制

            }
        }



        private Ellipse _highlightedPoint;

        private bool IsMouseNearPoint(Point mousePosition, Point point)
        {
            return (Math.Abs(mousePosition.X - point.X) < 30) && (Math.Abs(mousePosition.Y - point.Y) < 30);
        }


        //线段辅助跟随
        private void MouseMoveLine()
        {
            if (_isConnecting && _currentLine != null)
            {
                _currentLine.X2 = currentPosition.X;
                _currentLine.Y2 = currentPosition.Y;
                //return;  // 不执行拖动逻辑
            }
        }

        //线段辅助点高亮显示
        private void HighlightPoint()
        {
            if (!_isConnecting && _highlightedPoint == null)
            {
                ValueTuple<int, int> key = ((int)currentPosition.X, (int)currentPosition.Y);
                ValueTuple<Ellipse, int> value;
                if (_lineManager._EllipseDic.TryGetValue(key, out value))
                {
                    _highlightedPoint = value.Item1;
                    _highlightedPoint.Visibility = Visibility.Visible;
                }

            }
            if (_highlightedPoint != null)
            {

                if (_highlightedPoint.Tag is ValueTuple<int, int> po)
                {
                    Point point = new Point(po.Item1, po.Item2);
                    if (!IsMouseNearPoint(currentPosition, point))
                    {
                        _highlightedPoint.Visibility = Visibility.Hidden;
                        _highlightedPoint = null;
                    }

                }

            }
        }

        private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var targ = e.OriginalSource as UIElement;


            currentPosition = e.GetPosition(DesignCanvas);
            //Debug.WriteLine($"MouseMove move{currentPosition.X} {currentPosition.Y}");
            if (_draggedElement is PLCModuleControl)
                Debug.WriteLine($"PLCModuleControl:{_draggedElement is PLCModuleControl}");
            if (_draggedElement is Ellipse)
                Debug.WriteLine($"Ellipse: {_draggedElement is Ellipse}");


            MouseMoveLine();

            HighlightPoint();


            if (_isConnecting == false && _isDragging && _draggedElement != null && _draggedElement is PLCModuleControl)
            {

                double offsetX = currentPosition.X - _startPoint.X;
                double offsetY = currentPosition.Y - _startPoint.Y;
                if (offsetX < DragThreshold || offsetY < DragThreshold)
                {
                    return;
                }
                double newLeft = Canvas.GetLeft(_draggedElement) + offsetX;
                double newTop = Canvas.GetTop(_draggedElement) + offsetY;
                Debug.WriteLine("DesignCanvas_MouseMove");
                Canvas.SetLeft(_draggedElement, newLeft);
                Canvas.SetTop(_draggedElement, newTop);
                _startPoint = currentPosition;
            }

        }


        private void DesignCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _isConnecting == false)
            {
                _isDragging = false;
                _draggedElement.ReleaseMouseCapture();
                _draggedElement = null;
            }

        }

        private void DrawLine(Point startPoint, Point endPoint)
        {
            Polyline line = _PLCDesignGridServices.DrawLine(startPoint, endPoint);
            _lineManager.ToggleEllipsesHidden();
            if (line != null)
            {
                Debug.WriteLine($"{line.Points.Count}");
                foreach (var i in line.Points)
                {
                    Debug.WriteLine($"X :{i.X} Y {i.Y}");

                }
                var addLineCommand = new AddLineCommand<Polyline>(line, _lineManager);
                _undoManager.Execute(addLineCommand);
                //LineCanvas.Children.Add(line);
            }
            LineCanvas.Children.Remove(_currentLine);
            _isConnecting = false;
            _startLinePoint = default;
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            // 增加一行
            GridRowSize++;
            //CreateGrid(GridRowSize, GridColumnSize);
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (GridRowSize > 1)
            {

                var lists = LineCanvas.Children.Cast<Polyline>().ToList();

                //lists.Find( x=>x.Name)

                // 删除一行
                //GridRowSize--;
                //CreateGrid(GridRowSize, GridColumnSize);
            }
        }

        private void AddCloumn_Click(object sender, RoutedEventArgs e)
        {
            // 增加一列
            GridColumnSize++;
            //CreateGrid(GridRowSize, GridColumnSize);
        }

        private void DeleteCloumn_Click(object sender, RoutedEventArgs e)
        {
            if (GridColumnSize > 1)
            {
                // 删除一列
                GridColumnSize--;
                //CreateGrid(GridRowSize, GridColumnSize);
            }
        }

        private Point GetPointInDesignCanvas(Point point, bool isCalcuScroll)
        {
            var scrollOffset = new Point(DesignCanvasScrollViewer.HorizontalOffset, DesignCanvasScrollViewer.VerticalOffset);
            if (!isCalcuScroll)
            {
                scrollOffset = default;
            }
            double X = point.X + scrollOffset.X;
            double Y = point.Y + scrollOffset.Y;
            point = _PLCDesignGridServices.GetGridIndexPoint(X, Y);
            return point;

        }
        public void StartConnection(Ellipse startEllipse)
        {
            Point point = GetGlobalPosition(startEllipse);
            point = GetPointInDesignCanvas(point, true);
            //获取当前点所在的网格单元
            StartConnection(point);


        }

        public void StartConnection(Point point)
        {
            if (_isConnecting == false && _startLinePoint == default )
            {
                _startLinePoint = point;
                _isConnecting = true;
          
        
                _currentLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = point.X,
                    Y1 = point.Y,
                    X2 = point.X,
                    Y2 = point.Y
                };
                _lineManager.ToggleEllipsesVisibility();
                LineCanvas.Children.Add(_currentLine);
            }
            else if(_isConnecting  && _startLinePoint != default)
            {
                DrawLine(_startLinePoint, point);
            }
        }
        public Point GetGlobalPosition(UIElement element)
        {
            GeneralTransform transform = element.TransformToAncestor(this);
            Point position = transform.Transform(new Point(0, 0));
            return position;
        }
        private void DesignCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 如果当前处于连线模式
            if (_isConnecting)
            {
                // 移除未完成的连线
                if (_currentLine != null)
                {
                    _lineManager.ToggleEllipsesHidden();
                    LineCanvas.Children.Remove(_currentLine);
                    _currentLine = null;
                }

                // 重置连线状态
                _isConnecting = false;
                _startLinePoint = default;
                //Debug.WriteLine("连线已取消");
            }
            //e.Handled = true;
        }
        private void DeleteHorizontalLine_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DeleteHorizontalLine_Click");
            if (_PLCDesignGridServices._selectedRectangle != null)
            {

            }



        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
