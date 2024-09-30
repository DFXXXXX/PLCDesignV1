using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PLCDesignV1
{
    public  class CommonHelper
    {
        public static T FindParentCanvas<T>(DependencyObject child) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;

            T parentCanvas = parent as T;
            if (parentCanvas != null)
            {
                return parentCanvas;
            }
            else
            {
                return FindParentCanvas<T>(parent);
            }
        }


        public static void  CopyToClipboard<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Text, json);
            // 清空剪贴板

            Clipboard.Clear();
            Clipboard.SetDataObject(dataObject, true);
   
            MessageBox.Show("PLC 模块已复制到剪贴板");

        }

    }



}
