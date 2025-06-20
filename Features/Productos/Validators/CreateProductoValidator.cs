using Fact.Features.Productos.Commands;
using FluentValidation;

namespace Fact.Features.Productos.Validators
{
    public class CreateProductoValidator : AbstractValidator<CreateProductoCommand>
    {
        public CreateProductoValidator()
        {
            RuleFor(p => p.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");

            RuleFor(p => p.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres");

            RuleFor(p => p.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor que 0");

            RuleFor(p => p.ImagenUrl)
                .NotEmpty().WithMessage("La URL de la imagen es obligatoria")
                .Must(BeAValidUrl).WithMessage("URL de imagen inválida");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
