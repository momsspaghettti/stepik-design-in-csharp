using System;
using System.Collections.Generic;


namespace Delegates.TreeTraversal {
    public static class Traversal {
        private static IEnumerable<T> WalkAroundTree<TTreeType, T>(
            TTreeType tree,
            Func<TTreeType, bool> predicate,
            Func<TTreeType, IEnumerable<TTreeType>> treeWalker,
            Func<TTreeType, IEnumerable<T>> valuesGetter
        ) {
            if (tree == null)
                yield break;

            if (predicate(tree)) {
                foreach (var value in valuesGetter(tree)) {
                    yield return value;
                }
            }

            foreach (var newTree in treeWalker(tree)) {
                foreach (var value in WalkAroundTree(newTree, predicate, treeWalker, valuesGetter)) {
                    yield return value;
                }
            }
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> tree) {
            return WalkAroundTree(
                tree,
                t => true,
                t => new List<BinaryTree<T>> {t.Left, t.Right},
                t => new List<T> {t.Value}
            );
        }

        public static IEnumerable<Job> GetEndJobs(Job job) {
            return WalkAroundTree(
                job,
                j => j.Subjobs.Count == 0,
                j => j.Subjobs,
                j => new List<Job> {j}
            );
        }

        public static IEnumerable<Product> GetProducts(ProductCategory category) {
            return WalkAroundTree(
                category,
                c => true,
                c => c.Categories,
                c => c.Products
            );
        }
    }
}