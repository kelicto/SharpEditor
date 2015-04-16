// Name: MainWindow.xaml.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: Class for the main window. Will delegate all the operations on the main window and distribute all proper operations to their
// respective classes.

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
using System.IO;

namespace SharpEditor
{
    public partial class MainWindow : Window
    {
        private int _count = 1;
        private int _index = 0;
        private DocumentManager _currentDocument;
        private TabItem _newTab = new TabItem();
        private TabItem _previousTab = null;
        private String _previousDocument;
        private Dictionary<string, DocumentManager> _listDocuments = new Dictionary<string, DocumentManager>(); 


        public MainWindow()
        {
            InitializeComponent();
            DocumentManager _documentManager = new DocumentManager(body);

            // checks if user opened a document
            if (_documentManager.OpenDocument())
            {
                tabBar.Items.RemoveAt(0);
                _newTab.Header = System.IO.Path.GetFileName(_documentManager.FileName);
                _newTab.Content = body;

                tabBar.Items.Add(_newTab);
                status.Text = "Document loaded.";

                _listDocuments.Add(_newTab.Header.ToString(), _documentManager);
                _currentDocument = _documentManager;
            }

            else
            {
                tabBar.Items.RemoveAt(0);
                _newTab.Header = "new.txt";
                _newTab.Content = body;
                tabBar.Items.Add(_newTab);

                status.Text = "New Document.";

                _documentManager.NewDocument();
                _listDocuments.Add(_newTab.Header.ToString(), _documentManager);
                _currentDocument = _documentManager;
                _count++;
            }
        }

        private void TextEditorToolBar_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            // if the user selects an option on the texteditortoolbar the change is made respectively to the document
            ComboBox source = e.OriginalSource as ComboBox;
            if (source == null) return;
            switch (source.Name)
            {
                case "fonts":
                    _currentDocument.ApplyToSelection(TextBlock.FontFamilyProperty, source.SelectedItem);
                    break;
                case "fontSize":
                    _currentDocument.ApplyToSelection(TextBlock.FontSizeProperty, source.SelectedItem);
                    break;
                case "fontColour":
                    ComboBoxItem tempItem = new ComboBoxItem();
                    tempItem = (ComboBoxItem)source.SelectedItem;
                    if (tempItem != null)
                        _currentDocument.ApplyToSelection(TextBlock.ForegroundProperty, tempItem.Background);
                    break;
            }

            body.Focus();
        }

        private void body_SelectionChanged (object sender, RoutedEventArgs e)
        {
            toolbar.SynchronizeWith(body.Selection);
            status.Text = "Ln: " + LineNumber() + " Col: " + ColumnNumber();
        }

        private void NewDocument (object sender, ExecutedRoutedEventArgs e)
        {
            DocumentManager _documentManager = new DocumentManager(body);
            _newTab = new TabItem();

            _documentManager.NewDocument();

            _newTab.Header = "new" + _count + ".txt";
            _newTab.Content = body;

            tabBar.Items.Add(_newTab);
            tabBar.SelectedIndex = tabBar.Items.IndexOf(_newTab);

            status.Text = "New Document.";
            _listDocuments.Add(_newTab.Header.ToString(), _documentManager);
            _count++;
            body.Focus();
        }

        private void SaveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (tabBar.SelectedIndex < 0)
                return;

            TabItem tempItem;
            DocumentManager temp;

            _index = tabBar.SelectedIndex;
            tempItem = (TabItem)tabBar.Items[_index];
            temp = _listDocuments[tempItem.Header.ToString()];

            _listDocuments[tempItem.Header.ToString()].SaveDocument();
            
            _newTab = (TabItem)tabBar.Items[_index];
            if (System.IO.Path.GetFileName(_listDocuments[tempItem.Header.ToString()].FileName) != null)
            {
                _newTab.Header = System.IO.Path.GetFileName(_listDocuments[tempItem.Header.ToString()].FileName);
                _listDocuments.Remove(tempItem.Header.ToString());
                _listDocuments.Add(_newTab.Header.ToString(), temp);
            }

            tabBar.Items.RemoveAt(_index);
            tabBar.Items.Insert(_index, _newTab);
            tabBar.SelectedIndex = tabBar.Items.IndexOf(_newTab);
            
            status.Text = "Document Saved.";
            body.Focus();
        }

        private void SaveAsDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (tabBar.SelectedIndex < 0)
                return;

            TabItem tempItem;
            DocumentManager temp;

            _index = tabBar.SelectedIndex;
            tempItem = (TabItem)tabBar.Items[_index];
            temp = _listDocuments[tempItem.Header.ToString()];

            if (_listDocuments[tempItem.Header.ToString()].SaveDocumentAs())
            {
                _newTab = (TabItem)tabBar.Items[_index];
                _newTab.Header = System.IO.Path.GetFileName(_listDocuments[tempItem.Header.ToString()].FileName);
                _listDocuments.Remove(tempItem.Header.ToString());
                _listDocuments.Add(_newTab.Header.ToString(), temp);

                tabBar.Items.RemoveAt(_index);
                tabBar.Items.Insert(_index, _newTab);
                status.Text = "Document Saved.";
                body.Focus();
            }
        }

        private void OpenDocument(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentManager _documentManager = new DocumentManager(body);
            _newTab = new TabItem();

            if (_documentManager.OpenDocument())
            {
                _newTab.Header = System.IO.Path.GetFileName(_documentManager.FileName);
                _newTab.Content = body;

                tabBar.Items.Add(_newTab);
                tabBar.SelectedIndex = tabBar.Items.IndexOf(_newTab);

                status.Text = "Document loaded.";
                _listDocuments.Add(_newTab.Header.ToString(), _documentManager);
            }   
        }

        private void CloseDocument(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem tempItem;

            _index = tabBar.SelectedIndex;
            tempItem = (TabItem)tabBar.Items[_index];

            tabBar.Items.RemoveAt(_index);
            _listDocuments.Remove(tempItem.Header.ToString());

            status.Text = "";
        }

        // Method: PrintDocument()
        // Takes: object sender, ExecutedRoutedEventArgs e
        // Returns: Nothing
        // Purpose: Sends a document to be printed.

        private void PrintDocument(object sender, ExecutedRoutedEventArgs e)
        {
            PrintManager printManager = new PrintManager(body);
            printManager.Print();
        }

        // Method: PrintPreviewDocument
        // Takes: object sender, ExecutedRoutedEventArgs e
        // Returns: Nothing
        // Purpose: Opens a new window to show a print preview of the document.

        private void PrintPreviewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            PrintManager printManager = new PrintManager(body);          
        }

        // Method: LineNumber()
        // Takes: Nothing
        // Returns: int
        // Purpose: retrieve the current line number

        private int LineNumber()
        {
            TextPointer caretLineStart =
                           body.CaretPosition.GetLineStartPosition(0);
            TextPointer p = body.Document.ContentStart.GetLineStartPosition(0);
            int currentLineNumber = 1;

            while (true)
            {
                if (caretLineStart.CompareTo(p) < 0)
                {
                    break;
                }
                int result;
                p = p.GetLineStartPosition(1, out result);
                if (result == 0)
                {
                    break;
                }
                currentLineNumber++;
            }
            return currentLineNumber;
        }

        // Method: ColumnNumber()
        // Takes: Nothing
        // Returns: int
        // Purpose: retrieve the current column number

        private int ColumnNumber()
        {
            TextPointer caretPos = body.CaretPosition;
            TextPointer p = body.CaretPosition.GetLineStartPosition(0);
            int currentColumnNumber = Math.Max(p.GetOffsetToPosition(caretPos) - 1, 0);

            return currentColumnNumber;
        }

        // Method: tabBar_SelectionChanged
        // Takes: object sender, SelectionChangedEventArgs e
        // Returns: Nothing
        // Purpose: Handle the operations of updating the documents when they are switched out of focus 
        // and into focus

        private void tabBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tempControl = (TabControl)e.OriginalSource;
            TabItem tempItem;

            if (_previousTab != null && _listDocuments.ContainsKey(_previousTab.Header.ToString()))
            {
                _listDocuments[_previousTab.Header.ToString()].Content = _previousDocument;
            }
           
            _previousDocument = GetRtfStringFromRichTextBox(body);

            if (tempControl.Items.Count == 0 || tempControl.SelectedIndex < 0)
                 return;

            _index = tempControl.SelectedIndex;
            tempItem = (TabItem)tempControl.Items[_index];

            if (_listDocuments.ContainsKey(tempItem.Header.ToString()) && _previousTab != null)
            {
                _currentDocument = _listDocuments[tempItem.Header.ToString()];
                
                if (_currentDocument.Content != null)
                {
                    _currentDocument = _listDocuments[tempItem.Header.ToString()];
                    FlowDocument fd = new FlowDocument();
                    MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(_currentDocument.Content));
                    TextRange textRange = new TextRange(fd.ContentStart, fd.ContentEnd);
                    textRange.Load(ms, DataFormats.Rtf);
                    body.Document = fd;

                    _previousDocument = GetRtfStringFromRichTextBox(body);

                    if(_listDocuments.ContainsKey(tempItem.Header.ToString()))
                        _listDocuments[tempItem.Header.ToString()].Content = _previousDocument;
                }
            }

            if (_previousTab == tempItem)
                _previousTab = null;
            else
                _previousTab = tempItem;
        }

        // Method: GetRtfStringFromRichTextBox()
        // Takes: Richtext richTextBox
        // Returns: static string
        // Purpose: Retrieves a string value from a richtextbox with its formatting entact.

        public static string GetRtfStringFromRichTextBox(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            textRange.Save(ms, DataFormats.Rtf);

            return Encoding.Default.GetString(ms.ToArray());
        }

    }
}