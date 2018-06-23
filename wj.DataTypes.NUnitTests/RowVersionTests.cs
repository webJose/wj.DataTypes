using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes.NUnitTests
{
    [TestFixture]
    public class RowVersionTests
    {
        #region Static Section

        private const int ValueSize = 8;
        #endregion

        #region Helper Classes

        private class ConstructorValue
        {
            #region Properties

            public ulong ULongValue { get; set; }

            public DateTime DateValue { get; set; }

            public byte[] ByteValue { get; set; }
            #endregion
        }
        #endregion

        #region Test Data

        private static IEnumerable<ConstructorValue> ConstructorValues
        {
            get
            {
                yield return new ConstructorValue()
                {
                    ULongValue = 0UL,
                    ByteValue = new byte[8],
                    DateValue = new DateTime(0L)
                };
                yield return new ConstructorValue()
                {
                    ULongValue = 11UL,
                    ByteValue = new byte[8]
                    {
                        0xb, 0, 0, 0, 0, 0, 0, 0
                    },
                    DateValue = new DateTime(11L)
                };
                yield return new ConstructorValue()
                {
                    ULongValue = 560654894UL,
                    ByteValue = new byte[8]
                    {
                        0x2e, 0xea, 0x6a, 0x21, 0, 0, 0, 0
                    },
                    DateValue = new DateTime(560654894L)
                };
                yield return new ConstructorValue()
                {
                    ULongValue = 5465949798111989UL,
                    ByteValue = new byte[8]
                    {
                        0xf5, 0x9a, 0x13, 0x9c, 0x40, 0x6b, 0x13, 0
                    },
                    DateValue = new DateTime(5465949798111989L)
                };
            }
        }

        private static IEnumerable ByteConstructorULongTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.ByteValue)
                        .Returns(cv.ULongValue)
                        .SetName($"Construct {cv.ULongValue} From Array Tested as ULong");
                }
            }
        }

        private static IEnumerable ByteConstructorDateTimeTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.ByteValue)
                        .Returns(cv.DateValue)
                        .SetName($"Construct {cv.ULongValue} From Array Tested as DateTime");
                }
            }
        }

        private static IEnumerable ULongConstructorByteTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.ULongValue, cv.ByteValue)
                        .SetName($"Construct {cv.ULongValue} Tested as Byte Array");
                }
            }
        }

        private static IEnumerable ULongConstructorDateTimeTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.ULongValue)
                        .Returns(cv.DateValue)
                        .SetName($"Construct {cv.ULongValue} Tested as DateTime");
                }
            }
        }

        private static IEnumerable DateTimeConstructorULongTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.DateValue)
                        .Returns(cv.ULongValue)
                        .SetName($"Construct {cv.ULongValue} From DateTime Tested as ULong");
                }
            }
        }

        private static IEnumerable DateTimeConstructorByteTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.DateValue, cv.ByteValue)
                        .SetName($"Construct {cv.ULongValue} From DateTime Tested as Byte Array");
                }
            }
        }

        private static IEnumerable EqualityTestData
        {
            get
            {
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    yield return new TestCaseData(cv.ULongValue, cv.DateValue)
                        .Returns(true)
                        .SetName($"{cv.ULongValue} equals {cv.DateValue}");
                }
            }
        }

        private static IEnumerable ComparableTestData
        {
            get
            {
                List<Tuple<ulong, ulong, int>> data = new List<Tuple<ulong, ulong, int>>()
                {
                    new Tuple<ulong, ulong, int>(454UL, 455UL, -1),
                    new Tuple<ulong, ulong, int>(1610665UL, 455UL, 1),
                    new Tuple<ulong, ulong, int>(8987UL, 454599UL, -1),
                    new Tuple<ulong, ulong, int>(0UL, 26UL, -1),
                    new Tuple<ulong, ulong, int>(26UL, 23UL, 1),
                    new Tuple<ulong, ulong, int>(549UL, 355500UL, -1),
                };
                foreach (ConstructorValue cv in ConstructorValues)
                {
                    data.Add(new Tuple<ulong, ulong, int>(cv.ULongValue, (ulong)cv.DateValue.Ticks, 0));
                }
                foreach (Tuple<ulong, ulong, int> tuple in data)
                {
                    yield return new TestCaseData(tuple.Item1, tuple.Item2, tuple.Item3)
                        .SetName($"{tuple.Item1} compares to {(tuple.Item3 > 0 ? "larger" : (tuple.Item3 == 0 ? "equal" : "smaller"))}");
                }
            }
        }

        private static IEnumerable ToStringTestData
        {
            get
            {
                yield return new TestCaseData(0UL, null)
                    .Returns("0x0000000000000000")
                    .SetName("ToString() of 0");
                yield return new TestCaseData(0UL, "G")
                    .Returns("0x0000000000000000")
                    .SetName("ToString(G) of 0");
                yield return new TestCaseData(0UL, "g")
                    .Returns("0x0000000000000000")
                    .SetName("ToString(g) of 0");
                yield return new TestCaseData(0UL, "S")
                    .Returns("0x0000000000000000")
                    .SetName("ToString(S) of 0");
                yield return new TestCaseData(0UL, "s")
                    .Returns("0x0000000000000000")
                    .SetName("ToString(s) of 0");
                yield return new TestCaseData(0UL, "O")
                    .Returns("0")
                    .SetName("ToString(O) of 0");
                yield return new TestCaseData(0UL, "o")
                    .Returns("0")
                    .SetName("ToString(s) of 0");
                yield return new TestCaseData(0UL, "M")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(M) of 0");
                yield return new TestCaseData(0UL, "m")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(m) of 0");
                yield return new TestCaseData(0UL, "H")
                    .Returns("00 00 00 00 00 00 00 00")
                    .SetName("ToString(H) of 0");
                yield return new TestCaseData(0UL, "h")
                    .Returns("00 00 00 00 00 00 00 00")
                    .SetName("ToString(h) of 0");
                yield return new TestCaseData(1UL, null)
                    .Returns("0x0000000000000001")
                    .SetName("ToString() of 1");
                yield return new TestCaseData(1UL, "G")
                    .Returns("0x0000000000000001")
                    .SetName("ToString(G) of 1");
                yield return new TestCaseData(1UL, "g")
                    .Returns("0x0000000000000001")
                    .SetName("ToString(g) of 1");
                yield return new TestCaseData(1UL, "S")
                    .Returns("0x0000000000000001")
                    .SetName("ToString(S) of 1");
                yield return new TestCaseData(1UL, "s")
                    .Returns("0x0000000000000001")
                    .SetName("ToString(s) of 1");
                yield return new TestCaseData(1UL, "O")
                    .Returns("1")
                    .SetName("ToString(O) of 1");
                yield return new TestCaseData(1UL, "o")
                    .Returns("1")
                    .SetName("ToString(s) of 1");
                yield return new TestCaseData(1UL, "M")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(M) of 1");
                yield return new TestCaseData(1UL, "m")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(m) of 1");
                yield return new TestCaseData(1UL, "H")
                    .Returns("01 00 00 00 00 00 00 00")
                    .SetName("ToString(H) of 1");
                yield return new TestCaseData(1UL, "h")
                    .Returns("01 00 00 00 00 00 00 00")
                    .SetName("ToString(h) of 1");
                yield return new TestCaseData(10UL, null)
                    .Returns("0x000000000000000a")
                    .SetName("ToString() of 10");
                yield return new TestCaseData(10UL, "G")
                    .Returns("0x000000000000000A")
                    .SetName("ToString(G) of 10");
                yield return new TestCaseData(10UL, "g")
                    .Returns("0x000000000000000a")
                    .SetName("ToString(g) of 10");
                yield return new TestCaseData(10UL, "S")
                    .Returns("0x000000000000000A")
                    .SetName("ToString(S) of 10");
                yield return new TestCaseData(10UL, "s")
                    .Returns("0x000000000000000a")
                    .SetName("ToString(s) of 10");
                yield return new TestCaseData(10UL, "O")
                    .Returns("10")
                    .SetName("ToString(O) of 10");
                yield return new TestCaseData(10UL, "o")
                    .Returns("10")
                    .SetName("ToString(s) of 10");
                yield return new TestCaseData(10UL, "M")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(M) of 10");
                yield return new TestCaseData(10UL, "m")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(m) of 10");
                yield return new TestCaseData(10UL, "H")
                    .Returns("0A 00 00 00 00 00 00 00")
                    .SetName("ToString(H) of 10");
                yield return new TestCaseData(10UL, "h")
                    .Returns("0a 00 00 00 00 00 00 00")
                    .SetName("ToString(h) of 10");
                yield return new TestCaseData(16UL, null)
                    .Returns("0x0000000000000010")
                    .SetName("ToString() of 16");
                yield return new TestCaseData(16UL, "G")
                    .Returns("0x0000000000000010")
                    .SetName("ToString(G) of 16");
                yield return new TestCaseData(16UL, "g")
                    .Returns("0x0000000000000010")
                    .SetName("ToString(g) of 16");
                yield return new TestCaseData(16UL, "S")
                    .Returns("0x0000000000000010")
                    .SetName("ToString(S) of 16");
                yield return new TestCaseData(16UL, "s")
                    .Returns("0x0000000000000010")
                    .SetName("ToString(s) of 16");
                yield return new TestCaseData(16UL, "O")
                    .Returns("16")
                    .SetName("ToString(O) of 16");
                yield return new TestCaseData(16UL, "o")
                    .Returns("16")
                    .SetName("ToString(s) of 16");
                yield return new TestCaseData(16UL, "M")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(M) of 16");
                yield return new TestCaseData(16UL, "m")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(m) of 16");
                yield return new TestCaseData(16UL, "H")
                    .Returns("10 00 00 00 00 00 00 00")
                    .SetName("ToString(H) of 16");
                yield return new TestCaseData(16UL, "h")
                    .Returns("10 00 00 00 00 00 00 00")
                    .SetName("ToString(h) of 16");
                yield return new TestCaseData(255UL, null)
                    .Returns("0x00000000000000ff")
                    .SetName("ToString() of 255");
                yield return new TestCaseData(255UL, "G")
                    .Returns("0x00000000000000FF")
                    .SetName("ToString(G) of 255");
                yield return new TestCaseData(255UL, "g")
                    .Returns("0x00000000000000ff")
                    .SetName("ToString(g) of 255");
                yield return new TestCaseData(255UL, "S")
                    .Returns("0x00000000000000FF")
                    .SetName("ToString(S) of 255");
                yield return new TestCaseData(255UL, "s")
                    .Returns("0x00000000000000ff")
                    .SetName("ToString(s) of 255");
                yield return new TestCaseData(255UL, "O")
                    .Returns("255")
                    .SetName("ToString(O) of 255");
                yield return new TestCaseData(255UL, "o")
                    .Returns("255")
                    .SetName("ToString(s) of 255");
                yield return new TestCaseData(255UL, "M")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(M) of 255");
                yield return new TestCaseData(255UL, "m")
                    .Returns("0001-01-01 00:00:00")
                    .SetName("ToString(m) of 255");
                yield return new TestCaseData(255UL, "H")
                    .Returns("FF 00 00 00 00 00 00 00")
                    .SetName("ToString(H) of 255");
                yield return new TestCaseData(255UL, "h")
                    .Returns("ff 00 00 00 00 00 00 00")
                    .SetName("ToString(h) of 255");
                yield return new TestCaseData(UInt64.MaxValue, null)
                    .Returns("0xffffffffffffffff")
                    .SetName($"ToString(G) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "G")
                    .Returns("0xFFFFFFFFFFFFFFFF")
                    .SetName($"ToString(G) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "g")
                    .Returns("0xffffffffffffffff")
                    .SetName($"ToString(g) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "S")
                    .Returns("0xFFFFFFFFFFFFFFFF")
                    .SetName($"ToString(S) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "s")
                    .Returns("0xffffffffffffffff")
                    .SetName($"ToString(s) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "O")
                    .Returns(UInt64.MaxValue.ToString())
                    .SetName($"ToString(O) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "o")
                    .Returns(UInt64.MaxValue.ToString())
                    .SetName($"ToString(o) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "H")
                    .Returns("FF FF FF FF FF FF FF FF")
                    .SetName($"ToString(H) of {UInt64.MaxValue}");
                yield return new TestCaseData(UInt64.MaxValue, "h")
                    .Returns("ff ff ff ff ff ff ff ff")
                    .SetName($"ToString(h) of {UInt64.MaxValue}");
                yield return new TestCaseData(637000000156000065UL, null)
                    .Returns("0x08d714200b38df41")
                    .SetName("ToString() of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "G")
                    .Returns("0x08D714200B38DF41")
                    .SetName("ToString(G) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "g")
                    .Returns("0x08d714200b38df41")
                    .SetName("ToString(g) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "S")
                    .Returns("0x08D714200B38DF41")
                    .SetName("ToString(S) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "s")
                    .Returns("0x08d714200b38df41")
                    .SetName("ToString(s) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "O")
                    .Returns("637000000156000065")
                    .SetName("ToString(O) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "o")
                    .Returns("637000000156000065")
                    .SetName("ToString(s) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "M")
                    .Returns("2019-07-29 12:26:55")
                    .SetName("ToString(M) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "m")
                    .Returns("2019-07-29 12:26:55")
                    .SetName("ToString(m) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "H")
                    .Returns("41 DF 38 0B 20 14 D7 08")
                    .SetName("ToString(H) of 637000000156000065");
                yield return new TestCaseData(637000000156000065UL, "h")
                    .Returns("41 df 38 0b 20 14 d7 08")
                    .SetName("ToString(h) of 637000000156000065");
            }
        }
        #endregion

        #region Tests
        [Test]
        public void TestZeroProperty()
        {
            //Arrange.
            RowVersion rv = RowVersion.Zero;

            //Assert.
            Assert.That(rv.Value.SequenceEqual(new byte[8]), $"The {nameof(RowVersion)}.{nameof(RowVersion.Zero)} property does not contain an all-zero byte array.");
        }

        [Test]
        public void TestByteConstructorWrongByteLength()
        {
            //Arrange.
            byte[] data = new byte[12];

            //Assert.
            Assert.Throws<ArgumentException>(() => { RowVersion rv = new RowVersion(data); },
                $"{nameof(RowVersion)} byte array constructor did not throw the expected {nameof(ArgumentException)} exception.");
        }

        [Test]
        public void TestBytConstructorNullByteArray()
        {
            //Arrange.
            RowVersion rv = new RowVersion(null);

            //Assert.
            Assert.That(rv == RowVersion.Zero, $"Null byte array did not produce the equivalent of {nameof(RowVersion)}.{nameof(RowVersion.Zero)}.");
        }

        [Test]
        [TestCaseSource(nameof(ByteConstructorULongTestData))]
        public ulong ByteConstructorTestNumericResult(byte[] value)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            return (ulong)rv;
        }

        [Test]
        [TestCaseSource(nameof(ByteConstructorDateTimeTestData))]
        public DateTime ByteConstructorTestDateTimeResult(byte[] value)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            return (DateTime)rv;
        }

        [Test]
        [TestCaseSource(nameof(DateTimeConstructorByteTestData))]
        public void DateTimeConstructorTestByteResult(DateTime value, byte[] result)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            Assert.That(rv.Value.SequenceEqual(result), $"The binary value for date '{value:yyyy-MM-dd hh:mm:ss}' did not match.");
        }

        [Test]
        [TestCaseSource(nameof(DateTimeConstructorULongTestData))]
        public ulong DateTimeConstructorTestULongResult(DateTime value)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            return (ulong)rv;
        }

        [Test]
        [TestCaseSource(nameof(ULongConstructorByteTestData))]
        public void ULongConstructorTestByteResult(ulong value, byte[] result)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            Assert.That(rv.Value.SequenceEqual(result), $"The binary value for ulong '{value}' did not match.");
        }

        [Test]
        [TestCaseSource(nameof(ULongConstructorDateTimeTestData))]
        public DateTime ULongConstructorTestDateTimeResult(ulong value)
        {
            //arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            return (DateTime)rv;
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestData))]
        public bool EqualsTest(ulong value1, DateTime value2)
        {
            //Arrange.
            RowVersion rv1 = new RowVersion(value1);
            RowVersion rv2 = new RowVersion(value2);

            //Assert.
            return rv1.Equals(rv2);
        }

        [Test]
        [TestCaseSource(nameof(ComparableTestData))]
        public void ComparableTest(ulong value1, ulong value2, int expectedResult)
        {
            //Arrange.
            RowVersion rv1 = new RowVersion(value1);
            RowVersion rv2 = new RowVersion(value2);

            //Act.
            int result = rv1.CompareTo(rv2);

            //Assert.
            Assert.That(() =>
            {
                return (result == expectedResult && expectedResult == 0)
                    || (result < 0 && expectedResult < 0)
                    || (result > 0 && expectedResult > 0);
            }, $"{nameof(RowVersion)} values {rv1} and {rv2} did not compare as expected (result was {result} and expectancy was {expectedResult}).");
        }

        [Test]
        public void TestInvalidFormatString()
        {
            //Arrange.
            RowVersion rv = new RowVersion((ulong)DateTime.Now.Ticks);

            //Assert.
            Assert.Throws<FormatException>(() => { rv.ToString("ABC"); },
                $"{nameof(RowVersion)} did not throw a {nameof(FormatException)} exception when given an unknown format string.");
        }

        [Test]
        [TestCaseSource(nameof(ToStringTestData))]
        public string ToStringTests(ulong value, string formatString)
        {
            //Arrange.
            RowVersion rv = new RowVersion(value);

            //Assert.
            return formatString == null ? rv.ToString() : rv.ToString(formatString);
        }
        #endregion
    }
}
