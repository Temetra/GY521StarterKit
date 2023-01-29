<div align="center">

# GY-521/MPU-6050 Starter Kit

[![Arduino][Arduino.cc]][Arduino-url]
[![.net][dotnet]][dotnet-url]
[![Godot][Godotengine.org]][Godotengine-url]
[![Licence][license-shield]][license-url]

A project bundle demonstrating how to use an MPU-6050 accelerometer and gyroscope sensor.

[About](#about-this-project) •
[Requirements](#requirements) •
[Installation](#installation) •
[Report Bugs][bugs-url]

</div>

## About this project

This is a collection of projects demonstrating how to use data from a GY-521 breakout board in an external program.

It's intended to be a minimal-code example using the Digital Motion Processor (DMP) functionality available in the [I2C Device Library][i2cdevlib].

More information about MPU-6050/GY-521 can be found on [Arduino Playground][playground].

<div align="center">

![Video showing a prism being rotated by the sensor][product-screenshot]

</div>

## Requirements

* [Arduino IDE][arduino-ide]
* [Godot 3.5.1 Mono][godot3mono]
* [I2C Device Library][i2cdevlib]
* An Arduino (tested on Uno and Leonardo)
* A GY-521 breakout board
* Enough 4-wire ribbon cable to connect the breakout board to the Arduino

## Installation

* Download the [i2cdevlib][i2cdevlib] repository.
* From the `Arduino` folder, copy `I2Cdev` and `MPU6050` into your Arduino IDE `libraries` folder.
* Open the Arduino `GY521Server` project, and upload to your device.

Ensure that the SDA (data) and SCL (clock) pins of the breakout board are connected to the appropriate pins on the Arduino. For Uno, these are A4 (SDA) and A5 (SCL). For Leonardo, use pins 2 (SDA) and 3 (SCL).

## Usage

The project uses a baud rate of `19200` by default. Each set of Euler angles are given in radians as follows:

```
@ <psi> <theta> <phi>\r\n
```

This is an example of serial monitor output:

```
Starting
Ready
@ -0.10 0.01 -0.03
@ -0.08 0.00 -0.02
@ -0.06 0.00 -0.02
@ -0.04 -0.00 -0.02
@ -0.02 -0.00 -0.01
@ 0.02 -0.01 -0.01
@ 0.04 -0.02 -0.00
@ 0.07 -0.02 0.00
```


[license-shield]: https://img.shields.io/github/license/Temetra/GY521StarterKit.svg?style=flat
[license-url]: https://github.com/Temetra/GY521StarterKit/blob/master/LICENSE.txt
[product-screenshot]: images/demo.webp
[bugs-url]: https://github.com/Temetra/GY521StarterKit/issues
[Arduino.cc]: https://img.shields.io/badge/Arduino-00979D?style=flat&logo=arduino&logoColor=white
[Arduino-url]: https://www.arduino.cc/
[Godotengine.org]: https://img.shields.io/badge/Godot%20Engine-478CBF?style=flat&logo=godotengine&logoColor=white
[Godotengine-url]: https://godotengine.org/
[dotnet]: https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com/en-us/
[arduino-ide]: https://www.arduino.cc/en/software
[godot3mono]: https://godotengine.org/download/
[i2cdevlib]: https://github.com/jrowberg/i2cdevlib
[playground]: https://playground.arduino.cc/Main/MPU-6050/