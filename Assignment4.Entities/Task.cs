using System.ComponentModel.DataAnnotations;
using Assignment4.Core;
using System.Collections.Generic;
namespace Assignment4.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        public User AssignedTo { get; set; }
                
        public string Description { get; set; }
        [Required]
        public State state { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}
