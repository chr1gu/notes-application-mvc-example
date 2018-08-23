using System.Collections.Generic;

namespace NotesApplication.Models.ViewModels
{
    public class NotesListViewModel
    {
        public bool HideFinished { get; set; }
        public string CurrentSortOrder { get; set; }
        public IDictionary<string, string> AvailableSortOrders { get; set; }
        public IList<NoteViewModel> Notes { get; set; }
    }
}