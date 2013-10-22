// Guids.cs
// MUST match guids.h
using System;

namespace Wil.NamedBookmarks
{
    static class GuidList
    {
        public const string guidNamedBookmarksPkgString = "2f5b7c8c-8eb8-4963-973f-c2d8ef9d4060";
        public const string guidNamedBookmarksCmdSetString = "395c958a-1d3e-49b7-a6d3-5ef9668b67ab";

        public static readonly Guid guidNamedBookmarksCmdSet = new Guid(guidNamedBookmarksCmdSetString);
    };
}