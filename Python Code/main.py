import Modules.loadcell as loadcell
from Modules import lcd_init as lcd
import RPi.GPIO as GPIO
import datetime
import urllib2
from Modules import keypad
import json
from Modules.global_variables import global_variables
import os
import sys

date_and_time = ""
old_date_and_time = ""
menu = ["CHECK INTERNET", "RESTART SCRIPT", "SHUTDOWN MACHINE", "RESTART MACHINE", "CALIBRATE MACHINE", "SIGN IN", "SIGN OUT"]
lcd.display.clear()	
loadcell.init()

def display_date_and_time():
	global date_and_time, old_date_and_time
	old_date_and_time = date_and_time
	date_and_time = str(datetime.date.today().strftime("%-d-%-m-%y ") + datetime.datetime.now().time().strftime("%H:%M"))
	lcd.draw.clear(127-len(old_date_and_time)*4,0,127,4)
	lcd.draw.text(date_and_time,127-len(date_and_time)*4,0,font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/Modules/pyLCD/fonts/3x5.fnt")
	lcd.display.commit()
		
def display_weight():
	weight = int(loadcell.weight_in_gram(5))
	if weight <= 1000:
		weight = str(weight) + " g"
	else:
		weight = str(round(float(loadcell.weight_in_kg(5)), 3)) + " kg"
		
	lcd.draw.clear(0,20,127,40)	
	lcd.draw.text(weight, 63-len(weight)*5, 20, size = 32, font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/Modules/pyLCD/fonts/digital-7.ttf")
	lcd.display.commit()
	
def display_username():
	if global_variables.username != "":
		lcd.draw.text(global_variables.username,0,58,font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/Modules/pyLCD/fonts/3x5.fnt")
	
def check_internet():
	lcd.display.clear()
	lcd.draw.text("PLEASE WAIT...", 25, 25)
	lcd.display.commit()
	try:
		urllib2.urlopen("http://www.google.com", timeout = 3)
		lcd.display.clear()
		lcd.draw.text("CONNECTED TO THE INTERNET", 0, 0)
		lcd.draw.text("PRESS 'OK' TO CONTINUE", 0, 9)
		lcd.display.commit()
		key = keypad.get_value()
		while key != "right":
			key = keypad.get_value()
	except urllib2.URLError, e:
		lcd.display.clear()
		lcd.draw.text("NOT CONNECTED TO THE", 0, 0)
		lcd.draw.text("INTERNET", 0, 9)
		lcd.draw.text("PRESS 'OK' TO CONTINUE", 0, 18)
		lcd.display.commit()
		key = keypad.get_value()
		while key != "right":
			key = keypad.get_value()
			
def restart_script():
	sys.stdout.flush()
	GPIO.cleanup()
	os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')
	
def shutdown_machine():
	lcd.display.clear()
	lcd.display.commit()
	os.system("sudo poweroff")
	
def restart_machine():
	lcd.display.clear()
	lcd.display.commit()
	os.system("sudo reboot")
	
def calibrate_machine():
	loadcell.calibrate()
	
def show_menu():
	lcd.display.clear()
	counter = 1
	for i in range(len(menu)):
		if i == 0:
			lcd.draw.rectangle(0,0,127,8,fill=True)
			lcd.draw.text(menu[0],1,counter,clear=True)
		else:
			counter = counter + 9
			lcd.draw.text(menu[i], 1, counter)
	lcd.display.commit()
	counter = 0
	yaxis = 1
	while 1:
		key = keypad.get_value()
		if key == "left" or key == "cancel":
			lcd.display.clear()
			break
		elif key == "up":
			if counter > 0:
				lcd.draw.clear(0, counter*9, 127, counter*9+8)
				lcd.draw.text(menu[counter],1, yaxis)
				counter -= 1
				lcd.draw.clear(0, counter*9, 127, counter*9+8)
				lcd.draw.rectangle(0,counter*9,127,counter*9+8,fill=True)
				yaxis -= 9
				lcd.draw.text(menu[counter],1, yaxis, clear = True)
				lcd.display.commit()
		elif key == "down":
			if counter < len(menu) - 1:
				lcd.draw.clear(0, counter*9, 127, counter*9+8)
				lcd.draw.text(menu[counter],1, yaxis)
				counter += 1
				lcd.draw.clear(0, counter*9, 127, counter*9+8)
				lcd.draw.rectangle(0,counter*9,127,counter*9+8,fill=True)
				yaxis += 9
				lcd.draw.text(menu[counter],1, yaxis, clear = True)
				lcd.display.commit()
		elif key ==  "right":
			if counter == 0:
				check_internet()
				break
			elif counter == 1:
				restart_script()
			elif counter == 2:
				shutdown_machine()
			elif counter == 3:
				restart_machine()
			elif counter == 4:
				calibrate_machine()
			
	lcd.display.clear()	

try:
	while 1:
		
		display_date_and_time()
		display_weight()
		display_username()
		
		key = keypad.get_value()
		if key == "menu":
			show_menu()
		
except:
	GPIO.cleanup()


