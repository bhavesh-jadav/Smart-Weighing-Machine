
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
int keyCounterArray[16] = {0};
//here '-' will indicate end of command and null character will indicate end of string.
char* keyStringArray[16] = {
  "1.", "2abcABC", "3defDEF", "caps-no-small-",
  "4ghiGHI", "5jklJKL", "6mnoMNO", "up-",
  "7pqrsPQRS", "8tuvTUV", "9wxyzWXYZ", "down-",
  "left-cancel-", "0", "right-menu-", "tare-zero-"
};

//999 to indicate end of array
int alphaKeys[] = {1, 2, 4, 5, 6, 8, 9, 10, 999};
int singlePressKeyButtons[] = {0, 13, 999};
int singlePressCmdButtons[] = {3, 7, 11, 12, 999};
int longPressCmdButtons[] = {14, 15, 999};

static byte kpadState;
char keyToSend;
char *cmdToSend;
char textMode[10] = "no";
byte keyState = 0;  //0 = released, 1 = hold
int textModeKey = 3;

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

void getKeyFromKeyPress(int keyVal, int beginIndex, int endIndex){
  
  char subString[20] = {};
  strncpy(subString, keyStringArray[keyVal] + beginIndex, endIndex - beginIndex);

  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strchr(subString, keyToSend) != NULL)
      keyCounterArray[keyVal]++;
  else{
    keyCounterArray[keyVal] = 0;
    TimeInMillis = millis();
  }
  if(keyCounterArray[keyVal] == strlen(subString))
    keyCounterArray[keyVal] = 0;
  keyToSend = subString[keyCounterArray[keyVal]];
  Serial.println(keyToSend);
  cmdToSend = 1; //reset the cmd value variable to avoid unnecessaray results
}

void getCommandFromKeyPress(int keyVal){
  int counterLimit = 0;
  int i = 0, j = 0, k = 0;
  while(keyStringArray[keyVal][i] != '\0'){
    if(keyStringArray[keyVal][i] == '-')
      counterLimit++;
    i++;
  }
  if((millis() - TimeInMillis) < TimeDiffBtwnKeyPress && strstr(keyStringArray[keyVal], cmdToSend) != NULL)
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
  cmdToSend = cmds[keyCounterArray[keyVal]];
  Serial.println(cmdToSend);
  keyToSend = 1; //reset the key value variable to avoid unnecessaray results
}

//state 0 = released, 1 = hold
void getCommandFormLongKeyPress(int keyVal, int state){
  int i = 0, j = 0, k = 0;
  char cmds[10][10] = {0};
  while(keyStringArray[keyVal][k] != '\0'){
    while(keyStringArray[keyVal][k] != '-'){
      cmds[i][j] = keyStringArray[keyVal][k];
      k++; j++;
    }
    i++; k++; j = 0;
  }
  if(state == 0 && cmds[0] != NULL)
    cmdToSend = cmds[0];
  else if(state == 1 && cmds[1] != NULL)
    cmdToSend = cmds[1];
  else
    cmdToSend = NULL;
  Serial.println(cmdToSend);
   keyToSend = 1; //reset the key value variable to avoid unnecessaray results
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
        getCommandFormLongKeyPress(keyVal, keyState);
      }
      break;

    case RELEASED:
      if(keyState == 1)
        keyState = 0;
        
      else if(isValueInArray(keyVal, longPressCmdButtons))
        getCommandFormLongKeyPress(keyVal, keyState);
      else if(isValueInArray(keyVal, singlePressCmdButtons))
        getCommandFromKeyPress(keyVal);
        
      else if(isValueInArray(keyVal, alphaKeys) && strcmp(textMode, "caps") == 0)
        getKeyFromKeyPress(keyVal, (strlen(keyStringArray[keyVal])/2) + 1, strlen(keyStringArray[keyVal]));
      else if(isValueInArray(keyVal, alphaKeys) && strcmp(textMode, "small") == 0)
        getKeyFromKeyPress(keyVal, 1, strlen(keyStringArray[keyVal])/2);
      else if((isValueInArray(keyVal, singlePressKeyButtons) || isValueInArray(keyVal, alphaKeys)) && strcmp(textMode, "no") == 0)
        getKeyFromKeyPress(keyVal, 0, 1);
        
      else if(isValueInArray(keyVal, singlePressKeyButtons))
        getKeyFromKeyPress(keyVal, 0, strlen(keyStringArray[keyVal]));

      if(keyVal == textModeKey)
        strcpy(textMode, cmdToSend);
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

