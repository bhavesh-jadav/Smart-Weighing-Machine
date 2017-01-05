import serial
ser = serial.Serial('/dev/ttyUSB0', 9600)

def get_value():
	
	val = ser.readline()
	if val != "":
		if val.isspace():
			val = val.rstrip()
			val = " "
		else:
			val = val.rstrip()
		return val
	else:
		return ""
