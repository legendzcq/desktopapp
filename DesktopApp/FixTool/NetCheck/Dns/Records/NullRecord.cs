#region
// NULL record implementation written by Tom Nolan <tom at tinyint dot com>
// Released under GPL V3
#endregion

using System;

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A NULL Resource Record (RR) (RFC1035 3.3.10)
    /// This resource record is marked as experimental.
    /// </summary
    class NULLRecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly byte[] _data;
        // expose the binary data to the world
        public byte[] Data { get { return _data; } }

        /// <summary>
        /// Constructs a NULL record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="lengt">Length of data</param>
        internal NULLRecord(Pointer pointer, int length)
        {
            _data = pointer.ReadBytes(length);
        }

        /// <summary>
        /// Returns a base64 encoded string of the data
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return Convert.ToBase64String(_data);
        }
    }
}
