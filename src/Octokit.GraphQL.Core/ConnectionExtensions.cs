using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Octokit.GraphQL.Core;

namespace Octokit.GraphQL
{
    public static class ConnectionExtensions
    {
        public static Task<T> Run<T>(
            this IConnection connection,
            IQueryableValue<T> expression,
            CancellationToken cancellationToken = default)
        {
            return connection.Run(expression.Compile(), cancellationToken: cancellationToken);
        }

        public static Task<IEnumerable<T>> Run<T>(
            this IConnection connection,
            IQueryableList<T> expression,
            CancellationToken cancellationToken = default)
        {
            return connection.Run(expression.Compile(), cancellationToken: cancellationToken);
        }

        public static async Task<T> Run<T>(
            this IConnection connection,
            ICompiledQuery<T> query,
            Dictionary<string, object> variables = null,
            CancellationToken cancellationToken = default)
        {
            var run = query.Start(connection, variables);
            while (await run.RunPage(cancellationToken).ConfigureAwait(false)) { }
            return run.Result;
        }

        public static async IAsyncEnumerable<T> Enumerate<T>(
            this IConnection connection,
            IQueryableValue<T> expression,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var result in connection.Enumerate(expression.Compile(), cancellationToken: cancellationToken))
            {
                yield return result;
            }
        }

        public static async IAsyncEnumerable<T> Enumerate<T>(
            this IConnection connection,
            IQueryableList<T> expression,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var results in connection.Enumerate(expression.Compile(), cancellationToken: cancellationToken))
            {
                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        public static async IAsyncEnumerable<T> Enumerate<T>(
            this IConnection connection,
            ICompiledQuery<T> query,
            Dictionary<string, object> variables = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var run = query.Start(connection, variables);
            await foreach (var result in run.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return result;
            }
        }
    }
}
