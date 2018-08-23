using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NotesApplication.Models.DbModels;

namespace NotesApplication.Extensions
{
    public static class TestDataExtensions
    {
        public static IApplicationBuilder UseTestData(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<NoteDbContext>();
            
            context.Notes.Add(new Note
            {
                Title = "Note 1",
                Description = "Description 1",
                CreatedAt = DateTime.Now
            });
            
            context.Notes.Add(new Note
            {
                Title = "Note 2",
                Description = "Description 1",
                CreatedAt = DateTime.Now.AddDays(-2)
            });
            
            context.Notes.Add(new Note
            {
                Title = "Finished Note",
                Description = "This is already finisehd",
                CreatedAt = DateTime.Now.AddHours(-20),
                FinishedAt = DateTime.Now.AddHours(-2)
            });
 
            context.SaveChanges();

            return app;
        }
    }
}