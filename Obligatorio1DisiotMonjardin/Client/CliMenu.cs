using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class CliMenu
    {
        public static void showMenu(Dictionary<string,Action> options, string message = "") {
           // Console.Clear();
           if(message.Length > 0)
                Console.WriteLine(message);
            for (int i = 0; i < options.Count; i++)
            {
                var option = options.ElementAt(i);
                Console.WriteLine($"{i+1}.{option.Key}");
            }
            bool isCorrectNumber = false;
            int input = 1;
            string entry = "";
            while (!isCorrectNumber)
            {
                int min = 1;
                int max = options.Count;
                entry = Console.ReadLine();
                isCorrectNumber = Validation.IsValidNumber(entry, min, max);
                
                if (!isCorrectNumber)
                {
                    Console.WriteLine($"Elija un numero entre {min} y {max}");
                }
            }

            input = int.Parse(entry);
            var selectedOption = options.ElementAt(input - 1);
            var selectedAction = selectedOption.Value;
            selectedAction();
        }
    }
}
