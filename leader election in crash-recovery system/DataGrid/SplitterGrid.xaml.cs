using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace leader_election_in_crash_recovery_system
{
    public delegate void WidthChangedEventHandler(object sender, WidthChangedEventArgs e);

    public class WidthChangedEventArgs : RoutedEventArgs
    {
        public WidthChangedEventArgs(RoutedEvent RoutedEvent)
            : base(RoutedEvent)
        {
        }

        public double Width
        {
            get;
            set;
        }

        public double Offset
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for SplitterGrid.xaml
    /// </summary>
    partial class SplitterGrid : UserControl, IEnumerable
    {
        private static double arrowScroll = 10.0;
        private readonly double splitCapture = 2.0;
        private readonly double splitWidth = 1.0;
        private double colMinWidth = 20.0;
        private Brush SplitBrush = Brushes.Black;
        private bool CanResizeCols = true;

        public static readonly RoutedEvent WidthChangedEvent = EventManager.RegisterRoutedEvent("WidthChanged",
                                     RoutingStrategy.Bubble, typeof(WidthChangedEventHandler), typeof(SplitterGrid));

        public event WidthChangedEventHandler WidthChanged
        {
            add { AddHandler(WidthChangedEvent, value); }
            remove { RemoveHandler(WidthChangedEvent, value); }
        }

        public SplitterGrid()
        {
            InitializeComponent();
            SplitGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.0) });
            HighlightSplitterBrush = Brushes.Black;
            CanUserSortColumns = true;
        }

        public double SplitCapture { get { return splitCapture; } }

        public double SplitWidth { get { return splitWidth; } }

        public double ColumnMinWidth { get { return colMinWidth; } }

        public Brush SplitterBrush
        {
            get { return SplitBrush; }
            set
            {
                SplitBrush = value;

                for (int Idx = 0; Idx < SplitGrid.Children.Count; Idx++)
                {
                    if (SplitGrid.Children[Idx] is GridSplitter)
                        ((GridSplitter)SplitGrid.Children[Idx]).Background = SplitBrush;
                }
            }
        }

        public Brush HighlightSplitterBrush { get; set; }

        public bool CanUserSortColumns { get; set; }

        public bool CanUserResizeColumns
        {
            get { return CanResizeCols; }
            set
            {
                CanResizeCols = value;

                foreach (UIElement item in SplitGrid.Children)
                {
                    GridSplitter gridSplit = item as GridSplitter;

                    if (gridSplit != null)
                        gridSplit.Focusable = CanResizeCols;
                }
            }
        }

        private void AddSplitter(int Idx = -1)
        {
            ColumnDefinition cd = new ColumnDefinition() { Width = new GridLength(splitWidth) };

            if (Idx == -1)
                SplitGrid.ColumnDefinitions.Add(cd);
            else
                SplitGrid.ColumnDefinitions.Insert(Idx, cd);

            GridSplitter gridSplit = new GridSplitter()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ResizeDirection = GridResizeDirection.Columns,
                Background = Brushes.Black
            };

            gridSplit.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(gridSplit_DragDelta);
            gridSplit.PreviewKeyDown += new KeyEventHandler(gridSplit_PreviewKeyDown);
            gridSplit.Focusable = CanResizeCols;
            gridSplit.Background = SplitBrush;

            if (Idx == -1)
                SplitGrid.Children.Add(gridSplit);
            else
                SplitGrid.Children.Insert(Idx, gridSplit);

            Grid.SetColumn(gridSplit, (Idx == -1) ? SplitGrid.ColumnDefinitions.Count() : Idx);
            Grid.SetRow(gridSplit, 0);
        }

        private void gridSplit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            double Resize = 0.0;

            if (e.Key == Key.Left)
                Resize -= arrowScroll;
            else if (e.Key == Key.Right)
                Resize += arrowScroll;

            if (Resize != 0.0)
            {
                GridSplitter gridSplit = sender as GridSplitter;

                if (gridSplit != null)
                {
                    int Idx = Grid.GetColumn(gridSplit);

                    if (Idx > 0)
                    {
                        ColumnDefinition cd = SplitGrid.ColumnDefinitions[Idx - 1];
                        cd.Width = new GridLength(cd.Width.Value + Resize);
                        gridSplit_FireWidthChanged(sender);
                        e.Handled = true;
                    }
                }
            }
        }

        private void gridSplit_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            gridSplit_FireWidthChanged(sender);
        }

        private void gridSplit_FireWidthChanged(object sender)
        {
            GridSplitter gridSplit = sender as GridSplitter;

            if (gridSplit != null)
            {
                int Idx = Grid.GetColumn(gridSplit);

                if (Idx > 0)
                {
                    ColumnDefinition cd = SplitGrid.ColumnDefinitions[Idx - 1];
                    WidthChangedEventArgs args = new WidthChangedEventArgs(WidthChangedEvent);
                    args.Index = Idx / 2;
                    args.Width = cd.Width.Value + splitWidth;
                    args.Offset = cd.Offset;
                    RaiseEvent(args);
                }
            }
        }

        public void AddColumn(UIElement item, double Width = 144.0)
        {
            ColumnDefinition cd = new ColumnDefinition() { Width = new GridLength(Width) };

            cd.MinWidth = (colMinWidth < Width) ? colMinWidth : Width;
            SplitGrid.ColumnDefinitions.Insert(SplitGrid.ColumnDefinitions.Count() - 1, cd);  // Note: we have already inserted a column definition, which should remain as last element. Required for resizing last column
            item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            SplitGrid.Children.Add(item);

            Grid.SetColumn(item, SplitGrid.Children.Count - 1);
            Grid.SetRow(item, 0);

            AddSplitter(SplitGrid.ColumnDefinitions.Count() - 1);
        }

        private void SetGridSplitterBrushFromObj(object sender, Brush brush)
        {
            if (CanUserSortColumns && sender is UIElement)
            {
                int Idx = SplitGrid.Children.IndexOf((UIElement)sender);

                if (SplitGrid.Children[Idx + 1] is GridSplitter)
                {
                    GridSplitter gridSplit = (GridSplitter)SplitGrid.Children[Idx + 1];
                    gridSplit.Background = brush;
                }
            }
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            SetGridSplitterBrushFromObj(sender, SplitBrush);
        }

        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            SetGridSplitterBrushFromObj(sender, HighlightSplitterBrush);
        }

        public void RemoveColumn(int Idx)
        {
            if (Idx >= SplitGrid.ColumnDefinitions.Count() / 2)
                return;

            SplitGrid.Children.RemoveAt(Idx * 2 + 1);
            SplitGrid.Children.RemoveAt(Idx * 2);

            SplitGrid.ColumnDefinitions.RemoveAt(Idx * 2 + 1);
            SplitGrid.ColumnDefinitions.RemoveAt(Idx * 2);

            foreach (UIElement child in SplitGrid.Children)
            {
                int Idx2 = Grid.GetColumn(child);
                if (Idx2 > Idx * 2 + 1)
                    Grid.SetColumn(child, Idx2 - 2);
            }
        }

        public void SplitGrid_MouseMove(object sender, MouseEventArgs e)
        {
            bool Captured = false;
            int Idx = SplitGrid.Children.Count;

            if (CanResizeCols)
            {
                while (--Idx >= 0)
                {
                    if (SplitGrid.Children[Idx] is GridSplitter)
                    {
                        Point pt = Mouse.GetPosition(SplitGrid.Children[Idx]);

                        if ((pt.Y >= 0 && pt.Y <= ((FrameworkElement)SplitGrid.Children[Idx]).ActualHeight)
                            && (pt.X < splitCapture + ((FrameworkElement)SplitGrid.Children[Idx]).ActualWidth && pt.X > -splitCapture))
                        {
                            Mouse.Capture(SplitGrid.Children[Idx]);
                            Captured = true;
                            break;
                        }
                    }
                }
            }
            if (!Captured && CanUserSortColumns)
            {
                Idx = SplitGrid.Children.Count;

                while (--Idx >= 0)
                {
                    if (!(SplitGrid.Children[Idx] is GridSplitter))
                    {
                        Point pt = Mouse.GetPosition(SplitGrid.Children[Idx]);

                        if ((pt.Y >= 0 && pt.Y <= ((FrameworkElement)SplitGrid.Children[Idx]).ActualHeight)
                            && (pt.X <= ((FrameworkElement)SplitGrid.Children[Idx]).ActualWidth && pt.X >= 0))
                        {
                            Mouse.Capture(SplitGrid.Children[Idx]);
                            Captured = true;
                            break;
                        }
                    }
                }
            }
            if (!Captured)
                Mouse.Capture(null);
        }

        public int IndexOf(UIElement uiEle)
        {
            return SplitGrid.Children.IndexOf(uiEle) / 2;
        }

        public int Count
        {
            get { return SplitGrid.Children.Count / 2; }
        }

        public UIElement this[int index]
        {
            get { return SplitGrid.Children[index * 2]; }
            set { SplitGrid.Children.Insert(index * 2, value); }
        }

        public IEnumerator GetEnumerator()
        {
            for (int Idx = 0; Idx < Count; Idx++)
                yield return SplitGrid.Children[Idx * 2];
        }
    }
}