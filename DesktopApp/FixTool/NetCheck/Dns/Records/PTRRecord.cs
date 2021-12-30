namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A PTR Resource Record (RR) (RFC1035 3.3.12)
    /// </summary>
    class PTRRecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly string _domainName;
        // expose this domain name address r/o to the world
		public string DomainName	{ get { return _domainName; }}
				
		/// <summary>
		/// Constructs a PTR record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
		internal PTRRecord(Pointer pointer)
		{
			_domainName = pointer.ReadDomain();
		}

		public override string ToString()
		{
			return _domainName;
		}
    }
}
