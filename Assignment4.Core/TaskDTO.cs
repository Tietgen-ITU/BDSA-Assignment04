using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Core
{
    public record TaskDTO(int Id, string Title, string AssignedToName, IReadOnlyCollection<string> Tags, State State);

    public record TaskDetailsDTO(int Id, string Title, string Description, DateTime Created, string AssignedToName, IReadOnlyCollection<string> Tags, State State, DateTime StateUpdated) : TaskDTO(Id, Title, AssignedToName, Tags, State);

    public record TaskCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; init; }

        public int? AssignedToId { get; init; }

        public string Description { get; init; }

        public ICollection<string> Tags { get; init; }
    }

    public record TaskUpdateDTO : TaskCreateDTO
    {
        public int Id { get; init; }

        public State State { get; init; }
    }
}