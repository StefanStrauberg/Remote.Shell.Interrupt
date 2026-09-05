global using Xunit;
global using FluentAssertions;
global using NSubstitute;
global using System.Linq.Expressions;

// Domain
global using Remote.Shell.Interrupt.Storehouse.Domain.Common;
global using Remote.Shell.Interrupt.Storehouse.Domain.Gateway;
global using Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;
global using Remote.Shell.Interrupt.Storehouse.Domain.Organization;
global using Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;

// Application
global using Remote.Shell.Interrupt.Storehouse.Application.Helpers;
global using Remote.Shell.Interrupt.Storehouse.Application.Models.Request;
global using Remote.Shell.Interrupt.Storehouse.Application.Models.Response;
global using Remote.Shell.Interrupt.Storehouse.Application.Exceptions;
global using Remote.Shell.Interrupt.Storehouse.Application.Validations.SNMP;
global using Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPGet;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPWalk;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;
global using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;
global using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

// Infrastructure - QueryFilterParser (InternalsVisibleTo)
global using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;
global using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;
global using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;

// Infrastructure - Specification (InternalsVisibleTo)
global using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;
global using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
global using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
global using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
global using Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;
global using Microsoft.Extensions.Logging.Abstractions;
global using Remote.Shell.Interrupt.Storehouse.Application.Services.Mapping;
global using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.RemBillRep;
