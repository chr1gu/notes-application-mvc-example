using System;
using System.Collections.Generic;
using System.Linq;
using NotesApplication.Models.DbModels;
using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Services
{
    public class NoteService : INoteService
    {
        private readonly NoteDbContext db;
        
        public NoteService(NoteDbContext db)
        {
            this.db = db;
        }

        public NotesListViewModel GetNotesViewModel(string orderBy, bool hideFinished)
        {
            var notes = db.Notes.Select(MapToViewModel)
                //.OrderBy()
                .ToList();

            var availableSortOrders = new Dictionary<string, string>
            {
                {"finishedAt", "Finished date"},
                {"createdAt", "Creation date"},
                {"importance", "Importance"}
            };

            var currentSortOrder =
                availableSortOrders.ContainsKey(orderBy) ? orderBy : availableSortOrders.Keys.First();
            
            var viewModel = new NotesListViewModel
            {
                CurrentSortOrder = currentSortOrder,
                AvailableSortOrders = availableSortOrders,
                HideFinished = hideFinished,
                Notes = notes
            };

            return viewModel;
        }

        public NoteViewModel GetNoteViewModel(int id)
        {
            var note = db.Notes.FirstOrDefault(n => n.Id == id);

            return note != null ? MapToViewModel(note) : null;
        }

        public int AddNote(NoteFormModel model)
        {
            var note = CreateNote(model);
            db.Notes.Add(note);
            db.SaveChanges();

            return note.Id;
        }
        
        public bool UpdateNote(NoteFormModel model)
        {
            var note = db.Notes.FirstOrDefault(n => n.Id == model.Id);

            if (note != null)
            {
                note.Title = model.Title;
                note.Description = model.Description;
                note.FinishedAt = model.FinishedAt;
                db.SaveChanges();

                return true;
            }

            return false;
        }
        
        public bool DeleteNote(int id)
        {
            throw new NotImplementedException();
        }

        private static Note CreateNote(NoteFormModel model)
        {
            return new Note
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                FinishedAt = model.FinishedAt
            };
        }

        private static NoteViewModel MapToViewModel(Note note)
        {
            return new NoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                CreatedAt = note.CreatedAt,
                FinishedAt = note.FinishedAt
            };
        }
    }
}