using System;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class LineBuilderTest
    {
        [Theory]
        [InlineData("\r\n", "\t")]
        [InlineData("\n", "\t")]
        [InlineData("\r", "\t")]
        [InlineData("\r\n", "    ")]
        [InlineData("\n", "    ")]
        [InlineData("\r", "    ")]
        public void BuilderTest1(string nl, string tab)
        {
            LineBuilder lb = new(nl, tab);

            lb.Append("root ")
                .AppendLine("line")
                .AppendLv1Line("Start Level1")
                .AppendLv2Line("Record1")
                .AppendLv2Line("Record2")
                .AppendLv1Line("End Level1");

            string exp = $"root line{nl}{tab}Start Level1{nl}{tab}{tab}Record1{nl}{tab}{tab}Record2{nl}{tab}End Level1";

            Assert.Equal(exp.Length + nl.Length, lb.Length);
            Assert.Equal(tab, lb.Tab);
            Assert.Equal(nl, lb.NewLine);

            Assert.Equal(exp, lb.ToString(removeNewLine: true));
            Assert.Equal($"{exp}{nl}", lb.ToString(removeNewLine: false));
            Assert.Equal($"{exp}{nl}", lb.ToString());

            lb.RemoveNewLine();
            Assert.Equal(exp, lb.ToString());

            lb.Clear();
            Assert.Equal(0, lb.Length);
            Assert.Empty(lb.ToString());
        }

        [Theory]
        [InlineData("\r\n", "\t")]
        [InlineData("\n", "\t")]
        [InlineData("\r", "\t")]
        [InlineData("\r\n", "    ")]
        [InlineData("\n", "    ")]
        [InlineData("\r", "    ")]
        public void BuilderTest2(string nl, string tab)
        {
            LineBuilder lb = new(nl, tab);

            lb.AppendLv1("Record1 ")
                .AppendLv2("Record2 ")
                .AppendLv1("Record3 ");

            string exp = $"{tab}Record1 {tab}{tab}Record2 {tab}Record3 ";

            Assert.Equal(exp.Length, lb.Length);
            Assert.Equal(tab, lb.Tab);
            Assert.Equal(nl, lb.NewLine);

            Assert.Equal(exp, lb.ToString(removeNewLine: true));
            Assert.Equal(exp, lb.ToString(removeNewLine: false));
            Assert.Equal(exp, lb.ToString());

            lb.RemoveNewLine();
            Assert.Equal(exp, lb.ToString());

            lb.Clear();
            Assert.Equal(0, lb.Length);
            Assert.Empty(lb.ToString());
        }

        [Theory]
        [InlineData(null, "\t")]
        [InlineData("\r\n", "\t")]
        [InlineData("\n", "\t")]
        [InlineData("\r", "\t")]
        [InlineData("\r\n", "    ")]
        [InlineData("\n", "    ")]
        [InlineData("\r", "    ")]
        public void CtorTest1(string nl, string tab)
        {
            LineBuilder lb = new(nl, tab);

            Assert.Equal(nl ?? Environment.NewLine, lb.NewLine);
            Assert.Equal(tab, lb.Tab);

            Assert.Throws<ArgumentNullException>(() => lb = new(nl, tab: null));
        }

        [Fact]
        public void ContainsLastTest1()
        {
            LineBuilder lb = new();

            lb.Append("abcdef");
            Assert.True(lb.ContainsLast("f"));
            Assert.True(lb.ContainsLast("ef"));
            Assert.True(lb.ContainsLast("abcdef"));
            Assert.False(lb.ContainsLast("abcdefg"));
            Assert.False(lb.ContainsLast(string.Empty));
            Assert.False(lb.ContainsLast("x"));
            Assert.False(lb.ContainsLast("1234567890"));

            Assert.Throws<ArgumentNullException>(() => lb.ContainsLast(null));
        }

        [Fact]
        public void RemoveLastTest1()
        {
            LineBuilder lb = new();

            lb.Append("abcdef");
            Assert.Equal("abcdef", lb.RemoveLast(0).ToString());
            Assert.Equal("abcde", lb.RemoveLast(1).ToString());
            Assert.Equal("abc", lb.RemoveLast(2).ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() => lb.RemoveLast(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => lb.RemoveLast(-1));

            Assert.Equal("a", lb.RemoveLast(2).ToString());
            Assert.Equal(string.Empty, lb.RemoveLast(1).ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() => lb.RemoveLast(1));
        }


        [Fact]
        public void ToStringTest1()
        {
            LineBuilder lb = new();

            Assert.Equal(string.Empty, lb.ToString(removeNewLine: true));
            Assert.Equal(0, lb.Length);

            string wrd = "Hello";
            lb.AppendLine(wrd);
            Assert.Equal(wrd.Length + lb.NewLine.Length, lb.Length);
            Assert.Equal(wrd, lb.ToString(removeNewLine: true));
            Assert.Equal(wrd.Length + lb.NewLine.Length, lb.Length);

            lb.RemoveNewLine();
            Assert.Equal(wrd.Length, lb.Length);
        }

    }
}
