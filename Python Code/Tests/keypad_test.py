from Modules import keypad
from Modules import lcd_init as lcd

val = ""

try:
	lcd.draw.text("TYPE FROM KEYPAD", 0, 0)
	lcd.display.commit()
	while 1:
		key = keypad.get_value()
		if key != "" :
			val += key
			lcd.draw.rectangle(0,7,127,14,fill=True)
			lcd.draw.text(val, 0, 7, clear=True)
			lcd.display.commit()
except KeyboardInterrupt:
	lcd.display.clear()
	lcd.display.commit()
