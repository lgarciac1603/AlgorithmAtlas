## Overview

A **trie** (from re*trie*val, pronounced "try") is a tree where each node represents one character. A path from the root to a node marked as an end-of-word spells a stored string. Nodes for **shared prefixes are shared**, making tries extremely space-efficient for large dictionaries with common prefixes.

## How It Works

- The root represents the empty string.
- Each edge is labelled with a character; following the edge advances one character.
- Nodes that complete a valid word carry an **isEnd** flag.
- To **insert** "cat": create/walk nodes for 'c' → 'a' → 't', then set isEnd on 't'.
- To **search** "car": walk 'c' → 'a' → 'r'; return true only if the last node has isEnd set.

## Complexity

| Operation | Time | Why |
|-----------|------|-----|
| Insert | O(m) | One node per character, m = word length |
| Search | O(m) | Walk at most m edges |
| Starts-with | O(m) | Same as search, ignore isEnd |
| Space | O(m · n) | Worst case: no shared prefixes |

## Why it matters

- **Search is O(m)** — independent of how many words are stored. A hash table also achieves O(m) average, but a trie additionally supports prefix queries (`autocomplete`, `startsWith`) in O(m).
- Used in autocomplete engines, spell checkers, IP routing tables (CIDR), and T9 phone keyboards.

## Pseudocode

```
insert(root, word):
    node = root
    for ch in word:
        if ch not in node.children:
            node.children[ch] = new Node()
        node = node.children[ch]
    node.isEnd = true

search(root, word):
    node = root
    for ch in word:
        if ch not in node.children: return false
        node = node.children[ch]
    return node.isEnd
```
