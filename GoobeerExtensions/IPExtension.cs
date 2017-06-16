using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namspace GoobeerExtensions
{
public static class IPAddressExtension
    {
        public static long GetDistance(this IPAddress start,IPAddress end)
        {
            long distance = 0L;
            byte[] startIPBytes = start.GetAddressBytes(), endIPBytes = end.GetAddressBytes();

            if (startIPBytes[0] != endIPBytes[0])
            {
                distance = (byte.MaxValue - startIPBytes[3]) * (byte.MaxValue - startIPBytes[2]) * (byte.MaxValue - startIPBytes[1]) * (endIPBytes[0] - startIPBytes[0]) + (endIPBytes[1] - 1) * (endIPBytes[2] - 1) * (endIPBytes[3] - 1);
            }
            else
            {
                if (startIPBytes[1] != endIPBytes[1])
                {
                    distance = (byte.MaxValue - startIPBytes[3]) * (byte.MaxValue - startIPBytes[2]) * (endIPBytes[1] - startIPBytes[1]) + (endIPBytes[2] - 1) * (endIPBytes[3] - 1);
                }
                else
                {
                    if (startIPBytes[2] != endIPBytes[2])
                    {
                        distance = (byte.MaxValue - startIPBytes[3]) * (endIPBytes[2] - startIPBytes[2]) + (endIPBytes[3] - 1);
                    }
                    else
                    {
                        if (startIPBytes[3] != endIPBytes[3])
                        {
                            distance = endIPBytes[3] - startIPBytes[3] - 1;
                        }
                    }
                }
            }
            return distance;
        }

        public static int Compare(this IPAddress start,IPAddress end)
        {
            int result = 0;
            if (start.Equals(end))
            {
                return result;
            }
            result = (start.ToInt()-end.ToInt()) > 0 ? 1 : -1;
            return result;
        }

        public static Int64 ToInt(this IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();
            string ipBinary = string.Empty;
            for (int i = 0; i < ipBytes.Count(); i++)
            {
                var binaryStr = Convert.ToString(ipBytes[i], 2);
                while (binaryStr.Length!=8)
                {
                    binaryStr = "0" + binaryStr;
                }
                ipBinary += binaryStr;
            }
            return Convert.ToInt64(ipBinary, 2);
        }

        public static List<string> GenerateIPList(this IPAddress start, IPAddress end)
        {
            List<string> ipList = new List<string>();
            if (start.Equals(end))
            {
                ipList.Add(start.ToString());
                return ipList;
            }
            byte[] startIPBytes = start.GetAddressBytes(), endIPBytes = end.GetAddressBytes();

            long ipCount = GetDistance(start, end);

            ipList.Add(start.ToString());
            if (ipCount != 0)
            {
                IPAddress current = null;
                for (long i = 0; i < ipCount; i++)
                {
                    if (startIPBytes[3] < byte.MaxValue)
                    {
                        startIPBytes[3] += 1;
                        current = new IPAddress(startIPBytes);
                    }
                    else
                    {
                        if (startIPBytes[2] < byte.MaxValue)
                        {
                            startIPBytes[2] += 1;
                            startIPBytes[3] = 0;
                            current = new IPAddress(startIPBytes);
                        }
                        else
                        {
                            if (startIPBytes[1] < byte.MaxValue)
                            {
                                startIPBytes[1] += 1;
                                startIPBytes[2] = 0;
                                startIPBytes[3] = 0;
                                current = new IPAddress(startIPBytes);
                            }
                            else
                            {
                                if (startIPBytes[0] < byte.MaxValue)
                                {
                                    startIPBytes[0] += 1;
                                    startIPBytes[1] = 0;
                                    startIPBytes[2] = 0;
                                    startIPBytes[3] = 0;
                                    current = new IPAddress(startIPBytes);
                                }
                            }
                        }
                    }
                    ipList.Add(current.ToString());
                }
            }
            ipList.Add(end.ToString());

            return ipList;
        }
    }
}
