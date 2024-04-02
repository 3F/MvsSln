using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    public class CollectionExtensionTest
    {
        [Theory]
        [InlineData("\r\n")]
        [InlineData(null)]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\n\r")]
        [InlineData("")]
        public void UpdateNewLineTest1(string nl)
        {
            Dictionary<Type, HandlerValue> handlers = new()
            {
                [typeof(LProject)] = new(new _WhandlerA() { NewLine = "a" }),
                [typeof(LNestedProjects)] = new(new _WhandlerB() { NewLine = "b" }),
            };

            Assert.Equal("a", handlers[typeof(LProject)].handler.NewLine);
            Assert.Equal("b", handlers[typeof(LNestedProjects)].handler.NewLine);

            handlers.UpdateNewLine(nl);

            Assert.Equal(nl ?? Environment.NewLine, handlers[typeof(LProject)].handler.NewLine);
            Assert.Equal(nl ?? Environment.NewLine, handlers[typeof(LNestedProjects)].handler.NewLine);
        }

        private sealed class _WhandlerA: WAbstract
        {
            public override string Extract(object _) => nameof(_WhandlerA);
        }

        private sealed class _WhandlerB: WAbstract
        {
            public override string Extract(object _) => nameof(_WhandlerB);
        }
    }
}
