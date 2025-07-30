global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.CommandLine;
global using System.CommandLine.Invocation;
global using System.Diagnostics;
global using System.IO;
global using System.IO.Compression;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Reflection;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;

global using MCPHub.CommandLineApp.Commands;
global using MCPHub.CommandLineApp.Configuration;
global using MCPHub.CommandLineApp.Extensions;
global using MCPHub.CommandLineApp.Models;
global using MCPHub.CommandLineApp.Services;
global using MCPHub.CommandLineApp.Utilities;
global using MCPHub.Domain.Contracts.Responses;
global using MCPHub.Domain.Contracts.Services;
global using MCPHub.Domain.Entities;
global using MCPHub.Domain.ValueObjects;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using Spectre.Console;