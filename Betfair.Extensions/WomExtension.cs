namespace Betfair.Extensions
{
    public static class WomExtension
    {
        public static double Wom(this RunnerCache runner, double levels = 1)
        {
            if (runner is null) return 0;
            if (runner.BestAvailableToBack.Size(0) <= 0) return 0;

            var backSize = 0.0;
            var laySize = 0.0;
            for (var i = 0; i < levels; i++)
            {
                backSize += runner.BestAvailableToBack.Size(i);
                laySize += runner.BestAvailableToLay.Size(i);
            }

            return backSize / (backSize + laySize);
        }
    }
}
