leader election in crash–recovery systems

This Project delineates the leader election problem specially  at synchronous distributed systems where processes can crash and recover. Specifically, it offers a leader election functionality, in the crash–recovery failure model.
In this implementation, the processes do not have access to any form of stable storage, in particular when a process crashes all its variables lose their values. Processes start their execution with no leader in order to avoid the disagreement among unsteady processes, which will agree on the leader with correct processes after receiving the first message from the leader.

Technologies: C#, VS 2013, WPF
