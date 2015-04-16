// Name: TextEditorFileBar.xaml.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: Class designed to handle all the operations for the user control TextEditorFileBar

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpEditor
{
 
    public partial class TextEditorFileBar : UserControl
    {
        public TextEditorFileBar()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This text editor was created by Chris McManus.", "About");
        }
    }
}
