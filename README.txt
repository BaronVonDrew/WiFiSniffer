WiFi Sniffer by Powerbolt
-------------------------

About: WiFi Sniffer is built using C# on the .NET Framework version 4.5.

Description:
	WiFi Sniffer does as it's name suggests, and sniffs for incoming/outgoing connections through the local environment.
	Active connections are then queried for information regarding the where information is being sent to. In addition to 
	querying for information about a specific active connection, it also captures a total bytes sent/received through the 
	collection of connections. Double clicking an entry will send a DNS request in an attempt to resolve the Hostname
	of the address specified.
	
Usage:
	Double clicking inside the Data Grid will prompt the program to send a DNS request to resolve the address to a hostname.
	
	In the menus, you can set the refresh rate to any preset amount of time. (1, 5, 15, and 30 seconds respectively.)
	Additionally, you can freeze the refresh timer to analyze the group of connections as they are in a certain point in time.
	
Updates:
	1/15/2013 - Added live Send/Receive graphs to give a visual representation of how much data is being transmitted.
	
	1/16/2013 - Added process information to the connection lists. Requires admin access.
	
