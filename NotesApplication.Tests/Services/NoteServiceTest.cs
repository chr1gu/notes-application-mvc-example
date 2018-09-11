using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NotesApplication.Models.DbModels;
using NotesApplication.Models.FormModels;
using NotesApplication.Services;
using Xunit;

namespace NotesApplication.Tests.Services
{
    public class NotesServiceTest
    {
        private readonly NoteDbContext _dbContext;
        private readonly Mock<IUserSettingsService> _userSettingsServiceMock;
        
        public NotesServiceTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<NoteDbContext>();
            optionsBuilder.UseInMemoryDatabase("Testdatabase");
            
            _dbContext = new NoteDbContext(optionsBuilder.Options);
            _dbContext.Database.EnsureDeleted();
            
            _userSettingsServiceMock = new Mock<IUserSettingsService>();
        }
        
        [Fact]
        protected void GetNotesViewModel_WithoutNotes_ReturnsEmptyNotesList()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);

            // Act
            var result = service.GetNotesViewModel(orderBy: null, hideFinished: false);
            
            // Assert
            Assert.Empty(result.Notes);
        }
        
        [Fact]
        protected void GetNotesViewModel_WithoutFinishedNotesAndHideFinishedParameter_ReturnsAllNotes()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 1 }, new Note{ Id = 2 }, new Note{ Id = 3 });

            // Act
            var result = service.GetNotesViewModel(orderBy: null, hideFinished: true);
            
            // Assert
            Assert.Equal(3, result.Notes.Count);
        }
        
        [Fact]
        protected void GetNotesViewModel_WithFinishedNotesAndHideFinishedParameter_ReturnsUnFinishedNotes()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 1 }, new Note{ Id = 2, FinishedAt = DateTime.Today }, new Note{ Id = 3 });

            // Act
            var result = service.GetNotesViewModel(orderBy: null, hideFinished: true);
            
            // Assert
            Assert.Equal(2, result.Notes.Count);
            Assert.DoesNotContain(result.Notes, note => note.Id == 2);
        }
        
        [Fact]
        protected void GetNotesViewModel_WithFinishedNotesAndWithoutHideFinishedParameter_ReturnsAllNotes()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 1 }, new Note{ Id = 2, FinishedAt = DateTime.Today }, new Note{ Id = 3 });

            // Act
            var result = service.GetNotesViewModel(orderBy: null, hideFinished: false);
            
            // Assert
            Assert.Equal(3, result.Notes.Count);
            Assert.Contains(result.Notes, note => note.Id == 2);
        }
        
        [Fact]
        protected void GetNotesViewModel_WithCreatedAtOrder_ReturnsNotesOrderedByCreatedAtDescending()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, CreatedAt = DateTime.Today },
                new Note{ Id = 2, CreatedAt = DateTime.Today.AddHours(1) },
                new Note{ Id = 3, CreatedAt = DateTime.Today.AddHours(-1) });

            // Act
            var result = service.GetNotesViewModel(orderBy: "createdAt", hideFinished: false);
            
            // Assert
            Assert.True(result.Notes.Select(note => note.Id).SequenceEqual(new List<int?>{ 2, 1, 3 }));
        }
        
        [Fact]
        protected void GetNotesViewModel_WithFinishedAtOrder_ReturnsNotesOrderedByFinishedAtDescending()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, FinishedAt = DateTime.Today },
                new Note{ Id = 2, FinishedAt = DateTime.Today.AddHours(1) },
                new Note{ Id = 3, FinishedAt = DateTime.Today.AddHours(-1) });

            // Act
            var result = service.GetNotesViewModel(orderBy: "finishedAt", hideFinished: false);
            
            // Assert
            Assert.True(result.Notes.Select(note => note.Id).SequenceEqual(new List<int?>{ 2, 1, 3 }));
        }
        
        [Fact]
        protected void GetNotesViewModel_WithUnFinishedNotesAndFinishedAtOrder_ReturnsNotesOrderedByFinishedAtDescendingAndOthersAtTheEnd()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, FinishedAt = DateTime.Today },
                new Note{ Id = 2, FinishedAt = DateTime.Today.AddHours(1) },
                new Note{ Id = 3, FinishedAt = null },
                new Note{ Id = 4, FinishedAt = DateTime.Today.AddHours(-1) },
                new Note{ Id = 5, FinishedAt = null });

            // Act
            var result = service.GetNotesViewModel(orderBy: "finishedAt", hideFinished: false);
            
            // Assert
            Assert.True(result.Notes.Select(note => note.Id).SequenceEqual(new List<int?>{ 2, 1, 4, 3, 5 }));
        }
        
        [Fact]
        protected void GetNotesViewModel_WithUnknownOrder_ReturnsNotesInOriginalOrder()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, CreatedAt = DateTime.Today },
                new Note{ Id = 2, CreatedAt = DateTime.Today.AddHours(1) },
                new Note{ Id = 3, CreatedAt = DateTime.Today.AddHours(-1) });

            // Act
            var result = service.GetNotesViewModel(orderBy: "unknown", hideFinished: false);
            
            // Assert
            Assert.True(result.Notes.Select(note => note.Id).SequenceEqual(new List<int?>{ 1, 2, 3 }));
        }

        [Fact]
        protected void GetNotesViewModel_WithImportanceOrder_ReturnsNotesOrderedImportanceDescending()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Importance = 3 },
                new Note{ Id = 2, Importance = 5  },
                new Note{ Id = 3, Importance = 1  });

            // Act
            var result = service.GetNotesViewModel(orderBy: "importance", hideFinished: false);
            
            // Assert
            Assert.True(result.Notes.Select(note => note.Id).SequenceEqual(new List<int?>{ 2, 1, 3 }));
        }
        
        [Fact]
        protected void GetNoteViewModel_WithExistingNote_ReturnsMatchingNote()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Title = "Note 1" },
                new Note{ Id = 2, Title = "Note 2" },
                new Note{ Id = 3, Title = "Note 3" });

            // Act
            var result = service.GetNoteViewModel(2);
            
            // Assert
            Assert.Equal("Note 2", result.Title);
        }
        
        [Fact]
        protected void GetNoteViewModel_WithoutExistingNote_ReturnsNull()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Title = "Note 1" },
                new Note{ Id = 2, Title = "Note 2" },
                new Note{ Id = 3, Title = "Note 3" });

            // Act
            var result = service.GetNoteViewModel(5);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        protected void AddNote_WithNewNote_ReturnsNewNoteIdAndInsertsIntoDb()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);

            // Act
            var result = service.AddNote(new NoteFormModel{ Title = "Another Note" });
            
            // Assert
            Assert.Equal(1, result);
            Assert.Equal("Another Note", _dbContext.Notes.First(n => n.Id == result).Title);
        }
        
        [Fact]
        protected void DeleteNote_WithExistingNote_ReturnsTrueAndRemovesNoteFromDb()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Title = "Note 1" },
                new Note{ Id = 2, Title = "Note 2" },
                new Note{ Id = 3, Title = "Note 3" });

            // Act
            var result = service.DeleteNote(2);
            
            // Assert
            Assert.True(result);
            Assert.False(_dbContext.Notes.Any(n => n.Id == 2));
        }
        
        [Fact]
        protected void DeleteNote_WithoutExistingNote_ReturnsFalseAndRemovesNothingFromDb()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Title = "Note 1" },
                new Note{ Id = 2, Title = "Note 2" },
                new Note{ Id = 3, Title = "Note 3" });

            // Act
            var result = service.DeleteNote(5);
            
            // Assert
            Assert.False(result);
            Assert.Equal(3, _dbContext.Notes.Count());
        }
        
        [Fact]
        protected void UpdateNote_WithoutData_ReturnsFalse()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);

            // Act
            var result = service.UpdateNote(new NoteFormModel { Id = 5 });
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        protected void UpdateNote_WithoutMatchingNote_ReturnsFalse()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(
                new Note{ Id = 1, Title = "Note 1" },
                new Note{ Id = 3, Title = "Note 3" });

            // Act
            var result = service.UpdateNote(new NoteFormModel { Id = 2 });
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        protected void UpdateNote_WithMatchingNote_UpdatesTitleDescriptionAndImportance()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 2, Title = "Note 2", Description = "Description 2", Importance = 2 });

            // Act
            var result = service.UpdateNote(new NoteFormModel
            {
                Id = 2,
                Title = "Updated Title",
                Description = "Updated Description",
                Importance = 5
            });
            
            // Assert
            var note = _dbContext.Notes.Find(2);
            Assert.Equal("Updated Title", note.Title);
            Assert.Equal("Updated Description", note.Description);
            Assert.Equal(5, note.Importance);
            Assert.True(result);
        }
        
        [Fact]
        protected void UpdateNote_WithUnfinishedNoteAndFinishedDate_SetsGivenFinishedDate()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 2 });

            // Act
            var now = DateTime.Now;
            var result = service.UpdateNote(new NoteFormModel
            {
                Id = 2,
                FinishedAt = now
            });
            
            // Assert
            Assert.True(_dbContext.Notes.Find(2).FinishedAt == now);
            Assert.True(result);
        }
        
        [Fact]
        protected void UpdateNote_WithFinishedNoteAndFinishedDateButUncheckedIsFinished_RemovesFinishedAtDate()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 2, FinishedAt = DateTime.Now.AddDays(-1) });

            // Act
            var result = service.UpdateNote(new NoteFormModel
            {
                Id = 2,
                FinishedAt = DateTime.Now,
                IsFinished = false
            });
            
            // Assert
            Assert.True(_dbContext.Notes.Find(2).FinishedAt == null);
            Assert.True(result);
        }
        
        [Fact]
        protected void UpdateNote_WithFinishedNoteAndFinishedDateAndCheckedIsFinished_UpdatesFinishedAtDate()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 2, FinishedAt = DateTime.Now.AddDays(-1) });

            // Act
            var now = DateTime.Now;
            var result = service.UpdateNote(new NoteFormModel
            {
                Id = 2,
                FinishedAt = now,
                IsFinished = true
            });
            
            // Assert
            Assert.True(_dbContext.Notes.Find(2).FinishedAt == now);
            Assert.True(result);
        }
        
        [Fact]
        protected void UpdateNote_WithUnfinishedNoteAndCheckedIsFinished_SetsFinishedAtDateToNow()
        {
            // Arrange
            var service = new NoteService(_dbContext, _userSettingsServiceMock.Object);
            AddTestData(new Note{ Id = 2, FinishedAt = null });

            // Act
            var result = service.UpdateNote(new NoteFormModel
            {
                Id = 2,
                IsFinished = true
            });
            
            // Assert
            var finishedAt = _dbContext.Notes.Find(2).FinishedAt;
            Assert.True(finishedAt.HasValue && finishedAt.Value > DateTime.Now.AddMinutes(-1));
            Assert.True(result);
        }
        
        

        private void AddTestData(params Note[] notes)
        {
            _dbContext.Notes.AddRange(notes);
            _dbContext.SaveChanges();
        }
    }
}