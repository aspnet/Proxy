// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Proxy
{
    /// <summary>
    /// Proxy Options
    /// </summary>
    public sealed class ProxyOptions
    {
        public string Scheme { get; set; }
        public HostString Host { get; set; }
        public PathString PathBase { get; set; }
        public QueryString AppendQuery { get; set; }
    }
}
