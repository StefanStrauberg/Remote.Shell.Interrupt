global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Remote.Shell.Interrupt.Storehouse.API;
global using Remote.Shell.Interrupt.Storehouse.Application;
global using Remote.Shell.Interrupt.Storehouse.Application.Exceptions;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Create;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Delete;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Update;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetAll;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetByExpression;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Create;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Delete;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Create;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetById;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Get;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Walk;
global using Remote.Shell.Interrupt.Storehouse.Application.Middleware;
global using Remote.Shell.Interrupt.Storehouse.AppLogger;
global using Remote.Shell.Interrupt.Storehouse.Domain.SNMP;
global using Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;
global using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;
global using Serilog;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByVlanTag;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.Update;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.OrganizationName;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetAll;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Create;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetByExpression;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Delete;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Update;