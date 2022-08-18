# Netstat_Realtime

Polls TCP connections and listeners on an interval and reports new,removed,and changed observations.
'Events' are logged to console, if set to true, and a unique log file.
 
Usage: ```Netstat_Realtime.exe <poll_interval_milliseconds> <log_to_console (true/false)```
- Example 1 - Poll 10 times per second with console logging:
	```Netstat_Realtime.exe 100 true```
- Example 2 - Poll every 5 seconds without console logging:
	```Netstat_Realtime.exe 5000 false```

Example Output:
```
-----------------------------------------------------------
Logging to file: .\netstat_realtime_2022-08-18T22-48-25.csv
Press any key to exit
-----------------------------------------------------------

2022-08-18T22:48:26: NewSocket - [Local: 192.168.1.20:50104] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:26: NewSocket - [Local: 192.168.1.20:50105] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:26: NewSocket - [Local: 192.168.1.20:50106] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:26: NewSocket - [Local: 192.168.1.20:50109] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:26: NewSocket - [Local: 192.168.1.20:50112] [Remote: obfuscated_ip:443] [Old State: ] [New State: SynSent]
2022-08-18T22:48:27: NewSocket - [Local: 192.168.1.20:50113] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:27: DisconnectedSocket - [Local: 192.168.1.20:50104] [Remote: obfuscated_ip:443] [Old State: Established] [New State: ]
2022-08-18T22:48:27: DisconnectedSocket - [Local: 192.168.1.20:50105] [Remote: obfuscated_ip:443] [Old State: Established] [New State: ]
2022-08-18T22:48:27: DisconnectedSocket - [Local: 192.168.1.20:50106] [Remote: obfuscated_ip:443] [Old State: Established] [New State: ]
2022-08-18T22:48:27: DisconnectedSocket - [Local: 192.168.1.20:50109] [Remote: obfuscated_ip:443] [Old State: Established] [New State: ]
2022-08-18T22:48:27: DisconnectedSocket - [Local: 192.168.1.20:50112] [Remote: obfuscated_ip:443] [Old State: SynSent] [New State: ]
2022-08-18T22:48:28: NewSocket - [Local: 192.168.1.20:50115] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:31: NewSocket - [Local: 192.168.1.20:50117] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
2022-08-18T22:48:31: NewSocket - [Local: 192.168.1.20:50118] [Remote: obfuscated_ip:443] [Old State: ] [New State: Established]
```