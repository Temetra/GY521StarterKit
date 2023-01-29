using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Numerics;
using System.Text;

// Config
string portName = "COM10";
int baudRate = 19200;

// Serial processing
Timer? checkPortTimer = null;
SerialPort? port = null;
char[] buffer = new char[256];
StringBuilder sb = new();

// Output
Vector3 sensorRotation = Vector3.Zero;

// Test loop
checkPortTimer = new Timer(OpenPort, null, 0, 1000);
while (true)
{
	ReadFromPort();
	Thread.Sleep(TimeSpan.FromMilliseconds(1 / 60));
}

void OpenPort(Object? stateInfo)
{
	if (port == null)
	{
		port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);

		try
		{
			port.Open();
			Debug.WriteLine("Opened port successfully");
		}
		catch (Exception)
		{
			Debug.WriteLine("Failed to open port");
			port.Dispose();
			port = null;
		}
	}
}

void ReadFromPort()
{
	if (port?.IsOpen == true)
	{
		int charsRead = 0;

		try
		{
			// Avoid blocking by only reading number of chars available
			// This assumes we only ever send ASCII chars
			int maxChars = Math.Min(port.BytesToRead, buffer.Length);
			charsRead = port.Read(buffer, 0, maxChars);
		}
		catch (Exception)
		{
			// Failed to read data
			// The port will be kept open even if the device is disconnected
			// Values in SerialPort.GetPortNames will be stale
			// Easiest way to handle this is to destroy the port
			port?.Dispose();
			port = null;
			sb.Clear();
		}

		// If any chars were read, proceed to next step
		if (charsRead > 0)
		{
			BuildLines(buffer[..charsRead]);
		}
	}
}

void BuildLines(char[]? slice)
{
	if (slice == null) return;

	// Start and end of line markers
	int start = 0, 
		end = Array.IndexOf(slice, '\n');

	// Process lines while end marker exists in data
	while (end >= 0)
	{
		// Append range to stringbuilder
		sb.Append(slice[start..end]);

		// Remove carriage return and process line
		ProcessLine(sb.ToString().Replace("\r", ""));

		// Clear stringbuilder
		sb.Clear();

		// Get next line markers
		start = (end + 1);
		end = Array.IndexOf(slice, '\n', start);
	}

	// Append remainder to stringbuilder for next time
	sb.Append(slice[start..]);
}

void ProcessLine(string line)
{
	if (line != null)
	{
		if (line.Length > 0 && line[0] == '@')
		{
			ParseRotation(line);
			Debug.WriteLine(sensorRotation);
		}
		else
		{
			Debug.WriteLine(string.Format("out = {0}", line));
		}
	}
}

void ParseRotation(string data)
{
	var vals = data.Split(" ");
	if (vals.Length == 4)
	{
		_ = float.TryParse(vals[1], out float psi);
		_ = float.TryParse(vals[2], out float theta);
		_ = float.TryParse(vals[3], out float phi);
		sensorRotation = new Vector3(-theta, -phi, psi);
	}
}
