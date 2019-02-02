using FluentValidation.TestHelper;
using NoteLinks.Service.Validators;
using NoteLinks.Service.ViewModels;
using System;
using Xunit;

namespace NoteLinks.Service.Test.ValidatorTests
{
    public class CreateNoteModelValidatorTests
    {
        private CreateNoteModelValidator validator;

        public CreateNoteModelValidatorTests()
        {
            validator = new CreateNoteModelValidator();
        }

        [Fact]
        public void ShouldHaveErrorWhenCalendarCodeIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.CalendarCode, null as string);
        }

        [Fact]
        public void ShouldNotHaveErrorWhenCalendarCodeIsSpecified()
        {
            validator.ShouldNotHaveValidationErrorFor(x => x.CalendarCode, "asdf");
        }

        [Fact]
        public void ShouldHaveErrorWhenNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, null as string);
        }

        [Fact]
        public void ShouldNotHaveErrorWhenNameIsSpecified()
        {
            validator.ShouldNotHaveValidationErrorFor(x => x.Name, "Birthdays");
        }

        [Fact]
        public void ShouldHaveErrorWhenNameLengthIsGreaterThen128()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, new String('A', 129));
        }

        [Fact]
        public void ShouldHaveErrorWhenFromDateHasDefaultValue()
        {
            validator.ShouldHaveValidationErrorFor(x => x.FromDate, DateTime.MinValue);
        }

        [Fact]
        public void ShouldNotHaveErrorWhenFromDateIsSpecified()
        {
            validator.ShouldNotHaveValidationErrorFor(x => x.FromDate, DateTime.Now);
        }

        [Fact]
        public void ShouldNotHaveErrorWhenToDateIsNull()
        {
            var model = new CreateNoteModel()
            {
                CalendarCode = "asdf",
                Name = "Birthdays",
                FromDate = DateTime.Now,
                ToDate = null
            };

            validator.ShouldNotHaveValidationErrorFor(x => x.ToDate, model);
        }

        [Fact]
        public void ShouldHaveErrorWhenToDateHasDefaultValue()
        {
            var model = new CreateNoteModel()
            {
                CalendarCode = "asdf",
                Name = "Birthdays",
                FromDate = DateTime.Now,
                ToDate = DateTime.MinValue
            };

            validator.ShouldHaveValidationErrorFor(x => x.ToDate, model);
        }

        [Fact]
        public void ShouldHaveErrorWhenToDateLessThenFromDate()
        {
            var model = new CreateNoteModel()
            {
                CalendarCode = "asdf",
                Name = "Birthdays",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddHours(-1)
            };

            validator.ShouldHaveValidationErrorFor(x => x.ToDate, model);
        }

    }
}