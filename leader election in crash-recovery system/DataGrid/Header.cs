using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace leader_election_in_crash_recovery_system
{
    public enum SortType : byte
    {
        Ascending,
        Descending,
        NotOrdered
    }

    public delegate SortType HeaderClickedDelegate(Header header, SortType SortType);

    public class Header : Control
    {
        static Header()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Header), new FrameworkPropertyMetadata(typeof(Header)));
        }

        private HeaderClickedDelegate HeadClickDel;
        private string title;

        private SortType CurrentSortType = SortType.NotOrdered;

        public Header(string title, HeaderClickedDelegate HeadClickDel)
        {
            this.title = title;
            this.HeadClickDel = HeadClickDel;
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Header_PreviewMouseLeftButtonDown);
            this.GotMouseCapture += new MouseEventHandler(Header_GotMouseCapture);
            this.Loaded += new RoutedEventHandler(Header_Loaded);
        }

        private void Header_Loaded(object sender, RoutedEventArgs e)
        {
            Label labTitle = (Label)this.GetTemplateChild("Title");
            if (labTitle != null) labTitle.Content = title;
        }

        private void Header_GotMouseCapture(object sender, MouseEventArgs e)
        {
            Border HeaderBorder = (Border)this.GetTemplateChild("HeaderBorder");
            if (HeaderBorder != null) HeaderBorder.CaptureMouse();
        }

        private void Header_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentSortType = HeadClickDel(this, CurrentSortType);
            UpdateSortArrow();
        }

        internal void UpdateSortArrow()
        {
            Path Arrow = (Path)this.GetTemplateChild("Arrow");

            if (Arrow == null) return;

            string StyleName;

            switch (CurrentSortType)
            {
                case SortType.Ascending:
                    StyleName = "DGP_Head_UpArrowStyle";
                    break;
                case SortType.Descending:
                    StyleName = "DGP_Head_DownArrowStyle";
                    break;
                case SortType.NotOrdered:
                default:
                    StyleName = "NoArrowStyle";
                    break;
            }
            Arrow.Style = (Style)this.TryFindResource(StyleName);
        }

        public void ResetSortType()
        {
            if (CurrentSortType != SortType.NotOrdered)
            {
                CurrentSortType = SortType.NotOrdered;
                UpdateSortArrow();
            }
        }
    }
}