using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    /// <summary>
    /// InputParam.xaml 的交互逻辑
    /// </summary>
    public partial class InputParam : Window
    {
        public CustomEllipsControlDataModel DataModel { get; private set; }
        public InputParam()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DataModel = new CustomEllipsControlDataModel
            {
                Name = NameTextBox.Text,
                Address = AddressTextBox.Text,
                Symbol = SymbolTextBox.Text,
                VariableType = (VariableTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                DataType = (DataTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Comment = CommentTextBox.Text,
                IsInput = IsInputCheckBox.IsChecked == true
            };
            DialogResult = true;
  
        }
        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Name.Replace("TextBox", ""))
            {
                textBox.Text = "";
            }
        }

        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Name.Replace("TextBox", "");
            }
        }

    }
}
