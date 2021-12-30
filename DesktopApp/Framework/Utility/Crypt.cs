using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using Framework.Local;

using Microsoft.Win32;


namespace Framework.Utility
{
    public static class Crypt
    {

        //[DllImport("cm.dll")]
        //private static extern bool SetKey(byte[] key);

        /// <summary>
        /// 本地的主Key
        /// </summary>
        private static readonly byte[] MainKey = { 0x45, 0x34, 0x48, 0x44, 0x39, 0x68, 0x34, 0x44 };

        /// <summary>
        /// 初始化向量
        /// </summary>
        /// <remarks>fYfhHeDm</remarks>
        private static readonly byte[] MainIv = { 0x66, 0x59, 0x66, 0x68, 0x48, 0x65, 0x44, 0x6D };

        /// <summary>
        /// 新文件头
        /// </summary>
        private static readonly byte[] FilePrefix = { 0x43, 0x44, 0x45, 0x4c };

        private static readonly byte[] AesKey = { 0x35, 0xF1, 0x1C, 0x7B, 0xFE, 0x26, 0x7A, 0x12, 0x24, 0x33, 0x04, 0x43, 0xCD, 0xB7, 0x9A, 0x4F, 0x39, 0x2F, 0x5F, 0x6F, 0xD0, 0x22, 0x09, 0x37, 0xF9, 0x86, 0xF7, 0x62, 0xC9, 0xF7, 0xCB, 0x4F };

        private static readonly byte[] AesIv = { 0x07, 0xB0, 0x79, 0x5B, 0x4C, 0x03, 0x58, 0x2A, 0x03, 0xCD, 0xBD, 0x18, 0x8F, 0x76, 0x22, 0x5F };
        private static readonly byte[] DesKey =
        {
            0x63, 0x64, 0x65, 0x6c, 0x30, 0x39, 0x32, 0x37
        };
        public static byte[] MachineKey;

        private static readonly Random Rnd = new Random();

        private static readonly string AesKey_pre = "ud823sui#()!ajo93#@<";
        private static readonly string AesKey_suf = "_tui^(@=dijf;*[etr1]";

        static Crypt() => GetMachineKey();

        #region 获取设备加密密钥相关
        private static void GetMachineKey()
        {
            var da = new Baseinfo();
            var key = da.GetSecurityKey();
            var regKey = GetRegistrySavedKey();
            if (key == null || key.Length == 0)
            {
                MachineKey = regKey ?? GenKey(key == null);
                if (key != null)
                {
                    da.InsertKey(MachineKey);
                }
            }
            else
            {
                MachineKey = key;
            }
            SetRegistryKey(regKey ?? MachineKey);
            SetKey(MachineKey);
        }

        /// <summary>
        /// 获取注册表中添加的硬件信息
        /// </summary>
        /// <returns></returns>
        private static byte[] GetRegistrySavedKey()
        {
            RegistryKey rkey = Registry.LocalMachine.CreateSubKey("Software\\CDEL");
            if (rkey == null) return null;
            var obj = rkey.GetValue("Machineinfo", null);
            if (obj == null) return null;
            try
            {
                var skey = AesDecrypt((byte[])obj);
                var vkey = new byte[16];
                Buffer.BlockCopy(skey, 16, vkey, 0, 16);
                return vkey;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置注册表中添加的硬件信息
        /// </summary>
        /// <param name="key"></param>
        private static void SetRegistryKey(byte[] key)
        {
            var skey = new byte[16];
            Rnd.NextBytes(skey);
            key = skey.Concat(key).ToArray();
            RegistryKey rkey = Registry.LocalMachine.CreateSubKey("Software\\CDEL");
            if (rkey != null)
            {
                rkey.SetValue("Machineinfo", AesEncrypt(key));
            }
        }

        private delegate bool SetKeyDelegate(byte[] key);
        private delegate bool GetKeyDelegate(byte[] key);

        private static void SetKey(byte[] key)
        {
            var commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            var commonPathx86 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);

            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var cmPath = commonPath + "\\cdel\\cm.dll";
            if (!File.Exists(cmPath))
            {
                cmPath = commonPathx86 + "\\cdel\\cm.dll";
                if (!File.Exists(cmPath))
                {
                    cmPath = appPath + "cm.dll";
                    if (!File.Exists(cmPath))
                    {
                        Log.RecordLog(@"系统文件检测失败cm.dll，请重新安装");
                        return;
                    }
                }
            }

            IntPtr cmLib = NativeMethod.LoadLibrary(cmPath);

            IntPtr api = NativeMethod.GetProcAddress(cmLib, "SetKey");
            var setKey = (SetKeyDelegate)Marshal.GetDelegateForFunctionPointer(api, typeof(SetKeyDelegate));
            var value = setKey(key);

            api = NativeMethod.GetProcAddress(cmLib, "GetKey");
            var getKey = (GetKeyDelegate)Marshal.GetDelegateForFunctionPointer(api, typeof(GetKeyDelegate));
            var buffer = new byte[16];
            getKey(buffer);
            Trace.WriteLine("GetKeyResult:" + BitConverter.ToString(buffer).Replace("-", ""));
        }

        /// <summary>
        /// 获取机器Key的字符串
        /// </summary>
        /// <returns></returns>
        internal static string GetMachineKeyString() => BitConverter.ToString(MachineKey).Replace("-", string.Empty);

        /// <summary>
        /// 获取加密Key
        /// </summary>
        /// <param name="forOld"></param>
        /// <returns></returns>
        private static byte[] GenKey(bool forOld)
        {
            //Trace.WriteLine("ComputeSID");
            var sb = new StringBuilder();
            sb.Append("CDEL");
            ManagementClass mc;
            ManagementObjectCollection moc;
            var matchcnt = 0;
            try
            {
                mc = new ManagementClass("Win32_BIOS");
                moc = mc.GetInstances();
                foreach (var str in from ManagementObject mo in moc select mo.GetPropertyValue("SerialNumber") as string)
                {
                    sb.Append(str);
                    matchcnt++;
                }
            }
            catch
            {
                Trace.WriteLine("Win32_BIOS Error");
            }
            try
            {
                mc = new ManagementClass("Win32_BaseBoard");
                moc = mc.GetInstances();
                foreach (var str in from ManagementObject mo in moc select mo.GetPropertyValue("SerialNumber") as string)
                {
                    sb.Append(str);
                    matchcnt++;
                }
            }
            catch
            {
                Trace.WriteLine("Win32_BaseBoard Error");
            }
            if (!forOld)
            {
                try
                {
                    mc = new ManagementClass("Win32_Processor");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("ProcessorId") as string;
                        sb.Append(str);
                        matchcnt++;
                    }
                }
                catch
                {
                    Trace.WriteLine("Win32_Processor Error");
                }
                try
                {
                    mc = new ManagementClass("Win32_DiskDrive");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("SerialNumber") as string;
                        sb.Append(str);
                        matchcnt++;
                    }
                }
                catch
                {
                    Trace.WriteLine("Win32_DiskDrive Error");
                }
                try
                {
                    mc = new ManagementClass("Win32_PhysicalMemory");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("SerialNumber") as string;
                        sb.Append(str);
                        matchcnt++;
                    }
                }
                catch
                {
                    Trace.WriteLine("Win32_PhysicalMemory Error");
                }
            }
            if (matchcnt <= 2)
            {
                //如果获取的设备数量小于两种，那么则增加一个随机码来标识该计算机
                sb.AppendLine(Guid.NewGuid().ToString());
            }
            var info = sb.ToString();
            var buffer = Encoding.Default.GetBytes(info);
            Trace.WriteLine(info);
            var md5 = new Md5Impl();
            buffer = md5.ComputeHash(buffer);
            Trace.WriteLine("ComputeSID OK");
            return buffer;
        }

        #endregion

        #region 视频及相关文件处理相关
        /// <summary>
        /// 处理视频文件
        /// </summary>
        /// <param name="videoHeader"></param>
        /// <param name="fileBody"></param>
        /// <param name="fileSave"></param>
        /// <returns></returns>
        internal static bool TransformVideo(byte[] videoHeader, string fileBody, string fileSave)
        {
            var rnd = new Random();
            var cryptLenKb = rnd.Next(960) + 64;
            var savePath = Path.GetDirectoryName(fileSave);
            if (savePath != null && !Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //加上对文件已存在的处理，如果能删掉的就删除，删除不掉的，改名后，重启的时候删除
            SystemInfo.ForceDeleteFile(fileSave);

            FileStream saveStream = File.Create(fileSave);
            FileStream bodyStream = File.OpenRead(fileBody);
            var finfo = new FileInfo(fileBody);
            var sizeB = BitConverter.GetBytes(finfo.Length);
            var aes = new AesCryptoServiceProvider
            {
                Padding = PaddingMode.None,
                KeySize = 128,
                Key = MachineKey
            };
            var iv = aes.IV;
            bodyStream.Seek(videoHeader.Length, SeekOrigin.Current);
            saveStream.Write(FilePrefix, 0, 4);
            saveStream.Write(BitConverter.GetBytes((cryptLenKb << 16) | rnd.Next(65535)), 0, 4);
            saveStream.Write(iv, 0, 16);
            saveStream.Write(sizeB, 0, sizeB.Length);
            var buffer = new byte[1024];
            using (var cs = new CryptoStream(saveStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(videoHeader, 0, videoHeader.Length);
                var offset = videoHeader.Length;
                int byRead;
                while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
                {
                    cs.Write(buffer, 0, byRead);
                    offset += byRead;
                    if (offset >= 1024 * cryptLenKb) break;
                }
                cs.Flush();
                saveStream.Flush();
                while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
                {
                    saveStream.Write(buffer, 0, byRead);
                }
            }
            saveStream.Close();
            bodyStream.Close();
            return true;
        }

        /// <summary>
        /// 导入文件中的视频转换
        /// </summary>
        /// <param name="fileHead"></param>
        /// <param name="fileBody"></param>
        /// <param name="fileSave"></param>
        /// <param name="headKey"></param>
        /// <returns></returns>
        public static bool TransformVideo(string fileHead, string fileBody, string fileSave, byte[] headKey)
        {
            var rnd = new Random();
            var cryptLenKb = rnd.Next(960) + 64;
            var headByte = Convert.FromBase64String(File.ReadAllText(fileHead));
            headByte = TransformVideoHead(headByte, headKey);
            FileStream bodyStream = File.OpenRead(fileBody);
            var savePath = Path.GetDirectoryName(fileSave);
            if (savePath != null && !Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            FileStream saveStream = File.Create(fileSave);
            var finfo = new FileInfo(fileBody);
            var sizeB = BitConverter.GetBytes(finfo.Length);
            var aes = new AesCryptoServiceProvider
            {
                Padding = PaddingMode.None,
                KeySize = 128,
                Key = MachineKey
            };
            var iv = aes.IV;
            bodyStream.Seek(1024, SeekOrigin.Current);
            saveStream.Write(FilePrefix, 0, 4);
            saveStream.Write(BitConverter.GetBytes((cryptLenKb << 16) | rnd.Next(65535)), 0, 4);
            saveStream.Write(iv, 0, 16);
            saveStream.Write(sizeB, 0, sizeB.Length);
            var buffer = new byte[1024];
            using (var cs = new CryptoStream(saveStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(headByte, 0, 1024);
                var offset = 1024;
                int byRead;
                while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
                {
                    cs.Write(buffer, 0, byRead);
                    offset += byRead;
                    if (offset >= 1024 * cryptLenKb) break;
                }
                cs.Flush();
                saveStream.Flush();
                while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
                {
                    saveStream.Write(buffer, 0, byRead);
                }
            }
            bodyStream.Close();
            return true;
        }

        /// <summary>
        /// 解密导入视频的文件头
        /// </summary>
        /// <param name="head"></param>
        /// <param name="headKey"></param>
        /// <returns></returns>
        private static byte[] TransformVideoHead(byte[] head, byte[] headKey)
        {
            try
            {
                using (var des = new DESCryptoServiceProvider())
                {
                    des.Key = headKey;
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.None;
                    des.IV = MainIv;
                    using (var ms = new MemoryStream(head))
                    {
                        using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var mso = new MemoryStream())
                            {
                                var buffer = new byte[1024];
                                int byteRead;
                                while ((byteRead = cs.Read(buffer, 0, 1024)) > 0)
                                {
                                    mso.Write(buffer, 0, byteRead);
                                }
                                return Convert.FromBase64String(Encoding.UTF8.GetString(mso.ToArray()));
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("解密文件头失败");
            }
        }

        /*
                /// <summary>
                /// 加密处理本地文件
                /// </summary>
                /// <param name="fileName"></param>
                /// <param name="fileSave"></param>
                /// <returns></returns>
                public static bool TransformVideo(string fileName, string fileSave)
                {
                    var rnd = new Random();
                    var cryptLenKB = rnd.Next(960) + 64;
                    var finfo = new FileInfo(fileName);
                    if (finfo.Length < 1024 * 1024)
                    {
                        throw new Exception("文件太小");
                    }
                    var sizeB = BitConverter.GetBytes(finfo.Length);
                    var aes = new AesCryptoServiceProvider
                    {
                        Padding = PaddingMode.None,
                        KeySize = 128,
                        Key = MachineKey
                    };
                    var iv = aes.IV;
                    using (var outStream = File.Create(fileSave))
                    {
                        using (var inStream = File.OpenRead(fileName))
                        {
                            using (var cs = new CryptoStream(outStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                outStream.Write(FilePrefix, 0, 4);
                                outStream.Write(BitConverter.GetBytes((cryptLenKB << 16) | rnd.Next(65535)), 0, 4);
                                outStream.Write(iv, 0, 16);
                                outStream.Write(sizeB, 0, sizeB.Length);
                                outStream.Flush();

                                var buffer = new byte[1024];
                                var offset = 0;

                                int byRead;
                                while ((byRead = inStream.Read(buffer, 0, 1024)) > 0)
                                {
                                    cs.Write(buffer, 0, byRead);
                                    offset += byRead;
                                    if (offset >= 1024 * cryptLenKB) break;
                                }
                                cs.Flush();
                                outStream.Flush();
                                while ((byRead = inStream.Read(buffer, 0, 1024)) > 0)
                                {
                                    outStream.Write(buffer, 0, byRead);
                                }
                            }
                        }
                    }
                    return true;
                }
        */

        /// <summary>
        /// 解密zip包里的xml文件
        /// </summary>
        /// <param name="otherFileContent"></param>
        /// <returns></returns>
        internal static string TransformOtherFile(string otherFileContent)
        {
            var contentbuffer = Convert.FromBase64String(otherFileContent);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = MainKey;
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.None;
                des.IV = MainIv;
                using (var ms = new MemoryStream(contentbuffer))
                {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var mso = new MemoryStream())
                        {
                            var buffer = new byte[1024];
                            int byteRead;
                            while ((byteRead = cs.Read(buffer, 0, 1024)) > 0)
                            {
                                mso.Write(buffer, 0, byteRead);
                            }
                            return FixLast(Encoding.UTF8.GetString(mso.ToArray()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 修正 pkcs#7 Padding
        /// </summary>
        /// <param name="inputstr"></param>
        /// <returns></returns>
        private static string FixLast(string inputstr)
        {
            for (byte i = 1; i <= 7; i++)
            {
                inputstr = inputstr.Replace((char)i, (char)32);
            }
            inputstr = inputstr.Trim();
            return inputstr;
        }
        #endregion

        #region 加密算法相关
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        internal static string Md5(params string[] contents)
        {
            var content = string.Join(string.Empty, contents);
            var buffer = Encoding.UTF8.GetBytes(content);
            using (var md5 = new Md5Impl())
            {
                return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "");
            }
        }

        internal static string Sha1(Stream st)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(sha1.ComputeHash(st)).Replace("-", "");
            }
        }

        /// <summary>
        /// RC4加密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string Rc4EncryptString(string source)
        {
            var by = new byte[4];
            Rnd.NextBytes(by);
            source = BitConverter.ToString(by).Replace("-", string.Empty) + source;
            //if (source.Length < 5) source = new string((char)32, 5 - source.Length) + source;
            var buffer = Encoding.UTF8.GetBytes(source);
            Rc4(buffer, MachineKey);
            return BitConverter.ToString(buffer).Replace("-", string.Empty);
        }

        /// <summary>
        /// RC4解密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string Rc4DecryptString(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            var buffer = Enumerable.Range(0, source.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(source.Substring(x, 2), 16)).ToArray();
            Rc4(buffer, MachineKey);
            return Encoding.UTF8.GetString(buffer).Substring(8);
        }

        /// <summary>
        /// Rc4原生算法
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        private static void Rc4(byte[] bytes, byte[] key)
        {
            var s = new byte[256];
            var k = new byte[256];
            byte temp;
            int i;

            for (i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                k[i] = key[i % key.GetLength(0)];
            }

            var j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
            for (var x = 0; x < bytes.GetLength(0); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                var t = (s[i] + s[j]) % 256;
                bytes[x] ^= s[t];
            }
        }


        private static byte[] AesEncrypt(byte[] buffer)
        {
            using (var aes = new AesCryptoServiceProvider() { Key = AesKey, IV = AesIv })
            {
                ICryptoTransform cryptor = aes.CreateEncryptor();
                return cryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        private static byte[] AesDecrypt(byte[] buffer)
        {
            using (var aes = new AesCryptoServiceProvider() { Key = AesKey, IV = AesIv })
            {
                ICryptoTransform cryptor = aes.CreateDecryptor();
                return cryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        internal static string AesKeyCreatePassword()
        {
            var r = new Random();
            var index = r.Next(AesKey_pre.Length - 4); // int index = r.nextInt(AESKEY_PRE.length() - 4);
            var index2 = r.Next(AesKey_suf.Length - 5); // int index2 = r.nextInt(AESKEY_SUF.length() - 5);
            var i1 = (int)(r.NextDouble() * 90 + 10); //int i1 = (int)(Math.random() * 90 + 10);
            var i2 = (int)(r.NextDouble() * 90 + 10); //int i2 = (int)(Math.random() * 90 + 10);
            var i3 = i1 + i2;
            string str;
            if (i3 >= 100)
            {
                str = "" + i1 + i2 + i3;
            }
            else
            {
                str = "" + i1 + i2 + i3 + 0;
            }

            return AesKey_pre.Substring(index, 4) + str + AesKey_suf.Substring(index2, 5);

            // return
            // StringUtils.substring(AESKEY_PRE, index, index + 4)
            // + str.chars().boxed().map(c->Character.toString((char)(c ^ 5))).collect(Collectors.joining(""))
            // + StringUtils.substring(AESKEY_SUF, index2, index2 + 5);
        }

        internal static byte[] AesEncrypt(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC; // CBC加密模式
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7填充模式

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        internal static byte[] RSAEncrypt(byte[] DataToEncrypt, string PublicKey)
        {
            try
            {
                var RSA = new RSACryptoServiceProvider();
                KeyValuePair<byte[], byte[]> publicKeyPair = DecodeHexPublicKey(PublicKey);
                var RSAKeyInfo = new RSAParameters
                {
                    Modulus = publicKeyPair.Key,
                    Exponent = publicKeyPair.Value
                };
                RSA.ImportParameters(RSAKeyInfo);
                var encryptedData = RSA.Encrypt(DataToEncrypt, false);
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        internal static KeyValuePair<byte[], byte[]> DecodeHexPublicKey(string hexPublicKey)
        {
            try
            {
                var keyBytes = HexStringToByte(hexPublicKey); // 字符串转成十六进制字节数组
                if (keyBytes[0] != 0x30) // 第一个字节的文件头必须解析成SEQUENCE结构
                {
                    throw new Exception("head of file cannot decode to SEQUENCE");
                }

                // 解析主结构体（SEQUENCE结构体）
                var mainSequenceLength = 0;
                var currentIndex = 1; // 从第二个字节开始解析
                mainSequenceLength = DecodeTLV_Length(keyBytes, ref currentIndex); // 获取主SEQUENCE结构体的长度

                // 解析inner（SEQUENCE结构体）
                var innerSequenceLength = 15; // inner的固定长度15
                currentIndex += innerSequenceLength; // 跳过即可

                // 解析bitString（BitString类型）
                currentIndex++; // 下标跳过bitStirng数据的Tag字段，移向Length字段
                var bitStringLength = DecodeTLV_Length(keyBytes, ref currentIndex); // 获取BitString数据的长度
                currentIndex++; // bitString的Value字段带一个unused bit，跳过

                // 解析参数结构体（SEQUENCE结构体）
                currentIndex++; // 下标跳过参数结构体的Tag字段，移向Length字段
                var paramSequenceLength = DecodeTLV_Length(keyBytes, ref currentIndex); // 获取参数SEQUENCE结构体的长度

                // 解析modulus模数N
                var modulus = DecodeInteger(keyBytes, ref currentIndex);

                // 解析exponent公开幂E
                var exponent = DecodeInteger(keyBytes, ref currentIndex);

                // 公钥（E，N）
                var result = new KeyValuePair<byte[], byte[]>(modulus, exponent);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new KeyValuePair<byte[], byte[]>();
            }
        }

        /**
         * 字符串以十六进制的规则转为字节数组
         */
        private static byte[] HexStringToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /**
         * 解码ASN.1规则中的INTEGER类型，返回解码后的数据，处理完成后currentIndex的位置会移到该类型数据末尾的下一个位置
         * @param data 密钥字节数组(十六进制)
         * @param index 表示当前需要解码的TLV元组中的L字节的位置
         */
        private static byte[] DecodeInteger(byte[] dataInteger, ref int currentIndex)
        {
            if (dataInteger[currentIndex] != 0x02) throw new Exception("params id invalid");
            currentIndex += 1;
            var len = DecodeTLV_Length(dataInteger, ref currentIndex);
            var integerStart = currentIndex;
            var integerEnd = currentIndex + len - 1;

            //删除补零，>7f时补0
            while (dataInteger[currentIndex] == 0)
            {
                currentIndex++;
            }
            if (dataInteger[currentIndex] > 0x7f)
            {
                integerStart = currentIndex;
            }

            var result = dataInteger.Where((item, index) => index >= integerStart & index <= integerEnd).ToArray();
            currentIndex = integerEnd + 1; // 处理完成后currentIndex移到末尾的下一个位置
            return result;
        }

        /**
         * 解码TLV元组中的Length字节，返回实际长度，返回0表示错误，处理完成后currentIndex的位置会移到Length字段末尾的下一个位置
         * @param data 密钥字节数组(十六进制)
         * @param index 表示当前需要解码的TLV元组中的L字节的位置
         */
        private static int DecodeTLV_Length(byte[] data, ref int currentIndex)
        {
            try
            {
                int length = data[currentIndex]; // 获取当前字节，十进制表示
                if (length == 0) // 解码不能为：0000 0000，抛出异常
                {
                    throw new ArgumentOutOfRangeException("length", "parameter length in TLV cannot be 0x00");
                }
                else if (length < 0x80) // 解码为：0xxx xxxx，定长式、短形式：最高位为0，后7位表示长度的实际值
                {
                    ++currentIndex; // 处理完成后currentIndex移到末尾的下一个位置
                    return length;
                }
                else if (length == 0x80) // 解码为：1000 0000，不定长式：以两个连续的0x00结尾
                {
                    var result = 0;
                    var count = 0; // 连续遇到0x00的次数
                    while (count != 2)
                    {
                        ++currentIndex; // 获取下一个字节
                        int temp = data[currentIndex];
                        if (temp == 0x00) // 遇到0x00
                        {
                            ++count;
                        }
                        else
                        {
                            count = 0;
                        }
                        ++result;
                    }

                    ++currentIndex; // 处理完成后currentIndex移到末尾的下一个位置
                    return result;
                }
                else // 解码为：1xxx xxxx，定长式、长形式：最高位为1，后7位表示长度的实际值会占用接下的多少个字节数
                {
                    length &= 0x7F; // 当前值和“0111 1111”进行按位与，提取出length后7位的1，后7位的十进制表示长度的实际值占用的字节数
                    var result = 0;
                    for (var i = length - 1; i >= 0; i--)
                    {
                        ++currentIndex; // 获取下一个字节
                        var temp = data[currentIndex] << (i * 8); // 计算当前字节所在的位置表示的值，每多一个字节需要左移八位
                        result += temp;
                    }

                    ++currentIndex; // 处理完成后currentIndex移到末尾的下一个位置
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ++currentIndex; // 处理完成后currentIndex移到末尾的下一个位置
                return 0;
            }
        }

        public static byte[] DesDecrypt(byte[] buffer)
        {
            using (var des = new DESCryptoServiceProvider { Key = DesKey, Mode = CipherMode.ECB })
            {
                ICryptoTransform cryptor = des.CreateDecryptor();
                return cryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }
        #endregion

        #region 向手机发送课件相关
        internal static string EncryptContentForAndroid(string content, string encKey)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var keyBy = Enumerable.Range(0, encKey.Length - 1).Where(x => x % 2 == 0).Select(x => byte.Parse(encKey.Substring(x, 2), System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
            var tkey = new byte[16];
            Buffer.BlockCopy(keyBy, 0, tkey, 0, keyBy.Length > 16 ? 16 : keyBy.Length);
            data = EncryptAesAndroid(data, tkey);
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        internal static byte[] EncryptVideoHeaderForAndroid(byte[] header, string encKey)
        {
            var keyBy = Enumerable.Range(0, encKey.Length - 1).Where(x => x % 2 == 0).Select(x => byte.Parse(encKey.Substring(x, 2), System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
            var tkey = new byte[16];
            Buffer.BlockCopy(keyBy, 0, tkey, 0, keyBy.Length > 16 ? 16 : keyBy.Length);
            header = EncryptAesAndroid(header, tkey);
            return header;
        }

        private static byte[] EncryptAesAndroid(byte[] data, byte[] encKey)
        {
            if (data.Length % 16 == 0)
            {
                var buffer = new byte[data.Length + 16];
                Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
                for (var i = data.Length; i < buffer.Length; i++)
                {
                    buffer[i] = 0x10;
                }
            }
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = encKey;
                aes.IV = encKey;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.ECB;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        internal static void DecryptVideoFile(string fileName, string destFileName, out byte[] headerContent)
        {
            FileStream sourceStream = File.OpenRead(fileName);
            sourceStream.Position = 4;
            var encLenBy = new byte[4];
            sourceStream.Read(encLenBy, 0, 4);
            var enclen = (BitConverter.ToInt32(encLenBy, 0) >> 16) * 1024;
            var iv = new byte[16];
            sourceStream.Read(iv, 0, 16);
            sourceStream.Position = 32;
            var encData = new byte[enclen];
            sourceStream.Read(encData, 0, enclen);

            FileStream outStream = File.Create(destFileName);

            var aes = new AesCryptoServiceProvider
            {
                Padding = PaddingMode.None,
                KeySize = 128,
                Key = MachineKey,
                IV = iv
            };
            headerContent = new byte[1024];
            using (var ms = new MemoryStream(encData))
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    var buffer = new byte[enclen];
                    var byteLen = cs.Read(buffer, 0, enclen);
                    Buffer.BlockCopy(buffer, 0, headerContent, 0, 1024);
                    Buffer.BlockCopy(buffer, 1024, buffer, 0, 1024);
                    outStream.Write(buffer, 0, buffer.Length);
                    var data = new byte[4096];
                    int rl;
                    while ((rl = sourceStream.Read(data, 0, data.Length)) > 0)
                    {
                        outStream.Write(data, 0, rl);
                    }
                }
            }
            aes.Dispose();
            sourceStream.Close();
            outStream.Close();
        }
        #endregion
    }

    internal struct AbcdStruct
    {
        public uint A;
        public uint B;
        public uint C;
        public uint D;
    }

    public sealed class Md5Impl : HashAlgorithm
    {
        private byte[] _data;
        private AbcdStruct _abcd;
        private long _totalLength;
        private int _dataSize;

        public Md5Impl()
        {
            HashSizeValue = 0x80;
            Initialize();
        }

        public override void Initialize()
        {
            _data = new byte[64];
            _dataSize = 0;
            _totalLength = 0;
            //Intitial values as defined in RFC 1321
            _abcd = new AbcdStruct
            {
                A = 0x67452301,
                B = 0xefcdab89,
                C = 0x98badcfe,
                D = 0x10325476
            };
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var startIndex = ibStart;
            var totalArrayLength = _dataSize + cbSize;
            if (totalArrayLength >= 64)
            {
                Array.Copy(array, startIndex, _data, _dataSize, 64 - _dataSize);
                // Process message of 64 bytes (512 bits)
                Md5Core.GetHashBlock(_data, ref _abcd, 0);
                startIndex += 64 - _dataSize;
                totalArrayLength -= 64;
                while (totalArrayLength >= 64)
                {
                    Array.Copy(array, startIndex, _data, 0, 64);
                    Md5Core.GetHashBlock(array, ref _abcd, startIndex);
                    totalArrayLength -= 64;
                    startIndex += 64;
                }
                _dataSize = totalArrayLength;
                Array.Copy(array, startIndex, _data, 0, totalArrayLength);
            }
            else
            {
                Array.Copy(array, startIndex, _data, _dataSize, cbSize);
                _dataSize = totalArrayLength;
            }
            _totalLength += cbSize;
        }

        protected override byte[] HashFinal()
        {
            HashValue = Md5Core.GetHashFinalBlock(_data, 0, _dataSize, _abcd, _totalLength * 8);
            return HashValue;
        }
    }

    internal static class Md5Core
    {
        //Prevent CSC from adding a default public constructor

        internal static byte[] GetHashFinalBlock(byte[] input, int ibStart, int cbSize, AbcdStruct abcd, long len)
        {
            var working = new byte[64];
            var length = BitConverter.GetBytes(len);

            //Padding is a single bit 1, followed by the number of 0s required to make size congruent to 448 modulo 512. Step 1 of RFC 1321
            //The CLR ensures that our buffer is 0-assigned, we don't need to explicitly set it. This is why it ends up being quicker to just
            //use a temporary array rather then doing in-place assignment (5% for small inputs)
            Array.Copy(input, ibStart, working, 0, cbSize);
            working[cbSize] = 0x80;

            //We have enough room to store the length in this chunk
            if (cbSize < 56)
            {
                Array.Copy(length, 0, working, 56, 8);
                GetHashBlock(working, ref abcd, 0);
            }
            else //We need an aditional chunk to store the length
            {
                GetHashBlock(working, ref abcd, 0);
                //Create an entirely new chunk due to the 0-assigned trick mentioned above, to avoid an extra function call clearing the array
                working = new byte[64];
                Array.Copy(length, 0, working, 56, 8);
                GetHashBlock(working, ref abcd, 0);
            }
            var output = new byte[16];
            Array.Copy(BitConverter.GetBytes(abcd.A), 0, output, 0, 4);
            Array.Copy(BitConverter.GetBytes(abcd.B), 0, output, 4, 4);
            Array.Copy(BitConverter.GetBytes(abcd.C), 0, output, 8, 4);
            Array.Copy(BitConverter.GetBytes(abcd.D), 0, output, 12, 4);
            return output;
        }

        // Performs a single block transform of MD5 for a given set of ABCD inputs
        /* If implementing your own hashing framework, be sure to set the initial ABCD correctly according to RFC 1321:
        //    A = 0x67452301;
        //    B = 0xefcdab89;
        //    C = 0x98badcfe;
        //    D = 0x10325476;
        */

        internal static void GetHashBlock(byte[] input, ref AbcdStruct abcdValue, int ibStart)
        {
            var temp = Converter(input, ibStart);
            var a = abcdValue.A;
            var b = abcdValue.B;
            var c = abcdValue.C;
            var d = abcdValue.D;

            a = R1(a, b, c, d, temp[0], 7, 0xd76aa478);
            d = R1(d, a, b, c, temp[1], 12, 0xe8c7b756);
            c = R1(c, d, a, b, temp[2], 17, 0x242070db);
            b = R1(b, c, d, a, temp[3], 22, 0xc1bdceee);
            a = R1(a, b, c, d, temp[4], 7, 0xf57c0faf);
            d = R1(d, a, b, c, temp[5], 12, 0x4787c62a);
            c = R1(c, d, a, b, temp[6], 17, 0xa8304613);
            b = R1(b, c, d, a, temp[7], 22, 0xfd469501);
            a = R1(a, b, c, d, temp[8], 7, 0x698098d8);
            d = R1(d, a, b, c, temp[9], 12, 0x8b44f7af);
            c = R1(c, d, a, b, temp[10], 17, 0xffff5bb1);
            b = R1(b, c, d, a, temp[11], 22, 0x895cd7be);
            a = R1(a, b, c, d, temp[12], 7, 0x6b901122);
            d = R1(d, a, b, c, temp[13], 12, 0xfd987193);
            c = R1(c, d, a, b, temp[14], 17, 0xa679438e);
            b = R1(b, c, d, a, temp[15], 22, 0x49b40821);

            a = R2(a, b, c, d, temp[1], 5, 0xf61e2562);
            d = R2(d, a, b, c, temp[6], 9, 0xc040b340);
            c = R2(c, d, a, b, temp[11], 14, 0x265e5a51);
            b = R2(b, c, d, a, temp[0], 20, 0xe9b6c7aa);
            a = R2(a, b, c, d, temp[5], 5, 0xd62f105d);
            d = R2(d, a, b, c, temp[10], 9, 0x02441453);
            c = R2(c, d, a, b, temp[15], 14, 0xd8a1e681);
            b = R2(b, c, d, a, temp[4], 20, 0xe7d3fbc8);
            a = R2(a, b, c, d, temp[9], 5, 0x21e1cde6);
            d = R2(d, a, b, c, temp[14], 9, 0xc33707d6);
            c = R2(c, d, a, b, temp[3], 14, 0xf4d50d87);
            b = R2(b, c, d, a, temp[8], 20, 0x455a14ed);
            a = R2(a, b, c, d, temp[13], 5, 0xa9e3e905);
            d = R2(d, a, b, c, temp[2], 9, 0xfcefa3f8);
            c = R2(c, d, a, b, temp[7], 14, 0x676f02d9);
            b = R2(b, c, d, a, temp[12], 20, 0x8d2a4c8a);

            a = R3(a, b, c, d, temp[5], 4, 0xfffa3942);
            d = R3(d, a, b, c, temp[8], 11, 0x8771f681);
            c = R3(c, d, a, b, temp[11], 16, 0x6d9d6122);
            b = R3(b, c, d, a, temp[14], 23, 0xfde5380c);
            a = R3(a, b, c, d, temp[1], 4, 0xa4beea44);
            d = R3(d, a, b, c, temp[4], 11, 0x4bdecfa9);
            c = R3(c, d, a, b, temp[7], 16, 0xf6bb4b60);
            b = R3(b, c, d, a, temp[10], 23, 0xbebfbc70);
            a = R3(a, b, c, d, temp[13], 4, 0x289b7ec6);
            d = R3(d, a, b, c, temp[0], 11, 0xeaa127fa);
            c = R3(c, d, a, b, temp[3], 16, 0xd4ef3085);
            b = R3(b, c, d, a, temp[6], 23, 0x04881d05);
            a = R3(a, b, c, d, temp[9], 4, 0xd9d4d039);
            d = R3(d, a, b, c, temp[12], 11, 0xe6db99e5);
            c = R3(c, d, a, b, temp[15], 16, 0x1fa27cf8);
            b = R3(b, c, d, a, temp[2], 23, 0xc4ac5665);

            a = R4(a, b, c, d, temp[0], 6, 0xf4292244);
            d = R4(d, a, b, c, temp[7], 10, 0x432aff97);
            c = R4(c, d, a, b, temp[14], 15, 0xab9423a7);
            b = R4(b, c, d, a, temp[5], 21, 0xfc93a039);
            a = R4(a, b, c, d, temp[12], 6, 0x655b59c3);
            d = R4(d, a, b, c, temp[3], 10, 0x8f0ccc92);
            c = R4(c, d, a, b, temp[10], 15, 0xffeff47d);
            b = R4(b, c, d, a, temp[1], 21, 0x85845dd1);
            a = R4(a, b, c, d, temp[8], 6, 0x6fa87e4f);
            d = R4(d, a, b, c, temp[15], 10, 0xfe2ce6e0);
            c = R4(c, d, a, b, temp[6], 15, 0xa3014314);
            b = R4(b, c, d, a, temp[13], 21, 0x4e0811a1);
            a = R4(a, b, c, d, temp[4], 6, 0xf7537e82);
            d = R4(d, a, b, c, temp[11], 10, 0xbd3af235);
            c = R4(c, d, a, b, temp[2], 15, 0x2ad7d2bb);
            b = R4(b, c, d, a, temp[9], 21, 0xeb86d391);

            abcdValue.A = unchecked(a + abcdValue.A);
            abcdValue.B = unchecked(b + abcdValue.B);
            abcdValue.C = unchecked(c + abcdValue.C);
            abcdValue.D = unchecked(d + abcdValue.D);
        }

        //Manually unrolling these equations nets us a 20% performance improvement
        private static uint R1(uint a, uint b, uint c, uint d, uint x, int s, uint t) =>
            //                  (b + LSR((a + F(b, c, d) + x + t), s))
            //F(x, y, z)        ((x & y) | ((x ^ 0xFFFFFFFF) & z))
            unchecked(b + Lsr((a + ((b & c) | ((b ^ 0xFFFFFFFF) & d)) + x + t), s));

        private static uint R2(uint a, uint b, uint c, uint d, uint x, int s, uint t) =>
            //                  (b + LSR((a + G(b, c, d) + x + t), s))
            //G(x, y, z)        ((x & z) | (y & (z ^ 0xFFFFFFFF)))
            unchecked(b + Lsr((a + ((b & d) | (c & (d ^ 0xFFFFFFFF))) + x + t), s));

        private static uint R3(uint a, uint b, uint c, uint d, uint x, int s, uint t) =>
            //                  (b + LSR((a + H(b, c, d) + k + i), s))
            //H(x, y, z)        (x ^ y ^ z)
            unchecked(b + Lsr((a + (b ^ c ^ d) + x + t), s));

        private static uint R4(uint a, uint b, uint c, uint d, uint x, int s, uint t) =>
            //                  (b + LSR((a + I(b, c, d) + k + i), s))
            //I(x, y, z)        (y ^ (x | (z ^ 0xFFFFFFFF)))
            unchecked(b + Lsr((a + (c ^ (b | (d ^ 0xFFFFFFFF))) + x + t), s));

        // Implementation of left rotate
        // s is an int instead of a uint becuase the CLR requires the argument passed to >>/<< is of
        // type int. Doing the demoting inside this function would add overhead.
        private static uint Lsr(uint i, int s) => ((i << s) | (i >> (32 - s)));

        //Convert input array into array of UInts
        private static uint[] Converter(byte[] input, int ibStart)
        {
            if (null == input)
                throw new ArgumentNullException("input", @"Unable convert null array to array of uInts");

            var result = new uint[16];

            for (var i = 0; i < 16; i++)
            {
                result[i] = input[ibStart + i * 4];
                result[i] += (uint)input[ibStart + i * 4 + 1] << 8;
                result[i] += (uint)input[ibStart + i * 4 + 2] << 16;
                result[i] += (uint)input[ibStart + i * 4 + 3] << 24;
            }

            return result;
        }
    }
}