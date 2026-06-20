## Overview

An **AVL tree** (Adelson-Velsky and Landis, 1962) is a self-balancing Binary Search Tree. It maintains the **balance invariant**: for every node, the heights of its left and right subtrees differ by at most 1. This guarantees the tree height stays O(log n), so search, insert, and delete never degrade to O(n) the way an unbalanced BST can.

## Balance Factor

Each node tracks its **balance factor** (BF):

```
BF = height(left subtree) − height(right subtree)
```

A valid AVL node has BF ∈ {−1, 0, 1}. After an insertion, if any ancestor gets |BF| = 2, the tree is repaired with at most one or two **rotations**.

## The Four Rotation Cases

| Imbalance | Detection | Fix |
|-----------|-----------|-----|
| Left-Left (LL) | BF = +2, left child BF ≥ 0 | Right rotation at imbalanced node |
| Right-Right (RR) | BF = −2, right child BF ≤ 0 | Left rotation at imbalanced node |
| Left-Right (LR) | BF = +2, left child BF < 0 | Left rotation on left child, then right rotation |
| Right-Left (RL) | BF = −2, right child BF > 0 | Right rotation on right child, then left rotation |

## Why it matters

- Worst-case height ≤ 1.44 log₂(n) — only ~44% taller than a perfect tree.
- Every operation is **O(log n)** even for adversarial insertion orders that would destroy an ordinary BST.
- Used as the foundation for `TreeMap` / `SortedDictionary` in many standard libraries.

## Pseudocode — Insert + Rebalance

```
insert(root, value):
    root = bst_insert(root, value)
    walk up ancestors:
        update_height(node)
        if |BF(node)| > 1: rotate(node); break
```
