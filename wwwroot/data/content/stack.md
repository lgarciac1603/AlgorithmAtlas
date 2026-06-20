## Overview

A **stack** is a collection that follows the **LIFO** principle — *Last In, First Out*. The last element added is the first one removed, just like a stack of plates: you add and remove from the top only.

## How It Works

A stack supports two main operations:

- **Push** — add an element to the top.
- **Pop** — remove and return the top element.
- **Peek / Top** — look at the top element without removing it.

Stacks can be implemented using a **dynamic array** (push/pop at the end) or a **linked list** (push/pop at the head) — both give O(1) operations.

## Common Operations

| Operation | Time | Why                                  |
|-----------|------|----------------------------------------|
| Push      | O(1) | Add/remove only at one end             |
| Pop       | O(1) | Add/remove only at one end             |
| Peek      | O(1) | Direct access to the top element       |
| Search    | O(n) | Must pop or traverse to find an item   |

## Use Cases

- **Function call stack** — tracks active function calls and local variables (recursion).
- **Undo/Redo** in editors.
- **Expression evaluation** — matching parentheses, converting infix to postfix, evaluating postfix expressions.
- **Backtracking algorithms** (e.g. maze solving, DFS).
- Browser **back button** history.

## Pseudocode — Balanced Parentheses

```
function isBalanced(expression):
    stack = []
    for char in expression:
        if char in "([{":
            stack.push(char)
        else if char in ")]}":
            if stack.isEmpty():
                return false
            top = stack.pop()
            if not matches(top, char):
                return false
    return stack.isEmpty()
```

## Common Pitfalls

- Popping from an empty stack (always check `isEmpty()` first).
- Confusing stack order with queue order — stacks reverse the order of insertion.
- Using a stack when order should be preserved (a queue might be the correct choice).
