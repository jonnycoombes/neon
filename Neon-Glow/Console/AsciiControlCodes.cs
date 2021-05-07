/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     Static class that just contains the most commonly used ASCII codes (or low Unicode ones)
    /// </summary>
    public static class AsciiControlCodes
    {
        /// <summary>
        ///     Null
        /// </summary>
        public static readonly char NUL = '\x00';

        /// <summary>
        ///     Start of Heading
        /// </summary>
        public static readonly char SOH = '\x01';

        /// <summary>
        ///     Start of Text
        /// </summary>
        public static readonly char STX = '\x02';

        /// <summary>
        ///     End of Text
        /// </summary>
        public static readonly char ETX = '\x03';

        /// <summary>
        ///     End of Transmission
        /// </summary>
        public static readonly char EOT = '\x04';

        /// <summary>
        ///     Enquiry
        /// </summary>
        public static readonly char ENQ = '\x05';

        /// <summary>
        ///     Acknowledge
        /// </summary>
        public static readonly char ACK = '\x06';

        /// <summary>
        ///     Bell
        /// </summary>
        public static readonly char BEL = '\x07';

        /// <summary>
        ///     Backspace
        /// </summary>
        public static readonly char BS = '\x08';

        /// <summary>
        ///     Tabulate
        /// </summary>
        public static readonly char TAB = '\x09';

        /// <summary>
        ///     Line Feed
        /// </summary>
        public static readonly char LF = '\x0a';

        /// <summary>
        ///     Vertical Tabulation
        /// </summary>
        public static readonly char VT = '\x0b';

        /// <summary>
        ///     Form Feed
        /// </summary>
        public static readonly char FF = '\x0c';

        /// <summary>
        ///     Carriage Return
        /// </summary>
        public static readonly char CR = '\x0d';

        /// <summary>
        ///     Shift Out
        /// </summary>
        public static readonly char SO = '\x0e';

        /// <summary>
        ///     Shift In
        /// </summary>
        public static readonly char SI = '\x0f';

        /// <summary>
        ///     Data Link Escape
        /// </summary>
        public static readonly char DLE = '\x10';

        /// <summary>
        ///     Device Control 1 (XON)
        /// </summary>
        public static readonly char DC1 = '\x11';

        /// <summary>
        ///     Device Control 2
        /// </summary>
        public static readonly char DC2 = '\x12';

        /// <summary>
        ///     Device Control 3 (XOFF)
        /// </summary>
        public static readonly char DC3 = '\x13';

        /// <summary>
        ///     Device Control 4
        /// </summary>
        public static readonly char DC4 = '\x14';

        /// <summary>
        ///     Negative Acknowledge
        /// </summary>
        public static readonly char NAK = '\x15';

        /// <summary>
        ///     Synchronous Idle
        /// </summary>
        public static readonly char SYN = '\x16';

        /// <summary>
        ///     End of Transmission Block
        /// </summary>
        public static readonly char ETB = '\x17';

        /// <summary>
        ///     Cancel
        /// </summary>
        public static readonly char CAN = '\x18';

        /// <summary>
        ///     End of Medium
        /// </summary>
        public static readonly char EM = '\x19';

        /// <summary>
        ///     Substitute
        /// </summary>
        public static readonly char SUB = '\x1a';

        /// <summary>
        ///     Escape
        /// </summary>
        public static readonly char ESC = '\x1b';

        /// <summary>
        ///     File Separator
        /// </summary>
        public static readonly char FS = '\x1c';

        /// <summary>
        ///     Group Separator
        /// </summary>
        public static readonly char GS = '\x1d';

        /// <summary>
        ///     Record Separator
        /// </summary>
        public static readonly char RS = '\x1e';

        /// <summary>
        ///     Unit Separator
        /// </summary>
        public static readonly char US = '\x1f';

        /// <summary>
        ///     Space
        /// </summary>
        public static readonly char SPC = '\x20';

        /// <summary>
        ///     Delete
        /// </summary>
        public static readonly char DEL = '\x7f';
    }
}