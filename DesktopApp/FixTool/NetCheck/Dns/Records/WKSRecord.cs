#region
// WKS record implementation written by Tom Nolan <tom at tinyint dot com>
// Released under GPL V3
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// A WKS Resource Record (RR) (RFC1035 3.4.2)
    /// </summary
    public class WKSRecord : RecordBase
    {
        internal IPAddress _ipAddress;
        internal int _protocol;
        internal byte[] _bytemap;
        internal BitArray _bitmap;

        /// <summary>
        /// Returns the ip address
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
        }

        /// <summary>
        /// Returns the protocol number
        /// </summary>
        public int Protocol
        {
            get { return _protocol; }
        }

        /// <summary>
        /// Returns the unmodified bit map from the DNS response
        /// </summary>
        public byte[] ByteMap
        {
            get { return _bytemap; }
        }

        /// <summary>
        /// Returns the BitArray representation of the bit map of the DNS response
        /// Note: The bit array flips the bits around because of the endianness.
        /// Example: If you have a WKS for the ECHO service (tcp/7) you would have a bit map of 00000001 (0x01)
        ///          however when this is read into the BitArray it is read as 10000000 (0x80) 
        ///          This happens because Microsoft uses little endian in their implementation.
        ///          See: http://msdn.microsoft.com/en-us/library/system.collections.bitarray.aspx
        /// </summary>
        public BitArray BitMap
        {
            get { return _bitmap; }
        }

        /// <summary>
        /// Returns a list of service port numbers
        /// </summary>
        public int[] Services
        {
            get
            {
                int[] i = new int[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };;
                // If this is run on a non-Windows system that uses big endian then this should still return the proper value by reversing the array
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(i);

                List<int> s = new List<int>();

                for (int x = 0; x < _bytemap.Length; x++)
                {
                    if (_bytemap[x] != 0x00)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if ((_bytemap[x] & i[y]) == i[y])
                                s.Add(x * 8 + y);
                        }
                    }
                }

                return s.ToArray();
            }
        }
        
        /// <summary>
        /// This is an alternate solution to determining the services list from the bit map, it may look more elegant, but it is slightly less 
        /// efficient because it has to loop through every single bit of the BitArray rather than having the ability to skip over byte blocks == 0x00.
        /// Because of the decreased efficiency, I am including it as a private property, but wanted to include it in case anyone was curious about the
        /// implementation being used in the public property.
        /// </summary>
        private int[] Services2
        {
            get
            {
                List<int> s = new List<int>();
                for (int x=0; x < _bitmap.Count; x++)
                {
                    if (_bitmap[x])
                    {
                        if (BitConverter.IsLittleEndian)
                            // the math here is just a way to get around the endianness of the BitArray mentioned in the summary for the BitMap property
                            s.Add((x / 8 + 1) * 8 - (x % 8) - 1);
                        else
                            // If this is run on a non-Windows system that uses big endian then this should still return the proper value
                            s.Add(x);
                    }
                }

                return s.ToArray();
            }
        }


        /// <summary>
        /// Constructs a WKS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="lengt">Lengt of inputted data</param>
        internal WKSRecord(Pointer pointer, int length)
        {
            _ipAddress = new IPAddress(pointer.ReadBytes(4));
            _protocol = Convert.ToInt32(pointer.ReadByte());
            _bytemap = pointer.ReadBytes(length - 5);
            _bitmap = new BitArray(_bytemap);
        }


        /// <summary>
        /// Convert the server response to a logically formatted string version.
        /// </summary>
        /// <returns>String representing server response</returns>
        public override string ToString()
        {
            string _svclist = string.Join(" ", Array.ConvertAll<int, string>(this.Services, new Converter<int, string>(Convert.ToString)));

            return String.Format("{0} {1} {2}", _ipAddress.ToString(), _protocol.ToString(), _svclist);
        }
    }
}
