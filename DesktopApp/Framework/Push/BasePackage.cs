namespace Framework.Push
{
    public abstract class BasePackage
    {
        /// <summary>
        /// 包类型
        /// </summary>
        public byte PackageType { get; protected set; }

        /// <summary>
        /// 获取UDP包数据
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetPackageBytes();

        /// <summary>
        /// 从UDP包中解码数据
        /// </summary>
        /// <param name="bytearr"></param>
        public abstract void ReadFromPackageBytes(byte[] bytearr);
    }
}