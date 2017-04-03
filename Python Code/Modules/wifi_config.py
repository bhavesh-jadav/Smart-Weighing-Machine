from wifi import Cell, Scheme
from wifi.exceptions import ConnectionError
import lcd_init as lcd
import os
import time
import keypad
import sys
import RPi.GPIO as GPIO
from global_variables import global_variables
millis = lambda: int(round(time.time() * 1000))

networks = []
network_names = []


#displays single menu page used by show_menu()
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

#displays menu and return selected funtion as a string. else returns command
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
			return "__cancel__"	
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


def init():
	
	password = ""
	display_password = ""
	key = ""
	line = 1
	time_millis = millis()
	
	os.system("sudo ifconfig wlan0 up")
	lcd.display.clear()
	lcd.draw.text("PLEASE WAIT...", 25, 25)
	lcd.display.commit()
	
	networks = Cell.all('wlan0')
	if len(networks) <= 7:
		network_names.append([])
		count = 0
		for network in networks:
			data = str(count+1) + " " + network.ssid + " "
			if network.encrypted :
				data += 'PROTECTED'
			else:
				data += 'UNPROTECTED'
			network_names[0].append(data)
			count += 1
	else:
		count = (len(networks) / 7) + 1
		j = 0
		for i in range(count):
			if(j >= len(networks)):
				break
			location_names.append([])
			for k in range(7):
				if(j >= len(networks)):
					break
				data = str(j+1) + " " + network.ssid + " "
				if network.encrypted :
					data += 'PROTECTED'
				else:
					data += 'UNPROTECTED'
				network_names[0].append(data)
				j += 1
				
	data = show_menu(network_names)
	if data == "__cancel__":
		sys.stdout.flush()
		GPIO.cleanup()
		os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')

	ssid = data.split(' ')[1]
	
	selected_network = None
	password = ""
	
	for network in networks:
		if network.ssid == ssid:
			selected_network = network
			break
	
	if selected_network.encrypted:
		if selected_network.encrypted:
			#accept password
			lcd.display.clear()
			lcd.draw.text("PASSWORD FOR " + str(selected_network.ssid), 0, 0)
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
					os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')
				
	try:
		lcd.display.clear()
		lcd.draw.text("CONNECTING TO", 0, 0)
		lcd.draw.text(selected_network.ssid, 0, 9)
		lcd.draw.text("PLEASE WAIT...", 0, 18)
		lcd.display.commit()
		scheme = Scheme.for_cell('wlan0', selected_network.ssid, selected_network, password)
		try:
			scheme.save()
			scheme.activate()
		except AssertionError:
			scheme.activate()
			
		lcd.display.clear()
		lcd.draw.text("WIFI CONNECTION TO", 0, 0)
		lcd.draw.text(selected_network.ssid, 0, 9)
		lcd.draw.text("SUCCESSFULL", 0, 18)
		lcd.display.commit()
		time.sleep(4)
	except ConnectionError:
		lcd.display.clear()
		lcd.draw.text("WIFI CONNECTION TO", 0, 0)
		lcd.draw.text(selected_network.ssid, 0, 9)
		lcd.draw.text("UNSUCCESSFULL", 0, 18)
		lcd.draw.text("MAY BE WRONG PASSWORD", 0, 27)
		lcd.display.commit()
		time.sleep(4)
	except:
                lcd.display.clear()
		lcd.draw.text("WIFI CONNECTION TO", 0, 0)
		lcd.draw.text(selected_network.ssid, 0, 9)
		lcd.draw.text("UNSUCCESSFULL", 0, 18)
		lcd.draw.text("MAY BE WRONG PASSWORD", 0, 27)
		lcd.display.commit()
		time.sleep(4)
	
	sys.stdout.flush()
	GPIO.cleanup()
	os.execl('/home/pi/Desktop/Smart-Weighing-Machine/Bash Scripts/main.sh', '')

