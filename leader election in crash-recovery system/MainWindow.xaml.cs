using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace leader_election_in_crash_recovery_system
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string al;

        public string AlgorithmType
        {
            get { return al; }
            set { al = value; }
        }

        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void CreateNetwork_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumbersOfNode.Text))
            {
                txtNumbersOfNode.BorderBrush = Brushes.Red;
            }
            else
            {
                Network node = new Network(txtNumbersOfNode.Text, AlgorithmType);
                node.Show();
            }
        }

        private void nearcommunicationefficient_Checked(object sender, RoutedEventArgs e)
        {
            this.AlgorithmType = "Near Communication Efficient";
            txtNumbersOfNode.Focus();
            body.Visibility = Visibility.Visible;
        }

        private void communicationefficient_Checked(object sender, RoutedEventArgs e)
        {
            this.AlgorithmType = "Communication Efficient";
            body.Visibility = Visibility.Visible;
            txtNumbersOfNode.Focus();
        }
    }
}