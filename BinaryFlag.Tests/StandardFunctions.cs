using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinaryFlag.Tests
{
    [TestClass]
    public class StandardFunctions
    {
        [TestMethod]
        public void SetBinaryFlag_OneTrue_Random()
        {
            //Lets set random number 128!
            //You do not need initialize or ready bytes, 
            //just set your first flag.
            int flagIndex = 128;
            byte[] bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, true);

            Assert.IsTrue(
                bytes[15] == 128 && 
                bytes.Length == 16);
        }
        [TestMethod]
        public void SetBinaryFlag_OneTrue_Min()
        {
            //Lets set min or just first flag.
            int flagIndex = 1;
            byte[] bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, true);

            Assert.IsTrue(
                bytes[0] == 1 &&
                bytes.Length == 1);
        }
        [TestMethod]
        public void SetBinaryFlag_OneTrue_Max()
        {
            //Lets set max or last flag.
            //Int.MaxValue equal to bytes index 268435455 = 64(byte value)
            int flagIndex = int.MaxValue;
            byte[] bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, true);

            Assert.IsTrue(
                bytes[268435455] == 64 &&
                bytes.Length == 268435456);
        }
        [TestMethod]
        public void SetBinaryFlag_OneTrue_OneToEmpty()
        {
            //Lets set single flag to false.
            int flagIndex = 1;
            byte[] bytes = { 1 };

            bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, false, bytes);

            Assert.IsTrue(
                bytes[0] == 0 &&
                bytes.Length == 1);
        }
        [TestMethod]
        public void SetBinaryFlag_OneTrue_EmptyToOne()
        {
            //Lets set empty bytes to true flag.
            int flagIndex = 1;
            byte[] bytes = { 0 };

            bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, true, bytes);

            Assert.IsTrue(
                bytes[0] == 1 &&
                bytes.Length == 1);
        }
        [TestMethod]
        public void SetBinaryFlag_DoubleTrue()
        {
            int flagIndexFirst = 22;
            int flagIndexSecond = 100;
            byte[] bytes;

            bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndexFirst, true);
            bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndexSecond, true, bytes);

            Assert.IsTrue(
                bytes[2] == 32 &&
                bytes[12] == 8 &&
                bytes.Length == 13);
        }
        [TestMethod]
        public void SetBinaryFlag_DoubleOneFalse_TailTrue()
        {
            //What happens with tail zero bytes?
            //Back zeros would be removed.
            int flagIndex = 100;
            byte[] bytes = new byte[13];
            bytes[2] = 32;
            bytes[12] = 8;

            bytes = Standard.Functions.BinaryFunctions.SetBinaryFlag(
                flagIndex, false, bytes);

            Assert.IsTrue(
                bytes[2] == 32 &&
                bytes.Length == 3);
        }

        [TestMethod]
        public void HasBinaryFlag_True()
        {
            //Check if flag is setted.
            int flagIndexFirst = 22;
            int flagIndexSecond = 100;
            byte[] bytes = new byte[13];
            bytes[2] = 32;
            bytes[12] = 8;

            Assert.IsTrue(
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexFirst, bytes) &&
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexSecond, bytes));
        }
        [TestMethod]
        public void HasBinaryFlag_False()
        {
            //Check if flag is setted.
            int flagIndexFirst = 20;
            int flagIndexSecond = 99;
            byte[] bytes = new byte[13];
            bytes[2] = 32; // 22 Flag
            bytes[12] = 8; // 100 Flag

            Assert.IsFalse(
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexFirst, bytes) &&
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexSecond, bytes));
        }
        [TestMethod]
        public void HasBinaryFlag_FirstTrue()
        {
            //Check if flag is setted.
            int flagIndex = 1;
            byte[] bytes = { 1 };

            Assert.IsTrue(
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndex, bytes));
        }
        [TestMethod]
        public void HasBinaryFlag_LastTrue()
        {
            //Check if flag is setted.
            int flagIndex = int.MaxValue;
            byte[] bytes = new byte[268435456];
            bytes[268435455] = 64;

            Assert.IsTrue(
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndex, bytes));
        }

        [TestMethod]
        public void CreateBinaryIndexes()
        {
            int flagIndexFirst = 22;
            int flagIndexSecond = 100;
            int flagIndexThird = int.MaxValue;

            IEnumerable<int> flagIndexes = new List<int> { 22, 100 , int.MaxValue};
            byte[] bytes = Standard.Functions.BinaryFunctions.CreateBinaryIndexes(flagIndexes);

            Assert.IsTrue(
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexFirst, bytes) &&
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexSecond, bytes) &&
                Standard.Functions.BinaryFunctions.HasBinaryFlag(flagIndexThird, bytes));
        }

        [TestMethod]
        public void FindBinaryIndexes()
        {
            IEnumerable<int> flagIndexes = new List<int> { 22, 100, int.MaxValue };
            byte[] bytes = new byte[268435456];
            bytes[2] = 32; // 22 Flag
            bytes[12] = 8; // 100 Flag
            bytes[268435455] = 64;

            IEnumerable<int> flagIndexesResult = Standard.Functions.BinaryFunctions.FindBinaryIndexes(bytes);

            Assert.IsTrue(flagIndexesResult.SequenceEqual(flagIndexes));
        }
    }
}
