using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    using static net.r_eg.MvsSln.Static.Members;

    public class CollectionExtensionTest
    {
        [Theory]
        [MemberData(nameof(GetArrStringData))]
        public void ForEachTest1(string[] data)
        {
            StringBuilder sb = new();
            data.ForEach(s => sb.Append($"{s},"));
            Assert.Equal
            (
                string.Join(",", data ?? EmptyArray<string>()),
                sb.ToString().TrimEnd([','])
            );
        }

        [Theory]
        [MemberData(nameof(GetArrStringData))]
        public void ForEachTest2(string[] data)
        {
            StringBuilder sbf = new();
            data.ForEach((s, i) => sbf.Append($"{i}={s},"));

            if(data == null)
            {
                Assert.Equal(0, sbf.Length);
                return;
            }

            StringBuilder sb = new();
            for(int i = 0; i < data.Length; ++i)
            {
                sb.Append($"{i}={data[i]},");
            }

            Assert.Equal(sb.ToString(), sbf.ToString());
        }

#if !NET40

        [Theory]
        [MemberData(nameof(GetArrStringData))]
        public async Task ForEachAsyncTest1(string[] data)
        {
            StringBuilder sb = new();
            var chain = await data.ForEach(async s => sb.Append(await Task.FromResult($"{s},")));
            Assert.True(chain?.EqualsE(data) ?? data is null);

            Assert.Equal
            (
                string.Join(",", data ?? EmptyArray<string>()),
                sb.ToString().TrimEnd([','])
            );
        }

        [Theory]
        [MemberData(nameof(GetArrStringData))]
        public async Task ForEachAsyncTest2(string[] data)
        {
            StringBuilder sbf = new();
            var chain = await data.ForEach(async (s, i) => sbf.Append(await Task.FromResult($"{i}={s},")));

            Assert.True(chain?.EqualsE(data) ?? data is null);

            if(data is null)
            {
                Assert.Equal(0, sbf.Length);
                return;
            }

            StringBuilder sb = new();
            for(int i = 0; i < data.Length; ++i)
            {
                sb.Append($"{i}={data[i]},");
            }

            Assert.Equal(sb.ToString(), sbf.ToString());
        }

#endif

        public static IEnumerable<object[]> GetArrStringData()
        {
            yield return new object[] { new string[] { "one", "two", "three", "four", "five" } };
            yield return new object[] { new string[] { "one" } };
            yield return new object[] { EmptyArray<string>() };
            yield return new object[] { null };
            yield return new object[] { new string[] { "one", "two" } };
        }

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
