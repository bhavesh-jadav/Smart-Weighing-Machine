import Modules.loadcell as loadcell
from Modules import lcd_init as lcd
import RPi.GPIO as GPIO
import datetime
import urllib2

loadcell.init()


try:
	while 1:
		
		date_and_time = str(datetime.date.today().strftime("%-d-%-m-%y ") + datetime.datetime.now().time().strftime("%-H:%-M"))
		lcd.draw.text(date_and_time,127-len(date_and_time)*4,0,font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/Modules/pyLCD/fonts/3x5.fnt")
		lcd.display.commit()
		
		'''
		lcd.display.clear()
		weight = str(int(loadcell.weight_in_gram(5)))
		lcd.draw.text(weight, 0, 0, size = 22, font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/fonts/digital-7.ttf")
		lcd.display.commit()
		'''
except KeyboardInterrupt:
	GPIO.cleanup()

