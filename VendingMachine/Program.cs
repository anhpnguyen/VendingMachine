using System;
using System.Collections.Generic;
using System.Linq;

/*
 A vending machine sells items for various prices and can give change. At the start of
the day it is loaded with a certain number of coins of various denominations e.g. 100
x 1c, 50 x 5c, 50 x 10c 50 x25c etc. When an item is requested a certain number of
coins are provided. Write code that models the vending machine and calculates the
change to be given when an item is purchased (e.g. 2 x 25c used to purchase an
item costing 35c might return 1 x 10c and 1 x 5c).
*/

namespace VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get vending machine instance using Singleton to ensure there's only 1 vending machine at all time
            var vendingMachine = VendingMachine.Instance;

            //Hover over Coin to see Coin POCO definition
            var coins = new List<Coin>
            {
                new Coin(5, 50),
                new Coin(1, 100),
                new Coin(25, 50),
                new Coin(10, 50)
            };

            //Add coins to the vending machine
            vendingMachine.AddCoins(coins);

            RunExamples(vendingMachine);
        }

        private static void RunExamples(VendingMachine vendingMachine)
        {
            var example1 = vendingMachine.GetChangeInCoins(itemValue: 35, userPaid: 50);
            PrettyPrint(example1, itemValue: 35, userPaid: 50);

            var example2 = vendingMachine.GetChangeInCoins(itemValue: 15, userPaid: 20);
            PrettyPrint(example2, itemValue: 15, userPaid: 20);

            var example3 = vendingMachine.GetChangeInCoins(itemValue: 18, userPaid: 25);
            PrettyPrint(example3, itemValue: 18, userPaid: 25);

            var example4 = vendingMachine.GetChangeInCoins(itemValue: 4, userPaid: 10);
            PrettyPrint(example4, itemValue: 4, userPaid: 10);

            var example5 = vendingMachine.GetChangeInCoins(itemValue: 31, userPaid: 50);
            PrettyPrint(example5, 31, 50);
        }

        static void PrettyPrint(List<Coin> coins, int itemValue, int userPaid)
        {
            if (coins == null) Console.WriteLine("Not enough coin.");
            else
            {
                int change = userPaid - itemValue;
                Console.WriteLine($"User pays {userPaid} for an item that costs {itemValue}. Change expected is {change}.");
                int sum = 0;
                foreach (var c in coins)
                {
                    Console.WriteLine($"Coin Denomination: {c.Denomination}. Quantity: {c.Quantity}");
                    sum += (c.Denomination * c.Quantity);
                }
                Console.WriteLine($"Change returned is: {sum}");
            }
            Console.WriteLine("---------------------------------------------");
        }
    }

    /// <summary>
    /// Coin POCO
    /// Denomination: different coin denominations (1c, 5c, 10c, 25c)
    /// Quantity: amount per coin denomination
    /// </summary>
    class Coin
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
        public Coin(int dem, int qty)
        {
            Denomination = dem;
            Quantity = qty;
        }
    }

    /// <summary>
    /// Vending machine. (Singleton)
    /// </summary>
    class VendingMachine
    {
        /// <summary>
        /// Public methods and members
        /// </summary>
        public static VendingMachine Instance
        {
            get
            {
                if(vendingMachine == null)
                {
                    vendingMachine = new VendingMachine();
                }
                return vendingMachine;
            }
        }
        /// <summary>
        /// Gets the change in coins.
        /// </summary>
        /// <returns>The change in coins.</returns>
        /// <param name="itemValue">Item value.</param>
        /// <param name="userPaid">User paid.</param>
        public List<Coin> GetChangeInCoins(int itemValue, int userPaid)
        {
            int change = userPaid - itemValue;
            return GetChangeInCoins(Coins, change);
        }

        /// <summary>
        /// This method is an overload of the above method
        /// It iterates through the list of coins, find the appropriate quantity of coins per denomination recursively
        /// Returns a list of coins equivalent to the change to be given when an item is purchased
        /// </summary>
        /// <returns>The change in coins.</returns>
        /// <param name="coins">Coins.</param>
        /// <param name="change">Change.</param>
        /// <param name="start">Start.</param>
        public List<Coin> GetChangeInCoins(List<Coin> coins, int change, int start = 0)
        {
            for (int i = start; i < coins.Count; i++)
            {
                Coin coin = coins[i];
                /*
                 * Check if there's enough coin for a denomination
                 * and if the denomination is less than the value of change
                */
                if (coin.Quantity > 0 && coin.Denomination <= change)
                {
                    //Mod change by the coin denomination to get remainder
                    int rem = change % coin.Denomination;

                    //Get number of coins for that denomination
                    int numberOfCoins = (change - rem) / coin.Denomination;

                    //Take that number of coins if it's less than the coin quantity
                    //Otherwise take the coin quantity
                    int qty = Math.Min(coin.Quantity, numberOfCoins);

                    //List of coins to be returned
                    List<Coin> ret = new List<Coin>();

                    //Add the denomination and the calculated quantity to a list to return
                    ret.Add(new Coin(coin.Denomination, qty));

                    //Calculate if any coin is still needed to be added
                    //If remaining change is 0, meaning all the coins in the list add up to the change, return the list
                    //Otherwise, recursively call this function to get other coins of smaller denominations
                    int remainingChange = change - (qty * coin.Denomination);
                    if (remainingChange == 0)
                    {
                        return ret;
                    }

                    //Recursive call to get small denominations of coin
                    //i + 1 because we want the iteration to start on the next coin
                    List<Coin> smallerDenominations = GetChangeInCoins(coins, remainingChange, i + 1);

                    //Add the rest of coins to the list and return
                    if (smallerDenominations != null)
                    {
                        ret.AddRange(smallerDenominations);
                        return ret;
                    }                   
                }
            }
            return null;
        }

        /// <summary>
        /// Deposit coins into vending machine
        /// </summary>
        /// <param name="coins">Coins.</param>
        public void AddCoins(List<Coin> coins)
        {
            //Sort the coins by denomination
            Coins.AddRange(coins.OrderByDescending(c => c.Denomination).ToList());
        }

        /// <summary>
        /// Private constructor, methods, and members
        /// </summary>
        private static VendingMachine vendingMachine;
        private VendingMachine() { }
        private List<Coin> Coins = new List<Coin>();
    }
}