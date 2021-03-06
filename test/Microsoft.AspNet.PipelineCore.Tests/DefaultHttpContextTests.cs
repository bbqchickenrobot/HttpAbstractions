// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.HttpFeature;
using Xunit;

namespace Microsoft.AspNet.PipelineCore.Tests
{
    public class DefaultHttpContextTests
    {
        [Fact]
        public void EmptyUserIsNeverNull()
        {
            var context = new DefaultHttpContext(new FeatureCollection());
            Assert.NotNull(context.User);
            Assert.Equal(1, context.User.Identities.Count());
            Assert.True(object.ReferenceEquals(context.User, context.User));
            Assert.False(context.User.Identity.IsAuthenticated);
            Assert.True(string.IsNullOrEmpty(context.User.Identity.AuthenticationType));

            context.User = null;
            Assert.NotNull(context.User);
            Assert.Equal(1, context.User.Identities.Count());
            Assert.True(object.ReferenceEquals(context.User, context.User));
            Assert.False(context.User.Identity.IsAuthenticated);
            Assert.True(string.IsNullOrEmpty(context.User.Identity.AuthenticationType));

            context.User = new ClaimsPrincipal();
            Assert.NotNull(context.User);
            Assert.Equal(0, context.User.Identities.Count());
            Assert.True(object.ReferenceEquals(context.User, context.User));
            Assert.Null(context.User.Identity);

            context.User = new ClaimsPrincipal(new ClaimsIdentity("SomeAuthType"));
            Assert.Equal("SomeAuthType", context.User.Identity.AuthenticationType);
            Assert.True(context.User.Identity.IsAuthenticated);
        }

        [Fact]
        public async Task AuthenticateWithNoAuthMiddlewareThrows()
        {
            var context = CreateContext();
            Assert.Throws<InvalidOperationException>(() => context.Authenticate("Foo"));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await context.AuthenticateAsync("Foo"));
        }

        [Fact]
        public void ChallengeWithNoAuthMiddlewareMayThrow()
        {
            var context = CreateContext();
            context.Response.Challenge();
            Assert.Equal(401, context.Response.StatusCode);

            Assert.Throws<InvalidOperationException>(() => context.Response.Challenge("Foo"));
        }

        [Fact]
        public void SignInWithNoAuthMiddlewareThrows()
        {
            var context = CreateContext();
            Assert.Throws<InvalidOperationException>(() => context.Response.SignIn(new ClaimsIdentity("Foo")));
        }

        [Fact]
        public void SignOutWithNoAuthMiddlewareMayThrow()
        {
            var context = CreateContext();
            context.Response.SignOut();

            Assert.Throws<InvalidOperationException>(() => context.Response.SignOut("Foo"));
        }

        private HttpContext CreateContext()
        {
            var context = new DefaultHttpContext();
            return context;
        }
    }
}