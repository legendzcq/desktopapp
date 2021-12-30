#region
//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System.Net;

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// AAAA Resource Record (RR) (RFC 3596 2.1, 2.2, 2.3)
    /// </summary>
    public class AAAARecord : RecordBase
    {
        // An AAAA records consists simply of an IP address
        internal IPAddress _ipAddress;

        // expose this IP address r/o to the world
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
        }

        /// <summary>
        /// Constructs an AAAA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal AAAARecord(Pointer pointer)
        {
            byte[] b = pointer.ReadBytes(16);
            _ipAddress = new IPAddress(b);
        }

        public override string ToString()
        {
            return _ipAddress.ToString();
        }
    }
}
