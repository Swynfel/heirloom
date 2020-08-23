using System;
using Godot;

public struct N<T> where T : Node {
    string path;
    Node root;

    T _n;

    public N(string path, Node root) {
        this.path = path;
        this.root = root;
        this._n = null;
    }

    public T n {
        get {
            if (_n == null) {
                if (path == "") {
                    return null;
                }
                _n = root.GetNode<T>(path);
            }
            return _n;
        }
    }
}

public static class NExtension {
    public static N<T> C<T>(this Node node, string path) where T : Node {
        return new N<T>(path, node);
    }
}