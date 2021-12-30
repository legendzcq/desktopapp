namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A MINFO Resource Record (RR) (RFC1035 3.3.7)
    /// </summary>
    class MINFORecord : RecordBase
    {
        // the fields exposed outside the assembly
		private readonly string		_ownerMailbox;
		private readonly string		_errorMailbox;

		// expose this domain name address r/o to the world
		public string OwnerMailbox	{ get { return _ownerMailbox; }}
		public string ErrorMailbox	{ get { return _errorMailbox; }}
				
		/// <summary>
		/// Constructs a MINFO record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
		internal MINFORecord(Pointer pointer, int length)
		{
			/*_ownerMailbox = pointer.ReadStringS();
			_errorMailbox = pointer.ReadStringS();*/
            _ownerMailbox = "";
            _errorMailbox = pointer.ReadString(length);
		}

		public override string ToString()
		{
			return string.Format("Responsible mailbox: {0}, Error mailbox: {1}", _ownerMailbox, _errorMailbox);
		}
    }
}
