namespace Framework.Push
{
    public class HeartBeatReturnPackage : BasePackage
    {
        public HeartBeatReturnPackage()
        {
            PackageType = 0x01;
        }

        public int SsoUid { get; set; }

        public override byte[] GetPackageBytes()
        {
            var package = new UdpPackage();
            package.WriteByte(PackageType);
            package.WriteInt32(SsoUid);
            return package.GetAllBytes();
        }

        public override void ReadFromPackageBytes(byte[] bytearr)
        {
            var package = new UdpPackage(bytearr);
            PackageType = package.ReadByte();
            SsoUid = package.ReadInt32();
        }
    }
}