## Overview

**K-means** is the classic **unsupervised** clustering algorithm. Where the neural network above is *told* the right answer for each example, k-means gets **no labels at all** — it simply partitions points into `k` groups so that each point is close to its group's center. It's how you find structure in data you haven't labeled yet.

In this demo it clusters a sample of Kepler objects using two features you choose (planet radius, transit depth, insolation, …). Toggle **Reveal labels** afterward to see how well the unlabeled clusters line up with the real CONFIRMED / FALSE POSITIVE split.

## The Core Idea

Pick `k` cluster centers (**centroids**), then repeat two steps until nothing changes — this is **Lloyd's algorithm**:

```
kmeans(points, k):
    centroids = seed k centers            // k-means++ (see below)
    repeat:
        // 1. Assignment step
        for each point p:
            assign p to the nearest centroid

        // 2. Update step
        for each cluster c:
            centroid[c] = mean of points assigned to c
    until assignments stop changing
```

Each iteration can only *lower* the total within-cluster distance (**inertia**), so the process is guaranteed to converge — watch the inertia in the footer drop and settle.

## Smarter Seeding: k-means++

Random initial centers can land on top of each other and produce poor clusters. **k-means++** spreads the seeds out: after picking the first center at random, each subsequent center is chosen with probability proportional to its **squared distance** from the nearest existing center. This simple change dramatically improves both quality and convergence speed, and it's what the demo uses.

## Reading the Result

- **Inertia** — sum of squared distances from points to their centroid. Lower means tighter clusters. It also drives the "elbow method" for choosing `k`.
- **Cluster ↔ label agreement** — after convergence the demo assigns each cluster its majority true label and reports the fraction of points that match. High agreement means the physical features alone carry a real CONFIRMED-vs-FALSE-POSITIVE signal; modest agreement shows the honest limit of clustering two raw features without supervision.

## Complexity

For `n` points, `k` clusters, `d` dimensions, and `i` iterations:

| Step | Cost |
|---|---|
| Assignment | O(n · k · d) |
| Update | O(n · d) |
| Full run | O(i · n · k · d) |

Space is **O(n + k · d)**.

## Use Cases

- Customer / market segmentation and exploratory data analysis.
- Image color quantization and vector compression.
- Feature learning and pre-labeling before a supervised pass.

## Common Pitfalls

- **Choosing k** is on you — too few merges distinct groups, too many splinters them. Use the elbow (inertia) or silhouette score.
- **Sensitive to scale** — features must be standardized or the largest-range one dominates the distance.
- **Assumes round, similar-size clusters** (squared Euclidean distance); elongated or nested shapes need DBSCAN or spectral clustering.
- **Local minima** — different seeds give different results; k-means++ and multiple restarts help.
