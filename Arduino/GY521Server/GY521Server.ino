#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

MPU6050 mpu;

const int baudRate = 19200;
bool dmpReady = false;  // set true if DMP init was successful
uint8_t fifoBuffer[64]; // FIFO storage buffer
Quaternion q;           // [w, x, y, z]         quaternion container
float euler[3];         // [psi, theta, phi]    Euler angle container

void setup() {
  Serial.begin(baudRate);
  while (!Serial);
  Serial.println(F("Starting"));

  Wire.begin();
  Wire.setClock(400000);

  mpu.initialize();
  if (!mpu.testConnection()) {
    Serial.println(F("MPU6050 connection failed"));
  }
  else {
    uint8_t devStatus = mpu.dmpInitialize();
    if (devStatus == 0) {
      mpu.CalibrateAccel(6);
      mpu.CalibrateGyro(6);
      mpu.setDMPEnabled(true);
      dmpReady = true;
      Serial.println(F("Ready"));
    } else {
      Serial.println(F("DMP failed to initialise"));
    }
  }
}

void loop() {
  if (!dmpReady) return;

  if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
    mpu.dmpGetQuaternion(&q, fifoBuffer);
    mpu.dmpGetEuler(euler, &q);
    Serial.print("@ ");
    Serial.print(euler[0]);
    Serial.print(" ");
    Serial.print(euler[1]);
    Serial.print(" ");
    Serial.println(euler[2]);
  }
}
