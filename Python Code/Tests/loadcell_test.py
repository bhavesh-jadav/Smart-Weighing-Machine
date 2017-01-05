from Modules import loadcell

loadcell.calibrate()
'''
try:
	while 1:
		val = loadcell.weight_in_gram()
		print val
except KeyboardInterrupt:
	print "ok"
'''
