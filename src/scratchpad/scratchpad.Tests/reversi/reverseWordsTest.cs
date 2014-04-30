using scratchpad.Reversi;
using Xunit;

namespace scratchpad.Tests.reversi
{
    public class ReverseWordsTest
    {

        [Fact]
        public void ReverseWordsTest_InputNull()
        {
            var output = ReverseWords.Reverse(null);

            Assert.Equal("", output);
        }

        [Fact]
        public void ReverseWordsTest_InputLength()
        {
            var input = "abcd";
            var expected = 3;

            var output = ReverseWords.GetLastIndex(input);

            Assert.Equal(expected, output);
        }

        [Fact]
        public void ReverseWordsTest_Reversed()
        {
            var input = "ab";
            var expected = "ba";
            string actual;

            actual = ReverseWords.Reverse(input);

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void ReverseWordsTest_NoSpaceReturnLength()
        {
            var input = "abcd";
            var expected = 4;
            int actual;

            actual = ReverseWords.NextNonCharIndex(input, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReverseWordsTest_SpaceReturnsIndex()
        {
            var input = "ab cd";
            var expected = 2;

            int actual = ReverseWords.NextNonCharIndex(input, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReverseWordsTest_SpaceFirstReturnsZero()
        {
            var input = " abcd";
            var expected = 0;

            int actual = ReverseWords.NextNonCharIndex(input, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReverseWordsTest_SpaceLastReturnsLengthMinusOne()
        {
            var input = "abcd ";
            var expected = 4;

            int actual = ReverseWords.NextNonCharIndex(input, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReverseWordsTest_MultipleSpacesReturnsFirstSpaceIndex()
        {
            var input = "ab cd ";
            var expected = 2;

            int actual = ReverseWords.NextNonCharIndex(input, 0);

            Assert.Equal(expected, actual);
        }

        public void ReverseWordsTest_RegexReturnsTrueIfLetter()
        {
            var input = 'a';
            var expected = true;

            bool actual = ReverseWords.IsLetter(input);

            Assert.Equal(expected, actual);
        }
        //[Fact]
        //public void ReverseWordsTest_PunctuationReturnsIndex()
        //{
        //    var input = "ab!cd";
        //    var expected = 2;

        //    int actual = ReverseWords.NextNonCharIndex(input, 0);
        //}
    }
}
