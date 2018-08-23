using System;

namespace NotesApplication.Models.ViewModels
{
    public class NoteViewModel
    {
        public int? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Importance { get; set; }

        public bool IsFinished {
            get => FinishedAt.HasValue;
            set => FinishedAt = value ? DateTime.Now : (DateTime?)null;
        }
    }
}
