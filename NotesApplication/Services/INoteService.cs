using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Services
{
    public interface INoteService
    {
        NotesListViewModel GetNotesViewModel(string orderBy, bool hideFinished);
        
        NoteViewModel GetNoteViewModel(int id);

        int AddNote(NoteFormModel model);
        
        bool UpdateNote(NoteFormModel model);
        
        bool DeleteNote(int id);
    }
}