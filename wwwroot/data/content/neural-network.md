## Overview

A **neural network** learns a function directly from examples. A **multi-layer perceptron (MLP)** stacks layers of neurons: each neuron takes a weighted sum of the previous layer, adds a bias, and passes it through a non-linear **activation**. With one or more hidden layers it can approximate highly non-linear decision boundaries.

Here the network solves a **supervised** task on real Kepler data: given the physical measurements of a *Kepler Object of Interest* — orbital period, transit depth, planet radius, insolation, stellar temperature, and so on — decide whether it is a **CONFIRMED** exoplanet or a **FALSE POSITIVE**.

> The demo deliberately withholds `koi_score` and the `koi_fpflag_*` columns. Those are NASA's own vetting verdicts; feeding them in would let the network "cheat" instead of learning from the physics.

## The Core Idea: Backpropagation

Training means finding the weights that minimize a **loss** — here **binary cross-entropy**, which punishes confident wrong answers. Backpropagation computes the gradient of the loss with respect to *every* weight efficiently, by applying the chain rule layer by layer, from the output back toward the input.

Each training step has two passes:

**1. Forward pass** — push an example through the network to get a prediction.

```
a⁰ = x                          // input
zˡ = Wˡ · aˡ⁻¹ + bˡ            // weighted sum
aˡ = activation(zˡ)            // hidden: tanh, output: sigmoid
ŷ  = aᴸ                        // predicted P(confirmed)
```

**2. Backward pass** — measure the error at the output and propagate it backward.

```
δᴸ = aᴸ − y                     // sigmoid + cross-entropy → clean delta
δˡ = (Wˡ⁺¹ᵀ · δˡ⁺¹) ⊙ activation'(zˡ)   // chain rule through each hidden layer
∂L/∂Wˡ = δˡ · (aˡ⁻¹)ᵀ           // gradient for the weights
∂L/∂bˡ = δˡ                     // gradient for the biases
```

**3. Update** — nudge every weight down its gradient (**gradient descent**):

```
Wˡ ← Wˡ − η · ∂L/∂Wˡ            // η = learning rate
```

Repeating this over the data — one **epoch** at a time, in small mini-batches — steadily lowers the loss and raises accuracy. Watch the edges in the diagram thicken and change colour (green = positive weight, red = negative) as the network learns.

## Design Choices in the Demo

- **Architecture** `inputs → hidden (tanh) → 1 output (sigmoid)`. Sigmoid squashes the output to a probability.
- **Weight init** Xavier/Glorot scaling keeps signal variance stable so training starts cleanly.
- **Standardization** features are log-compressed where heavy-tailed (period, depth, insolation…) then z-scored, so no single feature dominates the gradient.
- **Split** a stratified 75 / 25 train/test split reports honest generalization, not memorization.

## Complexity

Let `W` be the total number of weights, `n` the training examples, and `E` the epochs.

| Operation | Cost |
|---|---|
| Forward pass | O(W) |
| Backprop (one example) | O(W) |
| One epoch | O(n · W) |
| Full training | O(E · n · W) |

Memory is **O(W)** for the weights and their gradients.

## Use Cases

- Classification and regression on tabular, image, or text features.
- Any problem with enough labeled data to learn a non-linear mapping.
- The foundation under deeper architectures (CNNs, RNNs, transformers), which all train by backpropagation.

## Common Pitfalls

- **Learning rate too high** → the loss oscillates or diverges; too low → painfully slow. Try the slider both ways.
- **Data leakage** — including a feature that encodes the answer (like `koi_score`) inflates accuracy but teaches nothing.
- **Unscaled inputs** make gradients lopsided and training unstable.
- **Overfitting** — a network that aces training data but fails on the test set has memorized, not generalized.
