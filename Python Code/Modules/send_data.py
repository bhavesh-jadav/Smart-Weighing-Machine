import requests
import MySQLdb as sqldb
import json

def send():
	url = "https://swmw.me/api/machine_data"
	headers = {'content-type': 'application/json'}
	f = '%Y-%m-%d %H:%M:%S'
	try:
		db = sqldb.connect(host = "localhost", user="local", db="swm")
		cursor = db.cursor()
		cursor.execute("SELECT * FROM data")
		rows = cursor.fetchall()
		for row in rows:
			payload = {
						'ProductId':int(row[2]),
						'Weight':int(row[3]),
						'LocationId':int(row[1]),
						'DateAndTime':row[4].strftime(f),
						'UserId':str(row[6]),
						'MachineId':int(row[5])
					  }
			response = requests.post(url, data=json.dumps(payload), headers=headers)
			if response.status_code == 200:
				cursor.execute("DELETE FROM data WHERE id = %s", (row[0], ))
				db.commit()
		return True
		
	except Exception as e:
		print str(e)
		return False
	finally:
		cursor.close()
		db.close()
		
def main():
	send()
	
if __name__ == "__main__":
	main()
