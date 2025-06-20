using Fact.Features.Clientes.Commands;
using Fact.Features.Clientes.Queries;
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
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _mediator.Send(new GetAllClientesQuery());
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _mediator.Send(new GetClienteByIdQuery(id));
            return cliente != null ? Ok(cliente) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateClienteCommand command)
        {
            try
            {
                var cliente = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
            }
            catch (ValidationException ex)
            {
                return HandleValidationException(ex);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID en ruta no coincide con ID en cuerpo");

            try
            {
                var cliente = await _mediator.Send(command);
                return Ok(cliente);
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
            var result = await _mediator.Send(new DeleteClienteCommand(id));
            return result ? NoContent() : NotFound();
        }

        private IActionResult HandleValidationException(ValidationException ex)
        {
            var errors = new Dictionary<string, string[]>
           {
               { "Error", new[] { ex.Message } }
           };

            return ValidationProblem(new ValidationProblemDetails(errors));
        }
    }
}
