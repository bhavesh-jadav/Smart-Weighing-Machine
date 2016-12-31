
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
#include <assert.h>
#include <stdlib.h>

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
//here '-' will indicate end of command and null character will indicate end of string.
char* KeyStringArray[16] = {
  "1.", "2abc", "3def", "no-caps-small-",
  "4ghi", "5jkl", "6mno", "up-",
  "7pqrs", "8tuv", "9wxyz", "down-",
  "left-", "0", "right-", "tare-zero-"
};

//999 to indicate end of array
int singlePressKeyButtons[] = {0,1,2,4,5,6,8,9,10,999};
int singlePressCmdButtons[] = {3,7,11,12,13,14,999};
int longPressCmdButtons[] = {15,999};

static byte kpadState;
char keyToSend;
char *cmdToSend;
char *textMode;

unsigned long TimeInMillis;

void setup() {
  Serial.begin(9600);
  key_pad.begin(makeKeymap(keys));
  key_pad.addEventListener(keypadEvent);
  //key_pad.setDebounceTime(100);
  key_pad.setHoldTime(2000);
  TimeInMillis = millis();
}

void loop() {
  int val = key_pad.getKey();
  if (val){
    //Serial.println(val);
    //Serial.println(keyToSend);
  }
}

int TimeDiffBtwnKeyPress = 1500;

char getKeyFromKeyPress(int keyVal){
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

char *getCommandFromKeyPress(int keyVal){
  int counterLimit = 0;
  int i = 0, j = 0, k = 0;
  while(KeyStringArray[keyVal][i] != '\0'){
    if(KeyStringArray[keyVal][i] == '-')
      counterLimit++;
    i++;
  }
  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strstr(KeyStringArray[keyVal], cmdToSend) != NULL)
      KeyCounterArray[keyVal]++;
  else{
    KeyCounterArray[keyVal] = 0;
    TimeInMillis = millis();
  }
  if(KeyCounterArray[keyVal] == counterLimit)
    KeyCounterArray[keyVal] = 0;
    
  i = 0, j = 0, k = 0;
  char cmds[10][10] = {0};
  while(KeyStringArray[keyVal][k] != '\0'){
    while(KeyStringArray[keyVal][k] != '-'){
      cmds[i][j] = KeyStringArray[keyVal][k];
      k++; j++;
    }
    i++; k++; j = 0;
  }
  cmdToSend = cmds[KeyCounterArray[keyVal]];
}

void keypadEvent(KeypadEvent key){
  kpadState = key_pad.getState();
  int keyVal = key;
  switch(kpadState){
    case PRESSED:
    if(isValueInArray(keyVal, singlePressCmdButtons)){ 
      getCommandFromKeyPress(keyVal);
      Serial.println(cmdToSend);
    }
    else if(isValueInArray(keyVal, singlePressKeyButtons)){
      getKeyFromKeyPress(keyVal);
      Serial.println(keyToSend);
    }
    break;

    case HOLD:
      Serial.println("HOLD");
      break;
  }
}

bool isValueInArray(int val, int *arr){
    int i = 0;
    while (arr[i] != 999) {
        if (arr[i] == val)
            return true;
        i++;
    }
    return false;
}

