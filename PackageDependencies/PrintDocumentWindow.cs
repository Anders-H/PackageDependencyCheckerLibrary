#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PackageDependencyCheckerLibrary;

namespace PackageDependencies;

public partial class PrintDocumentWindow : Form
{
    private const int RowsPerPage = 46;
    private int _currentPage;
    private readonly Font _font;
    private bool _hasMorePages;
    public DependencyInfoList? Dependencies;

    public PrintDocumentWindow()
    {
        _font = new Font("Courier New", 7);
        InitializeComponent();
    }

    private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        if (Dependencies == null)
            throw new SystemException("Not correct initialized.");

        btnPrevPage.Enabled = _currentPage > 0;
        var pageData = new List<string> { Dependencies.GetFixedWidthTextHeader() };
        pageData.AddRange(GetPageData());
        printDocument1.DocumentName = "Package dependencies";
        var xStart = printDocument1.DefaultPageSettings.Margins.Left;
        var yStart = printDocument1.DefaultPageSettings.Margins.Top;
        const int yStep = 20;

        for (var i = 0; i < pageData.Count; i++)
        {
            if (i == 0)
            {
                using var boldFont = new Font(_font, FontStyle.Bold);
                e.Graphics.DrawString(pageData[i], boldFont, Brushes.Black, xStart, yStart);
                yStart += yStep;
                e.Graphics.DrawString(Dependencies.GetSeparator(_currentPage == 0 ? '=' : '-'), _font, Brushes.Black, xStart, yStart);
                yStart += yStep;
                continue;
            }
            
            e.Graphics.DrawString(pageData[i], _font, Brushes.Black, xStart, yStart);
            yStart += yStep;
        }

        yStart += yStep;
        e.Graphics.DrawString($"Page {_currentPage + 1}", _font, Brushes.Black, xStart, yStart);
    }

    private List<string> GetPageData()
    {
        if (Dependencies == null)
            throw new SystemException("Not correct initialized.");

        var start = _currentPage * RowsPerPage;

        if (start >= Dependencies.Count)
        {
            _hasMorePages = false;
            return [];
        }

        var response = Dependencies.GetFixedWidthTextRowsAsList(start, RowsPerPage);
        _hasMorePages = response.Count == RowsPerPage;

        if (_hasMorePages)
        {
            var tempStart = (_currentPage + 1) * RowsPerPage;
            var temp = Dependencies.GetFixedWidthTextRowsAsList(tempStart, RowsPerPage);
            
            if (temp.Count <= 0)
                _hasMorePages = false;
        }

        btnNextPage.Enabled = _hasMorePages;
        return response;
    }

    private void btnPrevPage_Click(object sender, EventArgs e)
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            btnNextPage.Enabled = true;
        }

        btnPrevPage.Enabled = _currentPage > 0;
        printPreviewControl1.InvalidatePreview();
    }

    private void btnNextPage_Click(object sender, EventArgs e)
    {
        if (_hasMorePages)
        {
            _currentPage++;
            btnPrevPage.Enabled = true;
        }

        printPreviewControl1.InvalidatePreview();
        btnNextPage.Enabled = _hasMorePages;
    }
}