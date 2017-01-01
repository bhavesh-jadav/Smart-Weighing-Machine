
/**************************************************************************************************
 * Auther: Bhavesh Jadav
 * File name: keypad.ino
 * *************************************************************************************************
 * keys numbering are as follows 
 * 
 *      0    1   2   3
 *      4    5   6   7
 *      8    9   10  11
 *      12   13  14  15
 *      
 * this arduino code is used to assign multiple characters or strings to single button in keypad matrix.
 * there are 3 different scenarios
 * 1) assign multiple characters to single button(which I refered to as key in this program) and cycle through them one by one by pressing same button in some time interval.
 * 2) assign multiple strings to single button(which I refered to as cmd in this program) and cycle through them one by one by pressing same button in some time interval.
 *    note that this strings can be made of single characters to multiple characters. 
 * 3) assign long press i.e. hold button functionality any of the above case
 */

#include <Keypad.h>
#include <String.h>

//change following according to the keypad
const byte ROWS = 4; //four rows
const byte COLS = 4; //three columns
byte rowPins[ROWS] = {5, 4, 3, 2}; //connect to the row pinouts of the keypad
byte colPins[COLS] = {19, 18, 17, 16}; //connect to the column pinouts of the keypad

//assign number to the each key as shwon in below. since I am using 4x4 keypad hence numbers are from 0 to 15.
//MAKE SURE NUMBERING STARTS FROM 0 AND THERE IS NO NUMBERS ARE MISSING BETWEEN TWO NUMBERS!
char keys[ROWS][COLS] = {
    { 0,1,2,3 },
    { 4,5,6,7 },
    { 8,9,10,11 },
    { 12,13,14,15 }
};

/*
 * Following array is used to assign characters and strings to the keys
 * 1) If you want to assign character to the keys then assign characters without space for e.g. '2abcABC'
 * 2) If you want to assign strings to keys then assign strings with '-' at the end for e.g. 'caps-small-no'
 * 3) If you want to assign both character and string then you can do it like 'A-a-hello-*-ok'. by adding '-' between characters and strings.
 */
char* keyStringArray[20] = {
  "1.", "2abcABC", "3defDEF", "caps-small-no-",
  "4ghiGHI", "5jklJKL", "6mnoMNO", "up-",
  "7pqrsPQRS", "8tuvTUV", "9wxyzWXYZ", "down-",
  "left-", "0", "right-", "tare-"
};

/*
 * if you want to assign long press button functionality to any button then assign function using following array.
 * make sure to assign to appropriate button.
 */
char* longPressKeyStringArray[20] = {
  "", "", "", "",
  "", "", "", "",
  "", "", "", "",
  "cancel", "", "menu", "zero"
};

//999 to indicate end of array
int alphaKeys[] = {1, 2, 4, 5, 6, 8, 9, 10, 999};  //add numbers of keys which are going to represent alphabet.
int singlePressKeyButtons[] = {0, 1, 2, 4, 5, 6, 8, 9, 10, 13, 999};  //add numbers of keys which are going to cycle through characters
int singlePressCmdButtons[] = {3, 7, 11, 12, 14, 15, 999}; //add numbers of keys which are going to cycle through strings
int longPressCmdButtons[] = {12, 14, 15, 999};  //add nubers of characters which are going to have long press button functionality

static byte kpadState;  //to store key state i.e. PRESSED, HOLD or RELEASED
char textMode[10] = "no";  //initial text mode, no = number, caps = capital alphabet, small = small alphabet
byte keyState = 0;  //0 = released, 1 = hold
int textModeKey = 3; //assign textmode key number to this variable.
char valueToSend[50] = {}; //it will store final value to send to the serial monitor.

unsigned long TimeInMillis;

//initialize the keypad
Keypad key_pad(makeKeymap(keys), rowPins, colPins, sizeof(rowPins), sizeof(colPins));

//follwoing array will used to store the number of times the key is pressed which will help to cycle through characters or strings.
int keyCounterArray[16] = {0};

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
}

//minimum time in milliseconds required between two key press to cycle through values
int TimeDiffBtwnKeyPress = 1500;

//following function is used to cycle through charactes
//startIndex and endIndex are used to cycle through particular part of character array. this is important because
//in 'caps' text mode we only want to cycle through capital letters, in 'small' text mode we only want to cycle through small letters etc.
void getKeyFromKeyPress(int keyVal, int beginIndex, int endIndex){  
  char subString[50] = {};
  strncpy(subString, keyStringArray[keyVal] + beginIndex, endIndex - beginIndex);

  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strstr(subString, valueToSend) != NULL)
      keyCounterArray[keyVal]++;
  else{
    keyCounterArray[keyVal] = 0;
    TimeInMillis = millis();
  }
  if(keyCounterArray[keyVal] == strlen(subString))
    keyCounterArray[keyVal] = 0;
  valueToSend[0] = subString[keyCounterArray[keyVal]];
  valueToSend[1] = '\0';
  Serial.println(valueToSend);
}

//following function is used to cycle through strings
void getCommandFromKeyPress(int keyVal){
  int counterLimit = 0;
  int i = 0, j = 0, k = 0;
  while(keyStringArray[keyVal][i] != '\0'){
    if(keyStringArray[keyVal][i] == '-')
      counterLimit++;
    i++;
  }
  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strstr(keyStringArray[keyVal], valueToSend) != NULL && valueToSend != "")
      keyCounterArray[keyVal]++;
  else{
    keyCounterArray[keyVal] = 0;
    TimeInMillis = millis();
  }
  if(keyCounterArray[keyVal] == counterLimit)
    keyCounterArray[keyVal] = 0;
    
  i = 0, j = 0, k = 0;
  char cmds[10][10] = {0};
  while(keyStringArray[keyVal][k] != '\0'){
    while(keyStringArray[keyVal][k] != '-'){
      cmds[i][j] = keyStringArray[keyVal][k];
      k++; j++;
    }
    i++; k++; j = 0;
  }
  strcpy(valueToSend, cmds[keyCounterArray[keyVal]]);
  Serial.println(valueToSend);
}

//following function is used for long press functionality
void getCommandFormLongKeyPress(int keyVal){
  if(longPressKeyStringArray[keyVal] != ""){
    strcpy(valueToSend, longPressKeyStringArray[keyVal]);
    Serial.println(valueToSend);
  }
}

void keypadEvent(KeypadEvent key){
  kpadState = key_pad.getState();
  int keyVal = key;
  switch(kpadState){
    case PRESSED:
    break;

    case HOLD:
      if(isValueInArray(keyVal, longPressCmdButtons)){
        keyState = 1;
        getCommandFormLongKeyPress(keyVal);
      }
      break;

    case RELEASED:
      if(keyState == 1)
        keyState = 0;
        
      else if(isValueInArray(keyVal, singlePressCmdButtons))
        getCommandFromKeyPress(keyVal);
      
      //following part of code will use startIndex and endIndex of particular part of character array to cycle through on that much part.
      else if(isValueInArray(keyVal, alphaKeys) && strcmp(textMode, "caps") == 0)
        getKeyFromKeyPress(keyVal, (strlen(keyStringArray[keyVal])/2) + 1, strlen(keyStringArray[keyVal]));
      else if(isValueInArray(keyVal, alphaKeys) && strcmp(textMode, "small") == 0)
        getKeyFromKeyPress(keyVal, 1, strlen(keyStringArray[keyVal])/2 + 1);
      else if(isValueInArray(keyVal, singlePressKeyButtons) && strcmp(textMode, "no") == 0)
        getKeyFromKeyPress(keyVal, 0, 1);
        
      else if(isValueInArray(keyVal, singlePressKeyButtons))
        getKeyFromKeyPress(keyVal, 0, strlen(keyStringArray[keyVal]));

      if(keyVal == textModeKey)
        strcpy(textMode, valueToSend);
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

