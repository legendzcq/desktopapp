#region
// 
// Bdev.Net.Dns SSHFP Resource record implementation by Thor Erik Lie <thorerik.lie@gmail.com>
#endregion

using System;

namespace NetCheck.Dns.Records
{
	/// <summary>
	/// SSHFP Resource Record (RR) (RFC4255)
	/// </summary>
	public class SSHFPRecord : RecordBase
	{
		
		private readonly int _algorithm;
		
		private readonly int _fpType;
		
		private readonly string _fingerprint;
		
		public int Algorithm { get { return _algorithm; } }
		
		public int FingerprintType { get { return _fpType; } }
		
		public string Fingerprint { get { return _fingerprint; } }
		
		
		/// <summary>
		/// Constructs an SSHFP record by reading bytes from a return message
		/// </summary>
		/// <param name="pointer">A logical pointer to the bytes holding the record</param>
		/// <param name="recordLength">Length of the record</param>
		internal SSHFPRecord(Pointer pointer, int recordLength)
		{			
			_algorithm = pointer.ReadByte();
			
			_fpType = pointer.ReadByte();
			
			_fingerprint = pointer.ReadString(recordLength - 2);
		}
		
		
		public override string ToString()
		{
			return String.Format("{0} {1} {2}", _algorithm, _fpType, _fingerprint);
		}
	}
}
