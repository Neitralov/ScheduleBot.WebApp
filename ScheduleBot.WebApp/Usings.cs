global using static ScheduleBot.WebApp.NLogLogger;

global using File = System.IO.File;
global using System.IO.Compression;

global using ScheduleBot.Enums;
global using ScheduleBot.Structs;
global using ScheduleBot.DataBase;

global using Telegram.Bot;
global using Telegram.Bot.Exceptions;
global using Telegram.Bot.Polling;
global using Telegram.Bot.Types;
global using Telegram.Bot.Types.Enums;
global using Telegram.Bot.Types.InputFiles;

global using System.Diagnostics;

global using Microsoft.EntityFrameworkCore;

global using NLog;