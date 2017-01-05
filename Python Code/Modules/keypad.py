import serial
ser = serial.Serial('/dev/ttyUSB0', 9600)

def get_value():
	return ser.readline()
