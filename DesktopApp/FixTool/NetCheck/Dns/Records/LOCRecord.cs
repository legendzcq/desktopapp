using System;

namespace NetCheck.Dns.Records
{
 /// <summary>
	/// An LOC (Physical Location) Resource Record (RR) (RFC1035 3.3.9)
	/// </summary>
    [Serializable]
    public class LOCRequest : RecordBase
    {
        private readonly byte _version;
        private readonly double _size;
        private readonly double _hPrecision;
        private readonly double _vPrecision;

        private int _latdeg, _latmin, _latsec, _latsecfrag;
        private int _longdeg, _longmin, _longsec, _longsecfrag;
        private char _northsouth, _eastwest;
        private int _altmeters, _altfrag, _altsign;

        private readonly int _latitude;
        private readonly int _longitude;
        private readonly int _altitude;

        public byte Version { get { return _version; } }
        public double Size { get { return _size; } }
        public double HoritontalPrecision { get { return _hPrecision; } }
        public string HorizontalPrecisionReadable { get { return precsize_ntoa((byte)_hPrecision); } }
        public double VerticalPrecision { get { return _vPrecision; } }

        public int Latitude { get { return _latitude; } }
        public int Longitude { get { return _longitude; } }
        public int Altitude { get { return _altitude; } }

        public int LatitudeDegrees { get { return _latdeg; } }
        public int LatitudeMinutes { get { return _latmin; } }
        public int LatitudeSeconds { get { return _latsec; } }
        public int LatitudeMiliSeconds { get { return _latsecfrag; } }

        public int LongitudeDegrees { get { return _longdeg; } }
        public int LongitudeMinutes { get { return _longmin; } }
        public int LongitudeSeconds { get { return _longsec; } }
        public int LongitudeMiliSeconds { get { return _longsecfrag; } }

        public int AltidudeMeters { get { return _altmeters; } }
        public int AltitudeCentimeters { get { return _altfrag; } }




        /// <summary>
        /// Constructs an LOC record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal LOCRequest(Pointer pointer)
        {

            int _referenceAlt = 100000 * 100;
            int _latval, _longval, _altval;

            _version = pointer.ReadByte();
            _size = pointer.ReadByte();
            _hPrecision = pointer.ReadByte();
            _vPrecision = pointer.ReadByte();

            _latitude = pointer.ReadInt();
            _longitude = pointer.ReadInt();
            _altitude = pointer.ReadInt();

            _latval = _latitude - (1 << 31);
            _longval = _longitude - (1 << 31);

            if (_altitude < _referenceAlt)
            {
                //below WGS 84 spheroid 
                _altval = _referenceAlt - _altitude;
                _altsign = -1;
            }
            else
            {
                _altval = _altitude - _referenceAlt;
                _altsign = 1;
            }

            if (_latval < 0)
            {
                _northsouth = 'S';
                _latval = -_latval;
            }
            else
            {
                _northsouth = 'N';
            }

            //latitude calculation
            _latsecfrag = _latval % 1000;
            _latval /= 1000;
            _latsec = _latval % 60;
            _latval /= 60;
            _latmin = _latval % 60;
            _latval /= 60;
            _latdeg = _latval;

            if (_longval < 0)
            {
                _eastwest = 'W';
                _longval = -_longval;
            }
            else
            {
                _eastwest = 'E';
            }

            //longitude calculation
            _longsecfrag = _longval % 1000;
            _longval /= 1000;
            _longsec = _longval % 60;
            _longval /= 60;
            _longmin = _longval % 60;
            _longval /= 60;
            _longdeg = _longval;

            //altitude
            _altfrag = _altval % 100;
            _altmeters = (_altval / 100) * _altsign;

        }

        //port from C-code directory of RFC 1876 
        private string precsize_ntoa(byte prec)
        {
            uint[] poweroften = new uint[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 
											   10000000, 100000000, 1000000000 };
            ulong val;
            int mantissa, exponent;

            mantissa = (int)((prec >> 4) & 0x0f) % 10;
            exponent = (int)((prec >> 0) & 0x0f) % 10;

            val = (ulong)mantissa * poweroften[exponent];

            return string.Format("{0}.{1}m", (val / 100).ToString("0"), (val % 100).ToString("00"));

        }


        public override string ToString()
        {
            return string.Format("Latitude: {0}� {1}'{2}.{3} {4}, Longitude: {5}� {6}'{7}.{8} {9}, Altitude: {10}.{11}m, Size: {12}, Precision h:{13}, Precision v:{14}",
                _latdeg, _latmin.ToString("00"), _latsec.ToString("00"), _latsecfrag.ToString("000"), _northsouth,
                _longdeg, _longmin.ToString("00"), _longsec.ToString("00"), _longsecfrag.ToString("000"), _eastwest,
                _altmeters, _altfrag, precsize_ntoa((byte)_size), precsize_ntoa((byte)_hPrecision), precsize_ntoa((byte)_vPrecision));
        }
    }
}
