using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinaryFlag.Tests
{
    [TestClass]
    public class DotnetFunctionsExceptions
    {
        [TestMethod]
        public void SetBinaryFlag_WrongIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                NET.Functions.BinaryFunctions.SetBinaryFlag(0, true);
            });
        }
        [TestMethod]
        public void SetBinaryFlag_ByteArrayOutOfRange_Length()
        {
            byte[] bytes = new byte[268435457]; // Wrong length, maximum is 268435456

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.SetBinaryFlag(1, true, bytes);
            });
        }
        [TestMethod]
        public void SetBinaryFlag_ByteArrayOutOfRange_LengthValue()
        {
            byte[] bytes = new byte[268435456]; // Corrent Length
            bytes[268435455] = 92; // Maximum is index[268435455] = 64(byte)

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.SetBinaryFlag(1, true, bytes);
            });
        }

        [TestMethod]
        public void HasBinaryFlag_WrongIndex()
        {
            byte[] bytes = new byte[1]; // Corrent Length

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                NET.Functions.BinaryFunctions.HasBinaryFlag(0, bytes);
            });
        }
        [TestMethod]
        public void HasBinaryFlag_ByteArrayOutOfRange_Length()
        {
            byte[] bytes = new byte[268435457]; // Wrong length, maximum is 268435456

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.SetBinaryFlag(1, true, bytes);
            });
        }
        [TestMethod]
        public void HasBinaryFlag_ByteArrayOutOfRange_LengthValue()
        {
            byte[] bytes = new byte[268435456]; // Corrent Length
            bytes[268435455] = 92; // Maximum is index[268435455] = 64(byte)

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.SetBinaryFlag(1, true, bytes);
            });
        }

        [TestMethod]
        public void FindBinaryIndexes_ByteArrayOutOfRange_Length()
        {
            byte[] bytes = new byte[268435457]; // Wrong length, maximum is 268435456

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.FindBinaryIndexes(bytes);
            });
        }
        [TestMethod]
        public void FindBinaryIndexes_ByteArrayOutOfRange_LengthValue()
        {
            byte[] bytes = new byte[268435456]; // Corrent Length
            bytes[268435455] = 92; // Maximum is index[268435455] = 64(byte)

            Assert.ThrowsException<ArgumentException>(() =>
            {
                NET.Functions.BinaryFunctions.FindBinaryIndexes(bytes);
            });
        }

    }
}
