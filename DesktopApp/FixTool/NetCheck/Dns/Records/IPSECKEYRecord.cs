#region
// 
// Bdev.Net.Dns IPSECKEY Resource record implementation by Thor Erik Lie <thorerik.lie@gmail.com>
#endregion

using System;

namespace NetCheck.Dns.Records
{
	/// <summary>
	/// IPSECKEY Resource Record (RR) (RFC4025)
	/// </summary>
	public class IPSECKEYRecord : RecordBase
	{
		
		private int _precedence;
		
		private int _gatewayType;
		
		private int _algorthm;
		
		private string _gateway;
		
		private string _publicKey;
		
		
		
		public int Precedence { get { return _precedence; } }
		
		public int GatewayType { get { return _gatewayType; } }
		
		public int Algorithm { get { return _algorthm; } }
		
		public string Gateway { get { return _gateway; } }
		
		public string PublicKey { get { return _publicKey; } }
		
		/// <summary>
		/// Constructs an IPSECKEY record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
		/// <param name="recordLength">Length of the record</param>
		internal IPSECKEYRecord(Pointer pointer, int recordLength)
		{			
			
			_precedence = pointer.ReadByte();
			
			_gatewayType = pointer.ReadByte();
			
			_algorthm = pointer.ReadByte();
			
			switch(_gatewayType)
			{
				case 0:
					break;
				case 1:
					_gateway = String.Format("{0}",pointer.ReadBytes(4));
					break;
				case 2:
					_gateway = String.Format("{0}",pointer.ReadBytes(16));
					break;
				case 3:
					_gateway = pointer.ReadDomain();
					break;
			}
			
			_publicKey = pointer.ReadString();
		}
		
		
		public override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4}", 
									_precedence, 
									_gatewayType, 
									_algorthm,
									_gateway,
									_publicKey);
		}
	}
}
