#!/usr/bin/env pythonpix
# -*- coding: utf-8 -*-
# Copyright (C) 2013 Julian Metzler
# See the LICENSE file for the full license.

"""
Script to display a countdown on a GLCD
"""

import datetime
from pyLCD import pylcd
import sys
import time

#FOLLOWING ARE BCM PIN NUMBERS!
PINMAP = {
	'RS': 2,
	'E': 3,
	'D0': 14,
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

#draw.fill_screen(draw.PATTERN_HORIZONTAL_STRIPES)
#draw.line(10,10,60,60)

for i in range(100):
    display.clear()
    draw.text(str(i), 10, 10, size=34)
    display.commit()
    time.sleep(0.2)

#display.clear()
display.commit()
