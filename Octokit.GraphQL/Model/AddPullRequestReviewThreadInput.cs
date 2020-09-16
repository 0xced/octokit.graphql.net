namespace Octokit.GraphQL.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Autogenerated input type of AddPullRequestReviewThread
    /// </summary>
    public class AddPullRequestReviewThreadInput
    {
        /// <summary>
        /// Path to the file being commented on.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Body of the thread's first comment.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The node ID of the pull request reviewing
        /// </summary>
        public ID? PullRequestId { get; set; }

        /// <summary>
        /// The Node ID of the review to modify.
        /// </summary>
        public ID? PullRequestReviewId { get; set; }

        /// <summary>
        /// The line of the blob to which the thread refers. The end of the line range for multi-line comments.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The side of the diff on which the line resides. For multi-line comments, this is the side for the end of the line range.
        /// </summary>
        public DiffSide? Side { get; set; }

        /// <summary>
        /// The first line of the range to which the comment refers.
        /// </summary>
        public int? StartLine { get; set; }

        /// <summary>
        /// The side of the diff on which the start line resides.
        /// </summary>
        public DiffSide? StartSide { get; set; }

        /// <summary>
        /// A unique identifier for the client performing the mutation.
        /// </summary>
        public string ClientMutationId { get; set; }
    }
}