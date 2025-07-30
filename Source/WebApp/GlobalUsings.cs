global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Text;
global using System.Threading.Tasks;

global using MCPHub.Domain.Contracts.Requests;
global using MCPHub.Domain.Contracts.Responses;
global using MCPHub.Domain.Entities;
global using MCPHub.Domain.ValueObjects;
global using MCPHub.WebApp;
global using MCPHub.WebApp.Components;
global using MCPHub.WebApp.Components.Shared;
global using MCPHub.WebApp.Components.UI.Models;
global using MCPHub.WebApp.Components.UI.Security;
global using MCPHub.WebApp.Services;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Routing;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.Web.Virtualization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.JSInterop;

global using MudBlazor;
global using MudBlazor.Services;