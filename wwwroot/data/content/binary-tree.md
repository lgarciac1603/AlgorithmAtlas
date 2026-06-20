## Overview

A **Binary Search Tree (BST)** is a hierarchical structure where each **node** has at most two children: a **left** child and a **right** child. It maintains the **BST property**:

> For every node, all values in its left subtree are smaller, and all values in its right subtree are larger.

This ordering makes searching, inserting, and deleting efficient — roughly **O(log n)** on average.

## How It Works

- The topmost node is the **root**.
- A node with no children is a **leaf**.
- The **height** of the tree is the longest path from root to a leaf.
- To search for a value, compare it with the current node and recurse **left** (if smaller) or **right** (if larger), starting from the root — halving the search space at each step, similar to binary search.

### Balance Matters

If values are inserted in sorted order, a BST degenerates into a **linked list** (height = n), making operations O(n). **Self-balancing trees** (AVL, Red-Black) guarantee O(log n) by re-arranging nodes after insertions/deletions.

## Common Operations

| Operation | Average    | Worst Case | Why                                         |
|-----------|------------|------------|------------------------------------------------|
| Search    | O(log n)   | O(n)       | Worst case: degenerate (linked-list-like) tree  |
| Insert    | O(log n)   | O(n)       | Same                                              |
| Delete    | O(log n)   | O(n)       | Same — plus handling 0/1/2-child cases           |

## Traversals

- **In-order** (left, node, right) — visits nodes in sorted order for a BST.
- **Pre-order** (node, left, right) — useful for copying/serializing a tree.
- **Post-order** (left, right, node) — useful for deleting a tree bottom-up.
- **Level-order** (BFS using a queue) — visits nodes level by level.

## Use Cases

- Maintaining a sorted, dynamically changing collection (e.g. `TreeMap`, `SortedSet`).
- Implementing range queries ("find all values between X and Y").
- Symbol tables in compilers, file system directory structures.

## Pseudocode — Search

```
function search(node, target):
    if node == null:
        return null
    if target == node.value:
        return node
    if target < node.value:
        return search(node.left, target)
    return search(node.right, target)
```

## Common Pitfalls

- Assuming O(log n) without considering whether the tree is balanced.
- Deleting a node with two children incorrectly — must replace it with its in-order successor (or predecessor) and remove that one instead.
- Confusing "binary tree" (any tree with ≤2 children per node) with "binary **search** tree" (which has the ordering property).
