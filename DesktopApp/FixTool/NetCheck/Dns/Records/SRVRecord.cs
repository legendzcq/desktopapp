using System;

namespace NetCheck.Dns.Records
{
    /// <summary>
    /// An SRV (service locator) Resource Record (RR) (RFC2782)
    /// </summary>
    [Serializable]
    class SRVRecord : RecordBase, IComparable
    {
        private readonly short _priority;
        private readonly short _weight;
        private readonly short _port;
        private readonly string _host;

        public short Priority
        {
            get { return _priority; }
        }

        public short Weight
        {
            get { return _weight; }
        }

        public short Port
        {
            get { return _port; }
        }

        public string Host
        {
            get { return _host; }
        }

        internal SRVRecord(Pointer pointer)
        {
            _priority = pointer.ReadShort();
            _weight = pointer.ReadShort();
            _port = pointer.ReadShort();
            _host = pointer.ReadDomain();
        }


        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", _priority, _weight, _port, _host);
        }

        #region IComparable Members

        // TODO: fix so that it checks both Priority AND weighting
        public int CompareTo(object obj)
        {
            SRVRecord otherSRV = (SRVRecord) obj;

            if (otherSRV._priority < _priority) return 1;
            if (otherSRV._priority > _priority) return -1;

            return -String.CompareOrdinal(otherSRV._host, _host);
        }

        public static bool operator ==(SRVRecord record1, SRVRecord record2)
        {
            if (record1 == null) throw new ArgumentNullException("record1");

            return record1.Equals(record2);
        }

        public static bool operator !=(SRVRecord record1, SRVRecord record2)
        {
            return !(record1 == record2);
        }

        public bool Equals(SRVRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._priority == _priority && other._weight == _weight && other._port == _port && Equals(other._host, _host);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(SRVRecord) && Equals((SRVRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _priority.GetHashCode();
                result = (result * 397) ^ _weight.GetHashCode();
                result = (result * 397) ^ _port.GetHashCode();
                result = (result * 397) ^ _host.GetHashCode();
                return result;
            }
        }

        #endregion

        
    }
}
