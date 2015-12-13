Client / Server Messaging Application

-----Diagram of Client / Server Messaging program-----
       
              Async Communication                                                                           
                      |                                                                              
+-------+------+      v                                                                              
|Server Manager+<--------+                                                                          
+-------+------+         v                                                                          
        |             +--+---+                                                                      
        |             |Client|                                                                                                                                                 
        v             +--+---+                                                                                                                                                               
  +-----+-----+          ^                                                                       
  |Chat Server+<---------+                                                                          
  +-----------+       ^
                      |
             Async Communication


------Server-----

First the program will start up the Server Manager program. The server initializes by creating it's listener
and a call back is start to listen for when clients connect that will give them a list of chat servers to
connect. After that the server manager creates a list of chat channels for clients to also connect to. Each
chat server will be initialize it's self by creating a listener and call back for when clients send messages
and when messages are to be sent to the clients. After the chat channels are created we go into an infinite 
loop that broadcasts random messages to randomly to a chat server.

------Client-----
When the clients starts up it will first connect to the main server. The main server will then send the client
data on the list of channels a client can connect to. The client then goes into the an infinite loop where it 
will read a line in and either connect to a channel or send the message to all channels the client is connected.

-----List of client commands-----
/c * will try and connect to channel of id * if no channel exists nothing will happen
*    will send the message to every channel the client is connect to if it is not a null or empty string


1)This application to run multiple instances on a single computer
2)The ipAddress and ports are hard-coded as constants in Defines.cs and will probably have to be modified to run on each computer
