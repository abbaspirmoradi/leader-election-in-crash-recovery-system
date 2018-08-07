leader election in crash–recovery systems

This Project addresses the leader election problem in partially synchronous distributed systems where processes can crash and recover. More precisely, it provides a leader election functionality, in the crash–recovery failure model.
In this implementation, which is near communication- efficient and the processes do not have access to any form of stable storage, in particular when a process crashes all its variables lose their values. Processes start their execution with no leader in order to avoid the disagreement among unstable processes, which will agree on the leader with correct processes after receiving the first message from the leader.

Technologies: C#, WPF
