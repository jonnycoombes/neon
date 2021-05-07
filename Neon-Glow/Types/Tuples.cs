#region

using System;

#endregion

namespace JCS.Neon.Glow.Types
{
    /// <summary>
    ///     Parameterised pair type which may be used instead of the standard C#
    ///     tuple type.  Has a few useful functor-like methods in support of it
    /// </summary>
    /// <typeparam name="S">The type of the left element in the pair</typeparam>
    /// <typeparam name="T">The type of the right element in the pair</typeparam>
    public class Pair<S, T>
    {
        /// <summary>
        ///     Constructor taking a left and right value
        /// </summary>
        /// <param name="left">The left value for the pair</param>
        /// <param name="right">The right value for the pair</param>
        public Pair(S left, T right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        ///     First element in the pair, or leftmost
        /// </summary>
        public S Left { get; set; }

        /// <summary>
        ///     Second element in the pair, or rightmost
        /// </summary>
        public T Right { get; set; }

        /// <summary>
        ///     A left projection function
        /// </summary>
        /// <param name="f">Function which types type S to type V</param>
        /// <typeparam name="V">Projected type</typeparam>
        /// <returns>f(Left)</returns>
        public V LeftProjection<V>(Func<S, V> f)
        {
            return f(Left);
        }

        /// <summary>
        ///     A right projection function
        /// </summary>
        /// <param name="g">Function which types type T to type V</param>
        /// <typeparam name="V">Projected type</typeparam>
        /// <returns>g(Right)</returns>
        public V RightProjection<V>(Func<T, V> g)
        {
            return g(Right);
        }

        /// <summary>
        ///     Mapping (functor) function
        /// </summary>
        /// <param name="f">Function taking type S to type V</param>
        /// <param name="g">Funciton taking type T to type W</param>
        /// <typeparam name="V">Left projected type</typeparam>
        /// <typeparam name="W">Right projected type</typeparam>
        /// <returns>Pair(f(Left), g(Right))</returns>
        public Pair<V, W> Map<V, W>(Func<S, V> f, Func<T, W> g)
        {
            return new(f(Left), g(Right));
        }

        /// <summary>
        ///     Reduction function (sort of like a homomorphism)
        /// </summary>
        /// <param name="f">Function taking a pair (S, T) to a type V</param>
        /// <typeparam name="V">Reduced type</typeparam>
        /// <returns>f(Left, Right)</returns>
        public V Reduce<V>(Func<S, T, V> f)
        {
            return f(Left, Right);
        }
    }
}