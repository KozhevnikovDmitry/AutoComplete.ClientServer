using System.Linq;
using AutoComplete.Server;
using NUnit.Framework;

namespace AutoComplete.Tests
{
    [TestFixture]
    public class IndexBuilderTests
    {
        [Test]
        public void Build_ForEveryLengthOfPrefix_Test()
        {
            // Arrange
            var vacobulary = new[] { "AAA" };

            // Act
            var index = IndexBuilder.BuildIndex(vacobulary, 1);

            // Assert
            Assert.AreEqual(index.Count, 3);
            Assert.AreEqual(index["A"].Single(), "AAA");
            Assert.AreEqual(index["AA"].Single(), "AAA");
            Assert.AreEqual(index["AAA"].Single(), "AAA");
        }

        [Test]
        public void Build_TakeOnlyCompletionCountInCertainOrder_Test()
        {
            // Arrange
            var vacobulary = new[] { "AAA", "AA" };

            // Act
            var index = IndexBuilder.BuildIndex(vacobulary, 1);

            // Assert
            Assert.AreEqual(index.Count, 3);
            Assert.AreEqual(index["A"].Single(), "AAA");
            Assert.AreEqual(index["AA"].Single(), "AAA");
            Assert.AreEqual(index["AAA"].Single(), "AAA");
        }

        [Test]
        public void Build_AccumulateCompletionCount_Test()
        {
            // Arrange
            var vacobulary = new[] { "AAA", "AA" };

            // Act
            var index = IndexBuilder.BuildIndex(vacobulary, 2);

            // Assert
            Assert.AreEqual(index.Count, 3);
            Assert.AreEqual(index["A"][0], "AAA");
            Assert.AreEqual(index["AA"][0], "AAA");
            Assert.AreEqual(index["AAA"][0], "AAA");
            Assert.AreEqual(index["A"][1], "AA");
            Assert.AreEqual(index["AA"][1], "AA");
        }


        [Test]
        public void Build_TakeToCompletionEqualOrLongerWords_Test()
        {
            // Arrange
            var vacobulary = new[] {"AAAA", "AAA", "AA", "A"};

            // Act
            var index = IndexBuilder.BuildIndex(vacobulary, 4);

            // Assert
            Assert.AreEqual(index.Count, 4);
            Assert.AreEqual(index["AAAA"].Single(), "AAAA");
            Assert.AreEqual(index["AAA"][0], "AAAA");
            Assert.AreEqual(index["AAA"][1], "AAA");
            Assert.AreEqual(index["AAA"].Length, 2);

        }
    }
}
