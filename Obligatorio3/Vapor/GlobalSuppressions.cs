// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "<Pending>", Scope = "member", Target = "~M:Server.TextCommand.#ctor(Common.NetworkUtils.Interfaces.INetworkStreamHandler)")]
[assembly: SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "<Pending>", Scope = "member", Target = "~M:Server.Commands.CreateGamePage.#ctor(Common.NetworkUtils.Interfaces.INetworkStreamHandler)")]
[assembly: SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "<Pending>", Scope = "member", Target = "~M:Server.Commands.CreateGamePage.Respond(Common.Domain.GamePage)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Critical Bug", "S2551:Shared resources should not be used for locking", Justification = "<Pending>", Scope = "member", Target = "~M:Server.DownloadCover.ParsedRequestHandler(System.String[])~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Critical Code Smell", "S3998:Threads should not lock on objects with weak identity", Justification = "<Pending>", Scope = "member", Target = "~M:Server.DownloadCover.ParsedRequestHandler(System.String[])~System.Threading.Tasks.Task")]
