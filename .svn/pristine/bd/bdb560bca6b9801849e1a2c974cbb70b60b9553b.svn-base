#region "  © Copyright 2005-07 to Marcos Meli - http://www.marcosmeli.com.ar"

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;
using System.Globalization;

namespace FileHelpers {
    /// <summary>Class that provides static methods that returns a default <see cref="ConverterBase">Converter</see> to the basic types.</summary>
    /// <remarks>Used by the <see cref="FileHelpers.FieldConverterAttribute"/>.</remarks>
    internal sealed class ConvertHelpers {

        #region "  Convert Classes  "

        internal sealed class NullableDateTimeConverter : ConverterBase {
            string mFormat;

            public NullableDateTimeConverter()
                : this(ConverterBase.DefaultDateTimeFormat) {
            }

            public NullableDateTimeConverter(string format) {
                if (format == null || format == String.Empty)
                    throw new Exception("The format of the DateTime Converter can be null or empty.");

                try {
                    string tmp = DateTime.Now.ToString(format);
                } catch {
                    throw new Exception("The format: '" + format + " is invalid for the DateTime Converter.");
                }

                mFormat = format;
            }

            //static CultureInfo mInvariant = System.Globalization.CultureInfo.InvariantCulture;

            public override object StringToField(string from) {
                if (from == null) from = string.Empty;

                object val;
                try {
                    val = DateTime.ParseExact(from.Trim(), mFormat, null);
                } catch {
                    string extra = String.Empty;
                    if (from.Length > mFormat.Length)
                        extra = " There are more chars than in the format string: '" + mFormat + "'";
                    else if (from.Length < mFormat.Length)
                        extra = " There are less chars than in the format string: '" + mFormat + "'";
                    else
                        extra = " Using the format: '" + mFormat + "'";


                    throw new ConvertException(from, typeof(DateTime), extra);
                }
                return val;
            }

            public override string FieldToString(object from) {
                if (from == null)
                    return "";
                else
                    return Convert.ToDateTime(from).ToString(mFormat);
            }
        }

        #endregion
    }
}
