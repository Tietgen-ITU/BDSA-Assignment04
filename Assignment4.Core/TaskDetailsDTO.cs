using System.Collections.Generic;

namespace Assignment4.Core
{
    public record TaskDetailsDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int? AssignedToId { get; init; }
        public string AssignedToName { get; init; }
        public string AssignedToEmail { get; init; }
        public IEnumerable<string> Tags { get; init; }
        public State State { get; init; }
    }
}
