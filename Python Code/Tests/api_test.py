import requests
import hashlib

username = "bhavesh"
password = "bhavesh123"

hash_object = hashlib.sha256(password.encode())
hex_dig = hash_object.hexdigest()
print hex_dig

url = ""
