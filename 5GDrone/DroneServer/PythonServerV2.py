import socket
import threading, select, socket, time, tempfile, multiprocessing, struct, os, sys
import ps_drone
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

s.bind(('10.57.55.125', 3003))
#s.bind(('192.168.1.5', 3003))
print("here!")

s.listen(1)
conn, addr = s.accept()
print("Connected!")

drone = ps_drone.Drone()                                 # Start using drone					
drone.startup()                                          # Connects to drone and starts subprocesses

drone.reset()                                            # Sets drone's status to good (LEDs turn green when red)
while (drone.getBattery()[0] == -1):   time.sleep(0.1)   # Wait until drone has done its reset
#print "Battery: "+str(drone.getBattery()[0])+"%  "+str(drone.getBattery()[1])	# Gives a battery-status
if drone.getBattery()[1] == "empty":   sys.exit()        # Give it up if battery is empty

drone.useDemoMode(True)                                  # Just give me 15 basic dataset per second (is default anyway)
drone.getNDpackage(["demo","vision detect"])             #Packets to decoded
#drone.getNDpackage(["demo")                             #Packets to decoded
time.sleep(0.5)                                          # Give it some time to awake fully after reset

CDC = drone.ConfigDataCount
drone.setConfigAllID()                                  #Go to multiconfiguration-mode
drone.sdVideo()                                         #Choose lower resolution
drone.frontCam()                                        #Choose front view
#while CDC==drone.ConfigDataCount: time.sleep(0.001)     #Wait until it is done
#drone.startVideo()                                      #Start video-function
#drone.showVideo()                                       #Display the video


print("<space> to toggle front- and groundcamera, any other key to stop")
IMC = drone.VideoImageCount                             #Number of encoded videoframes
stop = False
ground = False                                          #To toggle front- and groundcamera
#while drone.VideoImageCount==IMC: time.sleep(0.01)  #Wait for next image
IMC = drone.VideoImageCount                         #Number of encoded videoframes
key = drone.getKey()
if key==" ":        
    if ground:                                      ground = False
    else:                                           ground = True
    drone.groundVideo(ground)                       #Toggle between front- and groundcamera.
elif key and key != " ": stop = True

while 1:
    data = conn.recv(1024)
    if not data:
        break
    #conn.sendall(data)
    print(data)

    if data == "TAKEOFF":
        print("Take-off command")
        drone.takeoff()                                          # Fly, drone, fly !
        while drone.NavData["demo"][0][2]:     time.sleep(0.1)   # Wait until the drone is really flying (not in landed-mode anymore)

    elif data =="LAND":          drone.land()
    elif data =="FORWARDS":      drone.moveForward()
    elif data =="BACKWARDS":     drone.moveBackward()
    elif data =="MOVELEFT":      drone.moveLeft()
    elif data =="MOVERIGHT":     drone.moveRight()
    elif data =="TURNLEFT":      drone.turnLeft()
    elif data =="TURNRIGHT":     drone.turnRight()
    elif data =="UP":            drone.moveUp()
    elif data =="DOWN":          drone.moveDown()
    elif data =="HOVER":         drone.hover()
    elif data == "STOP":         drone.stop()



conn.close()