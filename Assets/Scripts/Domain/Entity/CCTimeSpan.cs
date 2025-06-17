namespace Domain.Entity
{
    public class CCTimeSpan
    {
        private readonly CCDateTime startDate;
        private readonly CCDateTime endDate;

        public CCTimeSpan(CCDateTime startDate, CCDateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public bool IsBetween(CCDateTime date)
        {
            return this.startDate <= date && date <= this.endDate;
        }
    }
}
