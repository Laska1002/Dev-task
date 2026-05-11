namespace DevTaskManager.Domain.Entities
{
    public class DeveloperTechnology
    {
        public uint Id { get; set; }
        public uint DeveloperId { get; set; }
        public uint TechnologyId { get; set; }

        public Developer Developer { get; set; } = null!;
        public Technology Technology { get; set; } = null!;
    }
}
