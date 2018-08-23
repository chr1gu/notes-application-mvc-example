using Microsoft.AspNetCore.Mvc;
using NotesApplication.Extensions;
using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;
using NotesApplication.Services;

namespace NotesApplication.Controllers
{
    public class NotesController : Controller
    {
        private readonly INoteService noteService;
        
        public NotesController(INoteService noteService)
        {
            this.noteService = noteService;
        }

        public IActionResult Index(string orderBy = "createdAt", bool hideFinished = false)
        {
            var viewModel = noteService.GetNotesViewModel(orderBy, hideFinished);

            return View(viewModel);
        }

        public IActionResult Add()
        {
            return View();
        }
        
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Id must be specified");
            }
            
            var viewModel = noteService.GetNoteViewModel(id.Value);
            
            if (viewModel == null)
            {
                return NotFound($"Note with id {id} not found!");
            }

            return View(viewModel.ToFormModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SubmitNote(NoteFormModel model)
        {
            var success = false;
            var id = model.Id;

            if (id.HasValue)
            {
                success = noteService.UpdateNote(model);
            }
            else
            {
                id = noteService.AddNote(model);
            }

            return RedirectToAction("Edit", "Notes", new {id});
        }
    }
}