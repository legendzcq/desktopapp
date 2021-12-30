namespace Framework.Push
{
	public class HeartBeatPackage : BasePackage
	{
		public HeartBeatPackage()
		{
			PackageType = 0x00;

			AppId = Remote.Interface.AppId;
		}

		public int SsoUid { get; set; }

		public short AppId { get; private set; }

		public string CourseList { get; set; }

		public override byte[] GetPackageBytes()
		{
			var package = new UdpPackage();
			package.WriteByte(PackageType);
			package.WriteInt32(SsoUid);
			package.WriteInt16(AppId);
			package.WriteString(CourseList);
			return package.GetAllBytes();
		}

		public override void ReadFromPackageBytes(byte[] bytearr)
		{
			var package = new UdpPackage(bytearr);
			PackageType = package.ReadByte();
			SsoUid = package.ReadInt32();
			AppId = package.ReadInt16();
			CourseList = package.ReadString();
		}
	}
}
