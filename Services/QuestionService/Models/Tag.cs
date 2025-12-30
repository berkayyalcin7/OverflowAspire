using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Models
{
    public class Tag
    {
        [MaxLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Slug { get; set; }
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;


    }
}
