namespace Framework.Push
{
	public class QueryPackage : BasePackage
	{
		public QueryPackage()
		{
			PackageType = 0x03;
			AppId = Remote.Interface.AppId;
		}

		public int SsoUid { get; set; }

		public string QueryDateTime { get; set; }

		public short AppId { get; set; }

		public override byte[] GetPackageBytes()
		{
			var package = new UdpPackage();
			package.WriteByte(PackageType);
			package.WriteInt32(SsoUid);
			package.WriteString(QueryDateTime);
			package.WriteInt16(AppId);
			return package.GetAllBytes();
		}

		public override void ReadFromPackageBytes(byte[] bytearr)
		{
			var package = new UdpPackage(bytearr);
			PackageType = package.ReadByte();
			SsoUid = package.ReadInt32();
			QueryDateTime = package.ReadString();
			AppId = package.ReadInt16();
		}
	}
}