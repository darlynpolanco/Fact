using Fact.Features.Clientes.Commands;
using FluentValidation;

namespace Fact.Features.Clientes.Validators
{
    public class CreateClienteValidator : AbstractValidator<CreateClienteCommand>
    {
        public CreateClienteValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("El email no puede exceder los 100 caracteres");

            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .Matches(@"^[0-9]{8,15}$").WithMessage("Teléfono inválido");

            RuleFor(c => c.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria")
                .MaximumLength(200).WithMessage("La dirección no puede exceder los 200 caracteres");
        }
    }
}
