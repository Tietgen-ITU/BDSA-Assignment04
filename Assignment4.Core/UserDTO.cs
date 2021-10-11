using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Core
{
    public record UserDTO(int Id, string Name, string Email, IReadOnlyCollection<Task> Tasks);

    public record UserDetailsDTO(int Id, string Name, string Email,IReadOnlyCollection<Task> Tasks) : UserDTO(Id, Name,Email,Tasks);

    public record UserCreateDTO
    {
       [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }

    public record UserUpdateDTO : UserCreateDTO
    {
        public int Id { get; init; }

    }
}