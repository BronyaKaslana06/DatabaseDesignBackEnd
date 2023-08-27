namespace webapi.Tools
{
    public class Calculator
    {
        private const double EarthRadiusMeters = 6371000.0; // 地球半径 米

        public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusMeters * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        public static double ComputeSimilarityScore(string s, string keyword)
        {
            int[,] dp = new int[s.Length + 1, keyword.Length + 1];

            for (int i = 0; i <= s.Length; i++)
            {
                for (int j = 0; j <= keyword.Length; j++)
                {
                    if (i == 0)
                    {
                        dp[i, j] = j;
                    }
                    else if (j == 0)
                    {
                        dp[i, j] = i;
                    }
                    else if (s[i - 1] == keyword[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1];
                    }
                    else
                    {
                        dp[i, j] = 1 + Math.Min(Math.Min(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1]);
                    }
                }
            }

            // 关键词相似性分数，归一化
            double similarity = 1.0 - (double)dp[s.Length, keyword.Length] / keyword.Length;

            return similarity;
        }
    }
}
