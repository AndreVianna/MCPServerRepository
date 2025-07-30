global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;

global using AngleSharp;
global using AngleSharp.Html.Dom;

global using Bogus;

global using DotNet.Testcontainers.Builders;
global using DotNet.Testcontainers.Configurations;
global using DotNet.Testcontainers.Containers;

global using FluentAssertions;

global using MCPHub.Common.Services;
global using MCPHub.Data;
global using MCPHub.Domain.Common;
global using MCPHub.Domain.Contracts.Requests;
global using MCPHub.Domain.Contracts.Responses;
global using MCPHub.Domain.Contracts.Services;
global using MCPHub.Domain.Entities;
global using MCPHub.Domain.Repositories;
global using MCPHub.Domain.ValueObjects;
global using MCPHub.IntegrationTests.Builders;
global using MCPHub.IntegrationTests.Infrastructure;
global using MCPHub.IntegrationTests.Support;
global using MCPHub.PublicApi;
global using MCPHub.Storage;
global using MCPHub.WebApp;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using NSubstitute;

global using OpenQA.Selenium;
global using OpenQA.Selenium.Chrome;
global using OpenQA.Selenium.Support.UI;

global using TechTalk.SpecFlow;

global using Testcontainers.PostgreSql;
global using Testcontainers.Redis;

global using Xunit;

global using DomainSecurityScanSeverity = MCPHub.Domain.ValueObjects.SecurityScanSeverity;
global using DomainSortDirection = MCPHub.Domain.Entities.SortDirection;
global using LoggingLogLevel = Microsoft.Extensions.Logging.LogLevel;
