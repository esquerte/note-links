using System;
using FluentValidation;
using NoteLinks.Service.ViewModels;

namespace NoteLinks.Service.Validators
{
    public class NoteModelValidator : AbstractValidator<NoteModel>
    {
        public NoteModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty().GreaterThan(x => x.FromDate).When(x => x.ToDate.HasValue);
        }
    }
}
