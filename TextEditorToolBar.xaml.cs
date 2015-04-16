// Name: TextEditorToolBar.xaml.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: A class designed to handle all the operations for the user control TextEditorToolBar.

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
using System.Drawing;
using System.Reflection;

namespace SharpEditor
{
    public partial class TextEditorToolBar : UserControl
    {
        public TextEditorToolBar()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (double i = 8; i < 48; i += 2)
            {
                fontSize.Items.Add(i);
            }

            Type colorType = typeof(System.Drawing.Color);
            ComboBoxItem tempItem;

            System.Drawing.Color tempDrawingColour;
            System.Windows.Media.Color tempMediaColour;
            SolidColorBrush tempBrush;

            foreach (var colorValue in colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                tempDrawingColour = System.Drawing.Color.FromName(colorValue.Name);
                tempMediaColour = System.Windows.Media.Color.FromArgb(tempDrawingColour.A, tempDrawingColour.R, tempDrawingColour.G, tempDrawingColour.B);
                tempBrush = new SolidColorBrush();
                tempBrush.Color = tempMediaColour;

                tempItem = new ComboBoxItem();
                tempItem.Content = colorValue.Name;
                tempItem.Height = 25;
                tempItem.Background = tempBrush;
                fontColour.Items.Add(tempItem);
            }

        }

        public void SynchronizeWith(TextSelection selection)
        {
            isSynchronizing = true;
            Synchronized<double>(selection, TextBlock.FontSizeProperty,
                                              SetFontSize);
            Synchronized<double>(selection, TextBlock.FontSizeProperty,
                                              SetZoomSlider);
            Synchronized<SolidColorBrush>(selection, TextBlock.ForegroundProperty, SetColour);
            Synchronized<FontWeight>(selection, TextBlock.FontWeightProperty, SetFontWeight);
            Synchronized<System.Windows.Media.FontFamily>(selection, TextBlock.FontFamilyProperty, SetFontFamily);
            isSynchronizing = false;
        }

        private void Synchronized<T>(TextSelection selection, DependencyProperty property, Action<T> methodToCall)
        {
            object value = selection.GetPropertyValue(property);
            if (value != DependencyProperty.UnsetValue)
            {
                methodToCall((T)value);
            }
        }

        private void SetFontSize(double size)
        {
            fontSize.SelectedValue = size;
        }

        private void SetFontFamily (System.Windows.Media.FontFamily font)
        {
            fonts.SelectedItem = font;
        }

        private void SetColour(SolidColorBrush colour)
        {
            // Wasn't sure how to get this to synchronize properly. Not fully implemented.

            ComboBoxItem tempItem = new ComboBoxItem();
            tempItem.Background = colour;
            tempItem.Content = System.Drawing.Color.FromArgb(colour.Color.A, colour.Color.R, colour.Color.B, colour.Color.G).Name;
            tempItem.Height = 25;
        }

        private void SetZoomSlider(double size)
        {
            zoomSlider.Value = size;
        }

        private void SetFontWeight(FontWeight weight)
        {
            boldButton.IsChecked = weight == FontWeights.Bold;
        }

        public bool isSynchronizing { get; private set; }

        private void zoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.zoomSlider.Value <= 1)
                this.zoomSlider.Value = 0;

            this.zoomSlider.Value -= 1;
        }

        private void zoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.zoomSlider.Value >= 9)
                this.zoomSlider.Value = 10;
            
            this.zoomSlider.Value += 1;
        }
    }
}
