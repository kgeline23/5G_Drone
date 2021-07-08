import socket, struct, traceback, sys, subprocess
import threading, select, time, tempfile, multiprocessing, struct, os, sys
import ps_drone
import bmp_library as bmpsensor #starts the sensors
import ultra_library
import RPi.GPIO as GPIO
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

print("Open for connection")
s.bind(('0.0.0.0', 3003))
s.listen(1)

conn, addr = s.accept()
print("Connected!")
#proc = subprocess.Popen(["sh rtsp-stream.sh"])

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
            
    except Exception:
        print(traceback.format_exc())
    except KeyboardInterrupt:
        conn.close()
        print("Socket closed")
        #proc.terminate()

conn.close()
