using System;

namespace com.kfs.Reach.Reporting
{
    /// <summary>
    /// Reach Sponsorship Supporter Object
    /// </summary>
    public class Supporter
    {
        public int? account_id;
        public DateTime? created_at;
        public int? id;
        public string payment_type;
        public string recurring_period;
        public string share_type_id;
        public Sponsorship sponsorship;
        public int? sponsorship_id;
        public int? supporter_id;
        public decimal? total_given;
        public decimal? total_needed;
    }

    /// <summary>
    /// Reach Sponsorship Object
    /// </summary>
    public class Sponsorship
    {
        public string full_url;
        public int? id;
        public string permalink;
        public Place place;
        public int? sponsorship_type_id;
        public string sponsorship_type_permalink;
        public string sponsorship_type_title;
    }

    /// <summary>
    /// Reach Place Object
    /// </summary>
    public class Place
    {
        public DateTime? created_at;
        public string description;
        public int? id;
        public string permalink;
        public string title;
    }
}
