using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotesApplication.Controllers;
using NotesApplication.Extensions.Alerts;
using NotesApplication.Models.FormModels;
using NotesApplication.Models.ViewModels;
using NotesApplication.Services;
using Xunit;

namespace NotesApplication.Tests.Controllers
{
    public class NotesControllerTest
    {
        private readonly Mock<INoteService> _noteServiceMock;
        private readonly Mock<IUserSettingsService> _userSettingsServiceMock;
        private readonly Mock<ITestDataService> _testDataServiceMock;
        
        public NotesControllerTest()
        {
            _noteServiceMock = new Mock<INoteService>();
            _userSettingsServiceMock = new Mock<IUserSettingsService>();
            _testDataServiceMock = new Mock<ITestDataService>();
        }

        [Fact]
        protected void Index_WithoutSortOrderParameter_UsesCurrentSortOrderAndDoesNotUpdateTheValue()
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            controller.Index();
            
            // Assert
            _userSettingsServiceMock.Verify(_ => _.GetCurrentSortOrderKey(), Times.Once());
            _userSettingsServiceMock.Verify(_ => _.ChangeSortOrder(It.IsAny<string>()), Times.Never());
        }
        
        [Fact]
        protected void Index_WithoutHideFinishedParameter_DoesNotHideFinishedNotesAndUpdateValue()
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            controller.Index();
            
            // Assert
            _userSettingsServiceMock.Verify(_ => _.GetShouldHideFinished(), Times.Once());
            _userSettingsServiceMock.Verify(_ => _.ChangeHideFinished(It.IsAny<bool>()), Times.Never());
            _noteServiceMock.Verify(_ => _.GetNotesViewModel(It.IsAny<string>(), It.Is<bool>(hideFinished => hideFinished == false)), Times.Once());
        }

        [Fact]
        protected void Index_WithSortOrderParameter_UpdatesCurrentSortOrder()
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            controller.Index(orderBy: "customSortOrder");
            
            // Assert
            _userSettingsServiceMock.Verify(_ => _.GetCurrentSortOrderKey(), Times.Never());
            _userSettingsServiceMock.Verify(_ => _.ChangeSortOrder(It.Is<string>(sortOrder => sortOrder.Equals("customSortOrder"))), Times.Once());
            _noteServiceMock.Verify(_ => _.GetNotesViewModel(It.Is<string>(sortOrder => sortOrder.Equals("customSortOrder")), It.IsAny<bool>()), Times.Once());
        }
        
        [Fact]
        protected void Index_WithHideFinishedParameter_UpdatesHideFinished()
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            controller.Index(hideFinished: true);
            
            // Assert
            _userSettingsServiceMock.Verify(_ => _.GetShouldHideFinished(), Times.Never());
            _userSettingsServiceMock.Verify(_ => _.ChangeHideFinished(It.Is<bool>(hideFinished => hideFinished)), Times.Once());
            _noteServiceMock.Verify(_ => _.GetNotesViewModel(It.IsAny<string>(), It.Is<bool>(hideFinished => hideFinished)), Times.Once());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        protected void Edit_WithInvalidId_ReturnsBadRequestResponse(int? id)
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            var result = controller.Edit(id);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, viewResult.StatusCode);
        }
        
        [Fact]
        protected void Edit_WithoutExistingNote_ReturnsNotFoundResponse()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.GetNotesViewModel(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(() => null);

            // Act
            var result = controller.Edit(42);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, viewResult.StatusCode);
        }
        
        [Fact]
        protected void Edit_WithExistingNote_ReturnsEditView()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.GetNoteViewModel(It.IsAny<int>()))
                .Returns(() => new NoteViewModel
                {
                    Id = 42,
                    Title = "Testnote"
                });

            // Act
            var result = controller.Edit(42);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<NoteFormModel>(viewResult.ViewData.Model);
            Assert.Equal("Testnote", model.Title);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        protected void Delete_WithInvalidId_ReturnsBadRequestResponse(int? id)
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            var result = controller.Delete(id);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, viewResult.StatusCode);
        }
        
        [Fact]
        protected void Delete_WithoutExistingNote_ReturnsNotFoundResponse()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.DeleteNote(It.IsAny<int>())).Returns(() => false);

            // Act
            var result = controller.Delete(42);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, viewResult.StatusCode);
        }
        
        [Fact]
        protected void Delete_WithExistingNote_RedirectsToIndex()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.DeleteNote(It.IsAny<int>())).Returns(() => true);

            // Act
            var result = controller.Delete(42);
            
            // Assert
            var viewResult = Assert.IsType<AlertDecoratorResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(viewResult.Result);
            Assert.Equal("Notes", redirectResult.ControllerName);
            Assert.Equal("Index", redirectResult.ActionName);
        }
        
        [Fact]
        protected void SubmitNote_WithInvalidModelAndWithoutId_RedirectsToAddActionWithError()
        {
            // Arrange
            var controller = GetConfiguredController();
            controller.ModelState.AddModelError("Title", "Title is required!");

            // Act
            var result = controller.SubmitNote(new NoteFormModel{ Id = null });
            
            // Assert
            var viewResult = Assert.IsType<AlertDecoratorResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(viewResult.Result);
            Assert.Equal("Title is required!", viewResult.Body);
            Assert.Equal("Validation error!", viewResult.Title);
            Assert.Equal("Notes", redirectResult.ControllerName);
            Assert.Equal("Add", redirectResult.ActionName);
        }
        
        [Fact]
        protected void SubmitNote_WithInvalidModelAndWithId_RedirectsToEditActionWithError()
        {
            // Arrange
            var controller = GetConfiguredController();
            controller.ModelState.AddModelError("Title", "Title is required!");

            // Act
            var result = controller.SubmitNote(new NoteFormModel{ Id = 42 });
            
            // Assert
            var viewResult = Assert.IsType<AlertDecoratorResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(viewResult.Result);
            Assert.Equal("Title is required!", viewResult.Body);
            Assert.Equal("Validation error!", viewResult.Title);
            Assert.Equal("Notes", redirectResult.ControllerName);
            Assert.Equal("Edit", redirectResult.ActionName);
        }
        
        [Fact]
        protected void SubmitNote_WithoutSuccessfulUpdateResult_ReturnsNotFoundError()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.UpdateNote(It.IsAny<NoteFormModel>())).Returns(() => false);

            // Act
            var result = controller.SubmitNote(new NoteFormModel{ Id = 42 });
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, viewResult.StatusCode);
        }
        
        [Fact]
        protected void SubmitNote_WithSuccessfulUpdateResult_RedirectsToIndex()
        {
            // Arrange
            var controller = GetConfiguredController();
            _noteServiceMock.Setup(_ => _.UpdateNote(It.IsAny<NoteFormModel>())).Returns(() => true);

            // Act
            var result = controller.SubmitNote(new NoteFormModel{ Id = 42 });
            
            // Assert
            var viewResult = Assert.IsType<AlertDecoratorResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(viewResult.Result);
            Assert.Equal("Notes", redirectResult.ControllerName);
            Assert.Equal("Index", redirectResult.ActionName);
        }
        
        [Fact]
        protected void Add_InGeneral_ReturnsViewResult()
        {
            // Arrange
            var controller = GetConfiguredController();

            // Act
            var result = controller.Add();
            
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        private NotesController GetConfiguredController()
        {
            var controller = new NotesController(
                noteService: _noteServiceMock.Object,
                userSettingsService: _userSettingsServiceMock.Object,
                testDataService: _testDataServiceMock.Object);
            
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            return controller;
        }
    }
}