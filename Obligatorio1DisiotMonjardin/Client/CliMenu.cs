using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class CliMenu
    {
        public static void showMenu(Dictionary<string,Action> options, string message) {
           // Console.Clear();
            Console.WriteLine(message);
            for (int i = 0; i < options.Count; i++)
            {
                var option = options.ElementAt(i);
                Console.WriteLine($"{i+1}.{option.Key}");
            }
            bool isCorrectNumber = false;
            int input = 1;
            while (!isCorrectNumber)
            {
                bool isANumber = int.TryParse(Console.ReadLine(), out input);
                int min = 1;
                int max = options.Count;
                if (isANumber)
                {
                    bool inRange = input >= min && input <= max;
                    if (inRange) isCorrectNumber = true;
                }
                if (!isCorrectNumber)
                {
                    Console.WriteLine($"Elija un numero entre {min} y {max}");
                }
            }
            var selectedOption = options.ElementAt(input - 1);
            var selectedAction = selectedOption.Value;
            selectedAction();
        }
    }
}
