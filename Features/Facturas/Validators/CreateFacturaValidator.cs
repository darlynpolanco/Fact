using Fact.Features.Facturas.Commands;
using FluentValidation;

namespace Fact.Features.Facturas.Validators
{
    public class CreateFacturaValidator : AbstractValidator<CreateFacturaCommand>
    {
        public CreateFacturaValidator()
        {
            RuleFor(f => f.ClienteId)
                .GreaterThan(0).WithMessage("ClienteId inválido");

            RuleFor(f => f.Items)
                .NotEmpty().WithMessage("La factura debe tener al menos un item");

            RuleForEach(f => f.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductoId)
                        .GreaterThan(0).WithMessage("ProductoId inválido");

                    item.RuleFor(i => i.Cantidad)
                        .GreaterThan(0).WithMessage("La cantidad debe ser al menos 1");
                });
        }
    }
}