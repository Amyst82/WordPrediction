using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WordPrediction
{
    public static class NGramPrediction
    {
        private static string[] wordsInCorpus;
        public static void Init(string path) //load data from file
        {
            var content = File.ReadAllText(path);
            InitFromText(content);
        }
        public static void InitFromText(string content) //load data from any provided text
        {
            var wordPattern = new Regex(@"\w+"); //word pattern (i guess)
            wordsInCorpus = wordPattern.Matches(content).Cast<Match>().Select(m => m.Value).ToArray(); //using regex to get list of words
        }
        private static Dictionary<string, int> countWordsInList(List<string> listOfWords) //Count frequency of each word in a list
        {
            Dictionary<string, int> words = new Dictionary<string, int>();
            for (int i = 0; i < listOfWords.Count; i++)
            {
                int currentCount = 0;
                words.TryGetValue(listOfWords[i], out currentCount);

                currentCount++;
                words[listOfWords[i]] = currentCount;
            }
            return words;
        }

        private static Dictionary<string, int> next_word_freq(string sentence) //This function calculates the freq of the (i+1)th word in the whole corpus, where i is the index of the sentence or the word.
        {
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                List<string> listFreq = new List<string>();
                int sen_len = sentence.Split(' ').Length;
                List<string> word_list = new List<string>();
                for (int i = 0; i < wordsInCorpus.Length - 1; i++)
                {
                    if (i + sen_len >= wordsInCorpus.Length)
                    {
                        sen_len = wordsInCorpus.Length - i;
                    }
                    string line = string.Join(" ", wordsInCorpus, i, sen_len).ToLower();
                    if (line == sentence.ToLower())
                    {
                        if (i + sen_len < wordsInCorpus.Length - 1)
                        {
                            //word_list.Add(wordsInCorpus[i + sen_len]);
                            listFreq.Add(wordsInCorpus[i + sen_len]);
                        }
                    }
                }

                return countWordsInList(listFreq);
            }
            else
            {
                return countWordsInList(new List<string>() { "" });
            }
        }
        private static Dictionary<string, double> CDF(string word) //Calculate the CDF of each word in the sentence
        {
            Dictionary<string, int> dict = next_word_freq(word);
            Dictionary<string, double> listFreq = new Dictionary<string, double>();
            double prob_sum = 0;
            double sum = dict.Values.Sum();
            for (int i = 0; i < dict.Count; i++)
            {
                double pmf = dict.ElementAt(i).Value / sum;
                prob_sum += pmf;
                listFreq.Add(dict.ElementAt(i).Key, prob_sum);
            }
            return listFreq;
        }
        static Random random = new Random();
        private static double GetRandomNumber(double minimum = 0d, double maximum = 1d)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public static string GetPredictedString(bool useCDF, string word, int len)
        {
            string outResult = word + " ";
            outResult += string.Join(" ", GetPredictedWords(useCDF, word, len));
            return outResult;
        }
        public static List<string> GetPredictedWords(bool useCDF, string word, int len)
        {
            var result = getPredicts(useCDF, word, len); //Trying get list of predicted words by the whole sentence
            if (result[0] == null) //In case if nothing found we search by the last word
            {
                var wordPattern = new Regex(@"\w+");
                string word2 = wordPattern.Matches(word).Cast<Match>().Select(m => m.Value).LastOrDefault(); //getting last word of sentence
                var tryByLastWord = getPredicts(useCDF, word2, len);
                if (tryByLastWord[0] == null)
                {
                    return new List<string>() { "" }; //Searching by last words didn't help :(
                }
            }
            return result;
        }
        private static List<string> getPredicts(bool useCDF, string word, int len) //Len is count of words you want to get predicted (initial word is not counted)
        {
            List<string> outResult = new List<string>();
            for (int i = 0; i < len; i++)
            {
                try
                {
                    if (useCDF)
                    {
                        double rand = GetRandomNumber(); //OP used a random number to predict the next word. The word having its CDF greater than or equal to rand and less than or equal to 1.
                        Dictionary<string, double> getCDF = CDF(word);
                        for (int j = 0; j < getCDF.Count; j++)
                        {
                            if (rand <= getCDF.ElementAt(j).Value)
                            {
                                outResult.Add(getCDF.ElementAt(j).Key);
                                word = getCDF.ElementAt(j).Key;
                                break;
                            }
                        }
                    }
                    else //In case you just want to get list of most frequent words that comes after your sentence/word
                    {
                        Dictionary<string, int> dict = next_word_freq(word);
                        string a = dict.OrderByDescending(x => x.Value).FirstOrDefault().Key;
                        outResult.Add(a);
                        word = a;
                    }

                }
                catch { }
            }
            var distinctItems = outResult.GroupBy(x => x).Select(y => y.First()).ToList(); //excluding duplicates

            return distinctItems;
        }
    }
}
