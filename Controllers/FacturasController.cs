using Fact.Features.Facturas.Commands;
using Fact.Features.Facturas.Queries;
using Fact.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fact.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FacturasController> _logger;

        public FacturasController(IMediator mediator, ILogger<FacturasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(FacturaDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateFacturaCommand command)
        {
            try
            {
                var factura = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = factura.Id }, factura);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear factura");
                return HandleValidationException(ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al crear factura");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear factura");
                return Problem(
                    title: "Error al crear factura",
                    detail: "Ocurrió un error inesperado al procesar la factura",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FacturaDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var factura = await _mediator.Send(new GetFacturaByIdQuery(id));
                return factura != null ? Ok(factura) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura con ID: {id}");
                return Problem(
                    title: "Error al obtener factura",
                    detail: "Ocurrió un error inesperado al obtener la factura",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        private IActionResult HandleValidationException(ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return ValidationProblem(new ValidationProblemDetails(errors));
        }
    }
}
