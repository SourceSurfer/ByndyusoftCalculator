using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tests
{
    [TestClass()]
    public class CalculatorTests
    {
        private Calculator calculator;

        [SetUp]
        public void Setup()
        {
            calculator = new Calculator();
            calculator.AddOperation("^", (a, b) => Math.Pow(a, b)); // Adding a custom operation
        }

        [Test]
        public void Test_Addition()
        {
            Assert.AreEqual(5, calculator.Evaluate("2 + 3"));
        }

        [Test]
        public void Test_Unary_Minus()
        {
            Assert.AreEqual(-2, calculator.Evaluate("-5 + 3"));
        }

        [Test]
        public void Test_Unary_Plus()
        {
            Assert.AreEqual(-3, calculator.Evaluate("+2 - 5"));
        }

        [Test]
        public void Test_Power_Operation()
        {
            Assert.AreEqual(8, calculator.Evaluate("2 ^ 3"));
        }

        [Test]
        public void Test_Division_By_Zero()
        {
            NUnit.Framework.Assert.Throws<DivideByZeroException>(() => calculator.Evaluate("8 / 0"));
        }
    }
}