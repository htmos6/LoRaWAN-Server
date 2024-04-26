using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoRaWANServer
{
    public class ApplicationServer
    {
        private TcpListener server;
        private TcpClient client;
        private NetworkStream stream;
        private IPAddress ip;


        /// <summary>
        /// Starts the server on the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address to listen on.</param>
        /// <param name="port">The port number to listen on.</param>
        /// <returns>True if the server started successfully, otherwise false.</returns>
        private bool Start(string ipAddress, int port)
        {
            try
            {
                // Parse IP address and initialize server
                ip = IPAddress.Parse(ipAddress);

                // Create a new TcpListener instance bound to the specified IP address and port
                server = new TcpListener(ip, port);

                // Start listening for incoming connections
                server.Start();

                // Output server's local endpoint
                Console.WriteLine("Server started on " + server.LocalEndpoint);

                // Return true indicating successful server start
                return true;
            }
            catch (Exception ex)
            {
                // Output error message if an exception occurs
                Console.WriteLine("Error starting server: " + ex.Message);

                // Return false indicating failure to start the server
                return false; 
            }
        }


        /// <summary>
        /// Listens for incoming client connections. If connection found, establish a connection.
        /// </summary>
        /// <returns>A TcpClient representing the accepted client connection, or null if an error occurs.</returns>
        private bool ListenThenConnectToClient()
        {
            try
            {
                // Output a message indicating waiting for a connection
                Console.WriteLine("Waiting for a connection...");

                // Accept a pending connection request
                client = server.AcceptTcpClient();

                // Return the TcpClient representing the connected client
                return true;
            }
            catch (Exception ex)
            {
                // Output an error message if an exception occurs during accepting the connection
                Console.WriteLine("Error accepting client connection: " + ex.Message);

                // Return null to indicate an error occurred while accepting the connection
                return false;
            }
        }


        /// <summary>
        /// Receives a message from the connected client.
        /// </summary>
        /// <returns>The received message as a string, or null if an error occurs.</returns>
        private string ReceiveMessage()
        {
            try
            {
                // Indicate a EndPoint of the connected client.
                Console.WriteLine("Connected to client " + ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

                // Get the network stream for reading message from the client
                stream = client.GetStream();

                // Create a byte array to store the received message
                byte[] buffer = new byte[1024];

                // Read message from the network stream into the buffer
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // Convert the received bytes into a string
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Output the received message
                Console.WriteLine("Received message: " + message);

                // Return the received message
                return message;
            }
            catch (Exception ex)
            {
                // Output an error message if an exception occurs during message receiving
                Console.WriteLine("Error receiving message: " + ex.Message);

                // Return null to indicate an error occurred while receiving the message
                return null;
            }
        }


        /// <summary>
        /// Sends a response message to the connected client.
        /// </summary>
        /// <param name="message">The message to send as a response.</param>
        /// <returns>True if the response was successfully sent, otherwise false.</returns>
        private bool SendResponse(string message)
        {
            try
            {
                // Create the response message to send to the client
                string response = "Server response: " + message;

                // Convert the response message to a byte array
                byte[] responseBuffer = Encoding.ASCII.GetBytes(response);

                // Write the response message to the network stream
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                // Output a message indicating that the response was sent
                Console.WriteLine("Response sent.");

                // Return true to indicate that the response was successfully sent
                return true;
            }
            catch (Exception ex)
            {
                // Output an error message if an exception occurs during response sending
                Console.WriteLine("Error sending response: " + ex.Message);

                // Return false to indicate that an error occurred while sending the response
                return false;
            }
        }


        /// <summary>
        /// Stops the server.
        /// </summary>
        private bool DisconnectFromClient()
        {
            try
            {
                string clientIpEndPoint = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();

                // Close the client connection after sending the response
                client.Close();

                // Close the network stream
                stream.Close();

                // Output a message indicating that the server has stopped
                Console.WriteLine("Disconnected from client" + clientIpEndPoint);

                // Return true indicating successful server stop
                return true;
            }
            catch (Exception ex)
            {
                // Output an error message if an exception occurs during server stop
                Console.WriteLine("Error stopping server: " + ex.Message);

                // Return false if an exception occurs during server stop
                return false;
            }

        }


        /// <summary>
        /// Starts the server and responds back to connected clients.
        /// </summary>
        /// <param name="ipAddress">The IP address to listen on.</param>
        /// <param name="port">The port number to listen on.</param>
        public void Run(string ipAddress, int port)
        {
            // Start the server
            if (this.Start(ipAddress, port))
            {
                // Continuously listen for and respond to client connections
                while (true)
                {
                    // Listen for a client connection and connect to it
                    if (this.ListenThenConnectToClient())
                    {
                        // Receive a message from the connected client
                        string message = this.ReceiveMessage();

                        // If a message is received successfully
                        if (message != null)
                        {
                            // Send a response back to the client
                            this.SendResponse(message);
                        }

                        // Disconnect from the client
                        this.DisconnectFromClient();
                    }
                }
            }
            else
            {
                // Stop the server if failed to start
                server.Stop();
            }
        }
    }
}
