using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using KR;
namespace HOST
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(Service1)))
            {
                host.OpenTimeout = new TimeSpan(0, 2, 0);
                host.CloseTimeout = new TimeSpan(0, 2, 0);
                host.Open();
                Console.ReadLine();
                host.Close();
            }

            Console.ReadLine();
        }
    }
}
