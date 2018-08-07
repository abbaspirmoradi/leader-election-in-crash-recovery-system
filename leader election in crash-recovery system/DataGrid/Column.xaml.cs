using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace leader_election_in_crash_recovery_system
{
    public enum ColumnType : byte
    {
        TextBox,
        CheckBox,
        ComboBox,
        DatePicker,
        TextBlock
    }

    public delegate bool KeyDownDelegate(Key key, int Row, Column Col, bool ShiftPressed);

    /// <summary>
    /// Interaction logic for Column.xaml
    /// </summary>
    ///
    partial class Column : UserControl
    {
        private ColumnType ColType;
        private KeyDownDelegate keydowndel = null;
        private DataGridPCtrls dataGridPCtrls;

        public double GetRowHeight()
        {
            object Obj = TryFindResource("DGP_Row_Height");
            return (Obj is GridLength) ? ((GridLength)Obj).Value : 25.0;
        }

        internal Brush GridLinesColor
        {
            set
            {
                for (int Idx = 0; Idx < CtrlStack.Children.Count; Idx++)
                    ((Border)CtrlStack.Children[Idx]).BorderBrush = value;
            }
        }

        internal Brush GridBackColor
        {
            set
            {
                for (int Idx = 0; Idx < CtrlStack.Children.Count; Idx++)
                    ((Border)CtrlStack.Children[Idx]).Background = value;
            }
        }

        public Column(DataGridPCtrls dataGridPCtrls, KeyDownDelegate keydowndel, ColumnType ct = ColumnType.TextBox, double width = 145.0)
        {
            InitializeComponent();
            this.keydowndel = keydowndel;
            this.dataGridPCtrls = dataGridPCtrls;
            ColType = ct;
            Width = width;
        }

        public void AddRow(object value = null)
        {
            Control ctrl = null;

            switch (ColType)
            {
                case ColumnType.TextBlock:
                    ctrl = new Label();
                    ctrl.FlowDirection = FlowDirection.LeftToRight;
                    ctrl.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
                    ctrl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    ctrl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    ((Label)ctrl).Content = (string)value;
                    break;
                case ColumnType.TextBox:
                    ctrl = new TextBox();
                    ctrl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    ctrl.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
                    ctrl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    ((TextBox)ctrl).Text = (value as TextBox).Text;
                    ((TextBox)ctrl).IsEnabled = (value as TextBox).IsEnabled;

                    break;

                case ColumnType.CheckBox:
                    ctrl = new CheckBox();
                    ctrl.IsEnabled = true;
                    ctrl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    ctrl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    if (value != null)
                    {
                        ((CheckBox)ctrl).IsChecked = (value as CheckBox).IsChecked;
                        ((CheckBox)ctrl).IsEnabled = (value as CheckBox).IsEnabled;
                    }

                    break;

                case ColumnType.ComboBox:
                    ctrl = new ComboBox();
                    ctrl = value as ComboBox;
                    ctrl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    ctrl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    ((ComboBox)ctrl).Text = (string)(value as ComboBox).Text;
                    ctrl.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
                    break;

                case ColumnType.DatePicker:
                    ctrl = new DatePicker();

                    if (value is DateTime)
                        ((DatePicker)ctrl).SelectedDate = (DateTime)value;

                    ctrl.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
                    break;
            }
            if (ctrl != null)
            {
                ctrl.Background = Brushes.Transparent;
                ctrl.PreviewKeyDown += new KeyEventHandler(ctrl_PreviewKeyDown);
            }

            Border border = new Border();
            border.BorderBrush = dataGridPCtrls.GridLinesColor;
            border.Background = dataGridPCtrls.GridBackColor;
            border.BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0);
            border.Margin = new Thickness(0.0);
            border.Child = ctrl;
            border.Height = GetRowHeight();

            CtrlStack.Children.Add(border);
        }

        public void ResetRowHeight()
        {
            foreach (UIElement uie in CtrlStack.Children)
                ((Border)uie).Height = GetRowHeight();
        }

        public Control GetRowControl(int Row)
        {
            return (Control)((Border)CtrlStack.Children[Row]).Child;
        }

        public void SetRowControl(int Row, Control ctrl)
        {
            // Prevent a control of the wrong type being set for the row
            if (ctrl != null)
            {
                switch (ColType)
                {
                    case ColumnType.TextBox:
                        if (!(ctrl is TextBox))
                            return;
                        break;
                    case ColumnType.TextBlock:
                        if (!(ctrl is Label))
                            return;
                        break;
                    case ColumnType.CheckBox:
                        if (!(ctrl is CheckBox))
                            return;
                        break;

                    case ColumnType.ComboBox:
                        if (!(ctrl is ComboBox))
                            return;
                        break;

                    case ColumnType.DatePicker:
                        if (!(ctrl is DatePicker))
                            return;
                        break;
                }
            }
            ((Border)CtrlStack.Children[Row]).Child = ctrl;
        }

        public object GetRowValue(int Row)
        {
            Control ctrl = (Control)((Border)CtrlStack.Children[Row]).Child;

            switch (ColType)
            {
                case ColumnType.TextBox:
                    return ((TextBox)ctrl).Text;
                case ColumnType.TextBlock:
                    return ((Label)ctrl).Content;
                case ColumnType.ComboBox:
                    return ((ComboBox)ctrl).Text;

                case ColumnType.CheckBox:
                    return ((CheckBox)ctrl).IsChecked;

                case ColumnType.DatePicker:
                    return ((DatePicker)ctrl).SelectedDate;
            }
            return null;
        }

        public void SetRowValue(int Row, object value)
        {
            Control ctrl = (Control)((Border)CtrlStack.Children[Row]).Child;

            switch (ColType)
            {
                case ColumnType.TextBox:
                    ((TextBox)ctrl).Text = (string)value;
                    break;

                case ColumnType.TextBlock:
                    ((Label)ctrl).Content = (string)value;
                    break;

                case ColumnType.ComboBox:
                    ((ComboBox)ctrl).Text = (string)value;
                    break;

                case ColumnType.CheckBox:
                    ((CheckBox)ctrl).IsChecked = (bool)value;
                    break;

                case ColumnType.DatePicker:
                    ((DatePicker)ctrl).SelectedDate = (DateTime)value;
                    break;
            }
        }

        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right
                || e.Key == Key.PageDown || e.Key == Key.PageUp || e.Key == Key.Home || e.Key == Key.End)
            {
                int iRow = CtrlStack.Children.IndexOf((UIElement)((Control)sender).Parent);
                e.Handled = keydowndel(e.Key, iRow, this, (Keyboard.Modifiers == ModifierKeys.Shift));
            }
        }

        public void SetActive(int iRow)
        {
            Border bor = CtrlStack.Children[iRow] as Border;

            if (bor != null)
            {
                Control ctrl = bor.Child as Control;

                if (ctrl != null)
                {
                    ctrl.Focus();
                }
            }
        }

        public void RemoveRow()
        {
            if (CtrlStack.Children.Count > 0)
                CtrlStack.Children.RemoveAt(CtrlStack.Children.Count - 1);
        }

        public int RowCount()
        {
            return CtrlStack.Children.Count;
        }
    }
}