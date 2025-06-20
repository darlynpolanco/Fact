using Fact.Features.Productos.Commands;
using Fact.Features.Productos.Queries;
using Fact.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Fact.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _mediator.Send(new GetAllProductosQuery());
            return Ok(productos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _mediator.Send(new GetProductoByIdQuery(id));
            return producto != null ? Ok(producto) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductoCommand command)
        {
            try
            {
                var producto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
            }
            catch (ValidationException ex)
            {
                return HandleValidationException(ex);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID en ruta no coincide con ID en cuerpo");

            try
            {
                var producto = await _mediator.Send(command);
                return Ok(producto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ValidationException ex)
            {
                return HandleValidationException(ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductoCommand(id));
            return result ? NoContent() : NotFound();
        }

        private IActionResult HandleValidationException(ValidationException ex)
        {

            var errors = new Dictionary<string, string[]>
           {
               { "Error", new[] { ex.Message } } // Use the exception message as the error
           };

            return ValidationProblem(new ValidationProblemDetails(errors));
        }
    }
}
