namespace GrammarChecker
{
    public class Data
    {
        public int Weight { get; set; }

        public Data(int weight = 0)
        {
            Weight = weight;
        }

        public override string ToString()
        {
            return Weight.ToString();
        }
    }
}