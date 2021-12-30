namespace NetCheck.Dns.Records
{
    class CNAMERecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly string _domainName;
        // expose this domain name address r/o to the world
		private string DomainName	{ get { return _domainName; }}
				
		/// <summary>
		/// Constructs a CNAME record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal CNAMERecord(Pointer pointer)
		{
            _domainName = pointer.ReadDomain();            
		}

		public override string ToString()
		{
			return _domainName;
		}

    }
}
