
/*
 * keys numbering are as follows 
 * 
 *      0    1   2   3   
 *      4    5   6   7
 *      8    9   10  11
 *      12   13  14  15
 */

#include <Keypad.h>
#include <String.h>

const byte ROWS = 4; //four rows
const byte COLS = 4; //three columns

byte rowPins[ROWS] = {5, 4, 3, 2}; //connect to the row pinouts of the keypad
byte colPins[COLS] = {19, 18, 17, 16}; //connect to the column pinouts of the keypad

char keys[ROWS][COLS] = {
    { 0,1,2,3 },
    { 4,5,6,7 },
    { 8,9,10,11 },
    { 12,13,14,15 }
};

Keypad key_pad(makeKeymap(keys), rowPins, colPins, sizeof(rowPins), sizeof(colPins));

int KeyCounterArray[16] = {0};
char* KeyStringArray[16] = {
  "1.", "2abc", "3def", "",
  "4ghi", "5jkl", "6mno", "",
  "7pqrs", "8tuv", "9wxyz", "",
  "no-caps-small", "0", "", ""
};

static byte kpadState;
char keyToSend;

unsigned long TimeInMillis;

void setup() {
  Serial.begin(9600);
  key_pad.begin(makeKeymap(keys));
  key_pad.addEventListener(keypadEvent);
  //key_pad.setDebounceTime(100);
  TimeInMillis = millis();
}

void loop() {
  int val = key_pad.getKey();
  if (val){
    //Serial.println(val);
    //Serial.println(keyToSend);
  }
}

char getKeyFromKeyPress(int keyVal){
  int TimeDiffBtwnKeyPress = 1500;
  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strchr(KeyStringArray[keyVal], keyToSend) != NULL)
          KeyCounterArray[keyVal]++;
        else{
          KeyCounterArray[keyVal] = 0;
          TimeInMillis = millis();
        }
        if(KeyCounterArray[keyVal] == strlen(KeyStringArray[keyVal]))
          KeyCounterArray[keyVal] = 0;
        keyToSend = KeyStringArray[keyVal][KeyCounterArray[keyVal]];
}

void keypadEvent(KeypadEvent key){
  kpadState = key_pad.getState();
  int keyVal = key;
  switch(kpadState){
    case PRESSED:
      getKeyFromKeyPress(keyVal);
      Serial.println(keyToSend);
      break;
  }
    
}

