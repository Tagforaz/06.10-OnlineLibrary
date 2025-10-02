using OnlineLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.ConsoleApp.Helpers
{
    public class ManagementApplication
    {
        public void Run()
        {

            int num = 0;
            string str = null;
            bool result = false;


            while (!(num == 0 && result))
            {
                Console.WriteLine("1.Create Book\n2.Delete Book\n3.Get Book By Id\n4.Show All Books\n5.Create Author\n6.Show All Authors" +
                    "              \n7.Show Author's Books\n8.Reserve Book\n9.Reservation List\n10.Change Reservation List\n11.User's Reservations List\n0.Exit");
                str = Console.ReadLine();
                Console.Clear();
                result = int.TryParse(str, out num);
                switch (num)
                {
                    case 1:
                        Console.WriteLine("Create Book");
                        break;
                    case 2:
                        Console.WriteLine("Delete Book");
                        break;
                    case 3:
                        Console.WriteLine("Get Book By Id");
                        break;
                    case 4:
                        Console.WriteLine("Show All Books");
                        break;
                    case 5:
                        Console.WriteLine("Create Author");
                        break;
                    case 6:
                        Console.WriteLine("Show All Authors");
                        break;
                    case 7:
                        Console.WriteLine("Show Author's Books");
                        break;
                    case 8:
                        Console.WriteLine("Reserve Book");
                        break;
                    case 9:
                        Console.WriteLine("Reservation List");
                        break;
                    case 10:
                        Console.WriteLine("Change Reservation List");
                        break;
                    case 11:
                        Console.WriteLine("User's Reservations List");
                        break;
                    case 0:
                        Console.WriteLine("Program ended");
                        break;
                    default:
                        Console.WriteLine("Wrong Input. Please Try Again");
                        break;
                }
            }
        }
    }
}
 
