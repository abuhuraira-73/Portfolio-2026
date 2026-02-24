using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace VS_portfolio_2026.Models
{
    public class GitHubContributionData
    {
        [JsonPropertyName("total")]
        public Dictionary<string, int> Total { get; set; }

        [JsonPropertyName("contributions")]
        public List<Contribution> Contributions { get; set; }
    }

    public class Contribution
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }
    }

    public class CalendarViewModel
    {
        public List<string> MonthLabels { get; set; } = new List<string>();
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
    }

    public class CalendarDay
    {
        public string Date { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
    }
}
