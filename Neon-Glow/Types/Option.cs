#region

using System;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

#endregion

namespace JCS.Neon.Glow.Types
{
    /// <summary>
    ///     Simple option value type which just wraps a type which may have a value,
    ///     or may not have a value. Very loosely based on similar functional types
    ///     such as F# Option, Scala Option or Haskell Maybe
    /// </summary>
    public readonly struct Option<T>
        where T : notnull
    {
        /// <summary>
        ///     Construct a None value
        /// </summary>
        public static Option<T> None => default;

        /// <summary>
        ///     Construct a Some value - requires a value
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns></returns>
        public static Option<T> Some(T value)
        {
            return new(value);
        }

        private readonly bool _some;

        /// <summary>
        ///     The underlying value
        /// </summary>
        private readonly T _value;

        /// <summary>
        ///     Whether this option doesn't have a value
        /// </summary>
        public readonly bool IsNone => !_some;

        /// <summary>
        ///     Default constructor which just wraps a value of type T, also sets the internal isSome flag
        /// </summary>
        /// <param name="value">The value to wrap</param>
        #pragma warning disable 8618
        public Option(T value)
            #pragma warning restore 8618
        {
            _value = value;
            _some = _value is { };
        }

        /// <summary>
        ///     Checks whether the option has a value, and allows for it to be
        ///     "unpacked" through a passed in out parameter.  See the extension methods
        ///     below for usage of this to chain calls.  The MaybeNullWhen attribute is
        ///     used to raise compiler errors/warnings
        /// </summary>
        /// <param name="value">An out parameter which might contain a value.</param>
        /// <returns></returns>
        public bool IsSome(out T value)
        {
            value = _value;
            return _some;
        }

        /// <summary>
        ///     Checks whether the option has a value, but doesn't allow it to
        ///     be unpacked from within the wrapping Option.
        /// </summary>
        /// <returns></returns>
        public bool IsSome()
        {
            return _some;
        }
    }

    /// <summary>
    ///     Extension methods for the <see cref="Option{T}" /> struct which define some of
    ///     basic monadic operations you'd expect for an option
    /// </summary>
    public static class OptionOps
    {
        /// <summary>
        ///     Standard functor for mapping the contents of an option
        /// </summary>
        /// <param name="option"></param>
        /// <param name="f">The mapping function</param>
        /// <typeparam name="V">The target type</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<V> Map<T, V>(this Option<T> option, Func<T?, V> f)
            where T : notnull where V : notnull
        {
            return option.Bind(value => Option<V>.Some(f(value)));
        }

        /// <summary>
        ///     Fold function that will convert an option to a different type based on
        ///     whether or not the option holds a value or not
        /// </summary>
        /// <param name="option"></param>
        /// <param name="onSome">A function to execute on the option value if it exists</param>
        /// <param name="onNone">A function to execute on the option if the wrapped value is null</param>
        /// <typeparam name="T">The wrapped type for the option</typeparam>
        /// <typeparam name="V">The target type</typeparam>
        /// <returns></returns>
        public static V Fold<T, V>(this Option<T> option, Func<T?, V> onSome, Func<V> onNone)
            where T : notnull where V : notnull
        {
            return option.IsSome(out var value) ? onSome(value) : onNone();
        }

        /// <summary>
        ///     The standard Kleisli arrow for monadic binding operations over the <see cref="Option{T}" />
        ///     type
        /// </summary>
        /// <param name="option">The actual option</param>
        /// <param name="binder">Lifting function which can take a value and map to an option</param>
        /// <typeparam name="T">The source option contained type</typeparam>
        /// <typeparam name="V">The target option contained type</typeparam>
        /// <returns></returns>
        public static Option<V> Bind<T, V>(this Option<T> option, Func<T?, Option<V>> binder)
            where V : notnull where T : notnull
        {
            return option.Fold(
                binder,
                () => Option<V>.None
            );
        }

        /// <summary>
        ///     Emulates the Cats/Scalaz GetOrElse method on an option.  Basically, unpacks the value
        ///     or allows for a default value to be synthesised through a lambda.  The default value is
        ///     marked as being nullable
        /// </summary>
        /// <param name="option">The source option wrapping type T</param>
        /// <param name="f">A lambda which can provide a default value</param>
        /// <typeparam name="T">The wrapped type</typeparam>
        /// <returns></returns>
        public static T? GetOrElse<T>(this Option<T> option, Func<T?> f)
            where T : notnull
        {
            return option.IsSome(out var value) ? value : f();
        }
    }
}