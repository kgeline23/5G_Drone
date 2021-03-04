import threading, select, time, tempfile, multiprocessing, struct, os, sys
import ps_drone


drone = ps_drone.Drone()                                        # Start using drone					
drone.startup()                                                 # Connects to drone and starts subprocesses
print("Connected!")

drone.reset()                                                   # Sets drone's status to good (LEDs turn green when red)
while (drone.getBattery()[0] == -1):   time.sleep(0.1)          # Wait until drone has done its reset
print ("Battery: "+str(drone.getBattery()[0])+"%  "+str(drone.getBattery()[1]))	# Gives a battery-status
if drone.getBattery()[1] == "empty":   sys.exit()               # Give it up if battery is empty

drone.useDemoMode(True)                                         # Just give me 15 basic dataset per second (is default anyway)
drone.getNDpackage(["demo","vision detect"])                    #Packets to decoded
#drone.getNDpackage(["demo")                                    #Packets to decoded
time.sleep(0.5)                                                 # Give it some time to awake fully after reset

CDC = drone.ConfigDataCount
drone.setConfigAllID()                                          #Go to multiconfiguration-mode
drone.sdVideo()                                                 #Choose lower resolution
drone.frontCam()                                                #Choose front view
#while CDC==drone.ConfigDataCount: time.sleep(0.001)            #Wait until it is done
#drone.startVideo()                                             #Start video-function
#drone.showVideo()                                              #Display the video


print("<space> to toggle front- and groundcamera, any other key to stop")
IMC = drone.VideoImageCount                                     #Number of encoded videoframes
stop = False
ground = False                                                  #To toggle front- and groundcamera
#while drone.VideoImageCount==IMC: time.sleep(0.01)             #Wait for next image
IMC = drone.VideoImageCount                                     #Number of encoded videoframes
key = drone.getKey()
if key==" ":        
    if ground:                                      ground = False
    else:                                           ground = True
    drone.groundVideo(ground)                                   #Toggle between front- and groundcamera.
elif key and key != " ": stop = True


print ("READY TO FLY")

