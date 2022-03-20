using Digigrade.Otto.Comms.Commands;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;

namespace Digigrade.Otto.Comms.SerialUtilities
{
    public class OttoSerialPort : INotifyPropertyChanged
    {
        /// <summary>
        /// String used to identify the CP210X UART/USB Adapter,  This found in the Device Driver
        /// </summary>
        public const string CP210x = "CP210x";

        /// <summary>
        /// Default baud rate to use with the CP210X UART/USB.
        /// </summary>
        public const int DefualtBaudRate = 250000;

        #region COMMANDS THAT NEED TO BE REWORKED INTO BETTER STRUCTURE

        // Separator Characters
        const char SEPARATOR_SCRIPT = '~';
        const char SEPARATOR_CMD_SEGMENT = '%';  // Group Separator char

        // Command Characters
        const char CMD_START_BYTE = '$';  // Start of Heading char
        const char CMD_END_BYTE = '#';  // End of Transmission char
        const char CMD_CONTINUE = '@';

        // Programing Characters
        const char PROGRAM_MODE_CHAR = '0';
        const char NORMAL_MODE_CHAR = '1';
        const char SETTINGS_CHAR = '2';
        const char SAVE_EEPROM_CHAR = '3';
        const char EEPROM_AVAIL_CHAR = '4';
        const char CLEAR_SCRIPTS_CHAR = '5';
        const char ADD_SCRIPT_CHAR = '6';
        const char FORCE_RESET_CHAR = '7';

        // Settings Characters
        const char SETTINGS_EP_CHAR = 'a';
        const char SETTINGS_RAD_KEY_CHAR = 'b';
        const char SETTINGS_RADIO_CHAN_CHAR = 'c';
        const char SETTINGS_RADIO_PA_CHAR = 'd';
        const char SETTINGS_BYTES_CHAR = 'e';
        const char SETTINGS_FIRMWARE_CHAR = 'f';
        const char SETTINGS_RADIO_RETRY_DELAY_CHAR = 'g';
        const char SETTINGS_RADIO_DATA_RATE_CHAR = 'h';
        const char SETTINGS_RADIO_CRC_LENGTH_CHAR = 'i';
        const char SETTINGS_RADIO_PAYLOAD_SIZE_CHAR = 'j';

        #endregion

        public string? DriverName { get; set; }
        public string? Name { get; set; }
        public int Port { get; set; }
        public int BaudRate { get; set; }

        private const int ReadTimeout = 1000;
        private const int WriteTimeout = 1000;
        private Thread? readThread = null;
        private List<byte> _dataBuffer = new List<byte>();
        private List<string> _commandQueue = new List<string>();
        private SerialPort? _serialPort;

        public SerialPort? SerialPort
        {
            get
            {
                if (_serialPort == null && !string.IsNullOrWhiteSpace(Name))
                {
                    _serialPort = new SerialPort(Name)
                    {
                        BaudRate = BaudRate,
                        ReadTimeout = ReadTimeout,
                        WriteTimeout = WriteTimeout
                    };
                }
                return _serialPort;
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        public static List<OttoSerialPort> AllCompatiblePorts()
        {
            var returnValue = new List<OttoSerialPort>();
            var portNames = new List<string>();

            foreach (string portName in SerialPort.GetPortNames())
            {
                portNames.Add(portName);
            }

            // If the port's hardware driver contains the text then use this device.
            //if (port.DriverName?.Contains(OttoSerialPort.CP210x) ?? false)
            //{
            //    ConnectedDevice = new PicoOttoDeviceProfile();
            //    ConnectedDevice.Connect(port);
            //}

            //try
            //{
            //    List<string> allPortsFullString = new List<string>();
            //    System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("Select * from WIN32_SerialPort");

            //    foreach (String curPortName in returnValue)
            //    {
            //        String curPortFullString = curPortName;

            //        foreach (System.Management.ManagementObject port in searcher.Get())
            //        {
            //            string s = port.GetPropertyValue("Name").ToString();
            //            int start = s.IndexOf("(") + 1;
            //            int end = s.IndexOf(")", start);
            //            string result = s.Substring(start, end - start);

            //            if (result == curPortName)
            //            {
            //                curPortFullString += " " + port.GetPropertyValue("Description").ToString();
            //            }
            //        }

            //        allPortsFullString.Add(curPortFullString);
            //    }

            //    return allPortsFullString;
            //}
            //catch
            //{
            //    return returnValue;
            //}

            return returnValue;
        }

        public bool IsOpen { get => SerialPort?.IsOpen ?? false; }
        public bool IsConnected { get; set; } = false;
        public bool IsPaused { get; set; } = false;
        public bool AttemptingToConnect 
        { 
            get => _connecting; 
            set
            {
                if (_connecting != value)
                {
                    _connecting = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        private bool _connecting = false;

        /// <summary>
        /// Directly raises a property changed event.  If no value is passed, the calling member's name will be used.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Stops communication and cleans up managed port. 
        /// </summary>
        /// <param name="forceUnsafe">Opptional value to override safe shutdown procedures.</param>
        public void Shutdown(bool forceUnsafe = false)
        {
            IsConnected = false;
            AttemptingToConnect = true;
            IsPaused = false;
            SerialPort?.Close();
        }

        public DeviceCommand.ProcessCommandDelegate? ProcessCommandDelegate { get; set; }

        public void Read()
        {
            var lastCapturedTicks = DateTime.Now.Ticks;
            var timeOutLoopCounter = 0;

            // Loop continually when inSensor is connected 
            while (IsOpen)
            {
                try
                {
                    if (!IsPaused)
                    {
                        var elapsedTicks = DateTime.Now.Ticks - lastCapturedTicks;
                        var elapsedSpan = new TimeSpan(elapsedTicks);

                        // If there is data ready to be read from the serial device
                        if (SerialPort?.BytesToRead > 0)
                        {
                            // Read the next byte from the serial device
                            var readByte = (byte)SerialPort.ReadByte();

                            if (readByte == CMD_CONTINUE)
                            {
                                SendNextQueuedCommand();
                            }
                            else
                            {
                                _dataBuffer.Add(readByte);

                                if ((char)readByte == CMD_END_BYTE)
                                {
                                    ProcessDataBuffer();
                                }
                            }

                            lastCapturedTicks = DateTime.Now.Ticks;
                            timeOutLoopCounter = 0;
                        }
                        else if (Math.Round(elapsedSpan.TotalMilliseconds, 0) > 100 && _dataBuffer.Count > 0)
                        {
                            if (IsConnected)
                            {
                                var fullMessage = "";

                                foreach (byte curByte in _dataBuffer)
                                {
                                    fullMessage += (char)curByte;
                                }

                                //inSensor.myParentWindow.PrintToDebug_NewLine(fullMessage);
                                _dataBuffer.Clear();
                                lastCapturedTicks = DateTime.Now.Ticks;
                            }
                            else
                            {
                                if (timeOutLoopCounter < 10)
                                {
                                    timeOutLoopCounter += 1;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
                catch
                {
                    // If there is an error, stop reading data and update the connection
                    //if (inSensor.IsConnected == true)
                    //{
                    //    inSensor.myParentWindow.PrintToDebug_NewLine("PC: LOST CONNECTION WITH DEVICE!");
                    //}
                    _dataBuffer.Clear();
                    //inSensor.KillThread();
                    //IsOpen = false;
                    IsConnected = false;
                    AttemptingToConnect = false;
                    //mySavedDevice = null;
                    //inSensor.myParentWindow.UpdateSavedDevices();
                    //Thread.CurrentThread.Abort();
                    return;
                }
            }
        }

        public void SendCommand(string inString)
        {
            try
            {
                string sendString = "";
                sendString += CMD_START_BYTE;
                sendString += inString;
                sendString += CMD_END_BYTE;
                PrintString(sendString);
            }
            catch
            {
            }
        }
        public void SendCommand(char inChar)
        {
            try
            {
                String sendString = "";
                sendString += CMD_START_BYTE;
                sendString += inChar;
                sendString += CMD_END_BYTE;
                PrintString(sendString);
            }
            catch
            {
            }
        }
        public void AddQueCommand(string value)
        {
            String addString = "";
            addString += CMD_START_BYTE;
            addString += value;
            addString += CMD_END_BYTE;

            _commandQueue.Add(addString);
        }
        public void AddQueCommand(char value)
        {
            String addString = "";
            addString += CMD_START_BYTE;
            addString += value;
            addString += CMD_END_BYTE;
            _commandQueue.Add(addString);
        }
        public void PrintString(string value)
        {
            SerialPort?.Write(value);
        }

        private void ProcessCompleteMessage(List<string> inCommand)
        {
            var deviceCommand = new DeviceCommand();
            //if (inCommand[0][0] == PROGRAM_MODE_CHAR)
            //{
            //    IsConnected = true;
            //    SendGetSettings();
            //    myParentWindow.PrintToDebug_NewLine("DEVICE: <-LOADING PROGRAM MODE");
            //    myParentWindow.UpdateDeviceModeStatusLabel("PROGRAM", true);
            //}
            //else if (inCommand[0][0] == SAVE_EEPROM_CHAR)
            //{
            //    myParentWindow.PrintToDebug_NewLine("DEVICE <-PROGRAMMING");
            //    SendCommand((char)FORCE_RESET_CHAR + "");
            //    StopReading();
            //    System.Threading.Thread.Sleep(1000);
            //    ResumeReading();
            //}
            //else if (inCommand[0][0] == NORMAL_MODE_CHAR)
            //{
            //    myParentWindow.PrintToDebug_NewLine("DEVICE: <-NORMAL MODE");
            //    myParentWindow.UpdateDeviceModeStatusLabel("RUNNING", false);
            //}

            //else if (inCommand[0][0] == SETTINGS_EP_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");

            //    String EP0 = inCommand[1];
            //    if (EP0 == "0")
            //    {
            //        EP0 = "X";
            //    }
            //    String EP1 = inCommand[2];
            //    if (EP1 == "0")
            //    {
            //        EP1 = "X";
            //    }
            //    myParentWindow.SetEP_Address(EP0, EP1);
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_RETRY_DELAY_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRetryDelay(inCommand[1], inCommand[2]);
            //}

            //else if (inCommand[0][0] == SETTINGS_FIRMWARE_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.UpdateFirmwareLabel(inCommand[1]);
            //}

            //else if (inCommand[0][0] == SETTINGS_RAD_KEY_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_Password(inCommand[1]);

            //    if (mySavedDevice != null)
            //    {
            //        mySavedDevice.GetPassKey = inCommand[1];
            //    }
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_CHAN_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_Channel(inCommand[1]);

            //    if (mySavedDevice != null)
            //    {
            //        mySavedDevice.GetChannel = inCommand[1];
            //    }
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_PA_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_PA(inCommand[1]);
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_DATA_RATE_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_DataRate(inCommand[1]);
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_CRC_LENGTH_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_CRC_Length(inCommand[1]);
            //}

            //else if (inCommand[0][0] == SETTINGS_RADIO_PAYLOAD_SIZE_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetRadio_PayloadSize(inCommand[1]);
            //}

            //else if (inCommand[0][0] == SETTINGS_BYTES_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetSettingsBytes(inCommand[1], inCommand[2], inCommand[3]);
            //}

            //else if (inCommand[0][0] == EEPROM_AVAIL_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(".");
            //    myParentWindow.SetAvailableEEPROM(inCommand[1], inCommand[2]);
            //}

            //else if (inCommand[0][0] == ADD_SCRIPT_CHAR)
            //{
            //    if (inCommand[1] != "")
            //    {
            //        foreach (String curString in ScriptedCommandClass.SeparateParameters(inCommand[1]))
            //        {
            //            myParentWindow.PrintToDebug_Continue(".");
            //            myParentWindow.LoadCommand(curString);
            //        }
            //    }
            //}
            //else if (inCommand[0][0] == SETTINGS_CHAR)
            //{
            //    myParentWindow.PrintToDebug_Continue(" [FINISHED]");
            //}
            //else
            //{
            //    String fullCommand = "";
            //    foreach (String curString in inCommand)
            //    {
            //        fullCommand += curString + " ";
            //    }
            //    myParentWindow.PrintToDebug_NewLine("DEVICE <-String = " + fullCommand);
            //}

            ProcessCommandDelegate?.Invoke(deviceCommand.Pack());
        }

        public void SendNextQueuedCommand()
        {
            if (_commandQueue.Count > 0)
            {
                SerialPort?.Write(_commandQueue.First());
                _commandQueue.Remove(_commandQueue.First());
                Thread.Sleep(20);
            }
        }

        public void ProcessDataBuffer()
        {
            if (_dataBuffer.Count > 0)
            {
                try
                {
                    var curMessage = "";

                    int ContentsStartIndex = _dataBuffer.IndexOf((byte)CMD_START_BYTE);
                    int ContentsEndIndex = _dataBuffer.IndexOf((byte)CMD_END_BYTE);

                    for (int i = ContentsStartIndex + 1; i <= ContentsEndIndex - 1; i++)
                    {
                        curMessage += (char)_dataBuffer[i];
                    }

                    for (int k = 0; k <= ContentsEndIndex; k++)
                    {
                        _dataBuffer.RemoveAt(0);
                    }

                    var CommandStrings = curMessage.Split(SEPARATOR_CMD_SEGMENT).ToList();
                    ProcessCompleteMessage(CommandStrings);
                }
                catch
                {

                }
            }
        }

        public void Connect()
        {
            try
            {
                if (SerialPort is not null)
                {
                    if (!SerialPort.IsOpen)
                    {
                        SerialPort.Open();
                    }

                    if (readThread is null)
                    {
                        if (!AttemptingToConnect)
                        {
                            AttemptingToConnect = true;
                        }

                        // Start the Separate Reading Thread
                        readThread = new Thread(() => Read());
                        readThread.Start();
                    }

                    if (!IsConnected)
                    {
                        // Send the Program Mode Command to Serial Device
                        SendCommand(PROGRAM_MODE_CHAR);
                    }
                }
            }
            catch
            {
                IsConnected = false;
                AttemptingToConnect = true;
                IsPaused = false;
                SerialPort?.Close();
            }
        }
    }
}