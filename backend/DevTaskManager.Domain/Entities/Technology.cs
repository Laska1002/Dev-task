namespace DevTaskManager.Domain.Entities
{
    public class Technology
    {
        public uint Id { get; set; }
        public uint ProjectTypeId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public ProjectType ProjectType { get; set; } = null!;
    }
}
