// Name: PrintManager.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: A class designed to handle all business operations for a printer.

using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SharpEditor
{
    class PrintManager
    {
        public static readonly int DPI = 96;
        private readonly RichTextBox _textBox;

        public PrintManager(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        public RichTextBox textBox
        {
            get { return textBox; }
        }

        public bool Print()
        {
            PrintDialog dlg = new PrintDialog();

            if (dlg.ShowDialog() == true)
            {
                PrintQueue printQueue = dlg.PrintQueue;
  

                DocumentPaginator paginator = GetPaginator(_textBox.Document.PageWidth,
                                                           _textBox.Document.PageHeight);
                dlg.PrintDocument(paginator, "TextEditor Printing");

                return true;
            }
            return false;
        }

        public DocumentPaginator GetPaginator(double pageWidth, double pageHeight)
        {
            TextRange originalRange = new TextRange(_textBox.Document.ContentStart, _textBox.Document.ContentEnd);

            MemoryStream memoryStream = new MemoryStream();

            originalRange.Save(memoryStream, DataFormats.Xaml);

            FlowDocument copy = new FlowDocument();

            TextRange copyRange = new TextRange(copy.ContentStart, copy.ContentEnd);

            copyRange.Load(memoryStream, DataFormats.Xaml);

            DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

            return new PrintingPaginator(paginator, new Size(pageWidth, pageHeight), new Size(DPI, DPI));
        }
    }
}