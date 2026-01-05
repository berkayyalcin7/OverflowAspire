using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Models
{
    public class Question
    {
        // GUID
        [MaxLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(300)]
        public required string Title { get; set; }
        [MaxLength(5000)]
        public required string Content { get; set; }
        [MaxLength(36)]
        public required string AskerId { get; set; }
        [MaxLength(100)]
        public required string AskerDisplayName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int ViewCount { get; set; } = 0;

        public List<string> TagSlugs { get; set; } = new List<string>();

        public bool HasAcceptedAnswer { get; set; }

        public int Votes { get; set; } = 0;

        public int AnswerCount { get; set; } = 0;

        public Answer[] Answer { get; set; } = [];


    }
}
