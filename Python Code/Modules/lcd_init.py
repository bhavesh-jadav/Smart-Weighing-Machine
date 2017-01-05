from pyLCD import pylcd

#FOLLOWING ARE BCM PIN NUMBERS!
PINMAP = {
	'RS': 2,
	'E': 3,
	'D0': 22,
	'D1': 15,
	'D2': 10,
	'D3': 9,
	'D4': 11,
	'D5': 8,
	'D6': 7,
	'D7': 0,
	'CS1': 1,
	'CS2': 5,
	'RST': 6,
	'LED': 18
}

display = pylcd.ks0108.Display(backend = pylcd.GPIOBackend, pinmap = PINMAP, debug = False)
draw = pylcd.ks0108.DisplayDraw(display)
display.commit(full = True)
