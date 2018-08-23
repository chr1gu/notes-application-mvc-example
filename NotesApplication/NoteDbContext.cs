using Microsoft.EntityFrameworkCore;
using NotesApplication.Models.DbModels;

namespace NotesApplication
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options)
        {
        }
 
        public DbSet<Note> Notes { get; set; }
    }
}