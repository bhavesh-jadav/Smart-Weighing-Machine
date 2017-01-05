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
import Modules.keypad as keypad
from Modules.global_variables import global_variables

known_weight_in_gram = 0.0
known_weight_diff = 0.0
base_value = 0.0

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
		
def weight_in_gram(sample = 1):
	weight = abs(((base_value - read_average_value(sample)) / known_weight_diff) * known_weight_in_gram)
	return weight

def weight_in_kg(sample = 1):
	return weight_in_gram(sample) / 1000.0
	
def init():
	
	global known_weight_in_gram
	global known_weight_diff
	global base_value
	
	with open('loadcell_data.json', 'r') as f:
		data = json.load(f)
		
	known_weight_in_gram = data['known_weight_in_gram']
	known_weight_diff = data['known_weight_diff']
	base_value = data['base_value']
	
def calibrate():
	key = ""
	global known_weight_diff, known_weight_in_gram, base_value
	
	known_weight_diff = ""
	known_weight_in_gram = ""
	base_value = ""
	
	try:
		#calculate base value
		lcd.display.clear()
		lcd.draw.text("REMOVE ANY OBJECT", 15, 23)
		lcd.draw.text("FROM WEIGHING MACHINE", 3, 32)
		lcd.display.commit()
		time.sleep(5)
		lcd.display.clear()
		lcd.draw.text("PLEASE WAIT...", 25, 25)
		lcd.display.commit()
		base_value = calculate_base_value()
		time.sleep(0.1)
		
		#accept known weight in grams
		lcd.display.clear()
		lcd.draw.text("ENTER KNOWN WEIGHT IN", 0, 0)
		lcd.draw.text("GRAMS", 0, 9)
		lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
		lcd.display.commit()
		while key != "right" or len(known_weight_in_gram) <= 0:
			key = keypad.get_value()
			if key.isdigit():
				known_weight_in_gram += key
			elif key == "left" and len(known_weight_in_gram) > 0:
				known_weight_in_gram = known_weight_in_gram[:-1]
			elif key == "number" or key == "small" or key == "caps":
				lcd.draw.clear(127 - (len(global_variables.textmode))*6, 56, 127, 64)
				global_variables.textmode = key
				lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
			if len(known_weight_in_gram) >= 0:
				lcd.draw.clear(0, 19, 127, 26)
				if len(known_weight_in_gram) != 0:
					lcd.draw.text(known_weight_in_gram, 0, 19)
				else:
					lcd.draw.text(" ", 0, 19)
			lcd.display.commit()
		known_weight_in_gram = float(known_weight_in_gram)
		time.sleep(0.1)

		#calculate diff between base load cell value and known weight load cell value
		lcd.display.clear()
		lcd.draw.text("PUT KNOWN WEIGHT ON", 0, 0)
		lcd.draw.text("WEIGHING MACHINE THEN", 0, 9)
		lcd.draw.text("PRESS 'OK' TO CONTINUE", 0, 18)
		lcd.display.commit()
		key = ""
		while key != "right":
			key = keypad.get_value()
		lcd.display.clear()
		lcd.draw.text("PLEASE WAIT...", 25, 25)
		lcd.display.commit()
		val = read_average_value(15)
		known_weight_diff = float(abs(base_value - val))
		
		#store calculated values in json file
		data = {'known_weight_in_gram':known_weight_in_gram, 'known_weight_diff':known_weight_diff, 'base_value':base_value}
		with open('loadcell_data.json', 'w') as f:
			json.dump(data,f)
			
		
		lcd.display.clear()
		lcd.draw.text("DONE CALIBRATING", 15, 25)
		lcd.display.commit()
		time.sleep(3)
		
	except:
		lcd.display.clear()
		lcd.draw.text("THERE IS A PROBLEM", 0, 0)
		lcd.draw.text("IN CALIBRATING", 0, 9)
		lcd.display.commit()
		time.sleep(5)
		
#def zero():
	
#def tare():
