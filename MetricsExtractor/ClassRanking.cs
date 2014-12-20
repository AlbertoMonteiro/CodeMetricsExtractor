namespace MetricsExtractor
{
    public class ClassRanking
    {
        public ClassRanking(string className, ClassRank rank)
        {
            ClassName = className;
            Rank = rank;
        }

        public string ClassName { get; private set; }

        public ClassRank Rank { get; private set; }
    }
}