namespace rocks.kfs.StepsToCare.Model
{
    public partial class WorkerResult
    {
        public int Count { get; set; }
        public CareWorker Worker { get; set; }
        public bool HasCategory { get; set; } = false;
        public bool HasCampus { get; set; } = false;
        public bool HasAgeRange { get; set; } = false;
        public bool HasGender { get; set; } = false;
    }
}