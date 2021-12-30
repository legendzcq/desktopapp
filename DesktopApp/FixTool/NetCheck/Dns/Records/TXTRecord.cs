#region
// TXT record implementation based on the NS record implementation by Rob Philpott, written by Thor Erik Lie <thorerik.lie [at] gmail.com>
// Released under GPL V3
#endregion

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A TXT Resource Record (RR) (RFC1035 3.3.14)
    /// </summary
    class TXTRecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly string _text;
        // expose this domain name address r/o to the world
		public string DomainName	{ get { return _text; }}
				
		/// <summary>
		/// Constructs a TXT record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="lengt">Lengt of inputted data</param>
		internal TXTRecord(Pointer pointer, int length)
		{
            pointer.ReadByte();
            _text = pointer.ReadString(length - 1);
		}

		public override string ToString()
		{
			return _text;
		}
    }
}
