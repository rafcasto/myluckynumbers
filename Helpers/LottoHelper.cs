using System.Collections.Generic;
using DotNetCoreSelenium.Models;
using DotNetCoreSelenium.Repositories;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;

namespace DotNetCoreSelenium.Helpers
{
    public class LottoHelper
    {
        private Dictionary<int, IEnumerable<IGrouping<string, string>>> _myLuckyNumbers;
        private Dictionary<int, List<MyLuckyNumbers>> _nextLotteryNumbers;
        private List<MyLuckyNumbers> _LuckNumberForNextLotto;

        private LottoRepository _lottoRepository;
        private List<Lotto> _lottoWinningNumbers;
        private List<Lotto> _luckyNumbers;
        private List<string[]> _lottoNumbersHistory;
        private string _lottoNumbersPath = "lottoNumbers.json";
        private string _winnerNumbers = "winnerNumbers.json";
        private List<MyLuckyNumbers> _LuckNumberForNextLottoClassA;

        private List<MyLuckyNumbers> _LuckNumberForNextLottoClassB;

        private List<MyLuckyNumbers> _LuckNumberForNextLottoClassC;

        private List<MyLuckyNumbers> _LuckNumberForNextLottoClassD;

        private List<MyLuckyNumbers> _LuckNumberForNextLottoClassE;
        private string[] _categories;
        public LottoHelper()
        {
            _lottoRepository = new LottoRepository();
            _lottoWinningNumbers = new List<Lotto>();
            _luckyNumbers = new List<Lotto>();
            _lottoNumbersHistory = new List<string[]>();
            _LuckNumberForNextLottoClassA = new List<MyLuckyNumbers>();
            _LuckNumberForNextLottoClassB = new List<MyLuckyNumbers>();
            _LuckNumberForNextLottoClassC = new List<MyLuckyNumbers>();
            _LuckNumberForNextLottoClassD = new List<MyLuckyNumbers>();
            _LuckNumberForNextLottoClassE = new List<MyLuckyNumbers>();
            _myLuckyNumbers = new Dictionary<int, IEnumerable<IGrouping<string, string>>>();
            _nextLotteryNumbers = new Dictionary<int, List<MyLuckyNumbers>>();
            _LuckNumberForNextLotto = new List<MyLuckyNumbers>();
            _categories = new string[] { "A", "B", "C", "D", "E" };
        }


        public Dictionary<string, List<MyLuckyNumbers>> GetListOfMyNextWinningNumbers(bool loadFromService)
        {
            Dictionary<string, List<MyLuckyNumbers>> listOfWinnerNumbers = new Dictionary<string, List<MyLuckyNumbers>>();
            var lotoNumbers = GetLuckyNumbersFrom(1725, 1828, false);
            PrintLottoNumbers(lotoNumbers);

            _myLuckyNumbers = GetMyLuckNumbers(lotoNumbers);
            if (loadFromService)
            {

                _nextLotteryNumbers = SortLuckyNumbers(_myLuckyNumbers);
                listOfWinnerNumbers = GetNextLuckyNumbers(_nextLotteryNumbers);

            }

            foreach (var cat in _categories)
            {

                var isLottoNumbersPresent = File.Exists(cat + _winnerNumbers);
                if (isLottoNumbersPresent && !loadFromService)
                {
                    using (StreamReader r = new StreamReader(cat + _winnerNumbers))
                    {
                        string json = r.ReadToEnd();
                        var winnerNumberInLoop = JsonConvert.DeserializeObject<Dictionary<string, List<MyLuckyNumbers>>>(json);
                        listOfWinnerNumbers.Add(cat, winnerNumberInLoop[cat]);
                    }

                }

            }

            return GetWinnerNumbers(listOfWinnerNumbers, _myLuckyNumbers);


        }

        public List<string[]> GetLuckyNumbersFrom(int fromLottoGameId, int toLottoGameId, bool newLottoList)
        {


            var isLottoNumbersPresent = File.Exists(_lottoNumbersPath);
            if (isLottoNumbersPresent)
            {
                using (StreamReader r = new StreamReader(_lottoNumbersPath))
                {
                    string json = r.ReadToEnd();
                    _lottoNumbersHistory = JsonConvert.DeserializeObject<List<string[]>>(json);
                }

            }
            if (newLottoList || !_lottoNumbersHistory.Any())
            {
                var listOfWinningNumbers = GetLastWinningNumbers(fromLottoGameId, toLottoGameId);
                var sortWinnerNumbers = listOfWinningNumbers.
                OrderBy(x => x.LottoWinningNumbers.Numbers[0]).
                ThenBy(x => x.LottoWinningNumbers.Numbers[1]).
                ThenBy(x => x.LottoWinningNumbers.Numbers[2]).
                ThenBy(x => x.LottoWinningNumbers.Numbers[3]).
                ThenBy(x => x.LottoWinningNumbers.Numbers[4]).
                ThenBy(x => x.LottoWinningNumbers.Numbers[5]);

                foreach (var number in sortWinnerNumbers)
                {
                    _lottoNumbersHistory.Add(number.LottoWinningNumbers.Numbers);
                }
                SaveLottoNumbers();
            }



            //  var sortedNumbers = from element in _lottoNumbersHistory orderby element[0] select element;
            return _lottoNumbersHistory;
        }

        private void SaveLottoNumbers()
        {
            using (StreamWriter file = File.CreateText("lottoNumbers.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, _lottoNumbersHistory);
            }
            // var lottoNumbersJson = JsonConvert.SerializeObject(_lottoNumbersHistory);
            // File.WriteAllText(_lottoNumbersPath, lottoNumbersJson);
        }

        private void SaveWinnerNumbers(Dictionary<string, List<MyLuckyNumbers>> winnerNumbers, string category)
        {
            using (StreamWriter file = File.CreateText(category + _winnerNumbers))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, winnerNumbers);
            }
        }

        private List<Lotto> GetLastWinningNumbers(int fromLottoGameId, int toLottoGameId)
        {

            var lottoId = fromLottoGameId;
            while (lottoId <= toLottoGameId)
            {
                var lottoNumber = _lottoRepository.GetLottoBy(lottoId.ToString());
                _lottoWinningNumbers.Add(lottoNumber);
                lottoId++;
            }
            return _lottoWinningNumbers;
        }

        public void PrintLottoNumbers(List<string[]> lotoNumbers)
        {
            TextWriter tw = new StreamWriter("log2.txt");

            foreach (var numbers in lotoNumbers)
            {

                string lotoNumber = "";
                foreach (var lotoNum in numbers)
                {
                    lotoNumber += lotoNum;
                }
                tw.WriteLine(lotoNumber);
            }
            tw.Close();

        }

        public Dictionary<int, IEnumerable<IGrouping<string, string>>> GetMyLuckNumbers(List<string[]> lotoNumbers)
        {
            Dictionary<int, IEnumerable<IGrouping<string, string>>> myLuckyNumbers = new Dictionary<int, IEnumerable<IGrouping<string, string>>>();
            for (int i = 0; i <= 5; i++)
            {
                var numberInColum = lotoNumbers.Select(x => x[i]).ToList();
                myLuckyNumbers.Add(i, numberInColum.GroupBy(x => x).OrderByDescending(x => x.Count()));
            }
            return myLuckyNumbers;
        }

        public Dictionary<int, List<MyLuckyNumbers>> SortLuckyNumbers(Dictionary<int, IEnumerable<IGrouping<string, string>>> listOfLuckyNumbers)
        {
            Dictionary<int, List<MyLuckyNumbers>> luckyNumbers = new Dictionary<int, List<MyLuckyNumbers>>();
            foreach (var element in listOfLuckyNumbers)
            {
                List<MyLuckyNumbers> myLuckyNUmbers = new List<MyLuckyNumbers>();
                //    tw.WriteLine("Coulum " + element.Key);
                var oderLIst = element.Value;
                foreach (var numberinList in oderLIst)
                {
                    var myNumber = new MyLuckyNumbers
                    {
                        Frequeny = numberinList.Count(),
                        Number = numberinList.Key
                    };

                    myLuckyNUmbers.Add(myNumber);


                    //   tw.WriteLine("number {0}  frequency {1}", numberinList.Key, numberinList.Count());
                }
                luckyNumbers.Add(element.Key, myLuckyNUmbers);
            }
            return luckyNumbers;


        }

        public Dictionary<string, List<MyLuckyNumbers>> GetNextLuckyNumbers(Dictionary<int, List<MyLuckyNumbers>> luckyNumbers)
        {
            Dictionary<string, List<MyLuckyNumbers>> luckyNumbersDictionary = new Dictionary<string, List<MyLuckyNumbers>>();
            string lottoNumber = "";

            var column1 = luckyNumbers[0];
            var column2 = luckyNumbers[1];
            var column3 = luckyNumbers[2];
            var column4 = luckyNumbers[3];
            var column5 = luckyNumbers[4];
            var column6 = luckyNumbers[5];



            // lottoNumbersCol1 
            foreach (var numbersInCoulmn1 in column1)
            {
                var totalOccurences = 0;


                foreach (var numbersInCoulmn2 in column2)
                {

                    foreach (var numbersInColumn3 in column3)
                    {

                        foreach (var numbersInColumn4 in column4)
                        {

                            foreach (var numbersInColumn5 in column5)
                            {

                                foreach (var numbersInColumn6 in column6)
                                {

                                    var totalFrequency = numbersInCoulmn1.Frequeny +
                                                         numbersInCoulmn2.Frequeny +
                                                         numbersInColumn3.Frequeny +
                                                         numbersInColumn4.Frequeny +
                                                         numbersInColumn5.Frequeny +
                                                         numbersInColumn6.Frequeny;

                                    lottoNumber = numbersInCoulmn1.Number +
                          numbersInCoulmn2.Number +
                          numbersInColumn3.Number +
                          numbersInColumn4.Number +
                          numbersInColumn5.Number +
                          numbersInColumn6.Number;

                                    var arrayLotoNumber = new string[]
                                                      {
                                                   numbersInCoulmn1.Number ,
                                                      numbersInCoulmn2.Number ,
                                                      numbersInColumn3.Number ,
                                                      numbersInColumn4.Number ,
                                                      numbersInColumn5.Number ,
                                                      numbersInColumn6.Number
                                                          };
                                    var arrayHasDuplicateNumbers = arrayLotoNumber.Length != arrayLotoNumber.Distinct().Count();

                                    if (!arrayHasDuplicateNumbers)
                                    {
                                        MyLuckyNumbers luckNumber = new MyLuckyNumbers()
                                        {
                                            Frequeny = totalFrequency,
                                            Number = lottoNumber,
                                            LotoNumber = arrayLotoNumber
                                        };

                                        if (totalFrequency >= 35 && totalFrequency < 40)
                                        {

                                            _LuckNumberForNextLottoClassA.Add(luckNumber);
                                        }
                                        else if (totalFrequency >= 40 && totalFrequency < 45)
                                        {
                                            _LuckNumberForNextLottoClassB.Add(luckNumber);
                                        }
                                        else if (totalFrequency >= 45 && totalFrequency < 50)
                                        {
                                            _LuckNumberForNextLottoClassC.Add(luckNumber);
                                        }
                                        else if (totalFrequency >= 50 && totalFrequency < 55)
                                        {
                                            _LuckNumberForNextLottoClassD.Add(luckNumber);
                                        }
                                        else if (totalFrequency >= 55 && totalFrequency < 60)
                                        {
                                            _LuckNumberForNextLottoClassE.Add(luckNumber);
                                        }

                                        totalOccurences++;
                                    }





                                }
                            }

                        }
                    }

                }

            }

            luckyNumbersDictionary.Add("A", _LuckNumberForNextLottoClassA);

            luckyNumbersDictionary.Add("B", _LuckNumberForNextLottoClassB);

            luckyNumbersDictionary.Add("C", _LuckNumberForNextLottoClassC);

            luckyNumbersDictionary.Add("D", _LuckNumberForNextLottoClassD);

            luckyNumbersDictionary.Add("E", _LuckNumberForNextLottoClassE);
            SaveWinnerNumbers(new Dictionary<string, List<MyLuckyNumbers>>(){
                {"A",_LuckNumberForNextLottoClassA}
            }, "A");
            SaveWinnerNumbers(new Dictionary<string, List<MyLuckyNumbers>>(){
                {"B",_LuckNumberForNextLottoClassB}
            }, "B");
            SaveWinnerNumbers(new Dictionary<string, List<MyLuckyNumbers>>(){
                {"C",_LuckNumberForNextLottoClassC}
            }, "C");
            SaveWinnerNumbers(new Dictionary<string, List<MyLuckyNumbers>>(){
                {"D",_LuckNumberForNextLottoClassD}
            }, "D");
            SaveWinnerNumbers(new Dictionary<string, List<MyLuckyNumbers>>(){
                {"E",_LuckNumberForNextLottoClassE}
            }, "E");
            return luckyNumbersDictionary;
        }



        public Dictionary<string, List<MyLuckyNumbers>> GetWinnerNumbers(
            Dictionary<string, List<MyLuckyNumbers>> myNextLuckNumbers, Dictionary<int, IEnumerable<IGrouping<string, string>>> _myLuckyNumbers)
        {
            Dictionary<string, List<MyLuckyNumbers>> myWinnerNumbers = new Dictionary<string, List<MyLuckyNumbers>>();
            var firstLuckyRow = _myLuckyNumbers[0];
            var secondLukyRow = _myLuckyNumbers[1];
            var thirdLukyRow = _myLuckyNumbers[2];
            var fourthLukyRow = _myLuckyNumbers[3];
            foreach (var element in myNextLuckNumbers)
            {
                List<MyLuckyNumbers> listOfWinnerNumbers = new List<MyLuckyNumbers>();


                if (string.Equals(element.Key, "E") || string.Equals(element.Key, "D"))
                {
                    TextWriter lottoNumbersOnClass = new StreamWriter($"Category{element.Key}_luckyNumbers.txt");
                    foreach (var luckyLottoNumber in firstLuckyRow)
                    {
                      
                                    var allLuckyNumber = 
                                    element.Value.FindAll(x => 
                                    string.Equals(x.LotoNumber[0], luckyLottoNumber.Key) 
                                        );
                                    if (allLuckyNumber != null && allLuckyNumber.Any() )
                                    {

                                        var max =  allLuckyNumber.Count -1 ;
                                        var min = 0;
                                        Random rdm = new Random();
                                        var randomLuckyNumber = rdm.Next(min, max);
                                        var winnerNumber = allLuckyNumber.ElementAt(randomLuckyNumber);
                                        lottoNumbersOnClass.WriteLine($"{winnerNumber.Number} {winnerNumber.Frequeny} element at {randomLuckyNumber}");
                                        listOfWinnerNumbers.Add(winnerNumber);
                            
                        }



                    }
                    lottoNumbersOnClass.Close();
                    myWinnerNumbers.Add(element.Key, listOfWinnerNumbers);
                }

                /*TextWriter lottoNumbersOnClass = new StreamWriter($"Category{element.Key}_luckyNumbers.txt");
                foreach (var luckyLottoNumber in firstLuckyRow)
                {
                    var allLuckyNumber = element.Value.FindAll(x => string.Equals(x.LotoNumber[0], luckyLottoNumber.Key));
                    if (allLuckyNumber != null && allLuckyNumber.Any())
                    {

                        var max = allLuckyNumber.Count - 1;
                        var min = 0;
                        Random rdm = new Random();
                        var randomLuckyNumber = rdm.Next(min, max);
                        var winnerNumber = allLuckyNumber.ElementAt(randomLuckyNumber);
                        lottoNumbersOnClass.WriteLine($"{winnerNumber.Number} {winnerNumber.Frequeny} element at {randomLuckyNumber}");
                        listOfWinnerNumbers.Add(winnerNumber);
                    }


                }
                lottoNumbersOnClass.Close();
                myWinnerNumbers.Add(element.Key, listOfWinnerNumbers);
 */
            }

            return myWinnerNumbers;
        }




        private void printLuckyNumgers(string category, int firstNumber, List<MyLuckyNumbers> luckyNumbers)
        {
            TextWriter documentLOg = new StreamWriter($"Category{category}_{firstNumber}.txt");
            foreach (var lottoNumbers in luckyNumbers)
            {
                documentLOg.WriteLine($"{lottoNumbers.Number} total frequency {lottoNumbers.Frequeny}");
            }
            documentLOg.Close();
        }

    }
}