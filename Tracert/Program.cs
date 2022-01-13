using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Tracert
{
    internal class Program
    {
        private const int _timeout = 10000;
        private const int _maxTTL = 30;
        private const int _bufferSize = 32;

        public static IEnumerable<(int, string)> GetTraceRoute(string ip)
        {
            byte[] buffer = new byte[_bufferSize];
            new Random().NextBytes(buffer);

            using var ping = new Ping();

            for (int ttl = 1; ttl <= _maxTTL; ttl++)
            {
                PingOptions options = new PingOptions(ttl, true);
                PingReply reply = ping.Send(ip, _timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    yield return (ttl, reply.Address.ToString());
                    break;
                }

                if (reply.Status == IPStatus.TtlExpired)
                {
                    yield return (ttl, reply.Address.ToString());
                }

                if (reply.Status == IPStatus.TimedOut)
                {
                    yield return (ttl, "***");
                }
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Введите IP-aдрес: ");
            var ip = Console.ReadLine();
            if (!IPAddress.TryParse(ip, out IPAddress value))
            {
                Console.WriteLine("Невалидный IP.");
                return;
            }

            Console.WriteLine($"Трассировка маршрута к {ip}");
            Console.WriteLine($"Максимальное число прыжков: {_maxTTL}");
            var ipAddresses = GetTraceRoute(ip);

            foreach (var item in ipAddresses)
            {
                Console.WriteLine(item.Item1 + ") " + item.Item2);
            }
            Console.WriteLine("Трассировка завершена.");
        }
    }
}
