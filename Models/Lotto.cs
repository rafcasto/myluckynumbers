namespace DotNetCoreSelenium.Models
{
    public class Lotto
    {
        public int DrawNumber { get; set; }
        public string DrawDay { get; set; }
        public string DrawDate { get; set; }
        public string DrawTime { get; set; }
        public LottoWinningNumbers LottoWinningNumbers { get; set; }

    }
}