namespace Digigrade.Otto.Comms.Commands
{
    public class DeviceCommand
    {
        /// <summary>
        /// Unit-separator char in the ASCII table.
        /// </summary>
        private const char SepChar = (char)1F;

        /// <summary>
        /// Delegate method.
        /// </summary>
        /// <param name="payload"></param>
        public delegate bool ProcessCommandDelegate(string payload);

        /// <summary>
        /// The header used to identify the type of command.
        /// </summary>
        public string CommandHeader { get; set; } = "";

        /// <summary>
        /// The command's message.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Joins a list of strings into a message.
        /// </summary>
        /// <param name="fields"></param>
        public static string JoinStrings(string[] strings)
        {
            return string.Join(SepChar, strings);
        }

        /// <summary>
        /// Separates message into a list of string.
        /// </summary>
        /// <param name="message"></param>
        public static string[] SeparateStrings(string message)
        {
            return message?.Split(SepChar) ?? Array.Empty<string>();
        }

        public static DeviceCommand Unpack(string serializedPayload)
        {
            var returnValue = new DeviceCommand();

            var strings = SeparateStrings(serializedPayload);

            if (strings?.Length > 0)
            {
                returnValue.CommandHeader = strings[0];
            }

            if (strings?.Length > 1)
            {
                returnValue.Message = strings[1];
            }

            return returnValue;
        }

        public string Pack()
        {
            var returnValue = "";

            if (!string.IsNullOrWhiteSpace(CommandHeader) && !string.IsNullOrWhiteSpace(Message))
            {
                returnValue = JoinStrings(new string[] { CommandHeader, Message });
            }
            else if (!string.IsNullOrWhiteSpace(CommandHeader))
            {
                returnValue = CommandHeader;
            }

            return returnValue;
        }
    }
}