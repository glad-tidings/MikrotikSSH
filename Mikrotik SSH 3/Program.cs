using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;


namespace Mikrotik_SSH_3
{
    class Program
    {
        static void Main(string[] args)
        {
            MK mikrotik = new MK("176.9.185.245", 8728);
            if (!mikrotik.Login("username", "password"))
            {
                Console.WriteLine("Could not log in");
                mikrotik.Close();
                return;
            }
            mikrotik.Send("/system/identity/getall");
            mikrotik.Send(".tag=sss", true);
            foreach (string h in mikrotik.Read())
            {
                Console.WriteLine(h);
            }
            Console.ReadKey();
        }
    }
}
