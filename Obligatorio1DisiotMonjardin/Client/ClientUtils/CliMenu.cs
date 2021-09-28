using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public static class CliMenu
    {
        public static void showMenu(Dictionary<string, Action> options, string message = "")
        {
            if (message.Length > 0)
                Console.WriteLine(message);
            for (int i = 0; i < options.Count; i++)
            {
                var option = options.ElementAt(i);
                Console.WriteLine($"{i + 1}.{option.Key}");
            }

            int min = 1;
            int max = options.Count;
            int input = Validation.ReadValidNumber($"Eliga un número entre {min} y {max}", min, max);

            var selectedOption = options.ElementAt(input - 1);
            var selectedAction = selectedOption.Value;
            selectedAction();
        }
    }
}
