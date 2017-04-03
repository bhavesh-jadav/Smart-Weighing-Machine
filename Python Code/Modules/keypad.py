import serial
from global_variables import global_variables
ser = serial.Serial('/dev/ttyUSB0', 9600, timeout = 0)

def flush_buffer():
	val = ser.read()
	if val != "":
		while ser.inWaiting() > 0:
			val = ser.read()

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
		
def send_value(value):
	if value == 'n':
		global_variables.textmode = "number"
	elif value == 's':
		global_variables.textmode = "small"
	elif value == 'c':
		global_variables.textmode = "caps"
	ser.write(value)
