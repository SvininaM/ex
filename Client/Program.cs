using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            /*Console.WriteLine("Старт работы клиента...");
            //var sHost = "SMAKBOOK";
            var sHost ="LAPTOP-A8CMBCCM";
            String usHost;
            Console.WriteLine("Введите адрес сервера или нажмите Enter для использования стандартного: ");
            usHost = Console.ReadLine();
            if (usHost.Length > 0) sHost = usHost;
            new Client(sHost);
            Console.ReadKey();*/
        }
    }
}
