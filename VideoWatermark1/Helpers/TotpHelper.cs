using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VideoWatermark1.Helpers
{
    public class TotpHelper
    {
        /// <summary>
        /// The number of ticks as Measured at Midnight Jan 1st 1970;
        /// </summary>
        const long unixEpochTicks = 621355968000000000L;
        /// <summary>
        /// A divisor for converting ticks to seconds
        /// </summary>
        const long ticksToSeconds = 10000000L;

        private readonly int step  = 30;
        private readonly int totpSize = 4;
        //private readonly TimeCorrection correctedTime;

        public byte[] GenerateSHA(string text)
        {
            var sha = new SHA1Managed();
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] resultHash = sha.ComputeHash(bytes);
            return resultHash;
        }

        public string GenerateSHAString(string text)
        {
            var data = GenerateSHA(text);
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
            //return System.Text.Encoding.UTF8.GetString(data, 0, data.Count());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GenerateSHALongAndText(string text)
        {
            var data = GenerateSHA(text);
            // The RFC has a hard coded index 19 in this value.
            // This is the same thing but also accomodates SHA256 and SHA512
            // hmacComputedHash[19] => hmacComputedHash[hmacComputedHash.Length - 1]

            int offset = data[data.Length - 1] & 0x0F;
            var longitem = (data[offset] & 0x7f) << 24
                | (data[offset + 1] & 0xff) << 16
                | (data[offset + 2] & 0xff) << 8
                | (data[offset + 3] & 0xff) % 1000000;
            return Digits(longitem, this.totpSize);
        }

        protected string Digits(long input, int digitCount)
        {
            var truncatedValue = ((int)input % (int)Math.Pow(10, digitCount));
            return truncatedValue.ToString().PadLeft(digitCount, '0');
        }

        public string GetTOTP(string secret = "test")
        {
            string totp = "";
            long time = CalculateTimeStepFromTimestamp(DateTime.UtcNow);
            totp = GenerateSHALongAndText(secret + time.ToString());
            //totp = Digits(totp, this.totpSize);
            return totp;
        }

        /// <summary>
        /// Takes a timestamp and calculates a time step
        /// </summary>
        private long CalculateTimeStepFromTimestamp(DateTime timestamp)
        {
            var unixTimestamp = (timestamp.Ticks - unixEpochTicks) / ticksToSeconds;
            var window = unixTimestamp / (long)this.step;
            return window;
        }
    }
}
