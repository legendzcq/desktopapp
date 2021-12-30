#region
//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;

namespace NetCheck.Dns.Records
{
	/// <summary>
	/// Represents a Resource Record as detailed in RFC1035 4.1.3
	/// </summary>
	[Serializable]
	public class ResourceRecord
	{
		// private, constructor initialised fields
		private readonly string		_domain;
		private readonly DnsType	_dnsType;
		private readonly DnsClass	_dnsClass;
		private readonly int		_ttl;
		private readonly RecordBase	_record;

		// read only properties applicable for all records
		public string		Domain		{ get { return _domain;		}}
		public DnsType		Type		{ get { return _dnsType;	}}
		public DnsClass		Class		{ get { return _dnsClass;	}}
		public int			Ttl			{ get { return _ttl;		}}
		public RecordBase	Record		{ get { return _record;		}}

		/// <summary>
		/// Construct a resource record from a pointer to a byte array
		/// </summary>
		/// <param name="pointer">the position in the byte array of the record</param>
		internal ResourceRecord(Pointer pointer)
		{
			// extract the domain, question type, question class and Ttl
			_domain = pointer.ReadDomain();
			_dnsType = (DnsType)pointer.ReadShort();
			_dnsClass = (DnsClass)pointer.ReadShort();
			_ttl = pointer.ReadInt();

			// the next short is the record length, we only use it for unrecognised record types
			int recordLength = pointer.ReadShort();

			// and create the appropriate RDATA record based on the dnsType
			switch (_dnsType)
			{
                case DnsType.A:     	_record = new ARecord(pointer);                     	break;
				case DnsType.NS:		_record = new NSRecord(pointer);	                	break;
                case DnsType.MD:    	_record = new MDRecord(pointer);                    	break;
                case DnsType.MF:    	_record = new MFRecord(pointer);                    	break;
                case DnsType.DNAME: 	_record = new DNAMERecord(pointer);                 	break;
                case DnsType.CNAME: 	_record = new CNAMERecord(pointer);                 	break;
				case DnsType.SOA:		_record = new SoaRecord(pointer);	                	break;
                case DnsType.MB:    	_record = new MBRecord(pointer);                    	break;
                case DnsType.MG:    	_record = new MGRecord(pointer);                    	break;
                case DnsType.MR:    	_record = new MRRecord(pointer);                    	break;
                case DnsType.NULL:  	_record = new NULLRecord(pointer, recordLength);    	break;
                case DnsType.WKS:   	_record = new WKSRecord(pointer, recordLength);     	break;
                case DnsType.PTR:   	_record = new PTRRecord(pointer);                   	break;
                case DnsType.HINFO: 	_record = new HINFORecord(pointer, recordLength);   	break;
                case DnsType.MINFO: 	_record = new MINFORecord(pointer, recordLength);   	break;
				case DnsType.MX:		_record = new MXRecord(pointer);	                	break;
                case DnsType.TXT:   	_record = new TXTRecord(pointer, recordLength);     	break;
                case DnsType.AAAA:  	_record = new AAAARecord(pointer);                  	break;
                case DnsType.LOC:   	_record = new LOCRequest(pointer);                  	break;
                case DnsType.SRV:   	_record = new SRVRecord(pointer);                   	break;
				case DnsType.SSHFP: 	_record = new SSHFPRecord(pointer, recordLength);		break;
				case DnsType.IPSECKEY:	_record = new IPSECKEYRecord(pointer, recordLength); 	break;

				default:
				{
					// move the pointer over this unrecognised record
					pointer += recordLength;
					break;
				}
			}
		}
	}

	// Answers, Name Servers and Additional Records all share the same RR format
	[Serializable]
	public class Answer : ResourceRecord
	{
		internal Answer(Pointer pointer) : base(pointer) {}
	}

	[Serializable]
	public class NameServer : ResourceRecord
	{
		internal NameServer(Pointer pointer) : base(pointer) {}
	}

	[Serializable]
	public class AdditionalRecord : ResourceRecord
	{
		internal AdditionalRecord(Pointer pointer) : base(pointer) {}
	}
}