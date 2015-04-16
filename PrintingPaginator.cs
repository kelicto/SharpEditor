// Name: PrintingPaginator.cs
// Author: Chris McManus
// Date: June, 21st, 2013
// Description: Designed to inherit from documentpaginator this is designated to create a paginator appropriate for a print preview.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SharpEditor
{
    class PrintingPaginator : DocumentPaginator
    {
        private readonly DocumentPaginator _originalPaginator;
        private readonly Size _pageSize;
        private readonly Size _pageMargin;

        public PrintingPaginator(DocumentPaginator paginator, Size pageSize, Size margin)
        {
            _originalPaginator = paginator;
            _pageSize = pageSize;
            _pageMargin = margin;

            _originalPaginator.PageSize = new Size(_pageSize.Width - _pageMargin.Width * 2,
                                                   _pageSize.Height - _pageMargin.Height * 2);

            _originalPaginator.ComputePageCount();
        }

        public override bool IsPageCountValid
        {
            get { return _originalPaginator.IsPageCountValid; }
        }

        public override int PageCount
        {
            get { return _originalPaginator.PageCount; }
        }

        public override Size PageSize
        {
            get { return _originalPaginator.PageSize; }
            set { _originalPaginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return _originalPaginator.Source; }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage originalPage = _originalPaginator.GetPage(pageNumber);
            ContainerVisual fixedPage = new ContainerVisual();
            fixedPage.Children.Add(originalPage.Visual);
            fixedPage.Transform = new TranslateTransform(_pageMargin.Width, _pageMargin.Height);
            return new DocumentPage(fixedPage, _pageSize,
                                    AdjustForMargins(originalPage.BleedBox),
                                    AdjustForMargins(originalPage.ContentBox));
        }

        public Rect AdjustForMargins(Rect rect)
        {
            if (rect.IsEmpty)
            {
                return rect;
            }
            else
            {
                return new Rect(rect.Left + _pageMargin.Width, rect.Top + _pageMargin.Height,
                                rect.Width, rect.Height);
            }
        }
    }
}
