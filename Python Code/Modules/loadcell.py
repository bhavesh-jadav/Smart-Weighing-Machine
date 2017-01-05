'''
*
* Project: 	   	Farm Produce: Logging and Monitoring
* Author: 		Bhavesh Jadav
* Filename:    		loadcell.py
*
'''

from Modules import lcd_init as lcd
import RPi.GPIO as GPIO
import time
import math
import json

known_weight_in_gram = 100.0
known_weight_diff = 1000.0
base_value = 3500.0

#to use physical pin numbers on board
GPIO.setmode(GPIO.BOARD)

SCK = 11  #define pin for serial clock input of HX711
DOUT = 13  #define pin for serial data output of HX711

GPIO.setup(SCK, GPIO.OUT)	#set output for sck pin for sending clock pulses
GPIO.setup(DOUT, GPIO.IN)	#set input for dout pin to receive data

#define array of size 24 for storing 24 bit data sent by HX711
bits = [0 for i in range(24)]


'''
*
* Function Name: 	read_cell_value
* Input: 		none
* Output: 		returns the calculated decimal value of the load cell reading
* Logic:		1) take bit by bit reading form HX711 module by sending clock pulses through SCK pin and store them in bits array
*			2) convert this binary data into decimal and then return the decimal value
* Example Call:		read_cell_value()
*
'''
def read_cell_value() :
	"this function reads the data provided by loadcell and return the value in decimal form"
	val = 0	#variable to store the decimal value
	#loop 24 times to get 24 bits
	for i in range(24) :
		GPIO.output(SCK, GPIO.HIGH)  #set SCK high
		bits[i] = GPIO.input(DOUT)   #store bit in bits array
		GPIO.output(SCK, GPIO.LOW)   #set sck low

	#sending 25th pulse to set gain 128 and to complete the transfer
	GPIO.output(SCK, GPIO.HIGH)
	GPIO.output(SCK, GPIO.LOW)

	"""
	GPIO.output(SCK, GPIO.HIGH)
	GPIO.output(SCK, GPIO.LOW)
	GPIO.output(SCK, GPIO.HIGH)
	GPIO.output(SCK, GPIO.LOW)
	"""

	power = 23  #for conversion for binary to decimal
	for i in range(24) :	#print the bits and equivalent decimal value
		#print(bits[i], end = "")
		val += bits[i] * math.pow(2, power)  #binary to decimal conversion
		power -= 1
	return val	#return the decimal value

'''
*
* Function Name: 	base_value
* Input: 		none
* Output: 		returns moving average value of the readings according to the number of samples defined
* Logic: 		Use moving average method to with certain number of samples to caclulate the stable base value of load cell when no load is applied
* Example Call:		base_value()
*
'''
def calculate_base_value():
	"to calculate initial base value with moving average method"
	sample = 50	#define number of samples to take
	count = 0
	tval = 0	#variable to sotre the final moving average value

	#loop till certain amount of sample are used
	while sample :
		#if hx711 is ready to send the data
		if is_ready():
			val = read_cell_value()	#then read the value from the load cell
			count += 1.0	#increment the counter
			tval = ((count - 1) / count) * tval + (1 / count) * val #claculate moving average
			sample -= 1

	return tval	#return final value

'''
*
* Function Name: 	read_average_value
* Input: 		sample: no. of values to considered for taking average value
* Output: 		returns the calculated average value of the load cell reading
* Logic: 		Use normal average method to calculate average
* Example Call:		read_average_value(10)
*
'''
def read_average_value(sample ) :
	"caculate average of load cell values according to sample passed"
	sval = 0
	i = sample
	#loop through all the readings and add them
	while i :
			if is_ready():
				val = read_cell_value()
				sval += val
				i -= 1
	sval /= sample	#divide the addition by number of samples to get average
	return sval

def is_ready():
	if GPIO.input(DOUT) == 0:
		return True
	else:
		return False
		
def weight_in_gram():
	while is_ready() != True:
		continue
	weight = abs(((base_value - read_cell_value()) / known_weight_diff) * known_weight_in_gram)
	return weight

def weight_in_kg():
	return weight_in_gram() / 1000.0
	
def init():
	with open('loadcell_data.json', 'r') as f:
		data = json.load(f)
		
	known_weight_in_gram = data['known_weight_in_gram']
	known_weight_diff = data['known_weight_diff']
	base_value = data['base_value']
	
def calibrate():
	
	lcd.draw.text("Enter known weight value", 0, 0, size = 12)
	lcd.display.commit()
	
	'''
	data = {'known_weight_in_gram':known_weight_in_gram, 'known_weight_diff':known_weight_diff, 'base_value':base_value}
	with open('loadcell_data.json', 'w') as f:
		json.dump(data,f)
		'''
		
#def zero():
	
#def tare():
