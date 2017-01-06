import serial
ser = serial.Serial('/dev/ttyUSB0', 9600, timeout = 0)

def get_value():
	val_to_send = ""
	val = ser.read()
	if val != "":
		if val.isspace():
			val_to_send = " "
		else:
			while val != '\r':
				val_to_send += val
				val = ser.read()
		return val_to_send
	else:
		return ""
