// Name: DocumentManager.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: Manager class for all documents. Template for all documents

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace SharpEditor
{
    class DocumentManager
    {
        // class members
        private string _currentFile;
        private RichTextBox _textBox;
        private String _content;
        
        // string tokens for regex C# syntax highlighting
        private string tokens = "(auto|double|int|struct|break|else|long|switch|case|enum|register|typedef|char|extern|return|union|const|float|short|unsigned|continue|for|signed|void|default|goto|sizeof|volatile|do|if|static|while)";

        public String FileName
        {
            get { return _currentFile; }
        }

        public String Content
        {
            set { _content = value; }
            get { return _content; }
        }
       
        public DocumentManager(RichTextBox textBox)
        {
            _textBox = textBox;
            _content = null;
        }

        public bool OpenDocument()
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.Filter = "Text Files (.txt)|*.txt|Rich Text Files (.rtf)|*.rtf|C# Files (.cs)|*.cs";
            dig.FilterIndex = 1;
            if (dig.ShowDialog() == true)
            {
                _currentFile = dig.FileName;

                using (Stream stream = dig.OpenFile())
                {
                    TextRange range = new TextRange(
                        _textBox.Document.ContentStart,
                        _textBox.Document.ContentEnd
                        );

                    switch (Path.GetExtension(_currentFile))
                    {
                        case ".rtf":
                            range.Load(stream, DataFormats.Rtf);
                            break;
                        case ".txt":
                            range.Load(stream, DataFormats.Text);
                            break;
                        case ".cs":
                            range.Load(stream, DataFormats.Rtf);
                            Regex rex = new Regex(tokens);
                            break;
                    }
                }
                return true;
            }
            return false;
        }

        public bool SaveDocument()
        {
            if (string.IsNullOrEmpty(_currentFile))
            {
                return SaveDocumentAs();
            }
            else
            {
                using (Stream stream = new FileStream(_currentFile, FileMode.Create))
                {
                    TextRange range = new TextRange(
                        _textBox.Document.ContentStart,
                        _textBox.Document.ContentEnd
                        );

                    switch(Path.GetExtension(_currentFile)) 
                    {
                        case ".rtf":
                            range.Save(stream, DataFormats.Rtf);
                            break;
                        case ".txt":
                            range.Save(stream, DataFormats.Text);
                            break;
                        case ".cs":
                            range.Load(stream, DataFormats.Text);
                            break;
                    }
                }
                return true;
            }
        }

        public bool SaveDocumentAs()
        {
            SaveFileDialog dig = new SaveFileDialog();
            dig.Filter = "Rich Text DocumentManager|*.rtf|Text  DocumentManager|*.txt";

            if (dig.ShowDialog() == true)
            {
                _currentFile = dig.FileName;
                return SaveDocument();
            }
            return false;
        }

        public void ApplyToSelection(DependencyProperty property, object value)
        {
            if (value != null)
                _textBox.Selection.ApplyPropertyValue(property, value);
        }

        public void NewDocument()
        {
            _currentFile = null;
            _textBox.Document = new FlowDocument();
        }
    }
}
