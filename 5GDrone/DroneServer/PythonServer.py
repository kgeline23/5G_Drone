import socket, struct
import threading, select, time, tempfile, multiprocessing, struct, os, sys
import ps_drone
import bmp_library as bmpsensor #starts the sensors
import ultra_library.py
import RPi.GPIO as GPIO
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

print("Open for connection")
s.bind(('0.0.0.0', 3003))
s.listen(1)

conn, addr = s.accept()
print("Connected!")

drone = ps_drone.Drone()                                # Start using drone					
drone.startup()                                         # Connects to drone and starts subprocesses

drone.reset()                                           # Sets drone's status to good (LEDs turn green when red)
while (drone.getBattery()[0] == -1):   time.sleep(0.1)  # Wait until drone has done its reset
#print "Battery: "+str(drone.getBattery()[0])+"%  "+str(drone.getBattery()[1])	# Gives a battery-status
if drone.getBattery()[1] == "empty":   sys.exit()       # Give it up if battery is empty

drone.useDemoMode(True)                                 # Just give me 15 basic dataset per second (is default anyway)
drone.getNDpackage(["demo","vision detect"])            #Packets to decoded
#drone.getNDpackage(["demo")                            #Packets to decoded
time.sleep(0.5)                                         # Give it some time to awake fully after reset

CDC = drone.ConfigDataCount
drone.setConfigAllID()                                  #Go to multiconfiguration-mode
drone.sdVideo()                                         #Choose lower resolution
drone.frontCam()                                        #Choose front view
#while CDC==drone.ConfigDataCount: time.sleep(0.001)    #Wait until it is done
#drone.startVideo()                                     #Start video-function
#drone.showVideo()                                      #Display the video


print("<space> to toggle front- and groundcamera, any other key to stop")
IMC = drone.VideoImageCount                             #Number of encoded videoframes
stop = False
ground = False                                          #To toggle front- and groundcamera
#while drone.VideoImageCount==IMC: time.sleep(0.01)     #Wait for next image
IMC = drone.VideoImageCount                             #Number of encoded videoframes
key = drone.getKey()
if key==" ":        
    if ground:
        ground = False
    else:                                           
        ground = True
    drone.groundVideo(ground)                           #Toggle between front- and groundcamera.
elif key and key != " ": stop = True


#this method is used to obtain the barometer(Pressure sensor) readings
def getBMP():
    #while 1: #loop "forever"
    altitude = bmpsensor.readBmp180()
    intAlt = bmpsensor.getInitialAlt()                  #the base height used to messarue the drones height (m)
    currentHeight = altitude - intAlt
    print("Height: ", currentHeight )
    height =  str(round(currentHeight, 2))
    print("\n")
    ba = bytes(height.encode("utf-8"))
    conn.send(ba)
    #time.sleep(0.5)
    
def getUltra():
    uDis = ultra_library.getUltra()                     #Gets the distance to an object in cm
    distance = str(round(uDis, 2))                      #rounds the number to 2 decimal places
    ba = bytes(distance.encode("utf-8"))
    conn.send(ba)

while 1:
    try:
        #####
        data = conn.recv(1024).decode("utf-8")          #receive message from Controller
          print("\n")
        if not data:
            break
        #conn.sendall(data)
        print("Cmd: " + data)                           
        if data == "BMP":                                  #if the controller wants the barometer value
            getBMP()
        if data == "ULT":                                  #if the controller wants the ultrasonic sensor value
            getUltra()
            
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
        elif cmd == "HEIGHT":        getHeight()
    except Exception:
        print(traceback.format_exc())
    except KeyboardInterrupt:
        conn.close()
        print("Socket closed")

conn.close()
