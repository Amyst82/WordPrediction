# WordPrediction
### Word Prediction using concepts of N–grams and CDF.

## This repo is a C# adoptation of this article https://www.geeksforgeeks.org/word-prediction-using-concepts-of-n-grams-and-cdf/

# Example
```
Input: we
Output using CDF: we can approach this random word from it gives the smallest
Output most frequent: we can find the input in

Input: approach
Output using CDF: approach Note for each word that or equal the input
Output most frequent: approach this problem using the input in

Input: Now
Output using CDF: Now if you will get a different output can
Output most frequent: Now if you can find the input in
```

# Usage
```
NGramPrediction.Init("PredictionData.txt");
```
or
```
NGramPrediction.InitFromText("Some long text or whatever...");
```
Initialize class with a path to a file or with a text directly.

```
//getting max 10 predicted words (duplicates will be excluded on output) using CDF
string output = NGramPrediction.GetPredictedString(true, "Some text", 10); 
```
or get the list of predicted words (excluding duplicates)
```
var output = NGramPrediction.GetPredictedWords(true, "Some text", 10);
```
