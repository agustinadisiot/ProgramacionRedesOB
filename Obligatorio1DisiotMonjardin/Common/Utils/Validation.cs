using Common.Domain;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public static class Validation
    {

        public static bool isValidId(int id)
        {
            return id >= 0;
        }

        public static bool isValidTitle(string title)
        {
            return IsValidEntry(title);
        }
        public static bool isValidSynopsis(string syn)
        {
            return IsValidEntry(syn);
        }

        public static bool isValidGenre(string genre)
        {
            return Game.genres.Contains(genre);
        }

        public static bool isValidESRBRating(int rating)
        {
            return rating > 0 && rating < Enum.GetValues(typeof(ESRBRating)).Length - 1;
        }

        private static bool IsValidEntry(string word)
        {
            bool isEmptyString = word.Length == 0;
            bool containsDelimiter = word.Contains(Specification.FIRST_DELIMITER);
            bool containsDelimiter2 = word.Contains(Specification.SECOND_DELIMITER);
            return (!isEmptyString && !containsDelimiter && !containsDelimiter2);
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
            string stringWord = Console.ReadLine();
            bool isValidString = IsValidEntry(stringWord);
            while (!isValidString)
            {
                Console.WriteLine(errorMessage);
                stringWord = Console.ReadLine();
                isValidString = IsValidEntry(stringWord);
            }
            return stringWord;
        }

        public static int ReadValidESRB()
        {
            List<ESRBRating> possibleESRB = Enum.GetValues(typeof(ESRBRating)).Cast<ESRBRating>().ToList();
            possibleESRB.RemoveAt(0);
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


        public static int ReadValidESRBOrEmpty()
        {
            string result = "";
            while (!((result == "s") || (result == "n")))
            {
                Console.Write("Quiere modificar la clasificación ESRB? (s/n)");
                result = Console.ReadLine();
            }
            if (result == "s")
                return ReadValidESRB();
            else
                return (int)ESRBRating.None; // TODO documentar que se usar para indicar que no se modificar, que vimos que es una convencion usar None https://stackoverflow.com/questions/4337193/how-to-set-enum-to-null
        }

        public static string ReadValidGenre()
        {
            int maxGenre = Game.genres.Length;
            for (int i = 0; i < maxGenre; i++)
            {
                Console.WriteLine($"{ i + 1}.{ Game.genres[i] }");
            }
            string stringGenre = Console.ReadLine();
            bool isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            while (!isValidNumber)
            {
                Console.WriteLine($"Elija un numero entre 1 y {maxGenre}");
                stringGenre = Console.ReadLine();
                isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            }
            int genrePos = int.Parse(stringGenre) - 1;
            return Game.genres[genrePos];
        }

        public static string ReadValidGenreOrEmpty()
        {
            string result = "";
            while (!((result == "s") || (result == "n")))
            {
                Console.Write("Quiere modificar el género ? (s/n)");
                result = Console.ReadLine();
            }
            if (result == "s")
                return ReadValidGenre();
            else
                return "";
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
            bool isCorrectFormat = coverPath.EndsWith(Specification.IMAGE_EXTENSION);
            while (!(isValidPath && isCorrectFormat))
            {
                Console.WriteLine(errorMessage);
                coverPath = Console.ReadLine();
                isValidPath = fileHandler.FileExists(coverPath);
                isCorrectFormat = coverPath.EndsWith(Specification.IMAGE_EXTENSION);
            }
            return coverPath;
        }
        public static string ReadValidPathModify(string coverPath, string errorMessage, FileHandler.FileHandler fileHandler)
        {
            bool isValidPath = fileHandler.FileExists(coverPath);
            bool isCorrectFormat = coverPath.EndsWith(Specification.IMAGE_EXTENSION);
            while (!isValidPath && !isCorrectFormat)
            {
                Console.WriteLine(errorMessage);
                coverPath = Console.ReadLine();
                isValidPath = fileHandler.FileExists(coverPath);
                isCorrectFormat = coverPath.EndsWith(Specification.IMAGE_EXTENSION);
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

        public static string ContainsDelimiter(string errorMessage)
        {
            string word = Console.ReadLine(); ;
            bool containsDelimiter = word.Contains(Specification.FIRST_DELIMITER);
            bool containsDelimiter2 = word.Contains(Specification.SECOND_DELIMITER);
            bool isValidEntry = containsDelimiter || containsDelimiter2;
            while (isValidEntry)
            {
                Console.WriteLine(errorMessage);
                word = Console.ReadLine();
                containsDelimiter = word.Contains(Specification.FIRST_DELIMITER);
                containsDelimiter2 = word.Contains(Specification.SECOND_DELIMITER);
                isValidEntry = containsDelimiter || containsDelimiter2;
            }
            return word;
        }
    }
}
