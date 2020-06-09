using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonNet;

namespace Server
{
    class Server
    {
        class ConnectedClient
        {
            
            public Socket cSocket;
            private NetMessaging net;
            public static List<ConnectedClient> clients = new List<ConnectedClient>();
            public string Name { get; private set; }
            public ConnectedClient(Socket s)
            {
                cSocket = s;
                net = new NetMessaging(cSocket);
                net.CheckNameCmdReceived += OnCheckName;
                net.SendData("CHECKNAME", "?");
                net.LoginCmdReceived += OnLogin;
                net.MessageCmdReceived += OnMessage;
                net.DisconnectCmdReceived += OnDisconnect;
                net.MatrixAnswerCmdReceived += OnMatrixAnswer;
                net.MatrixCmdReceived += OnMatrix;
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Не удалось получить данные от клиента :(");
                        clients.Remove(this);
                    }
                }).Start();
            }

            private void OnMessage(string command, string data)
            {
                clients.ForEach((client) =>
                {
                    if (client != this) 
                        client.net.SendData("MESSAGE", "["+Name+"]: "+data);
                    else
                        client.net.SendData("MESSAGE", "Я: " + data);
                });
            }

            private void OnCheckName(string command, string data)
            {
                bool v = false;
                clients.ForEach(client =>
                {
                    if (client.Name == data)
                        v = true;
                });
                if (v)
                    net.SendData("CHECKNAME", "NO");
                else
                {
                    //net.SendData("CHEKNAME", "YES");
                    //verify = true;
                    net.SendData("LOGIN", "?");
                }
                    
            }
            private void OnDisconnect(string command, string data)
            {
                clients.Remove(this);
                /*string list = "";
                clients.ForEach(client =>
                {
                    list += client.Name + ",";
                });
                clients.ForEach((client) =>
                {
                    net.SendData("USERLIST", list);
                });*/
                
            }
            private void OnLogin(string command, string data)
            {
                Name = data;
                string list = "";
                clients.ForEach(client =>
                {
                    list += client.Name + ",";
                });
                //net.SendData("USERLIST", list);
                clients.Add(this);
                net.SendData("START", "!");
            }
            private void OnMatrix(string command, string data)
            {
                //здесь должна быть проверка на наличие этих данных в файле сервера
                //если в файле есть такая матрица
                //net.SendData("MATRIX", "данные из файла");
                //если нет такой матрицы
                net.SendData("MATRIX", "NO");
            }
            private void OnMatrixAnswer(string command, string data)
            {
                //добавить в файл ответ как-то
            }
        }
        private String host;
        private Socket sSocket;
        private const int port = 8034;
        public Server()
        {
            Console.WriteLine("Получение локального адреса сервера");
            try
            {
                host = Dns.GetHostName();
                Console.WriteLine("Имя хоста: {0}", host);
                sSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                foreach (var addr in Dns.GetHostEntry(host).AddressList)
                {
                    try
                    {
                        sSocket.Bind(
                            new IPEndPoint(addr, port)
                        );
                        Console.WriteLine("Сокет связан с: {0}:{1}", addr, port);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не удалось связать с: {0}:{1}", addr, port);
                    }
                }

                sSocket.Listen(10);
                Console.WriteLine("Прослушивание началось...");
                while (true)
                {
                    Console.WriteLine("Ожидание нового подключения...");
                    var cSocket = sSocket.Accept();
                    Console.WriteLine("Соединение с клиентом установлено!");
                    new ConnectedClient(cSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }
    }
}
