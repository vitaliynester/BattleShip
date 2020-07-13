using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BattleShip.SERVER
{
    class Program
    {
        static readonly int port = 5050;
        static TcpListener listener = new TcpListener(IPAddress.Any, port);
        static List<ConnectedClient> clients = new List<ConnectedClient>();
        static List<PairClient> pairClients = new List<PairClient>();
        static void Main(string[] args)
        {
            listener.Start();
            var db = new Database();
            Console.WriteLine("Сервер запущен!");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Клиент подключен!");

                Task.Factory.StartNew(async () =>
                {
                    var sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        var line = sr.ReadLine();
                        Console.WriteLine(line);
                        if (line.Contains("Login: ") && line.Contains("Password: "))
                        {
                            var elements = line.Split(' ');
                            //если пользователя не существует, тогда регистрируем
                            if (!db.UserExist(elements[1]))
                            {
                                if (IsTrueLogin(elements[1]) && IsTruePassword(elements[3]))
                                {
                                    db.RegisterUser(elements[1], elements[3]);
                                    if (clients.FirstOrDefault(s => s.Name == elements[1]) == null)
                                    {
                                        clients.Add(new ConnectedClient(client, elements[1], elements[3]));
                                        Console.WriteLine($"Новое подключение: {elements[1]}");
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Пользователь {elements[1]} уже подключен к серверу!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Логин или пароль не удовлетворяет требованиям!");
                                    client.Client.Disconnect(false);
                                    break;
                                }
                            }
                            //если пользователь уже существует
                            else
                            {
                                if (db.LoginUser(elements[1], elements[3]))
                                {
                                    if (clients.FirstOrDefault(s => s.Name == elements[1]) == null)
                                    {
                                        clients.Add(new ConnectedClient(client, elements[1], elements[3]));
                                        Console.WriteLine($"Новое подключение: {elements[1]}");
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Пользователь {elements[1]} уже подключен к серверу!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неправильный логин или пароль");
                                    client.Close();
                                    break;
                                }
                            }
                        }
                        if (line.Contains("DataLogin: "))
                        {
                            var name = line.Replace("DataLogin: ", "");
                            var rdg = new ReportDataGenerate(name);
                            var data = rdg.Data();
                            var result = JsonConvert.SerializeObject(data, Formatting.Indented);
                            WriteMessage(client, result);
                            Console.WriteLine($"Были запрошены данные о пользователе {name}");
                            client.Client.Disconnect(false);
                        }
                    }

                    while (client.Connected)
                    {
                        try
                        {
                            if (clients.Count % 2 == 0 && clients.Count > 0)
                            {
                                Random r = new Random();
                                var new_pair = new PairClient(clients[clients.Count - 2], clients[clients.Count - 1]);
                                if (!pairClients.Contains(new_pair))
                                {
                                    pairClients.Add(new_pair);
                                    WriteMessage(clients[clients.Count - r.Next(1, 2)].Client, "/game ishost ");
                                    Console.WriteLine($"Создана новая пара {new_pair.client_one.Name} и {new_pair.client_two.Name}");
                                    clients.RemoveAt(clients.Count - 2);
                                    clients.RemoveAt(clients.Count - 1);
                                }
                                break;
                            }
                            while (pairClients.Count > 0)
                            {
                                Thread.Sleep(300);
                                foreach (var item in pairClients)
                                {
                                    await Task.Factory.StartNew(async () =>
                                     {
                                         if (item.client_one.Client.Connected)
                                         {
                                             if (item.match_id == -1)
                                             {
                                                 item.match_id = db.CreateMatch(item.client_one.Name, item.client_two.Name);
                                             }
                                             sr = new StreamReader(item.client_one.Client.GetStream());
                                             var line_msg = await sr.ReadLineAsync();
                                             var msgs = new MessageConcat(line_msg);
                                             if (msgs.Place.Length > 0 && msgs.Result.Length > 0)
                                             {
                                                 if (msgs.Result == "Корабль уничтожен")
                                                 {
                                                     item.client_one.ShipsLeft--;
                                                 }
                                                 db.MakeStep(item.match_id, item.client_two.Name, msgs.Place, msgs.Result, item.client_two.ShipsLeft, item.client_one.ShipsLeft);
                                             }
                                             WriteMessage(item.client_two.Client, line_msg);
                                             if (line_msg == null || line_msg.Contains("/game end ") || line_msg.Contains("/game end "))
                                             {
                                                 item.client_one.Client.Close();
                                                 item.client_two.Client.Close();
                                                 if (item.client_one.ShipsLeft == 0 || item.client_one.ShipsLeft == 1)
                                                 {
                                                     db.EndMatch(item.match_id, item.client_two.Name);
                                                 }
                                                 if (item.client_two.ShipsLeft == 0 || item.client_two.ShipsLeft == 1)
                                                 {
                                                     db.EndMatch(item.match_id, item.client_one.Name);
                                                 }
                                                 pairClients.Remove(item);
                                             }
                                             if (line_msg?.Length > 0)
                                             {
                                                 Console.WriteLine($"{item.client_one.Name} : {line_msg}");
                                             }
                                         }
                                         else
                                         {
                                             Console.WriteLine($"Пользователь {item.client_one.Name} отключился!");
                                             item.client_one.Client.Close();
                                             item.client_two.Client.Close();
                                             pairClients.Remove(item);
                                         }
                                     });
                                    await Task.Factory.StartNew(async () =>
                                     {
                                         if (item.client_two.Client.Connected)
                                         {
                                             sr = new StreamReader(item.client_two.Client.GetStream());
                                             var line_msg = await sr.ReadLineAsync();
                                             var msgs = new MessageConcat(line_msg);
                                             if (msgs.Place.Length > 0 && msgs.Result.Length > 0)
                                             {
                                                 if (msgs.Result == "Корабль уничтожен")
                                                 {
                                                     item.client_two.ShipsLeft--;
                                                 }
                                                 db.MakeStep(item.match_id, item.client_one.Name, msgs.Place, msgs.Result, item.client_one.ShipsLeft, item.client_two.ShipsLeft);
                                             }
                                             WriteMessage(item.client_one.Client, line_msg);
                                             if (line_msg == null || line_msg.Contains("/game end ") || line_msg.Contains("/game end "))
                                             {
                                                 item.client_one.Client.Close();
                                                 item.client_two.Client.Close();
                                                 if (item.client_one.ShipsLeft == 0 || item.client_one.ShipsLeft == 1)
                                                 {
                                                     db.EndMatch(item.match_id, item.client_two.Name);
                                                 }
                                                 if (item.client_two.ShipsLeft == 0 || item.client_two.ShipsLeft == 1)
                                                 {
                                                     db.EndMatch(item.match_id, item.client_one.Name);
                                                 }
                                                 pairClients.Remove(item);
                                             }
                                             if (line_msg.Length > 0)
                                             {
                                                 Console.WriteLine($"{item.client_two.Name} : {line_msg}");
                                             }
                                         }
                                         else
                                         {
                                             Console.WriteLine($"Пользователь {item.client_two.Name} отключился!");
                                             item.client_one.Client.Close();
                                             item.client_two.Client.Close();
                                             pairClients.Remove(item);
                                         }                       
                                     });
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }
        }
        private static void WriteMessage(TcpClient client, string message)
        {
            var sw = new StreamWriter(client.GetStream());
            sw.AutoFlush = true;
            sw.WriteLine(message);
        }
        private static bool IsTrueLogin(string login)
        {
            Regex loginPattern = new Regex(@"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-]{5,19}$");
            return loginPattern.IsMatch(login);
        }
        private static bool IsTruePassword(string password)
        {
            Regex passwordPattern = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            return passwordPattern.IsMatch(password);
        }
    }
}
