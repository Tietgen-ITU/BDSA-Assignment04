using System.ComponentModel.DataAnnotations;

namespace Assignment4.Core
{
    public record UserDTO(int Id, string Name, string Email);

    public record UserCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; }

        [EmailAddress]
        [Required]
        [StringLength(50)]
        public string Email { get; init; }
    }

    public record UserUpdateDTO : UserCreateDTO
    {
        public int Id { get; init; }
    }
}