import Modules.loadcell as loadcell
from Modules import lcd_init as lcd

loadcell.init()
while 1:
	lcd.display.clear()
	weight = str(int(loadcell.weight_in_gram()))
	lcd.draw.text(weight, 0, 0, size = 14, font="/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf")
	lcd.display.commit()
