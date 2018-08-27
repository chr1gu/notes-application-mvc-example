using System;

namespace NotesApplication.Models.DbModels
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Description { get; set; }
        public int Importance { get; set; }
    }
}