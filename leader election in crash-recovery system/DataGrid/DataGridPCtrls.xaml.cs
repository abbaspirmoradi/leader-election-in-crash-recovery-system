using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace leader_election_in_crash_recovery_system
{
    /// <summary>
    /// Interaction logic for PCtrlsDatagrid.xaml
    /// </summary>
    public partial class DataGridPCtrls : UserControl
    {
        private static readonly DependencyProperty GridLinesColorProperty =
                DependencyProperty.Register("GridLinesColor", typeof(Brush), typeof(DataGridPCtrls), new FrameworkPropertyMetadata(Brushes.Black, GridLinesColorChangedCallback));

        private static readonly DependencyProperty GridBackColorProperty =
                DependencyProperty.Register("GridBackColor", typeof(Brush), typeof(DataGridPCtrls), new FrameworkPropertyMetadata(GridBackColorChangedCallback));

        private static readonly DependencyProperty CanUserResizeColumnsProperty =
                DependencyProperty.Register("CanUserResizeColumns", typeof(bool), typeof(DataGridPCtrls));

        private static readonly DependencyProperty CanUserSortColumnsProperty =
                DependencyProperty.Register("CanUserSortColumns", typeof(bool), typeof(DataGridPCtrls));

        static DataGridPCtrls()
        {
            try
            {
                FrameworkPropertyMetadata CanUserResizeColumnsMetaData = new FrameworkPropertyMetadata();
                CanUserResizeColumnsMetaData.CoerceValueCallback = new CoerceValueCallback(CoerceCanUserResizeColumns);
                CanUserResizeColumnsProperty.OverrideMetadata(typeof(DataGridPCtrls), CanUserResizeColumnsMetaData);

                FrameworkPropertyMetadata CanUserSortColumnsMetaData = new FrameworkPropertyMetadata();
                CanUserSortColumnsMetaData.CoerceValueCallback = new CoerceValueCallback(CoerceCanUserSortColumns);
                CanUserSortColumnsProperty.OverrideMetadata(typeof(DataGridPCtrls), CanUserSortColumnsMetaData);

                FrameworkPropertyMetadata BorderThicknessMetaData = new FrameworkPropertyMetadata();
                BorderThicknessMetaData.CoerceValueCallback = new CoerceValueCallback(CoerceBorderThickness);
                UserControl.BorderThicknessProperty.OverrideMetadata(typeof(DataGridPCtrls), BorderThicknessMetaData);
            }
            catch (Exception)
            {
            }
        }

        private delegate void FuncColorDelegate(Column Col, Brush Value);

        private static void GridColorFuncChanged(DependencyObject d, object value, FuncColorDelegate FuncColDel)
        {
            DataGridPCtrls DatagridPer = d as DataGridPCtrls;

            if (DatagridPer != null && value is Brush)
            {
                for (int IdxCol = 0; IdxCol < DatagridPer.CtrlStack.Children.Count; IdxCol++)
                {
                    Column col = DatagridPer.CtrlStack.Children[IdxCol] as Column;

                    if (col != null)
                        FuncColDel(col, (Brush)value);
                }
            }
        }

        private static void GridLinesColorFun(Column col, Brush brush)
        {
            col.GridLinesColor = brush;
        }

        private static void GridLinesColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridColorFuncChanged(d, e.NewValue, GridLinesColorFun);
        }

        private static void GridBackColorFun(Column col, Brush brush)
        {
            col.GridBackColor = brush;
        }

        private static void GridBackColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridColorFuncChanged(d, e.NewValue, GridBackColorFun);
        }

        private static object CoerceCanUserResizeColumns(DependencyObject d, object value)
        {
            DataGridPCtrls DatagridPer = d as DataGridPCtrls;

            if (DatagridPer != null && value is bool)
                DatagridPer.Headers.CanUserResizeColumns = (bool)value;

            return value;
        }

        private static object CoerceCanUserSortColumns(DependencyObject d, object value)
        {
            DataGridPCtrls DatagridPer = d as DataGridPCtrls;

            if (DatagridPer != null && value is bool)
                DatagridPer.Headers.CanUserSortColumns = (bool)value;

            return value;
        }

        private static object CoerceBorderThickness(DependencyObject d, object value)
        {
            DataGridPCtrls DatagridPer = d as DataGridPCtrls;

            if (DatagridPer != null && value is Thickness)
                DatagridPer.GridBorder.BorderThickness = (Thickness)value;

            return new Thickness(0.0);
        }

        private bool HeadersInitialized = false;
        private bool HorizScrolling = false;

        private struct SortStruct
        {
            public List<UIElement> RowList;
            public object SortingObj;
        }

        public Brush GridLinesColor
        {
            get { return (Brush)GetValue(GridLinesColorProperty); }
            set { SetValue(GridLinesColorProperty, value); }
        }

        public Brush GridBackColor
        {
            get { return (Brush)GetValue(GridBackColorProperty); }
            set { SetValue(GridBackColorProperty, value); }
        }

        public bool CanUserResizeColumns
        {
            get { return (bool)GetValue(CanUserResizeColumnsProperty); }
            set { SetValue(CanUserResizeColumnsProperty, value); }
        }

        public bool CanUserSortColumns
        {
            get { return (bool)GetValue(CanUserSortColumnsProperty); }
            set { SetValue(CanUserSortColumnsProperty, value); }
        }

        public DataGridPCtrls()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DataGridPCtrls_Loaded);
        }

        private void DataGridPCtrls_Loaded(object sender, RoutedEventArgs e)
        {
            // Called when change theme, so any re-initialization here...
            // Brushes on SplitterGrid need to be re-set...
            SetSplitterBrushes();

            // Little up/down arrow needs to be re-displayed...
            foreach (UIElement ui in Headers)
                ((Header)ui).UpdateSortArrow();

            // Row height needs to be reset...
            foreach (UIElement ui in CtrlStack.Children)
                ((Column)ui).ResetRowHeight();
        }

        private void PART_HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!HorizScrolling)
            {
                HorizScrolling = true;
                HeaderScroll.ScrollToHorizontalOffset(e.NewValue);
                HorizScrolling = false;
            }
        }

        private void HeaderScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // To see why this is necessary: comment this, and try SHIFT+Tab repeatedly until tab into header,
            // which may result in it scrolling independently of CtrlScroll.
            // This code insures CtrlScroll is always mapped to header.
            // HorizScrolling variable is for insuring functions don't repeatedly call each other.
            if (!HorizScrolling)
            {
                HorizScrolling = true;
                CtrlScroll.ScrollToHorizontalOffset(e.NewValue);
                HorizScrolling = false;
            }
        }

        private void PART_VerticalScrollBar_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.Controls.Primitives.ScrollBar sb = sender as System.Windows.Controls.Primitives.ScrollBar;

            if (sb != null)
            {
                if (sb.IsVisible)
                    HeaderScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                else
                    HeaderScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private void SetSplitterBrushes()
        {
            object NormalBrush = TryFindResource("DGP_Head_NormalBorderBrush");

            if (NormalBrush != null)
                Headers.SplitterBrush = (Brush)NormalBrush;

            // Alternative notation:
            // Header.SplitterBrush = (Brush)this.Resources["DGP_Head_NormalBorderBrush"];

            object SelectBrush = TryFindResource("DGP_Head_HighlightBorderBrush");

            if (SelectBrush != null)
                Headers.HighlightSplitterBrush = (Brush)SelectBrush;
        }

        public void AddColumn(string strHeader, ColumnType ct = ColumnType.TextBox, double width = 145.0)
        {
            Header Head = new Header(strHeader, OnHeaderClicked);
            this.Headers.AddColumn(Head, width - Headers.SplitWidth);

            if (!HeadersInitialized)
            {
                SetSplitterBrushes();
                HeadersInitialized = true;
            }
            Column col = new Column(this, OnKeyDown, ct, width);
            col.GridLinesColor = GridLinesColor;
            col.GridBackColor = GridBackColor;

            if (CtrlStack.Children.Count != 0)
            {
                int Rows = ((Column)CtrlStack.Children[0]).RowCount();

                while (Rows-- != 0)
                    col.AddRow();
            }
            CtrlStack.Children.Add(col);
        }

        public void AddRow()
        {
            foreach (Column col in CtrlStack.Children)
                col.AddRow();
        }

        public void AddRow(object[] RowDetails)
        {
            for (int Idx = 0; Idx < CtrlStack.Children.Count; Idx++)
            {
                if (Idx < RowDetails.Count())
                    ((Column)CtrlStack.Children[Idx]).AddRow(RowDetails[Idx]);
                else
                    ((Column)CtrlStack.Children[Idx]).AddRow();
            }
        }

        public void GetRowDetails(int row, out object[] RowDetails)
        {
            if (row >= 0 && CtrlStack.Children.Count >= 1 && row < ((Column)CtrlStack.Children[0]).RowCount())
            {
                RowDetails = new object[CtrlStack.Children.Count];

                for (int Idx = 0; Idx < CtrlStack.Children.Count; Idx++)
                    RowDetails[Idx] = ((Column)CtrlStack.Children[Idx]).GetRowValue(row);
            }
            else
                RowDetails = null;
        }

        public void SetRowDetails(int row, object[] RowDetails)
        {
            if (row >= 0 && CtrlStack.Children.Count >= 1 && row < ((Column)CtrlStack.Children[0]).RowCount())
            {
                for (int Idx = 0; Idx < CtrlStack.Children.Count; Idx++)
                    ((Column)CtrlStack.Children[Idx]).SetRowValue(row, RowDetails[Idx]);
            }
        }

        public int GetRowCount(int iCol = 0)
        {
            if (iCol >= 0 && iCol < CtrlStack.Children.Count)
                return ((Column)CtrlStack.Children[iCol]).RowCount();

            return 0;
        }

        public Control GetControl(int iRow, int iCol)
        {
            Control Ctrl = null;

            if (iCol >= 0 && iCol < CtrlStack.Children.Count && iRow >= 0 && iRow < ((Column)CtrlStack.Children[iCol]).RowCount())
                Ctrl = ((Column)CtrlStack.Children[iCol]).GetRowControl(iRow);

            return Ctrl;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(HeaderScroll);

            double Scrollwidth = HeaderScroll.ActualWidth;

            ScrollBar VertScrollBar = CtrlScroll.Template.FindName("PART_VerticalScrollBar", CtrlScroll) as ScrollBar;

            if (HeaderScroll.ComputedVerticalScrollBarVisibility == Visibility.Visible && VertScrollBar != null)
                Scrollwidth -= VertScrollBar.Width;

            if (pt.Y >= 0 && pt.Y <= HeaderScroll.ActualHeight)
            {
                if (pt.X >= 0 && pt.X <= Scrollwidth)
                    Headers.SplitGrid_MouseMove(sender, e);
                else if (HeaderScroll.ComputedVerticalScrollBarVisibility == Visibility.Visible && pt.X > Scrollwidth && pt.X <= HeaderScroll.ActualWidth)
                    Mouse.Capture(VertScrollBar, CaptureMode.None);
            }
        }

        private void Headers_WidthChanged(object sender, WidthChangedEventArgs e)
        {
            Column col = CtrlStack.Children[e.Index] as Column;

            if (col != null)
            {
                col.Width = e.Width;
            }
        }

        private double GetHeaderHeight()
        {
            object Obj = TryFindResource("DGP_Head_Height");
            return (Obj is GridLength) ? ((GridLength)Obj).Value : 0.0;
        }

        private int GetVisibleRowCount()
        {
            double VisibleHeight = CtrlScroll.ActualHeight - GetHeaderHeight();

            if (CtrlScroll.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                VisibleHeight -= SystemParameters.HorizontalScrollBarHeight;

            double RowHeight = CtrlStack.Children.Count > 0 ? ((Column)CtrlStack.Children[0]).GetRowHeight() : 25.0;
            // Assume a row is visible if half or more of it is visible, not visible otherwise.
            return (int)Math.Floor(VisibleHeight / RowHeight + 0.5);
        }

        private bool OnKeyDown(Key key, int iRow, Column Col, bool ShiftPressed)
        {
            int iCol = CtrlStack.Children.IndexOf((UIElement)Col);
            bool handled = true;

            if (key == Key.Tab)
            {
                if (!ShiftPressed)
                {
                    if (iCol < CtrlStack.Children.Count - 1)
                    {
                        Column newCol = (Column)CtrlStack.Children[iCol + 1];
                        newCol.SetActive(iRow);
                    }
                    else
                    {
                        Column newCol = (Column)CtrlStack.Children[0];

                        if (iRow < newCol.RowCount() - 1)
                            newCol.SetActive(iRow + 1);
                        else
                            handled = false;
                    }
                }
                else
                {
                    if (iCol > 0)
                    {
                        Column newCol = (Column)CtrlStack.Children[iCol - 1];
                        newCol.SetActive(iRow);
                    }
                    else
                    {
                        Column newCol = (Column)CtrlStack.Children[CtrlStack.Children.Count - 1];

                        if (iRow > 0)
                            newCol.SetActive(iRow - 1);
                        else
                            handled = false;
                    }
                }
            }
            else if (key == Key.Up)
            {
                if (iRow != 0)
                    ((Column)CtrlStack.Children[iCol]).SetActive(iRow - 1);
            }
            else if (key == Key.Down)
            {
                if (iRow != ((Column)CtrlStack.Children[iCol]).RowCount() - 1)
                    ((Column)CtrlStack.Children[iCol]).SetActive(iRow + 1);
            }
            else if (key == Key.Left)
            {
                if (iCol != 0)
                    ((Column)CtrlStack.Children[iCol - 1]).SetActive(iRow);
            }
            else if (key == Key.Right)
            {
                if (iCol != CtrlStack.Children.Count - 1)
                    ((Column)CtrlStack.Children[iCol + 1]).SetActive(iRow);
            }
            else if (key == Key.PageDown)
            {
                int VisibleRows = GetVisibleRowCount();

                if (iRow + VisibleRows < ((Column)CtrlStack.Children[iCol]).RowCount())
                    ((Column)CtrlStack.Children[iCol]).SetActive(iRow + VisibleRows);
                else
                    ((Column)CtrlStack.Children[iCol]).SetActive(((Column)CtrlStack.Children[iCol]).RowCount() - 1);
            }
            else if (key == Key.PageUp)
            {
                int VisibleRows = GetVisibleRowCount();

                if (iRow - VisibleRows > 0)
                    ((Column)CtrlStack.Children[iCol]).SetActive(iRow - VisibleRows);
                else
                    ((Column)CtrlStack.Children[iCol]).SetActive(0);
            }
            else if (key == Key.Home)
            {
                ((Column)CtrlStack.Children[iCol]).SetActive(0);
            }
            else if (key == Key.End)
            {
                ((Column)CtrlStack.Children[iCol]).SetActive(((Column)CtrlStack.Children[iCol]).RowCount() - 1);
            }
            return handled;
        }

        private void Sort(int ColumnIndex, bool Ascending = true)
        {
            // Assume all rows are of same length - this is currently always the case.
            Column SelCol = CtrlStack.Children[ColumnIndex] as Column;

            if (SelCol != null)
            {
                SortStruct[] AllData = new SortStruct[SelCol.RowCount()];

                for (int Idx = 0; Idx < SelCol.RowCount(); Idx++)
                {
                    AllData[Idx].RowList = new List<UIElement>();
                    AllData[Idx].SortingObj = SelCol.GetRowValue(Idx);
                }

                for (int ColIdx = 0; ColIdx < CtrlStack.Children.Count; ColIdx++)
                {
                    Column Col = CtrlStack.Children[ColIdx] as Column;

                    if (Col != null)
                    {
                        for (int RowIdx = 0; RowIdx < SelCol.RowCount(); RowIdx++)
                        {
                            AllData[RowIdx].RowList.Add(Col.GetRowControl(RowIdx));
                        }
                    }
                }

                IEnumerable<SortStruct> Data;

                Data = (Ascending) ? from d in AllData
                                     orderby d.SortingObj ascending
                                     select d
                                  : from d in AllData
                                    orderby d.SortingObj descending
                                    select d;

                for (int ColIdx = 0; ColIdx < CtrlStack.Children.Count; ColIdx++)
                {
                    Column Col = CtrlStack.Children[ColIdx] as Column;

                    if (Col != null)
                    {
                        for (int RowIdx = 0; RowIdx < Col.RowCount(); RowIdx++)
                            Col.SetRowControl(RowIdx, null);
                    }
                }

                int RIdx = 0;
                foreach (SortStruct st in Data)
                {
                    for (int ColIdx = 0; ColIdx < CtrlStack.Children.Count; ColIdx++)
                    {
                        Column Col = CtrlStack.Children[ColIdx] as Column;

                        if (Col != null)
                            Col.SetRowControl(RIdx, (Control)st.RowList[ColIdx]);
                    }
                    RIdx++;
                }
            }
        }

        private SortType OnHeaderClicked(Header header, SortType PreviousSortType)
        {
            SortType NewSortType;

            int ColIdx = Headers.IndexOf((UIElement)header);

            if (PreviousSortType == SortType.NotOrdered)
            {
                // set all columns to unsorted.
                foreach (UIElement ui in Headers)
                    ((Header)ui).ResetSortType();

                // Alternative notation:
                //  for (int Idx = 0; Idx < Headers.Count; Idx++)
                //      ((Header)Headers[Idx]).ResetSortType();

                Sort(ColIdx, true);
                NewSortType = SortType.Ascending;
            }
            else if (PreviousSortType == SortType.Ascending)
            {
                Sort(ColIdx, false);
                NewSortType = SortType.Descending;
            }
            else //if (PreviousSortType == SortType.Descending)
            {
                Sort(ColIdx, true);
                NewSortType = SortType.Ascending;
            }
            return NewSortType;
        }
    }
}