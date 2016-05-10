using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LessMsi.IO
{
    partial class PathEx
    {
        private class FileNode
        {
            public FileNode(string path)
            {
                // To make some comparisons easier (like descendents/ancestors, etc.), we just want to prefix all paths in FileNode. Otherwise some long paths will have it and other ancesotors won't have the prefix.
                this.Path = PathEx.EnsureLongPathPrefix(path);
                this.Children = new List<FileNode>();
            }

            public string Path { get; private set; }
            public List<FileNode> Children { get; private set; }

            public bool Equals(string path)
            {
                return PathEx.Equals(this.Path, path);
            }

            public bool IsDescendentPath(string pathName)
            {
                if (string.IsNullOrEmpty(pathName))
                    throw new ArgumentNullException("pathName");
                // HACK: This isn't safely looking at path separtors, but since all of our paths are from the same source it's probably safe (famous last words)
                return pathName.Length > this.Path.Length
                    && pathName.StartsWith(this.Path, StringComparison.CurrentCultureIgnoreCase);
            }

            public FileNode FindChild(string pathName)
            {
                return Children.Find(f => string.Equals(pathName, f.Path, StringComparison.InvariantCultureIgnoreCase));
            }

            public FileNode FindDescendent(string pathName)
            {
                var ancestor = this.FindClosestAncestor(pathName);
                if (ancestor != null)
                {
                    return ancestor.FindChild(pathName);
                }
                return null;
            }

            /// <summary>
            /// Returns the FileNode that is the closest ancestor of the specified childPathName.
            /// If the current node and no child nodes are ancestors of the specified path, then null is returned.
            /// </summary>
            /// <param name="childPathName">The path for which an ancestor is sought.</param>
            public FileNode FindClosestAncestor(string childPathName)
            {
                return FindClosestAncestor(this, childPathName);
            }

            /// <summary>
            /// Returns the FileNode that is the closest ancestor of the specified childPathName
            /// </summary>
            /// <param name="tree">The tree to search.</param>
            /// <param name="childPathName">The path for which an ancestor is sought.</param>
            /// <returns>FileNode or null if the tree does not contain an ancestor.</returns>
            public static FileNode FindClosestAncestor(FileNode tree, string childPathName)
            {
                FileNode candidate = null;
                if (tree.IsDescendentPath(childPathName))
                {
                    candidate = tree;
                    var ancestor = tree.Children.Find(f => f.IsDescendentPath(childPathName));
                    if (ancestor != null)
                    {
                        candidate = ancestor;
                        // is there a closer ancestor?
                        var closer = FindClosestAncestor(ancestor, childPathName);
                        if (closer != null)
                            candidate = closer;
                    }
                }
                return candidate;
            }

            internal void Insert(FileNode newNode)
            {
                var parent = this.FindClosestAncestor(newNode.Path);
                Debug.Assert(parent.FindChild(newNode.Path) == null, "Node already exists!");
                Debug.Assert(!parent.Equals(newNode.Path), "adding node as a child of hisself!");
                parent.Children.Add(newNode);
            }

            /// <summary>
            /// Deletes the file or directory associated with this node and all children.
            /// </summary>
            public void DeleteFile()
            {
                this.Children.ForEach(f => f.DeleteFile());
                PathEx.DeleteFileOrDirectory(this.Path);
            }
        }
    }
}
