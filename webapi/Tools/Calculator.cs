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
            int m = 0;
            if(s != null) m = s.Length;
            int n = keyword.Length;

            int[,] dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                        dp[i, j] = 0;
                    else if (s[i - 1] == keyword[j - 1])
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    else
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }

            // 计算相似度分数
            double similarity = (double)dp[m, n] / Math.Max(m, n);

            return similarity;
        }

    }
}
