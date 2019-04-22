using FluentValidation;
using NoteLinks.Service.ViewModels;

namespace NoteLinks.Service.Validators
{
    public class CreateNoteModelValidator : AbstractValidator<CreateNoteModel>
    {
        public CreateNoteModelValidator()
        {
            RuleFor(x => x.CalendarCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty().GreaterThanOrEqualTo(x => x.FromDate).When(x => x.ToDate.HasValue);
        }
    }
}
