using System;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Models.DbModels;

namespace NotesApplication.Services
{
    public class TestDataService : ITestDataService
    {
        private readonly NoteDbContext _db;
        
        public TestDataService(NoteDbContext db)
        {
            _db = db;
        }

        public void RestoreTestData()
        {
            foreach (var note in _db.Notes)
            {
                _db.Entry(note).State = EntityState.Deleted;
            }
            
            _db.Notes.Add(new Note
            {
                Title = "Yeah it's called a shrug..",
                Description = @"¯\_(ツ)_/¯",
                Importance = 2,
                CreatedAt = DateTime.Now
            });
            
            _db.Notes.Add(new Note
            {
                Title = "Important Note",
                Description = "This one is a very important note:\nDON'T DELETE ME",
                Importance = 4,
                CreatedAt = DateTime.Now.AddDays(-2).AddHours(-1)
            });
            
            _db.Notes.Add(new Note
            {
                Title = "Finished Note",
                Description = "An already finished note! Wooh",
                Importance = 3,
                CreatedAt = DateTime.Now.AddHours(-20),
                FinishedAt = DateTime.Now.AddHours(-2)
            });
 
            _db.SaveChanges();
        }
    }
}