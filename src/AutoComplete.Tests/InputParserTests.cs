using System;
using System.Linq;
using AutoComplete.Server;
using NUnit.Framework;

namespace AutoComplete.Tests
{
    [TestFixture]
    public class InputParserTests
    {
        [Test]
        public void Parse_FrequencyDescendingOrder_Test()
        {
            // Arrange
            var input = new[] { "2", "aaa 10", "bbb 20" };

            // Act
            var result = InputParser.Parse(input);

            // Assert
            Assert.AreEqual(result[0], "bbb");
            Assert.AreEqual(result[1], "aaa");
        }

        [Test]
        public void Parse_FrequencyDescendingOrder_ThanByWordAscending_Test()
        {
            // Arrange
            var input = new[] { "3", "aaa 10", "ccc 20", "bbb 20", "2", "x", "z" };

            // Act
            var result = InputParser.Parse(input);

            // Assert
            Assert.AreEqual(result[0], "bbb");
            Assert.AreEqual(result[1], "ccc");
            Assert.AreEqual(result[2], "aaa");
        }
    }
}
