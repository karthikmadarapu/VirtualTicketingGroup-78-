using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtualTicketing.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;


        // ✅ Navigation property
        public ICollection<Event>? Events { get; set; }
    }
}