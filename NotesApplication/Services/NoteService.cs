using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using NotesApplication.Models.DbModels;
using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Services
{
    public class NoteService : INoteService
    {
        private readonly NoteDbContext _db;
        private readonly IUserSettingsService _userSettingsService;
        
        public NoteService(NoteDbContext db, IUserSettingsService userSettingsService)
        {
            _db = db;
            _userSettingsService = userSettingsService;
        }

        public NotesListViewModel GetNotesViewModel(string orderBy, bool hideFinished)
        {
            var notesQuery = _db.Notes.AsQueryable();

            if (hideFinished)
            {
                notesQuery = notesQuery.Where(n => !n.FinishedAt.HasValue);
            }

            if (orderBy == "createdAt")
            {
                notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
            }
            else if (orderBy == "finishedAt")
            {
                notesQuery = notesQuery.OrderByDescending(n => n.FinishedAt);
            }
            else if (orderBy == "importance")
            {
                notesQuery = notesQuery.OrderByDescending(n => n.Importance);
            }

            var notes = notesQuery.ToList().Select(MapToViewModel).ToList();
            var availableSortOrders = _userSettingsService.GetAvailableSortOrderLabelByKeys();
            var currentSortOrder = _userSettingsService.GetCurrentSortOrderKey();
            
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
            var note = _db.Notes.FirstOrDefault(n => n.Id == id);

            return note != null ? MapToViewModel(note) : null;
        }

        public int AddNote(NoteFormModel model)
        {
            var note = CreateNote(model);
            _db.Notes.Add(note);
            _db.SaveChanges();

            return note.Id;
        }
        
        public bool UpdateNote(NoteFormModel model)
        {
            var note = _db.Notes.FirstOrDefault(n => n.Id == model.Id);
            
            if (note != null)
            {
                note.Title = model.Title;
                note.Description = model.Description;
                note.Importance = model.Importance;

                if (model.IsFinished && !note.FinishedAt.HasValue)
                {
                    note.FinishedAt = DateTime.Now;
                }
                else if (!model.IsFinished && note.FinishedAt.HasValue)
                {
                    note.FinishedAt = null;
                }
                else
                {
                    note.FinishedAt = model.FinishedAt;
                }

                _db.SaveChanges();

                return true;
            }

            return false;
        }
        
        public bool DeleteNote(int id)
        {
            var note = _db.Notes.FirstOrDefault(n => n.Id == id);

            if (note != null)
            {
                _db.Remove(note);
                _db.SaveChanges();

                return true;
            }

            return false;
        }

        private static Note CreateNote(NoteFormModel model)
        {
            var finishedAt = model.IsFinished ? DateTime.Now : model.FinishedAt;

            return new Note
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                FinishedAt = finishedAt,
                Importance = model.Importance    
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
                FinishedAt = note.FinishedAt,
                Importance = note.Importance
            };
        }
    }
}