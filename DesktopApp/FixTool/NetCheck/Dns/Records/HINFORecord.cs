namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A Host Info Resource Record(RR) RFC 1035 3.3.11
    /// </summary>
    class HINFORecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly string _cpu;
        private readonly string _os;

        public string Cpu { get { return _cpu; } }
        public string Os { get { return _os; } }

        /// <summary>
        /// Constructs a HINFO record by reading bytes from return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="length">Length of record</param>
        internal HINFORecord(Pointer pointer, int length)
        {
            /*_cpu = pointer.ReadStringS();
            _os = pointer.ReadStringS();*/
            _cpu = "";
            _os = pointer.ReadString(length);
        }

        public override string ToString()
        {
            return string.Format("CPU: {0}, OS: {1}", _cpu, _os);
        }
    }
}
