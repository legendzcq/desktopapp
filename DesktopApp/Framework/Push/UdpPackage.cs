using System;
using System.Text;

namespace Framework.Push
{

    /// <summary>
    /// udp包类型
    /// </summary>
    public class UdpPackage
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private byte[] _arr = new byte[128];

        /// <summary>
        /// 当前读写位置
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// 当前包的读写模式
        /// </summary>
        private PackageReadWriteType _type;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UdpPackage()
        {
            _type = PackageReadWriteType.Write;
        }

        public UdpPackage(byte[] arr)
        {
            _type = PackageReadWriteType.Read;
            InitArray(arr);
        }

        /// <summary>
        /// 检查缓存长度是否满足条件，如果不满足，重定义缓存长度
        /// </summary>
        /// <param name="lenth"></param>
        private void EnsureLength(int lenth)
        {
            if (_type == PackageReadWriteType.Read)
            {
                throw new PackageTypeException();
            }
            if (_currentPosition + lenth > _arr.Length)
            {
                if (_arr.Length > short.MaxValue)
                {
                    throw new ArgumentOutOfRangeException();
                }
                var by = new byte[_arr.Length + lenth];
                Buffer.BlockCopy(_arr, 0, by, 0, _arr.Length);
                _arr = by;
            }
        }

        /// <summary>
        /// 检查缓存长度
        /// </summary>
        /// <param name="len"></param>
        private void CheckLength(int len)
        {
            if (_type == PackageReadWriteType.Write)
            {
                throw new PackageTypeException();
            }
            if (_currentPosition + len > _arr.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 写入字节值
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            EnsureLength(1);
            _arr[_currentPosition] = value;
            _currentPosition++;
        }

        /// <summary>
        /// 写入Int16
        /// </summary>
        /// <param name="value"></param>
        public unsafe void WriteInt16(short value)
        {
            EnsureLength(2);
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                *(short*)ptr = value;
            }
            _currentPosition += 2;
        }

        /// <summary>
        /// 写入Int32值
        /// </summary>
        /// <param name="value"></param>
        public unsafe void WriteInt32(int value)
        {
            EnsureLength(4);
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                *(int*)ptr = value;
            }
            _currentPosition += 4;
        }

        /// <summary>
        /// 写入Int64值
        /// </summary>
        /// <param name="value"></param>
        public unsafe void WriteInt64(long value)
        {
            EnsureLength(8);
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                *(long*)ptr = value;
            }
            _currentPosition += 8;
        }

        /// <summary>
        /// 写入字符串值
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteInt16(0);
            }
            else
            {
                var by = Encoding.UTF8.GetBytes(value);
                WriteInt16((short)by.Length);
                EnsureLength(by.Length);
                Buffer.BlockCopy(by, 0, _arr, _currentPosition, by.Length);
                _currentPosition += by.Length;
            }
        }

        /// <summary>
        /// 获取UDP包内容
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllBytes()
        {
            var by = new byte[_currentPosition + 4];
            if (_currentPosition > 0)
            {
                Buffer.BlockCopy(_arr, 0, by, 0, _currentPosition);
            }
            var checkSum = GetCheckSum(by, 0, _currentPosition);
            Buffer.BlockCopy(checkSum, 0, by, _currentPosition, checkSum.Length);
            return by;
        }

        /// <summary>
        /// 获取校验值
        /// </summary>
        /// <returns></returns>
        private byte[] GetCheckSum(byte[] arr, int index, int len)
        {
            //todo 计算校验值
            return new byte[4];
        }

        /// <summary>
        /// 读取时初始化Udp包
        /// </summary>
        /// <param name="arr"></param>
        private void InitArray(byte[] arr)
        {
            if (arr == null) throw new ArgumentNullException("arr");
            if (arr.Length < 4) throw new ArgumentOutOfRangeException();
            _arr = new byte[arr.Length - 4];
            //todo 检查校验值
            Buffer.BlockCopy(arr, 0, _arr, 0, arr.Length - 4);
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            CheckLength(1);
            return _arr[_currentPosition++];
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <returns></returns>
        public unsafe short ReadInt16()
        {
            CheckLength(2);
            short result;
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                result = *(short*)ptr;
            }
            _currentPosition += 2;
            return result;
        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <returns></returns>
        public unsafe int ReadInt32()
        {
            CheckLength(4);
            int result;
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                result = *(int*)ptr;
            }
            _currentPosition += 4;
            return result;
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <returns></returns>
        public unsafe long ReadInt64()
        {
            CheckLength(8);
            long result;
            fixed (byte* ptr = &_arr[_currentPosition])
            {
                result = *(long*)ptr;
            }
            _currentPosition += 8;
            return result;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            var len = ReadInt16();
            if (len == 0) return string.Empty;
            CheckLength(len);
            var result = Encoding.UTF8.GetString(_arr, _currentPosition, len);
            _currentPosition += len;
            return result;
        }
    }

    /// <summary>
    /// UDP包读写类型异常
    /// </summary>
    public class PackageTypeException : Exception
    { }

    /// <summary>
    /// UDP包读写类型
    /// </summary>
    public enum PackageReadWriteType
    {
        Read, Write
    }
}
