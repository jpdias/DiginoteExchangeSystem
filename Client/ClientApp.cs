﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    class ClientApp
    {
        static void Main(string[] args)
        {
            new ClientConsole();
        }

        class ClientConsole
        {
            private IDES _iDes;
            private Boolean loggedIn;
            Dictionary<Diginote, User> market;

            public ClientConsole()
            {
                RemotingConfiguration.Configure("Client.exe.config", false);
                _iDes = (IDES)RemoteNew.New(typeof(IDES));
                Console.WriteLine("Diginote Exchange System v0.1");
                Console.WriteLine("Welcome!");
                string option;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu:");
                    if(loggedIn == false)
                    {
                        Console.WriteLine("1 - Login");
                        Console.WriteLine("2 - Add User");
                        Console.WriteLine("3 - Remove User");
                    }
                    else
                    {
                        Console.WriteLine("1 - Logout");
                        Console.WriteLine("2 - Market Quote");
                    }
                    Console.WriteLine("0 - Exit");
                    Console.Write("Option: ");
                    option = Console.ReadLine();
                    if(loggedIn == false)
                    {
                        switch (option)
                        {
                            case "0":
                                Application.Exit();
                                break;
                            case "1":
                                Console.WriteLine();
                                login();
                                break;
                            case "2":
                                Console.WriteLine();
                                addUser();
                                break;
                            case "3":
                                Console.WriteLine();
                                removeUser();
                                break;
                            default:
                                Console.WriteLine("Invalid option!");
                                break;
                        }
                    }
                    else
                    {
                        switch (option)
                        {
                            case "0":
                                Application.Exit();
                                break;
                            case "1":
                                Console.WriteLine();
                                logout();
                                break;
                            case "2":
                                Console.WriteLine();
                                getQuote();
                                break;
                            default:
                                Console.WriteLine("Invalid option!");
                                break;
                        }
                    }
                    
                }
                while (option != "0");
            }

            public void addUser()
            {
                Console.WriteLine("Add User");

                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.AddUser(name, nickname, password);
                Console.WriteLine(result);
            }

            public void removeUser()
            {
                Console.WriteLine("Remove User");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.RemoveUser(nickname, password);
                Console.WriteLine(result);
            }

            public void login()
            {
                Console.WriteLine("Login");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Login(nickname, password);
                if (result.Equals("Login successful!"))
                {
                    loggedIn = true;
                }
                Console.WriteLine(result);
            }

            public void logout()
            {
                Console.WriteLine("Logout");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Logout(nickname, password);
                if (result.Equals("Logout successful!"))
                {
                    loggedIn = false;
                }
                Console.WriteLine(result);
            }

            public void listUsers()
            {
                Console.WriteLine("Users");

                ArrayList users = _iDes.GetUsersList();

                if (users.Count == 0)
                {
                    Console.WriteLine("No registered users!");
                }
                else
                {
                    var userIndex = 0;
                    foreach (User user in users)
                    {
                        Console.WriteLine();
                        Console.WriteLine("User #" + (userIndex + 1));
                        Console.WriteLine("  Name: " + user.Name);
                        Console.WriteLine("  Nickname: " + user.Nickname);
                        Console.WriteLine("  LoggedIn: " + user.LoggedIn);
                        userIndex += 1;
                    }
                }
            }

            public void listMarket()
            {
                Console.WriteLine("Market");

                market = _iDes.GetMarket();

                if (market.Count == 0)
                {
                    Console.WriteLine("Empty market!");
                }
                else
                {
                    foreach (var entry in market)
                    {
                        Console.WriteLine(entry.Key);

                        if (entry.Value != null)
                        {
                            Console.WriteLine(entry.Value);
                        }
                        else
                        {
                            Console.WriteLine("No associated user!");
                        }

                        Console.WriteLine();
                    }
                }
            }

            public void getQuote()
            {
                Console.WriteLine("Current Quote");

                float quote = _iDes.GetQuote();

                if (quote == 0.0f)
                {
                    Console.WriteLine("No diginotes available on the market!");
                }
                else
                {
                    Console.WriteLine(quote + "€");
                }
            }
        }

        /* Mechanism for instanciating a remote object through its interface, using the config file */
        class RemoteNew
        {
            private static Hashtable types = null;

            private static void InitTypeTable()
            {
                types = new Hashtable();
                foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    types.Add(entry.ObjectType, entry);
            }

            public static object New(Type type)
            {
                if (types == null)
                    InitTypeTable();
                WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
                if (entry == null)
                    throw new RemotingException("Type not found!");
                return RemotingServices.Connect(type, entry.ObjectUrl);
            }
        }
    }
}
