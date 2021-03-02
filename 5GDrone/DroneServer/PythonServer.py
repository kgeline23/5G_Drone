import socketserver
##### Suggested clean drone startup sequence #####
import time, sys
import ps_drone                                          # Import PS-Drone-API

class Handler_TCPServer(socketserver.BaseRequestHandler):
    """
    The TCP Server class for demonstration.

    Note: We need to implement the Handle method to exchange data
    with TCP client.

    """

    def handle(self):
        # self.request - TCP socket connected to the client
        self.data = self.request.recv(1024).strip()
        print("{} sent:".format(self.client_address[0]))
        print(self.data)
        # just send back ACK for data arrival confirmation
        #self.request.sendall("ACK from TCP Server".encode())

        if self.data == "b'TAKEOFF'":
            print("Take-off command")
            #drone.takeoff()                                          # Fly, drone, fly !
            #while drone.NavData["demo"][0][2]:     time.sleep(0.1)   # Wait until the drone is really flying (not in landed-mode anymore)

            #### Mainprogram begin
            #print "The Drone is flying now, land it with any key but <space>"

            #print("Drone is flying!")
            #print "Batterie: "+str(drone.getBattery()[0])+"%  "+str(drone.getBattery()[1])	# Gives a battery-status
        elif self.data =="b'LAND'":
            print("Landing command")
            drone.land()

if __name__ == "__main__":
    HOST, PORT = "192.168.178.192", 3003

    # Init the TCP server object, bind it to the localhost on 9999 port
    tcp_server = socketserver.TCPServer((HOST, PORT), Handler_TCPServer)

    # Activate the TCP server.
    # To abort the TCP server, press Ctrl-C.
    print("Server is on!")
    tcp_server.serve_forever()

    drone = ps_drone.Drone()                                 # Start using drone					
    drone.startup()                                          # Connects to drone and starts subprocesses

    drone.reset()                                            # Sets drone's status to good (LEDs turn green when red)
    while (drone.getBattery()[0] == -1):   time.sleep(0.1)   # Wait until drone has done its reset
    #print "Battery: "+str(drone.getBattery()[0])+"%  "+str(drone.getBattery()[1])	# Gives a battery-status
    if drone.getBattery()[1] == "empty":   sys.exit()        # Give it up if battery is empty

    drone.useDemoMode(True)                                  # Just give me 15 basic dataset per second (is default anyway)
    drone.getNDpackage(["demo"])                             # Packets, which shall be decoded
    time.sleep(0.5)                                          # Give it some time to awake fully after reset



