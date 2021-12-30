#region
//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

namespace NetCheck.Dns
{
	/// <summary>
	/// The DNS TYPE (RFC1035 3.2.2)
	/// </summary>
	public enum DnsType
	{
        A       	= 1,            // Host address
        NS      	= 2,            // Authoritative name server
        MD      	= 3,            // Mail destination (Obsolete - use MX)
        MF      	= 4,            // Mail forwarder (Obsolete - use MX)
        CNAME   	= 5,            // Canonical name for an alias
        SOA     	= 6,            // Start of a zone of authority
        MB      	= 7,            // Mailbox domain name (EXPERIMENTAL)
        MG      	= 8,            // Mail group member (EXPERIMENTAL)
        MR      	= 9,            // Mail rename domain (EXPERIMENTAL)
        NULL    	= 10,           // Null RR (EXPERIMENTAL)
        WKS     	= 11,           // Well known service
        PTR     	= 12,           // Domain name pointer
        HINFO   	= 13,           // Host information
        MINFO   	= 14,           // Mail box/list information
        MX      	= 15,           // Mail exchange
        TXT     	= 16,           // Text strings

        AAAA    	= 28,           // IPv6 host address                    [RFC3596]
        LOC     	= 29,           // Location info                        [RFC1876]
        SRV     	= 33,
        DNAME   	= 39,           // Non-Terminal DNS Name Redirection    [RFC2672]
		SSHFP   	= 44,			// SSH Fingerprint 						[RFC4255]
		IPSECKEY 	= 45			// IPSECKEY								[RFC4025]
	}

    /// <summary>
    /// The DNS QTYPE (RFC1035 3.2.3)
    /// </summary>
    public enum DnsQType
    {
        A = DnsType.A,
        NS = DnsType.NS,
        MD = DnsType.MD,
        MF = DnsType.MF,
        CNAME = DnsType.CNAME,
        SOA = DnsType.SOA,
        MB = DnsType.MB,
        MG = DnsType.MG,
        MR = DnsType.MR,
        NULL = DnsType.NULL,
        WKS = DnsType.WKS,
        PTR = DnsType.PTR,
        HINFO = DnsType.HINFO,
        MINFO = DnsType.MINFO,
        MX = DnsType.MX,
        TXT = DnsType.TXT,

        DNAME = DnsType.DNAME,
        AAAA = DnsType.AAAA,
        LOC = DnsType.LOC,
        SRV = DnsType.SRV,
		SSHFP = DnsType.SSHFP,
		IPSECKEY = DnsType.IPSECKEY,

        ANY = 255
    }

	/// <summary>
	/// The CLASS fields (RFC1035 3.2.4/5)
	/// </summary>
	public enum DnsClass
	{
        IN  = 1,                // The Internet
        CS  = 2,                // The CSNET class (Obsolete - used only for examples in some obsolete RFCs)
        CH  = 3,                // The CHAOS class
        HS  = 4,                // Hesiod [Dyer 87]
        ANY = 255               // Any class
	}

	/// <summary>
	/// (RFC1035 4.1.1) These are the return codes the server can send back
	/// </summary>
	public enum ResponseCode
	{
		NOERROR     = 0,        // No error                             [RFC1035]
		FORMERR     = 1,        // Format Error                         [RFC1035]
		SERVFAIL    = 2,        // Internal server failure              [RFC1035]
		NXDOMAIN    = 3,        // Name doesn't exist                   [RFC1035]
		NOTIMP      = 4,        // Not implemented                      [RFC1035]
		REFUSED     = 5,        // Server refused                       [RFC1035]
		YXDOMAIN    = 6,        // YX Name doesn't exist but should     [RFC2136]
        YXRESET     = 7,        // RRset exists but shouldn't           [RFC2136]
        NXRRSET     = 8,        // RRset doesn't exist but should       [RFC2136]
        NOTAUTH     = 9,        // Not authoritative for zone           [RFC2136]
        NOTZONE     = 10,       // Not within zone                      [RFC2136]
        RESERVED11  = 11,
        RESERVED12  = 12,
        RESERVED13  = 13,
        RESERVED14  = 14,
        RESERVED15  = 15,
        BADVERS     = 16,       // Version level not implemented        [RFC2671]

	}

	/// <summary>
	/// (RFC1035 4.1.1) These are the Query Types which apply to all questions in a request
	/// </summary>
	public enum Opcode
	{
		QUERY       = 0,        // Standard query                       [RFC1035]
        IQUERY      = 1,        // Inverse query                        [RFC1035]
        STATUS      = 2,        // Server status                        [RFC1035]
		RESERVED3   = 3,
		NOTIFY      = 4,        // Master to slave notification         [RFC1996]
		UPDATE      = 5,        // Dynamic updates                      [RFC2136]
		RESERVED6   = 6,
		RESERVED7   = 7,
		RESERVED8   = 8,
		RESERVED9   = 9,
		RESERVED10  = 10,
		RESERVED11  = 11,
		RESERVED12  = 12,
		RESERVED13  = 13,
		RESERVED14  = 14,
		RESERVED15  = 15,
	}
}
