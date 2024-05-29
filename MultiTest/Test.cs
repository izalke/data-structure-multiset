Public Class Test
    Imports MultiSetLib

Using System;
Using Xunit;

Namespace MultiSetLib
{
    Public Class MultiSetTests
    {

        [Fact]
        Public void Test_Add_And_Count()
        {
            var multiSet = New MultiSet<Char>();
            multiSet.Add('a');
            multiSet.Add('b', 2);
            multiSet.Add('c', 3);

            Assert.Equal(6, multiSet.Count);
            Assert.Equal(1, multiSet['a']);
            Assert.Equal(2, multiSet['b']);
            Assert.Equal(3, multiSet['c']);
            
        }

        [Fact]
        Public void Test_Remove()
        {
            var multiSet = New MultiSet<Char>(New[] { 'a', 'a', 'b', 'c', 'c', 'c' });
            multiSet.Remove('a');
            multiSet.Remove('c', 2);

            Assert.Equal(3, multiSet.Count);
            Assert.Equal(1, multiSet['a']);
            Assert.Equal(1, multiSet['b']);
            Assert.Equal(1, multiSet['c']);
            
        }

        [Fact]
        Public void Test_Contains()
        {
            var multiSet = New MultiSet<Char>(New[] { 'a', 'b', 'c' });
            Assert.True(multiSet.Contains('a'));
            Assert.False(multiSet.Contains('d'));
            
        }

        [Fact]
        Public void Test_Clear()
        {
            var multiSet = New MultiSet<Char>(New[] { 'a', 'b', 'c' });
            multiSet.Clear();
            Assert.Equal(0, multiSet.Count);
            
        }

        [Fact]
        Public void Test_UnionWith()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b' });
            var multiSet2 = New MultiSet<Char>(New[] { 'b', 'c' });
            multiSet1.UnionWith(multiSet2);

            Assert.Equal(4, multiSet1.Count);
            Assert.Equal(1, multiSet1['a']);
            Assert.Equal(2, multiSet1['b']);
            Assert.Equal(1, multiSet1['c']);
        }

        [Fact]
        Public void Test_IntersectWith()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b', 'b', 'c' });
            var multiSet2 = New MultiSet<Char>(New[] { 'b', 'c', 'c' });
            multiSet1.IntersectWith(multiSet2);

            Assert.Equal(2, multiSet1.Count);
            Assert.Equal(1, multiSet1['b']);
            Assert.Equal(1, multiSet1['c']);
        }

        [Fact]
        Public void Test_ExceptWith()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b', 'b', 'c' });
            var multiSet2 = New MultiSet<Char>(New[] { 'b', 'c', 'c' });
            multiSet1.ExceptWith(multiSet2);

            Assert.Equal(1, multiSet1.Count);
            Assert.Equal(1, multiSet1['a']);
            Assert.False(multiSet1.Contains('b'));
            Assert.False(multiSet1.Contains('c'));
        }

        [Fact]
        Public void Test_SymmetricExceptWith()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b', 'c' });
            var multiSet2 = New MultiSet<Char>(New[] { 'b', 'c', 'd' });
            multiSet1.SymmetricExceptWith(multiSet2);

            Assert.Equal(2, multiSet1.Count);
            Assert.Equal(1, multiSet1['a']);
            Assert.Equal(1, multiSet1['d']);
            Assert.False(multiSet1.Contains('b'));
            Assert.False(multiSet1.Contains('c'));
        }

        [Fact]
        Public void Test_MultiSetEquals()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b', 'b' });
            var multiSet2 = New MultiSet<Char>(New[] { 'b', 'a', 'b' });

            Assert.True(multiSet1.MultiSetEquals(multiSet2));

            multiSet2.Remove('b');
            Assert.False(multiSet1.MultiSetEquals(multiSet2));
        }

        [Fact]
        Public void Test_ProperSubset()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b' });
            var multiSet2 = New MultiSet<Char>(New[] { 'a', 'b', 'c' });

            Assert.True(multiSet1.IsProperSubsetOf(multiSet2));
            Assert.False(multiSet2.IsProperSubsetOf(multiSet1));
        }

        [Fact]
        Public void Test_ProperSuperset()
        {
            var multiSet1 = New MultiSet<Char>(New[] { 'a', 'b', 'c' });
            var multiSet2 = New MultiSet<Char>(New[] { 'a', 'b' });

            Assert.True(multiSet1.IsProperSupersetOf(multiSet2));
            Assert.False(multiSet2.IsProperSupersetOf(multiSet1));
        }

        [Fact]
        public void Test_AsSet()
        {
            var multiSet = new MultiSet<char>(new[] { 'a', 'b', 'b', 'c', 'c', 'c' });
            var set = multiSet.AsSet();

            Assert.Equal(3, set.Count);
            Assert.Contains('a', set);
            Assert.Contains('b', set);
            Assert.Contains('c', set);
        }

        [Fact]
        public void Test_AsDictionary()
        {
            var multiSet = new MultiSet<char>(new[] { 'a', 'b', 'b', 'c', 'c', 'c' });
            var dictionary = multiSet.AsDictionary();

            Assert.Equal(3, dictionary.Count);
            Assert.Equal(1, dictionary['a']);
            Assert.Equal(2, dictionary['b']);
            Assert.Equal(3, dictionary['c']);
        }
       



    }
}

End Class
