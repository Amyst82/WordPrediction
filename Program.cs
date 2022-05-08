using System;

namespace WordPrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading data...");
            NGramPrediction.Init("PredictionData.txt");
            Console.WriteLine("Data loaded!");
            while(true)
            {
                Console.WriteLine("\nEnter a word or a sentence: ");
                string input = Console.ReadLine();
                string output = NGramPrediction.GetPredictedString(true, input, 10); //getting max 10 predicted words (duplicates will be excluded on output) using CDF
                Console.WriteLine($"Using CDF: {output}");
                string output2 = NGramPrediction.GetPredictedString(false, input, 10); //getting 10 most frequent predicted words (duplicates will be excluded on output)
                Console.WriteLine($"Most frequent: {output2}");

                string output3 = NGramPrediction.GetLastWord(input);
                Console.WriteLine($"Next word: {output3}");

            }

        }
    }
}
