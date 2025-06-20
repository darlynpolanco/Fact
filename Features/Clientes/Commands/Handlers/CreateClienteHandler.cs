using MediatR;
using System.Data;
using Fact.Models;
using Microsoft.EntityFrameworkCore;
using Fact.Data.Entities;
using Fact.Data;
using Fact.Features.Clientes.Commands;

namespace Features.Clientes.Commands.Handlers;

public class CreateClienteHandler : IRequestHandler<CreateClienteCommand, ClienteDto>
{
    private readonly AppDbContext _context;

    public CreateClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Telefono = request.Telefono,
            Direccion = request.Direccion
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(cancellationToken);

        return new ClienteDto(
            cliente.Id,
            cliente.Nombre,
            cliente.Email,
            cliente.Telefono,
            cliente.Direccion
        );
    }
}