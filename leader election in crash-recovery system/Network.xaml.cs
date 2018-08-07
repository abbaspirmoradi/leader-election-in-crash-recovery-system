using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace leader_election_in_crash_recovery_system
{
    /// <summary>
    /// Interaction logic for Network.xaml
    /// </summary>
    public partial class Network : Window
    {
        private int NumberOfNodes;
        private string AlgorithmType;
        private static int constant = 2;
        private static int expiration = 60;
        private int NetworkFinalRunningTime = 0;
        private int NetworkCurrentTime = 0;
        private int NetworkMessagesCount = 0;
        private static List<Node> _networksNodes = new List<Node>();
        Node Pnode = new Node();
        Node Leadernode = new Node();
        XmlDocument doc = new XmlDocument();
        AutoResetEvent ae = new AutoResetEvent(false);

        private enum NodeType
        {
            None,
            EventuallyUp = 1,
            EventuallyDown = 2,
            Unstable = 3
        };

        public Network()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public Network(string NumberOf, string AlgorithmType)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TXTname.Text = AlgorithmType;
            // TODO: Complete member initialization
            this.AlgorithmType = AlgorithmType;
            Random rnd = new Random();

            //initualizing Network
            TablesDataGrid.AddColumn("NodeState       ", ColumnType.ComboBox, 110.0);
            TablesDataGrid.AddColumn("NodeName       ", ColumnType.TextBlock, 130.0);

            this.NumberOfNodes = int.Parse(NumberOf);
            for (int i = 0; i < NumberOfNodes; i++)
            {
                ComboBox combo = new ComboBox();
                combo.Items.Add("EventuallyUp");
                combo.Items.Add("EventuallyDown");
                combo.Items.Add("Unstable");
                int t = rnd.Next(0, 4);
                switch (t.ToString())
                {
                    case "1":
                        combo.Text = NodeType.EventuallyUp.ToString();
                        break;
                    case "2":
                        combo.Text = NodeType.EventuallyDown.ToString();
                        break;
                    case "3":
                        combo.Text = NodeType.Unstable.ToString();
                        break;
                    default:
                        combo.Text = NodeType.EventuallyUp.ToString();
                        break;
                }
                if (i == 0)
                {
                    combo.Text = NodeType.EventuallyUp.ToString();
                    TablesDataGrid.AddRow(new object[] { combo, "P" });
                }
                else
                    TablesDataGrid.AddRow(new object[] { combo, "Node " + (i).ToString() });
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNetTime.Text))
            {
                txtNetTime.BorderBrush = Brushes.Red;
            }
            else
            {
                //Create network
                Confirm.IsEnabled = false;
                NetworkFinalRunningTime = int.Parse(txtNetTime.Text);
                Random rnd = new Random();
                int nuberOfRows = TablesDataGrid.GetRowCount(0);
                for (int i = 0; i < nuberOfRows; i++)
                {
                    Node node = new Node();
                    node.NodeType = ((ComboBox)TablesDataGrid.GetControl(i, 0)).Text;
                    node.NodeName = ((Label)TablesDataGrid.GetControl(i, 1)).Content.ToString();
                    for (int j = 0; j < NumberOfNodes && j != i; j++)
                    {
                        node.Recovered[i] = rnd.Next(0, 4);
                    }
                    _networksNodes.Add(node);
                }

                if (this.AlgorithmType == "Near Communication Efficient")
                {
                    NearCommunicationEfficient();
                }
                else if (this.AlgorithmType == "Communication Efficient")
                {
                    CommunicationEfficient();
                }
                string leaderName = null;
                if (Pnode.LEADER == "0")
                {
                    leaderName = "P";
                }
                else
                {
                    leaderName = "Node " + Pnode.LEADER;
                }
                MessageBox.Show("Leader Of Network : " + leaderName + "\n" + "Number Of Comunications Messages : " + NetworkMessagesCount, "Network Resualts",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                this.Confirm.IsEnabled = true;
                this.Close();
            }
        }

        private void CommunicationEfficient()
        {
            if (File.Exists("DB1.xml"))
            {
                File.Delete("DB1.xml");
            }
            if (!File.Exists("DB1.xml"))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create("DB1.xml", settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("Nodes");

                //write incarnation  AND leader into stable storage

                WriteNodeData(writer, "0", "0");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }

            doc.Load("DB1.xml");

            XmlNode nodeINCARNATION = doc.SelectSingleNode("//INCARNATIONP");
            nodeINCARNATION.InnerText = (int.Parse(nodeINCARNATION.InnerText) + 1).ToString();
            Pnode.INCARNATION = int.Parse(nodeINCARNATION.InnerText);
            doc.DocumentElement.AppendChild(nodeINCARNATION);
            XmlNode nodeLEADER = doc.SelectSingleNode("//LEADERP");
            Pnode.LEADER = nodeLEADER.InnerText;

            //initialized p to  constant value+ incarnationp
            //and a Recoveredp vector to count the number of times that
            //each process has recovered (initialized to 0 for every other process,
            //and to incarnationp for p itself).
            for (int i = 1; i < NumberOfNodes; i++)
            {
                Pnode.timeoutq[i] = constant + Pnode.INCARNATION;
                Pnode.Recovered[i] = 0;
                // NetworkMessagesCount++;
            }

            Pnode.Recovered[0] = Pnode.INCARNATION;
            //if process p does not trust itself, then it resets a timer with respect to leaderp
            if (Pnode.LEADER != "0")
            {
                Pnode.timer = Pnode.timeoutq[int.Parse(Pnode.LEADER)];
            }

            Thread t1 = new Thread(new ThreadStart(Task1));
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();
            //Thread t2 = new Thread(new ThreadStart(Task2));
            //t2.SetApartmentState(ApartmentState.STA);
            //t2.Start();
            Thread t3 = new Thread(new ThreadStart(Task3));
            t3.SetApartmentState(ApartmentState.STA);
            t3.Start();

            //refresh all processes
            while (NetworkCurrentTime <= NetworkFinalRunningTime)
            {
                ae.WaitOne((constant * 1000));
                foreach (Node node in _networksNodes)
                {
                    node.LEADER = Pnode.LEADER;
                    node.timer += constant;
                }
                Pnode.timer += constant;
                NetworkCurrentTime += constant;
            }
        }

        private void Task1()
        {
            //waits  incarnationp+constant time units
            ae.WaitOne((constant + Pnode.INCARNATION) * 1000);

            //Write LEADERP into stable storage
            doc.Load("DB1.xml");
            XmlNode nodeLEADER = doc.SelectSingleNode("//LEADERP");
            nodeLEADER.InnerText = Pnode.LEADER.ToString();
            doc.DocumentElement.AppendChild(nodeLEADER);

            while (NetworkCurrentTime <= NetworkFinalRunningTime)
            {
                ae.WaitOne((constant) * 1000);
                if (Pnode.LEADER == "0")
                {
                    // p sends a LEADER message containing Recoveredp to the rest of processes
                    for (int i = 1; i < NumberOfNodes; i++)
                    {
                        Task2(i, _networksNodes[i]);
                        NetworkMessagesCount++;
                    }
                }
            }
        }

        private void Task2(int q, Node Qnode)
        {
            //p receives a LEADER message from another process q
            for (int i = 0; i < NumberOfNodes; i++)
            {
                //p updates Recoveredp with Recoveredq, taking the highest value for each component of the vector
                Pnode.Recovered[i] = Math.Max(Pnode.Recovered[i], Qnode.Recovered[i]);
            }

            //p checks if q is a better candidate than leaderp to become p’s leader,p sets q as its leader and resets timerp
            //to Timeoutp[q] in order to monitor q (i.e., leaderp) again.
            if (Pnode.Recovered[q] <= Pnode.Recovered[int.Parse(Pnode.LEADER)] ||
                (Pnode.Recovered[q] == Pnode.Recovered[int.Parse(Pnode.LEADER)] && q <= int.Parse(Pnode.LEADER)))
            {
                Pnode.LEADER = q.ToString();
                Pnode.timer = Pnode.timeoutq[q];
            }

            //p checks if it deserves to be leader comparing Recoveredp[p] with
            //Recoveredp[leaderp]. If it is the case leaderp is set to p and timerp is
            //stopped.
            if ((Pnode.Recovered[0] < Pnode.Recovered[int.Parse(Pnode.LEADER)]) ||
                (Pnode.Recovered[0] == Pnode.Recovered[int.Parse(Pnode.LEADER)] && 0 < int.Parse(Pnode.LEADER)))
            {
                Pnode.LEADER = "0";
                //stop timer
            }
        }

        private void Task3()
        {
            // this is activated whenever timerp expires
            if (Pnode.timer > expiration)
            {
                Pnode.timeoutq[int.Parse(Pnode.LEADER)]++;
                Pnode.LEADER = "0";
            }
        }

        private void WriteNodeData(XmlWriter writer, string leader, string incr)
        {
            writer.WriteStartElement("Node");
            writer.WriteElementString("LEADERP", leader);
            writer.WriteElementString("INCARNATIONP", incr);
            writer.WriteEndElement();
        }

        private void NearCommunicationEfficient()
        {
            //leaderp is initialized to the “no-leader”
            Pnode.LEADER = "*";

            //Timeoutp[q] is initialized to  for every other process q.

            for (int i = 1; i < NumberOfNodes; i++)
            {
                Pnode.timeoutq[i] = constant;
                Pnode.Recovered[i] = 0;
                NetworkMessagesCount++;
            }
            // Recoveredp[p] is initialized to 1.
            Pnode.Recovered[0] = 1;

            //p sends a RECOVERED message to the rest of processes
            for (int i = 1; i < NumberOfNodes; i++)
            {
                Task2forNearCommunicationEfficient("Recoverd", i, _networksNodes[i]);
                NetworkMessagesCount++;
            }

            Thread t1 = new Thread(new ThreadStart(Task1forNearCommunicationEfficient));
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            Thread t3 = new Thread(new ThreadStart(Task3forNearCommunicationEfficient));
            t3.SetApartmentState(ApartmentState.STA);
            t3.Start();

            while (NetworkCurrentTime <= NetworkFinalRunningTime)
            {
                ae.WaitOne((constant * 1000));
                foreach (Node node in _networksNodes)
                {
                    node.LEADER = Pnode.LEADER;
                    node.timer += constant;
                }
                Pnode.timer += constant;
                NetworkCurrentTime += constant;
            }
        }

        private void Task1forNearCommunicationEfficient()
        {
            while (NetworkCurrentTime <= NetworkFinalRunningTime)
            {
                AutoResetEvent ae = new AutoResetEvent(false);
                ae.WaitOne((constant) * 1000);
                //p sends a LEADER to the rest of processes.
                if (Pnode.LEADER == "0")
                {
                    for (int i = 1; i < NumberOfNodes; i++)
                    {
                        Task2forNearCommunicationEfficient("Leader", i, _networksNodes[i]);
                        NetworkMessagesCount++;
                    }
                }
                //p sends an ALIVE message to the rest of processes in order to help choosing an initial leader.
                else if (Pnode.LEADER == "*")
                {
                    for (int i = 1; i < NumberOfNodes; i++)
                    {
                        Task2forNearCommunicationEfficient("Alive", i, _networksNodes[i]);
                        NetworkMessagesCount++;
                    }
                }
            }
        }

        private void Task2forNearCommunicationEfficient(string MessageType, int q, Node Qnode)
        {
            switch (MessageType)
            {
                case "Recoverd":
                    Pnode.Recovered[q]++;
                    break;

                case "Alive":

                    //if leaderp =* p has received so far ALIVE from n/2 different processes, p considers itself the leader, setting leaderp to p.
                    if ((Pnode.LEADER == "*") && (q > NumberOfNodes / 2))
                    {
                        Pnode.LEADER = "0";
                    }
                    break;

                case "Leader":
                    //taking the highest value for each component of the vector
                    for (int i = 0; i < NumberOfNodes; i++)
                    {
                        Pnode.Recovered[i] = Math.Max(Pnode.Recovered[i], Qnode.Recovered[i]);
                    }
                    Pnode.timeoutq[0] = Math.Max(Pnode.timeoutq[q], Pnode.Recovered[0]);

                    //p checks if q deserves to become p’s leader p sets q as its leader and resets timerp to Timeoutp[q] in order to monitor q
                    if ((Pnode.LEADER == "*" && (Pnode.Recovered[q] < Pnode.Recovered[0] || (Pnode.Recovered[q] == Pnode.Recovered[0] && q < 0)))
                        || (Pnode.LEADER != "*" && (Pnode.Recovered[q] < Pnode.Recovered[int.Parse(Pnode.LEADER)] ||
                        (Pnode.Recovered[q] == Pnode.Recovered[int.Parse(Pnode.LEADER)] && q < int.Parse(Pnode.LEADER)))))
                    {
                        Pnode.LEADER = q.ToString();
                        Pnode.timer = Pnode.timeoutq[q];
                    }

                    //p also checks if it deserves to become the leader then p sets leaderp to p and stops timerp.
                    if ((Pnode.LEADER == "*" && (Pnode.Recovered[0] < Pnode.Recovered[int.Parse(Pnode.LEADER)] ||
                        (Pnode.Recovered[q] == Pnode.Recovered[int.Parse(Pnode.LEADER)] && q < int.Parse(Pnode.LEADER)))))
                    {
                        Pnode.LEADER = "0";
                        //stop timer
                    }
                    break;
            }
        }

        private void Task3forNearCommunicationEfficient()
        {
            //whenever timerp expires
            if (Pnode.timer > expiration)
            {
                Pnode.timeoutq[int.Parse(Pnode.LEADER)]++;
                Pnode.LEADER = "0";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}