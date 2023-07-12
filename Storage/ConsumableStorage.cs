using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    /*
    public class ConsumableStorage : MonoBehaviour
    {
        public int GetConsumableOfName(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; }
            }
            return 0;
        }
        public void AddNewConsumable(string name, int StartingAmount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return; }
            }
            NewCurrency.Add(new Tuple<string, int>(name, StartingAmount));
        }

        public void AddConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second += Amount; }
            }
        }
        public void SetConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second = Amount; }
            }
        }
        public void RemoveConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second -= Amount; if (entry.Second < 0) { entry.Second = 0; } }
            }
        }

        public int ReturnConsumableAmount(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; } 
            }
            return 0;
        }

        public List<Tuple<string, int>> NewCurrency = new List<Tuple<string, int>>();
    }
    */

    public class GlobalConsumableStorage : MonoBehaviour
    {
        public static int GetConsumableOfName(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; }
            }
            return 0;
        }
        public static void AddNewConsumable(string name, int StartingAmount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return; }
            }
            NewCurrency.Add(new Tuple<string, int>(name, StartingAmount));
        }

        public static void AddConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second += Amount; }
            }
        }
        public static void SetConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second = Amount; }
            }
        }
        public static void RemoveConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second -= Amount; if (entry.Second < 0) { entry.Second = 0; } }
            }
        }

        public static int ReturnConsumableAmount(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; }
            }
            return 0;
        }

        public static List<Tuple<string, int>> NewCurrency = new List<Tuple<string, int>>();
    }
}
