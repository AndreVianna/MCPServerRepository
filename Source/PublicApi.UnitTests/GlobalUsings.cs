global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

global using FluentAssertions;

global using MCPHub.Domain.Contracts.Requests;
global using MCPHub.Domain.Contracts.Responses;
// Domain
global using MCPHub.Domain.Entities;
// PublicApi
global using MCPHub.PublicApi.Configuration;
global using MCPHub.PublicApi.Services;
// Authentication
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
// ASP.NET Core Testing
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using NSubstitute;

global using Xunit;
