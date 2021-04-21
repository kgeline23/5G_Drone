import socket
import threading, select, socket, time, tempfile, multiprocessing, struct, os, sys
import struct
import bmp_library as bmpsensor #starts the sensors
import RPi.GPIO as GPIO

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

print("Open for connection")
#s.bind(('10.57.55.46', 3003))
s.bind(('192.168.0.16', 3003))
#s.bind(('192.168.178.109', 3003))
s.listen(1)


conn, addr = s.accept()
print("Connected!")
 
#GPIO Mode (BOARD / BCM)
GPIO.setmode(GPIO.BCM)
 
#set GPIO Pins
GPIO_TRIGGER = 18
GPIO_ECHO = 24
 
#set GPIO direction (IN / OUT)
GPIO.setup(GPIO_TRIGGER, GPIO.OUT)
GPIO.setup(GPIO_ECHO, GPIO.IN)

#this method is used to obtain the barometer(Pressure sensor) readings
def getBMP():
    #while 1: #loop "forever"
    altitude = bmpsensor.getAverage()
    intAlt = bmpsensor.getInitialAlt() #the base height used to messarue the drones height
    currentHeight = altitude - intAlt
    print("Height: ", currentHeight )
    height =  str(round(currentHeight, 2))
    print("\n")
    ba = bytes(height.encode("utf-8"))
    conn.send(ba)
    #time.sleep(0.5)
    
def getUltra():
    # set Trigger to HIGH
    GPIO.output(GPIO_TRIGGER, True)
    # set Trigger after 0.01ms to LOW
    time.sleep(0.00001)
    GPIO.output(GPIO_TRIGGER, False)

    StartTime = time.time()
    StopTime = time.time()

    # save StartTime
    while GPIO.input(GPIO_ECHO) == 0:
        StartTime = time.time()

    # save time of arrival
    while GPIO.input(GPIO_ECHO) == 1:
        StopTime = time.time()

    # time difference between start and arrival
    TimeElapsed = StopTime - StartTime
    # multiply with the sonic speed (34300 cm/s)
    # and divide by 2, because there and back
    temp = (TimeElapsed * 34300) / 2
    print ("Distance = %.1f cm" % temp)
    distance = str(round(temp, 2))
    ba = bytes(distance.encode("utf-8"))
    conn.send(ba)
    #time.sleep(0.5)


while 1: #loop "forever"
    data = conn.recv(1024).decode("utf-8")
    print("\n")    
    if not data:
        break
    print("Cmd: " + data)
    if data == "BMP":
        getBMP()
    if data == "ULT":
        getUltra()
        #bmpThread = threading.Thread(target= getBMP)
        #bmpThread.start()
        #ultraThread = threading.Thread(target= getUltra)
        #ultraThread.start()
        
            
#    time.sleep(1)


conn.close()
