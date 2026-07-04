namespace SportAcademy.Application.DTOs.SubscriptionDetailsDtos
{
    public class SubDetailsDropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly EndDate { get; set; }
        public int SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
    }
}
