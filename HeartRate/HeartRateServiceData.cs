using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRate
{
    [Serializable]
    public class HeartRateServiceData
    {
        public String timestamp;
        public bool isHrvLong;
        public bool isSensorContactSupported;
        public bool isSensorInContact;
        public bool isEnergyExpendedIncluded;
        public bool hasRrIntervals;
        public UInt16 HeartRateMeasurement;
        public UInt16[] RRIntervals;

        private const int MAX_RR = 9;

        public HeartRateServiceData(byte[] data)
        {
            timestamp = DateTime.Now.ToString("o");
            int index = 1;
            var flags = data[0];
            isHrvLong = ((flags & 0x01) != 0);
            isSensorContactSupported = ((flags & 0x02) != 0);
            isSensorInContact = ((flags & 0x04) != 0);
            isEnergyExpendedIncluded = ((flags & 0x08) != 0);
            hasRrIntervals = ((flags & 0x10) != 0);
            if (isHrvLong)
            {
                HeartRateMeasurement = BitConverter.ToUInt16(data, 1);
                index += 2;
            }
            else
            {
                HeartRateMeasurement = data[1];
                index += 1;
            }
            if (hasRrIntervals)
            {
                RRIntervals = new UInt16[(data.Length - index) / 2];
                for(int j=0; j< RRIntervals.Length; j++)
                {
                    RRIntervals[j] = BitConverter.ToUInt16(data, index);
                    index += 2;
                }
            }
            else
            {
                RRIntervals = new UInt16[0];
            }
        }

        public static string GetCsVHeader()
        {
            return "Timestamp,Contact,HeartRate,RR1,RR2,RR3,RR4,RR5,RR6,RR7,RR8,RR9";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            strings.Add(timestamp);
            strings.Add((isSensorInContact ? "TRUE": "FALSE"));
            strings.Add(HeartRateMeasurement.ToString());
            for(int i=0; i< MAX_RR; i++)
            {
                if(i< RRIntervals.Length)
                {
                    strings.Add(RRIntervals[i].ToString());
                }
                else
                {
                    strings.Add("");
                }
            }
            return String.Join(",", strings);
        }
    }
}
