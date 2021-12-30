namespace Framework.Push
{
    public class PushedPackage : BasePackage
    {
        public PushedPackage()
        {
            PackageType = 0x02;
        }

        public byte MessageType { get; set; }

        public string MessageContent { get; set; }

        public override byte[] GetPackageBytes()
        {
            var package = new UdpPackage();
            package.WriteByte(PackageType);
            package.WriteByte(MessageType);
            package.WriteString(MessageContent);
            return package.GetAllBytes();
        }

        public override void ReadFromPackageBytes(byte[] bytearr)
        {
            var package = new UdpPackage(bytearr);
            PackageType = package.ReadByte();
            MessageType = package.ReadByte();
            MessageContent = package.ReadString();
        }
    }
}