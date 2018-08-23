using System;
using System.ComponentModel.DataAnnotations;

namespace NotesApplication.Models.FormModels
{
    public class NoteFormModel
    {
        public int? Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        
        [StringLength(255)]
        [Required]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }
        
        public int Importance { get; set; }
        public bool IsFinished { get; set; }

        public static NoteFormModel New()
        {
            return new NoteFormModel
            {
                CreatedAt = DateTime.Now
            };
        }
    }
}
