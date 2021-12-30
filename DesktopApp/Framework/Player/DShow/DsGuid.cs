using System;
using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public DsGuid(string g)
        {
            guid = new Guid(g);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public DsGuid(Guid g)
        {
            guid = g;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Framework.Utility.Log.RecordLog(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Framework.Utility.Log.RecordLog(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }
    }
}