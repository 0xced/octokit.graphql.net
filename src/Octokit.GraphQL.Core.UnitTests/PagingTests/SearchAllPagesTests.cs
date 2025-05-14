using Octokit.GraphQL.Core.Builders;
using Octokit.GraphQL.Core.UnitTests.Models;
using Xunit;

namespace Octokit.GraphQL.Core.UnitTests
{
    public class SearchAllPagesTests
    {
        static SearchAllPagesTests()
        {
            ExpressionCompiler.IsUnitTesting = true;
        }

        [Fact]
        public void Creates_Query()
        {
            var query = new Query()
                .Search("query")
                .AllPages(pageSize: 123)
                .Select(issueOrPr => new { IssueNumber = issueOrPr.Switch<int>(when => when.Issue(issue => issue.Number)) })
                .Compile();

            const string expected = """
            query {
              search(query: "query", first: 123) {
                pageInfo {
                  hasNextPage
                  endCursor
                }
                nodes {
                  __typename
                  ... on Issue {
                    number
                  }
                }
              }
            }
            """;

            Assert.Equal(expected, query.ToString(), ignoreLineEndingDifferences: true);
        }
    }
}