using Godot;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

public class RotatePrism : MeshInstance
{
	// Config
	readonly string portName = "COM10";
	readonly int baudRate = 19200;

	// Serial processing
	System.Threading.Timer checkPortTimer;
	SerialPort port;
	char[] buffer = new char[256];
	StringBuilder sb = new StringBuilder();

	// Output
	Vector3 sensorRotation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Start a threaded timer to ensure port is open
		checkPortTimer = new System.Threading.Timer(OpenPort, null, 0, 1000);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		this.Rotation = sensorRotation;
	}

	public override void _PhysicsProcess(float delta)
	{
		ReadFromPort();
	}

	void OpenPort(System.Object stateInfo)
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
				BuildLines(new ArraySegment<char>(buffer, 0, charsRead).ToArray());
			}
		}
	}

	void BuildLines(char[] slice)
	{
		if (slice == null) return;

		// Start and end of line markers
		int start = 0,
			end = Array.IndexOf(slice, '\n');

		// Process lines while end marker exists in data
		while (end >= 0)
		{
			// Append range to stringbuilder
			sb.Append(new ArraySegment<char>(slice, start, end - start).ToArray());

			// Remove carriage return and process line
			ProcessLine(sb.ToString().Replace("\r", ""));

			// Clear stringbuilder
			sb.Clear();

			// Get next line markers
			start = (end + 1);
			end = Array.IndexOf(slice, '\n', start);
		}

		// Append remainder to stringbuilder for next time
		sb.Append(new ArraySegment<char>(slice, start, slice.Length - start).ToArray());
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
		var vals = data.Split(' ');
		if (vals.Length == 4)
		{
			_ = float.TryParse(vals[1], out float psi);
			_ = float.TryParse(vals[2], out float theta);
			_ = float.TryParse(vals[3], out float phi);
			sensorRotation = new Vector3(-theta, -phi, psi);
		}
	}
}
