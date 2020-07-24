#include <PololuLedStrip.h>  // LED Library
#include <LiquidCrystal.h>   // LCD screen Library
#include <Stepper.h>         // Stepper motor Library
#include <FastLED.h>

// LCD   --------------------------------------------------
// Associating LCD interface pin with the arduino pin number it is connected to
const int rs = 49, en = 48, d4 = 52, d5 = 53, d6 = 50, d7 = 51;
LiquidCrystal lcd(rs, en, d4, d5, d6, d7);

// LED Strips -------------------------------------------
#define NUM_LEDS    29
PololuLedStrip<30> ledsPlayer1;
PololuLedStrip<31> ledsPlayer2;
rgb_color colors[NUM_LEDS];

// Steppermotor ------------------------------------------
const int stepsPerRevolution = 200;            // 360/stepDegreeOfMotor = 200
Stepper wallStepper(stepsPerRevolution, 8, 9); // initialize the stepper pins (stepPerRev,step,dir)
const int IN_LeftLimitButton = 22;
const int IN_RightLimitButton = 23;
int stepsPerStep = 1;
const int stepStateDelay = 1000;
unsigned long stepStateTimer;


//Player Inputs ---------------------------------------------------
const int IN_GoalSensorPlayer1 = A1;
const int IN_GoalSensorPlayer2 = A0;
const int IN_pointPlayer1_low_Button = 27;
const int IN_pointPlayer1_medium_Button = 29;
const int IN_pointPlayer2_low_Button = 26;
const int IN_pointPlayer2_medium_Button = 28;

//Player Outputs ---------------------------------------------------
const int OUT_BuzzerPlayer1 = 40;
const int OUT_BuzzerPlayer2 = 41;

// WARNING || ENSURE ABOVE THAT NO PINS ARE CONFLICTING!!!!   -----------------


// GAME MECHANISM
// Game Const variables ---------------------------------------------------
const String player1 = "Player 1";
const String player2 = "Player 2";

const int soundDurationMs = 200;
const int soundWinDurationMs = 3000;
const int sound_low_Hz = 400;
const int sound_medium_Hz = 800;
const int sound_high_Hz = 1500;

const int goalSensor1Threshold = 230;
const int goalSensor2Threshold = 100;

const int winningScore = 1000;
const int point_low = 15;
const int point_medium = 30;
const int point_high = 100;
const int goalDelay = 2000;
const int pointDelay = 300;
const int winDelay = 5000;

// Game States --------------------------------------------------- 
bool buzzerPlayer1Played = true;
bool buzzerPlayer2Played = true;

const int ledDelay = 500;
unsigned long ledTimer;

int player1Score = 0;
int player2Score = 0; 
int player1LatestPoints = 0;
int player2LatestPoints = 0;

bool player1HasScored = false;
bool player2HasScored = false;

unsigned long player1GoalTimer = 0;
unsigned long player2GoalTimer = 0;
unsigned long player1PointTimer =0;
unsigned long player2PointTimer =0;

bool hasWon = false;
int scorePrinted = false;
String winner = "";

// ---------------------------------------------------

void setup() {
  delay( 2000 ); // power-up safety delay
  Serial.begin(9600);
  
  stepStateTimer = millis();
  player1GoalTimer = millis();
  player2GoalTimer = millis();
  player1PointTimer = millis();
  player2PointTimer = millis();

  ledTimer = millis();
  
  lcd.begin(16, 2); // set up the LCD's number of columns and rows:
  lcd.display();

  wallStepper.setSpeed(200);

  pinMode(IN_pointPlayer1_low_Button,INPUT);
  pinMode(IN_pointPlayer1_medium_Button,INPUT);
  pinMode(IN_pointPlayer2_low_Button,INPUT);
  pinMode(IN_pointPlayer2_medium_Button,INPUT);
  
  pinMode(IN_GoalSensorPlayer1,INPUT);
  pinMode(IN_GoalSensorPlayer2,INPUT);
  
  pinMode(IN_LeftLimitButton,INPUT);
  pinMode(IN_RightLimitButton,INPUT);
  pinMode(OUT_BuzzerPlayer1,OUTPUT);
  pinMode(OUT_BuzzerPlayer2,OUTPUT);
}

void loop() {
    determineState();
    showState();
}

// This main function transform all game states based various conditions and inputs
void determineState(){
  if(!hasWon){
    checkIfWinner();
    checkPlayerGoalScore();
    checkPlayerPointScore();
    checkScoredState();
    checkStepperDir();
  }
  else{
    setWinState();
  }
}

// This main function reads the game states and display them
void showState(){
  printScore();
  rainbowLights();
  playScoreSound();
  moveStepper();
}


//StateChecker ---------------------------------------------------
void checkIfWinner(){
  if(player1Score >= winningScore){
      winner = player1;
      hasWon = true;
  }
  else if(player2Score >= winningScore){
      winner = player2; 
      hasWon = true;
  }
}

void checkPlayerGoalScore(){
  int goalSensor1Value = analogRead(IN_GoalSensorPlayer1);
  int goalSensor2Value = analogRead(IN_GoalSensorPlayer2);
  Serial.println(String(goalSensor1Value)+"|"+String(goalSensor2Value));
  if(isReady(player1GoalTimer,goalDelay) && goalSensor1Value > goalSensor1Threshold){
      SetScore(player1,point_high);
      player1GoalTimer = millis();
  }
  if(isReady(player2GoalTimer,goalDelay) && goalSensor2Value > goalSensor2Threshold){
      SetScore(player2,point_high);
      player2GoalTimer = millis();
  }
}

void checkPlayerPointScore(){
    if(isReady(player1PointTimer,pointDelay)){
      if(isButHit(IN_pointPlayer1_low_Button)){
        SetScore(player1,point_low);
      }
      if(isButHit(IN_pointPlayer1_medium_Button)){
        SetScore(player1,point_medium);
      }   
      player1PointTimer = millis();
    }

  if(isReady(player2PointTimer,pointDelay)){
      if(isButHit(IN_pointPlayer2_low_Button)){
        SetScore(player2,point_low);
      }
      if(isButHit(IN_pointPlayer2_medium_Button)){
        SetScore(player2,point_medium);
      }
      player2PointTimer = millis();
    }
}

void checkScoredState(){
  if(player1HasScored){
    player1HasScored = false;
    scorePrinted = false;
    buzzerPlayer1Played = false;        
  }
  if(player2HasScored){
    player2HasScored = false;
    scorePrinted = false;
    buzzerPlayer2Played = false;
  }
}

void checkStepperDir(){
  if(isReady(stepStateTimer,stepStateDelay)){
    if(isStepperLimitHit()){    
      stepsPerStep *= -1;
      Serial.println(stepsPerStep);
      stepStateTimer = millis();
    }
  }
 
}

// Winning State
void(* resetFunc) (void) = 0; //declare reset function @ address 0
void setWinState(){
  printToDisplay(String(winner)+" has won!");
  turnOnWinnerLights();
  if(winner == player1){
    playWinningSound(OUT_BuzzerPlayer1);
  }
  else{
    playWinningSound(OUT_BuzzerPlayer2);
  }
  delay(winDelay);
  //Reset Game
  resetFunc();
}



// StateDisplayer  ---------------------------------------------------
void playScoreSound(){
  if(!buzzerPlayer1Played){
    int freq = getScoreSoundFreq(player1LatestPoints);
    tone(OUT_BuzzerPlayer1,freq,soundDurationMs);
    buzzerPlayer1Played = true;
  }
  if(!buzzerPlayer2Played){
    int freq = getScoreSoundFreq(player2LatestPoints);
    tone(OUT_BuzzerPlayer2,freq,soundDurationMs);
    buzzerPlayer2Played = true;
  } 
}

void playWinningSound(int playerBuzzer){
    for(int i = 0; i<3;i++){
      for(int i = 1000; i>=100;i--){
        tone(playerBuzzer,i);
        delay(1);
    }
  }
}

void printScore(){
  if(!scorePrinted){
    printToDisplay(String(player1Score)+" - "+String(player2Score));
    scorePrinted = true;
  }
}

void turnOnWinnerLights(){
  rgb_color color;
  if(winner == player1){
    for( int i = 0; i < NUM_LEDS; i++) {
      color.red = 0;
      color.green = 0;
      color.blue = 255;
      colors[i] = color;
    }
  }
  else{
    for(int i=0;i<NUM_LEDS;i++){
      color.red = 255;
      color.green = 0;
      color.blue = 0;
      colors[i] = color;
    }
  }
    showLights();
    ledTimer = millis(); 
}

void rainbowLights(){
  if(isReady(ledTimer,ledDelay)){
    for( int i = 0; i < NUM_LEDS; i++) {
      rgb_color color;
      color.red = random8();
      color.green = random8();
      color.blue = random8();
      colors[i] = color;
    }
    showLights();
    ledTimer = millis();
  }
}

void moveStepper(){
  wallStepper.step(stepsPerStep);
}


// Utils ---------------------------------------------------
void printToDisplay(String s){
  lcd.clear();
  lcd.print(s);
}

int getScoreSoundFreq(int pointGiven){
  switch (pointGiven){
    case point_low: return sound_low_Hz;
    case point_medium: return sound_medium_Hz;
    case point_high: return sound_high_Hz;
  }
}

void SetScore(String player,int points){
  if(player == player1){
      player1Score += points;
      player1LatestPoints = points;
      player1HasScored = true;
  }
  else if(player == player2){
      player2Score += points;
      player2LatestPoints = points;
      player2HasScored = true;
  }
}

bool isReady(unsigned long timer, int delayValue){
  return millis()-timer > delayValue;
}

bool isButHit(int button){
  return digitalRead(button) == HIGH;
}

bool isStepperLimitHit(){
  return isButHit(IN_LeftLimitButton)||isButHit(IN_RightLimitButton);
}

void showLights(){
  ledsPlayer1.write(colors, NUM_LEDS);
  ledsPlayer2.write(colors, NUM_LEDS);
}
