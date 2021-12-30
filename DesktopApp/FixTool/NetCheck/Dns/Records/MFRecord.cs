#region
// MF record implementation written by Tom Nolan <tom at tinyint dot com>
// Released under GPL V3
#endregion

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A MF Resource Record (RR) (RFC 1035 3.3.5)
    /// </summary>
    class MFRecord : RecordBase
    {
        private readonly string _mailForwarder;

        public string MailForwarder { get { return _mailForwarder; } }

        /// <summary>
        /// Construct a MF record by reading bytes from returned message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal MFRecord(Pointer pointer)
        {
            _mailForwarder = pointer.ReadDomain();
        }

        public override string ToString()
        {
            return _mailForwarder;
        }
    }
}
