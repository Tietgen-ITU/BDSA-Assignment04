using System.ComponentModel.DataAnnotations;

namespace Assignment4.Core
{
    public record TagDTO(int Id, string Name);

    public record TagCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; }
    }

    public record TagUpdateDTO : TagCreateDTO
    {
        public int Id { get; init; }
    }
}