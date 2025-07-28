global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

global using Xunit;
global using FluentAssertions;
global using NSubstitute;

// ASP.NET Core Testing
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// Authentication
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.IdentityModel.Tokens;

// Domain
global using MCPHub.Domain.Entities;
global using MCPHub.Domain.Contracts.Requests;
global using MCPHub.Domain.Contracts.Responses;

// PublicApi
global using MCPHub.PublicApi.Configuration;
global using MCPHub.PublicApi.Services;