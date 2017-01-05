import Modules.loadcell as loadcell
from Modules import lcd_init as lcd
import RPi.GPIO as GPIO

loadcell.init()
try:
	while 1:
		lcd.display.clear()
		weight = str(int(loadcell.weight_in_gram(5)))
		lcd.draw.text(weight, 0, 0, size = 22, font="/home/pi/Desktop/Smart-Weighing-Machine/Python Code/fonts/digital-7.ttf")
		lcd.display.commit()
except KeyboardInterrupt:
	GPIO.cleanup()

