using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class PropertyItemTest
    {
        [Fact]
        public void CtorTest1()
        {
            var p = new PropertyItem("name", "value", "true");
            Assert.Equal("name", p.name);
            Assert.Equal("value", p.unevaluatedValue);
            Assert.Null(p.evaluatedValue);
            Assert.Equal("true", p.condition);
            Assert.Null(p.parentProject);
            Assert.Null(p.parentProperty);

            Assert.False(p.isEnvironmentProperty);
            Assert.False(p.isGlobalProperty);
            Assert.False(p.isReservedProperty);
            Assert.False(p.isImported);
            Assert.True(p.isUserDef);
        }

        [Fact]
        public void CtorTest2()
        {
            var p = new PropertyItem("name", "$(value)");
            Assert.Equal("name", p.name);
            Assert.Equal("$(value)", p.unevaluatedValue);
            Assert.Null(p.evaluatedValue);
            Assert.Null(p.condition);
            Assert.Null(p.parentProject);
            Assert.Null(p.parentProperty);

            Assert.False(p.isEnvironmentProperty);
            Assert.False(p.isGlobalProperty);
            Assert.False(p.isReservedProperty);
            Assert.False(p.isImported);
            Assert.True(p.isUserDef);
        }

        [Fact]
        public void EqTest1()
        {
            Assert.NotEqual
            (
                new PropertyItem("name1", "value1", "true1"),
                new PropertyItem("name1", "value1", "true2")
            );

            Assert.NotEqual
            (
                new PropertyItem("name1", "value1", "true1"),
                new PropertyItem("name1", "value2", "true1")
            );

            Assert.NotEqual
            (
                new PropertyItem("name1", "value1", "true1"),
                new PropertyItem("name2", "value1", "true1")
            );

            Assert.Equal
            (
                new PropertyItem("name1", "value1", "true1"),
                new PropertyItem("name1", "value1", "true1")
            );

            Assert.NotEqual
            (
                new PropertyItem("name1", "value1", "true1"),
                new PropertyItem("name1", "value1")
            );
        }

        [Fact]
        public void EqTest2()
        {
            Assert.NotEqual
            (
                new PropertyItem("name1", "value1") { evaluatedValue = "value2" },
                new PropertyItem("name1", "value1")
            );

            Assert.Equal
            (
                new PropertyItem("name1", "value1") { evaluatedValue = "value2" },
                new PropertyItem("name1", "value1") { evaluatedValue = "value2" }
            );

            Assert.NotEqual
            (
                new PropertyItem("name1", "value1") { evaluatedValue = "value2" },
                new PropertyItem("name1", "value1") { evaluatedValue = "value3" }
            );
        }

        [Fact]
        public void EqTest3()
        {
            var p1 = new PropertyItem();
            Assert.Equal(p1, PropertyItem.None);

            Assert.True(p1.HasNothing);
            Assert.False(p1.HasValue);

            var p2 = new PropertyItem() { name = " " };
            Assert.NotEqual(p2, PropertyItem.None);

            Assert.True(p2.HasNothing);
            Assert.False(p2.HasValue);
        }

        [Fact]
        public void EqTest4()
        {
            var p = new PropertyItem("name", "value");
            Assert.NotEqual(p, PropertyItem.None);

            Assert.False(p.HasNothing);
            Assert.True(p.HasValue);

            p.unevaluatedValue = string.Empty;

            Assert.True(p.HasNothing);
            Assert.True(p.HasValue);

            p.unevaluatedValue = " ";

            Assert.True(p.HasNothing);
            Assert.True(p.HasValue);

            p.unevaluatedValue = null;

            Assert.True(p.HasNothing);
            Assert.False(p.HasValue);
        }
    }
}
