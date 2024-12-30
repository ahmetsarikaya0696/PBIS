using Domain.Entities;
using FluentValidation;

namespace Application.Features.Izinler.Commands.Create
{
    public class CreateIzinValidator : AbstractValidator<Izin>
    {
        public CreateIzinValidator()
        {
        }
    }
}
