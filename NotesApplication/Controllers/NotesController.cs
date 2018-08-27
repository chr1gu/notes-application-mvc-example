using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.Extensions;
using NotesApplication.Models.FormModels;
using NotesApplication.Services;

namespace NotesApplication.Controllers
{
    public class NotesController : BaseController
    {
        private readonly INoteService _noteService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ITestDataService _testDataService;
        
        public NotesController(
            INoteService noteService,
            IUserSettingsService userSettingsService,
            ITestDataService testDataService)
        {
            _noteService = noteService;
            _userSettingsService = userSettingsService;
            _testDataService = testDataService;
        }

        public IActionResult Index(string orderBy = null, bool? hideFinished = null)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                _userSettingsService.ChangeSortOrder(orderBy);
            }
            else
            {
                orderBy = _userSettingsService.GetCurrentSortOrderKey();
            }

            if (hideFinished.HasValue)
            {
                _userSettingsService.ChangeHideFinished(hideFinished.Value);
            }
            else
            {
                hideFinished = _userSettingsService.GetShouldHideFinished();
            }

            var viewModel = _noteService.GetNotesViewModel(orderBy, hideFinished ?? false);

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
                return UserFriendlyBadRequestError("A valid note id must be provided!");
            }
            
            var viewModel = _noteService.GetNoteViewModel(id.Value);
            
            if (viewModel == null)
            {
                return UserFriendlyNotFoundError($"Could not find a note with id {id}!");
            }

            return View(viewModel.ToFormModel());
        }
        
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return UserFriendlyBadRequestError("A valid note id must be provided!");
            }

            var success = _noteService.DeleteNote(id.Value);

            if (!success)
            {
                return UserFriendlyNotFoundError($"Could not delete a note with id {id}!");
            }

            return RedirectToAction("Index", "Notes").WithSuccess("Success!", $"Note {id} has been deleted.");
        }

        public IActionResult RestoreTestData()
        {
            _testDataService.RestoreTestData();
            
            return RedirectToAction("Index", "Notes").WithWarning("Testdata has been restored!", $"");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SubmitNote(NoteFormModel model)
        {
            var id = model.Id;
            
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(' ', ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                if (id.HasValue)
                {
                    return RedirectToAction("Edit", "Notes", new {id}).WithDanger("Validation error!", errorMessage);
                }
                else
                {
                    return RedirectToAction("Add", "Notes").WithDanger("Validation error!", errorMessage);
                }
            }

            if (id.HasValue)
            {
                var success = _noteService.UpdateNote(model);
                
                if (!success)
                {
                    return UserFriendlyNotFoundError($"Could not update a note with id {id}!");
                }

                return RedirectToAction("Index", "Notes").WithSuccess("Success!", $"Note {id} has been updated.");
            }
            else
            {
                id = _noteService.AddNote(model);
                
                return RedirectToAction("Index", "Notes").WithSuccess("Success!", $"Note {id} has been created.");
            }
        }
    }
}