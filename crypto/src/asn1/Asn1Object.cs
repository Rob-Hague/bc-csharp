using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
    public abstract class Asn1Object
		: Asn1Encodable
    {
        public override void EncodeTo(Stream output)
        {
            Asn1OutputStream.Create(output).WriteObject(this);
        }

        public override void EncodeTo(Stream output, string encoding)
        {
            Asn1OutputStream asn1Out = Asn1OutputStream.Create(output, encoding);
            Asn1Object asn1Object = this;

            if (Der.Equals(encoding))
            {
                Asn1Set asn1Set = asn1Object as Asn1Set;
                if (null != asn1Set)
                {
                    /*
                     * NOTE: Even a DerSet isn't necessarily already in sorted order (particularly from DerSetParser),
                     * so all sets have to be converted here.
                     */
                    asn1Object = new DerSet(asn1Set.elements);
                }
            }

            asn1Out.WriteObject(asn1Object);
        }

        /// <summary>Create a base ASN.1 object from a byte array.</summary>
        /// <param name="data">The byte array to parse.</param>
        /// <returns>The base ASN.1 object represented by the byte array.</returns>
        /// <exception cref="IOException">
        /// If there is a problem parsing the data, or parsing an object did not exhaust the available data.
        /// </exception>
        public static Asn1Object FromByteArray(
			byte[] data)
		{
            try
			{
                MemoryStream input = new MemoryStream(data, false);
                Asn1InputStream asn1 = new Asn1InputStream(input, data.Length);
                Asn1Object result = asn1.ReadObject();
                if (input.Position != input.Length)
                    throw new IOException("extra data found after object");
                return result;
			}
			catch (InvalidCastException)
			{
				throw new IOException("cannot recognise object in byte array");
			}
		}

		/// <summary>Read a base ASN.1 object from a stream.</summary>
		/// <param name="inStr">The stream to parse.</param>
		/// <returns>The base ASN.1 object represented by the byte array.</returns>
		/// <exception cref="IOException">If there is a problem parsing the data.</exception>
		public static Asn1Object FromStream(
			Stream inStr)
		{
			try
			{
				return new Asn1InputStream(inStr).ReadObject();
			}
			catch (InvalidCastException)
			{
				throw new IOException("cannot recognise object in stream");
			}
		}

		public sealed override Asn1Object ToAsn1Object()
        {
            return this;
        }

		internal abstract void Encode(Asn1OutputStream asn1Out);

		protected abstract bool Asn1Equals(Asn1Object asn1Object);
		protected abstract int Asn1GetHashCode();

		internal bool CallAsn1Equals(Asn1Object obj)
		{
			return Asn1Equals(obj);
		}

		internal int CallAsn1GetHashCode()
		{
			return Asn1GetHashCode();
		}
	}
}
