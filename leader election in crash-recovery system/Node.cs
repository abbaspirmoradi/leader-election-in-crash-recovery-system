using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace leader_election_in_crash_recovery_system
{
    internal class Node
    {
        public Node()
        {
            timeoutq = new int[1000];
            Recovered = new int[1000];
        }

        public int[] timeoutq;
        private int intc;

        private string nodename;

        public string NodeName
        {
            get { return nodename; }
            set { nodename = value; }
        }

        private string ntype;

        public string NodeType
        {
            get { return ntype; }
            set { ntype = value; }
        }

        private int time;

        public int timer
        {
            get { return time; }
            set { time = value; }
        }

        public int INCARNATION
        {
            get { return intc; }
            set { intc = value; }
        }

        private string lder;

        public string LEADER
        {
            get { return lder; }
            set { lder = value; }
        }

        private int[] recover;

        public int[] Recovered
        {
            get { return recover; }
            set { recover = value; }
        }
    }
}