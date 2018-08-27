using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Internal;

namespace NotesApplication.Models.FormModels
{
    public class NoteFormModel
    {
        public int? Id { get; set; }

        [Display(Name = "Created at")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Finished at")]
        [DataType(DataType.Date)] 
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? FinishedAt { get; set; }
        
        [Required]
        [StringLength(255)]
        [Display(Prompt = "Note title")]
        public string Title { get; set; }

        [StringLength(4000)]
        [Display(Prompt = "Note description")]
        public string Description { get; set; }

        [Range(0, 5)]
        public int Importance { get; set; } = 2;
        
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
