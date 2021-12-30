namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A MD Resource Record (RR) (RFC 1035 3.3.4)
    /// </summary>
    class MDRecord : RecordBase
    {
        private readonly string _mailDestination;

        public string MailDestination { get { return _mailDestination; } }

        /// <summary>
        /// Construct a MD record by reading bytes from returned message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal MDRecord(Pointer pointer)
        {
            _mailDestination = pointer.ReadDomain();
        }

        public override string ToString()
        {
            return _mailDestination;
        }
    }
}
