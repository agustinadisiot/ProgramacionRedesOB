﻿using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public static class Validation
    {
        private static bool IsValidEntry(string word)
        {
            bool isEmptyString = word.Length == 0;
            bool containsDelimiter = word.Contains(Specification.delimiter);
            bool containsDelimiter2 = word.Contains(Specification.secondDelimiter);
            return (!isEmptyString && !containsDelimiter);
        }

        private static bool IsValidNumber(string number, int min, int max)
        {
            int input;
            bool isANumber = int.TryParse(number, out input);
            bool inRange = (input >= min && input <= max);
            return isANumber && inRange;
        }

        public static string ReadValidString(string errorMessage)
        {
            string stringWord = Console.ReadLine(); ;
            bool isValidTitle = IsValidEntry(stringWord);
            while (!isValidTitle)
            {
                Console.WriteLine(errorMessage); 
                stringWord = Console.ReadLine();
                isValidTitle = IsValidEntry(stringWord);
            }
            return stringWord;
        }

        public static int ReadValidESRB()
        {
            List<ESRBRating> possibleESRB = Enum.GetValues(typeof(ESRBRating)).Cast<ESRBRating>().ToList();
            for (int i = 0; i < possibleESRB.Count; i++)
            {
                Console.WriteLine($"{ i + 1}.{ possibleESRB.ElementAt(i)}");
            }
            string esrb = Console.ReadLine();
            bool isANumber = IsValidNumber(esrb, 1, possibleESRB.Count);
            while (!isANumber)
            {
                Console.WriteLine($"Elija un numero entre 1 y {possibleESRB.Count}");
                esrb = Console.ReadLine();
                isANumber = IsValidNumber(esrb, 1, possibleESRB.Count);
            }
            return int.Parse(esrb);
        }

        public static int ReadValidNumber(string errorMessage, int min, int max)
        {
            
            string number = Console.ReadLine();
            bool isValidNumber = IsValidNumber(number, min, max);
            while (!isValidNumber)
            {
                Console.WriteLine(errorMessage);
                number = Console.ReadLine();
                isValidNumber = IsValidNumber(number, min, max);
            }
            return int.Parse(number);
        }

        public static string ReadValidPath(string errorMessage, FileHandler.FileHandler fileHandler)
        {
            string coverPath = Console.ReadLine();
            bool isValidPath = fileHandler.FileExists(coverPath);
            while (!isValidPath)
            {
                Console.WriteLine(errorMessage);
                coverPath = Console.ReadLine();
                isValidPath = fileHandler.FileExists(coverPath);
            }
            return coverPath;
        }

        public static string ReadValidDirectory(string errorMessage, FileHandler.FileHandler fileHandler)
        {
            string folderPath = Console.ReadLine();
            bool isValidPath = fileHandler.PathExists(folderPath);
            while (!isValidPath)
            {
                Console.WriteLine(errorMessage);
                folderPath = Console.ReadLine();
                isValidPath = fileHandler.PathExists(folderPath);
            }
            return folderPath;
        }

        public static void CouldDownload(string completePath, FileHandler.FileHandler fileHandler)
        {
            if (fileHandler.FileExists(completePath))
                Console.WriteLine($"Se descargó la caratula en {completePath}");
            else 
                Console.WriteLine($"Se intento descragar la caratula a {completePath} pero ocurrió un error y no se descargo");
        }

    }
}
