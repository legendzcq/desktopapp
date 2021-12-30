namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A MR Resource Record (RR) (RFC 1035 3.3.8)
    /// </summary>
    class MRRecord : RecordBase
    {
        private readonly string _mailbox;

        public string Mailbox { get { return _mailbox; } }

        /// <summary>
        /// Construct a MB record by reading bytes from returned message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal MRRecord(Pointer pointer)
        {
            _mailbox = pointer.ReadDomain();
        }

        public override string ToString()
        {
            return _mailbox;
        }
    }
}
