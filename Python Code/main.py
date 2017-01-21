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
from string import whitespace
import requests
import numpy
import time
millis = lambda: int(round(time.time() * 1000))

date_and_time = ""
old_date_and_time = ""
#One menu function should only take one line and there only will be 7 menu function per one menu page
menu_pages = [["1.CHECK INTERNET", "2.RESTART SCRIPT", "3.SHUTDOWN MACHINE", "4.RESTART MACHINE", "5.CALIBRATE MACHINE", "6.SIGN IN", "7.SIGN OUT"],
			  ["8.CONNECT TO WIFI", "9.ENTER IP ADDRESS"]]
menu_page_number = 0
menu_selected_function = 0
user_data = None
products = []
locations = []
location_names = []
product_names = []

lcd.display.clear()	
loadcell.init()

def init():
	global location_names, product_names, locations, products
	try:
		with open('user_data.json', 'r') as f:
			userContent = json.load(f)
		if userContent['fullName']:
			global_variables.username = userContent['fullName'].upper()
			
			count = 0
			while count < len(userContent['locationNames']):
				locations.append(userContent['locationNames'][count])
				count += 1
			count = 0
			while count < len(userContent['productNames']):
				products.append(userContent['productNames'][count])
				count += 1
			
			if len(userContent['locationNames']) <= 7:
				location_names.append([])
				count = 0
				while count < len(userContent['locationNames']):
					location_names[0].append(str(count+1) + "." + userContent['locationNames'][count]['value'].upper())
					count += 1
			else:
				count = (len(userContent['locationNames']) / 7) + 1
				j = 0
				for i in range(count):
					if(j >= len(userContent['locationNames'])):
						break
					location_names.append([])
					for k in range(7):
						if(j >= len(userContent['locationNames'])):
							break
						location_names[i].append(str(j+1) + "." + userContent['locationNames'][j]['value'].upper())
						j += 1
						
			if len(userContent['productNames']) <= 7:
				product_names.append([])
				count = 0
				while count < len(userContent['productNames']):
					product_names[0].append(str(count+1) + "." + userContent['productNames'][count]['value'].upper())
					count += 1
			else:
				count = (len(userContent['productNames']) / 7) + 1
				j = 0
				for i in range(count):
					if(j >= len(userContent['productNames'])):
						break
					product_names.append([])
					for k in range(7):
						if(j >= len(userContent['productNames'])):
							break
						product_names[i].append(str(j+1) + "." + userContent['productNames'][j]['value'].upper())
						j += 1
					
	except Exception as e:
		print str(e)
		

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
		lcd.draw.text("CONNECTED TO THE", 0, 0)
		lcd.draw.text("INTERNET", 0, 9)
		lcd.draw.text("PRESS 'OK' TO CONTINUE", 0, 18)
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
	
def add_data():
	if is_signed_in():
		val = show_menu(product_names)
		print val.split('.', 1)[1]
	else:
		lcd.display.clear()
		lcd.draw.text("NOT SIGNED IN", 0, 0)
		lcd.draw.text("SIGN IN TO CONTINUE", 0, 9)
		lcd.display.commit()
		time.sleep(4)
		lcd.display.clear()
		lcd.display.commit()
	
	lcd.display.clear()
	lcd.display.commit()
		
	
def is_signed_in():
	with open('user_data.json', 'r') as f:
		data = json.load(f)
	if data['userId']:
		return True
	else:
		return False

def signout():
	try:
		lcd.display.clear()
		lcd.draw.text("SIGNINIG OUT...", 0, 0)
		lcd.display.commit()
		with open('user_data.json', 'r') as f:
			data = json.load(f)
		if data['fullName']:
			data['fullName'] = ""
			data['userId'] = ""
			with open('user_data.json', 'w') as f:
				json.dump(data,f)
			global_variables.username = ""
			lcd.display.clear()
			lcd.draw.text("SIGN OUT SUCCESSFUL", 0, 0)
			lcd.display.commit()
			time.sleep(3)
			restart_script()
		else:
			lcd.display.clear()
			lcd.draw.text("NO USER SIGNED IN", 0, 0)
			lcd.display.commit()
			time.sleep(3)
	except Exception as e:
		print str(e)
		lcd.display.clear()
		lcd.draw.text("THERE IS A PROBLEM", 0, 0)
		lcd.draw.text("IN SIGNOUT PROCESS", 0, 9)
		lcd.display.commit()
		time.sleep(5)


def signin():
	global user_data, product_names, location_names
	try:
		userName = "kaushal"
		password = "kaushal123"
		display_password = ""
		key = ""
		userId = ""
		line = 1
		time_millis = millis()
		time.sleep(1)
		
		'''#accept user name
		lcd.display.clear()
		lcd.draw.text("ENTER USER NAME", 0, 0)
		keypad.send_value('s')
		lcd.draw.clear(127 - (len(global_variables.textmode))*6, 56, 127, 64)
		lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
		lcd.display.commit()
		while key != "right" or len(userName) <= 0:
			key = keypad.get_value()
			if millis() - time_millis < 500 and (key.isalpha() or key.isdigit()):
				userName = userName[:-1]
				time_millis = millis()
			if len(key) == 1 and (key.isalpha() or key.isdigit()):
				userName += key
				time_millis = millis()
			elif key == "left" and len(userName) > 0:
				userName = userName[:-1]
			elif key == "number" or key == "small" or key == "caps":
				lcd.draw.clear(127 - (len(global_variables.textmode))*6, 56, 127, 64)
				global_variables.textmode = key
				lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
			if len(userName) >= 0:
				lcd.draw.clear(0, 9, 127, 16)
				if len(userName) != 0:
					lcd.draw.text(userName, 0, 10)
				else:
					lcd.draw.text(" ", 0, 10)
			lcd.display.commit()
			if key == "cancel":
				sys.stdout.flush()
				GPIO.cleanup()
				os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')
		
		#accept password
		lcd.display.clear()
		lcd.draw.text("ENTER PASSWORD", 0, 0)
		keypad.send_value('s')
		lcd.draw.clear(127 - (len(global_variables.textmode))*6, 56, 127, 64)
		lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
		lcd.display.commit()
		while key != "right" or len(password) <= 0:
			key = keypad.get_value()
			if millis() - time_millis < 500 and (key.isalpha() or key.isdigit()):
				display_password = display_password[:-1]
				password = password[:-1]
				time_millis = millis()
			if len(key) == 1 and (key.isalpha() or key.isdigit()):
				password += key
				display_password += key
				time_millis = millis()
			elif key == "left" and len(password) > 0:
				display_password = display_password[:-1]
				password = password[:-1]
			elif key == "number" or key == "small" or key == "caps":
				lcd.draw.clear(127 - (len(global_variables.textmode))*6, 56, 127, 64)
				global_variables.textmode = key
				lcd.draw.text(global_variables.textmode, 127 - (len(global_variables.textmode))*6, 56)
			if len(password) >= 0:
				lcd.draw.clear(0, 9, 127, 16)
				if len(display_password) != 0:
					lcd.draw.text(display_password, 0, 10)
				else:
					lcd.draw.text(" ", 0, 10)
			if millis() - time_millis > 300 and len(display_password) > 0:
				display_password = display_password[:-1]
				display_password += "*"
			lcd.display.commit()
			if key == "cancel":
				sys.stdout.flush()
				GPIO.cleanup()
				os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')'''
				
		lcd.display.clear()
		lcd.draw.text("PLEASE WAIT...", 25, 25)
		lcd.display.commit()
		
		url = "https://swm2016.azurewebsites.net/api/login"
		payload = {'userName':userName, 'Password':password}
		headers = {'content-type': 'application/json'}
		response = requests.post(url, data=json.dumps(payload), headers=headers)
		if response.status_code == 200:
			userContent = json.loads(response.content)
			with open('user_data.json', 'w') as f:
				json.dump(userContent,f)
			restart_script()
			lcd.display.clear()
			lcd.draw.text("SIGN IN SUCCESSFUL", 0, 0)
			lcd.display.commit()
			time.sleep(3)
		else:
			lcd.display.clear()
			lcd.draw.text("SIGN IN FAILED", 0, 0)
			lcd.display.commit()
			time.sleep(3)
		
	except Exception as e:
		print str(e)
		lcd.display.clear()
		lcd.draw.text("THERE IS A PROBLEM", 0, 0)
		lcd.draw.text("IN SIGNIN PROCESS", 0, 9)
		lcd.display.commit()
		time.sleep(5)
		
		
def call_menu_functions(function):
	if function == menu_pages[0][0]:
		check_internet()
	elif function == menu_pages[0][1]:
		restart_script()
	elif function == menu_pages[0][2]:
		shutdown_machine()
	elif function == menu_pages[0][3]:
		restart_machine()
	elif function == menu_pages[0][4]:
		calibrate_machine()
	elif function == menu_pages[0][5]:
		signin()
	elif function == menu_pages[0][6]:
		signout()
	
def show_menu_page(menu):
	global menu_selected_function
	lcd.display.clear()
	counter = 1
	yaxis = (menu_selected_function*9) + 1
	for i in range(len(menu)):
		if i == menu_selected_function:
			lcd.draw.rectangle(0,menu_selected_function*9,127,(menu_selected_function*9+9)-1,fill=True)
			lcd.draw.text(menu[menu_selected_function],1,yaxis,clear=True)
			counter = counter + 9
			continue
		else:
			lcd.draw.text(menu[i], 1, counter)
			counter = counter + 9
			
	lcd.display.commit()
	counter = menu_selected_function
	while 1:
		key = keypad.get_value()
		if key == "left" or key == "cancel":
			return "__cancel__"
		elif key.isdigit():
			return key
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
			else:
				return "__prev__"
				
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
			else:
				return "__next__"		
		elif key ==  "right":
			return menu[counter]		
	lcd.display.clear()	

def show_menu(menu):
	global menu_page_number, menu_selected_function
	number = ""
	menu_fn = ""
	menu_page_number = 0
	menu_selected_function = 0
	found = 0
	while 1:
		keypad.send_value('n')
		val = show_menu_page(menu[menu_page_number])
		if val == "__cancel__":
			lcd.display.clear()
			break	
		elif val.isdigit():
			while 1:
				found = 0
				lcd.display.clear()
				if val.isdigit():
					number += val
				val = ""
				lcd.draw.text(number,1,54)
				for menu_page in menu:
					for menu_function in menu_page:
						if number in menu_function:
							found = 1
							menu_fn = menu_function
							break
					if found == 1:
						break
				if found == 0:
					lcd.draw.text("NOTHING FOUND",1,1)
				else:
					lcd.draw.text(menu_fn,1,1)
				lcd.display.commit()
				key = keypad.get_value()
				if key.isdigit():
					val = key
				elif key == "left":
					number = ""
					break
				elif key == "cancel":
					sys.stdout.flush()
					GPIO.cleanup()
					os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')
				elif key == "right":
					return menu_fn
		elif val == "__next__":
			if menu_page_number < len(menu)-1:
				menu_page_number += 1
				menu_selected_function = 0
			else:
				menu_page_number = 0
				menu_selected_function = 0
		elif val == "__prev__":
			if menu_page_number > 0:
				menu_page_number -= 1
				menu_selected_function = len(menu[menu_page_number])-1
			else:
				menu_page_number = len(menu) - 1
				menu_selected_function = len(menu[menu_page_number])-1
		else:
			return val
			
		number = ""
	
	
def show_main_menu():
	menu_fn = show_menu(menu_pages)
	call_menu_functions(menu_fn)
	lcd.display.clear()
	lcd.display.commit()
			
try:
	init()
	while 1:
		display_date_and_time()
		display_weight()
		display_username()
		
		key = keypad.get_value()
		if key == "menu":
			show_main_menu()
		elif key == "tare":
			loadcell.tare()
		elif key == "right":
			add_data()
		
except Exception as e:
	GPIO.cleanup()
	print str(e)


