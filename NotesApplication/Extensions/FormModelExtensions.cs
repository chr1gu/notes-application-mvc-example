using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Extensions
{
    public static class FormModelExtensions
    {
        public static NoteFormModel ToFormModel(this NoteViewModel viewModel)
        {
            return new NoteFormModel
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                FinishedAt = viewModel.FinishedAt,
                Title = viewModel.Title,
                Description = viewModel.Description,
                Importance = viewModel.Importance,
                IsFinished = viewModel.IsFinished
            };
        }
    }
}