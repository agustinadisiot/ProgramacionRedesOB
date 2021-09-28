using Common.Domain;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Utils
{
    public static class Validation
    {

        public static bool IsValidId(int id)
        {
            return id >= 0;
        }

        public static bool IsValidTitle(string title)
        {
            return IsValidEntry(title);
        }
        public static bool IsValidSynopsis(string syn)
        {
            return IsValidEntry(syn);
        }

        public static bool IsValidGenre(string genre)
        {
            return Game.genres.Contains(genre);
        }

        public static bool IsValidESRBRating(int rating)
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
            for (int i = 0; i < possibleESRB.Count - 1; i++)
            {
                Console.WriteLine($"{ i + 1}.{ possibleESRB.ElementAt(i)}");
            }
            string esrb = Console.ReadLine();
            bool isANumber = IsValidNumber(esrb, 1, possibleESRB.Count - 1);
            while (!isANumber)
            {
                Console.WriteLine($"Elija un número entre 1 y {possibleESRB.Count - 1}");
                esrb = Console.ReadLine();
                isANumber = IsValidNumber(esrb, 1, possibleESRB.Count - 1);
            }
            return int.Parse(esrb);
        }

        public static string ReadValidGenre()
        {
            int maxGenre = Game.genres.Length;
            for (int i = 0; i < maxGenre; i++)
            {
                Console.WriteLine($"{ i + 1}.{ Game.genres[i] }");
            }
            string stringGenre = Console.ReadLine(); ;
            bool isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            while (!isValidNumber)
            {
                Console.WriteLine($"Elija un número entre 1 y {maxGenre}");
                stringGenre = Console.ReadLine();
                isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            }
            int genrePos = int.Parse(stringGenre) - 1;
            return Game.genres[genrePos];
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
            bool isCorrectFormat = coverPath.EndsWith(LogicSpecification.IMAGE_EXTENSION);
            while (!(isValidPath && isCorrectFormat))
            {
                Console.WriteLine(errorMessage);
                coverPath = Console.ReadLine();
                isValidPath = fileHandler.FileExists(coverPath);
                isCorrectFormat = coverPath.EndsWith(LogicSpecification.IMAGE_EXTENSION);
            }
            return coverPath;
        }
        public static string ReadValidPathModify(string coverPath, string errorMessage, FileHandler.FileHandler fileHandler)
        {
            bool isValidPath = fileHandler.FileExists(coverPath);
            bool isCorrectFormat = coverPath.EndsWith(LogicSpecification.IMAGE_EXTENSION);
            while (!isValidPath && !isCorrectFormat)
            {
                Console.WriteLine(errorMessage);
                coverPath = Console.ReadLine();
                isValidPath = fileHandler.FileExists(coverPath);
                isCorrectFormat = coverPath.EndsWith(LogicSpecification.IMAGE_EXTENSION);
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


        public static string ReadValidFileName(string errorMessage, string folderPath, FileHandler.FileHandler fileHandler)
        {
            string name = Console.ReadLine();
            string completeFileName = GetValidCompleteFileName(folderPath, name);

            bool validFullPath = fileHandler.FileExists(completeFileName) && IsValidEntry(name);
            while (validFullPath)
            {
                Console.WriteLine(errorMessage);
                name = Console.ReadLine();
                completeFileName = GetValidCompleteFileName(folderPath, name);
                validFullPath = fileHandler.FileExists(completeFileName) && IsValidEntry(name);
            }
            return name;
        }

        public static string GetValidCompleteFileName(string path, string name)
        {
            if ((!path.Trim().EndsWith("\\")) && (!path.Trim().EndsWith("/")))
                path += "\\";

            string completeFileName = path + name + LogicSpecification.IMAGE_EXTENSION;
            return completeFileName;
        }

        public static int ReadValidESRBModify()
        {
            List<ESRBRating> possibleESRB = Enum.GetValues(typeof(ESRBRating)).Cast<ESRBRating>().ToList();
            for (int i = 0; i < possibleESRB.Count - 1; i++)
            {
                Console.WriteLine($"{ i + 1}.{ possibleESRB.ElementAt(i)}");
            }
            string esrb = Console.ReadLine();
            bool isANumber = IsValidNumber(esrb, 1, possibleESRB.Count - 1);
            while (!isANumber && esrb != "")
            {
                Console.WriteLine($"Elija un número entre 1 y {possibleESRB.Count - 1}");
                esrb = Console.ReadLine();
                isANumber = IsValidNumber(esrb, 1, possibleESRB.Count - 1);
            }
            if (isANumber) return int.Parse(esrb);
            return -1;

        }

        public static string ReadValidGenreModify()
        {
            int maxGenre = Game.genres.Length;
            for (int i = 0; i < maxGenre; i++)
            {
                Console.WriteLine($"{ i + 1}.{ Game.genres[i] }");
            }
            string stringGenre = Console.ReadLine();
            bool isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            while (!isValidNumber && stringGenre != "")
            {
                Console.WriteLine($"Elija un número entre 1 y {maxGenre} o vacio");
                stringGenre = Console.ReadLine();
                isValidNumber = IsValidNumber(stringGenre, 1, maxGenre);
            }
            if (isValidNumber)
            {
                int genrePos = int.Parse(stringGenre) - 1;
                return Game.genres[genrePos];
            }
            else { return ""; }

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
