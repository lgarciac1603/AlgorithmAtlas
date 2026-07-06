namespace AlgorithmAtlas.Services.NeuralNetworks;

/// <summary>
/// Which non-linearity a hidden layer applies. The output layer always uses the
/// logistic sigmoid so its values read as probabilities for binary classification.
/// </summary>
public enum Activation
{
    Tanh,
    ReLU,
    Sigmoid
}

/// <summary>
/// A minimal, dependency-free multi-layer perceptron trained by backpropagation.
///
/// The network is a stack of fully-connected layers: <c>input → hidden… → output</c>.
/// Hidden layers use the configured <see cref="Activation"/>; the output layer uses the
/// logistic sigmoid, and the loss is binary cross-entropy — so a single output neuron
/// yields P(class = 1). Everything is plain <c>double[]</c> arithmetic so the same code
/// runs in the browser (Blazor WASM) and under xUnit with no ML dependencies.
///
/// The visualizer drives training one epoch at a time via <see cref="TrainEpoch"/>, which
/// lets it read the loss/accuracy and redraw the weights between epochs.
/// </summary>
public sealed class MultiLayerPerceptron
{
    /// <summary>A single fully-connected layer: <c>Weights[out][in]</c> and one bias per output.</summary>
    public sealed class Layer
    {
        public readonly int Inputs;
        public readonly int Outputs;
        public readonly double[][] Weights;
        public readonly double[] Biases;

        // Per–forward-pass caches reused by backprop (one training example at a time).
        public double[] Z = [];   // pre-activation
        public double[] A = [];   // activation (output of this layer)

        public Layer(int inputs, int outputs, Random rng, Activation activation)
        {
            Inputs = inputs;
            Outputs = outputs;
            Weights = new double[outputs][];
            Biases = new double[outputs];

            // He init for ReLU, Xavier/Glorot otherwise — keeps signal variance stable across depth.
            var scale = activation == Activation.ReLU
                ? Math.Sqrt(2.0 / inputs)
                : Math.Sqrt(1.0 / inputs);

            for (var o = 0; o < outputs; o++)
            {
                Weights[o] = new double[inputs];
                for (var i = 0; i < inputs; i++)
                {
                    Weights[o][i] = NextGaussian(rng) * scale;
                }
            }
        }
    }

    private readonly Layer[] _layers;
    private readonly Activation _hidden;
    private readonly Random _rng;

    public IReadOnlyList<Layer> Layers => _layers;
    public int InputSize => _layers[0].Inputs;
    public int OutputSize => _layers[^1].Outputs;

    /// <param name="layerSizes">Neuron count per layer, e.g. <c>[11, 8, 1]</c> = 11 inputs, one hidden layer of 8, one output.</param>
    /// <param name="hidden">Activation for the hidden layers.</param>
    /// <param name="seed">RNG seed so training is reproducible.</param>
    public MultiLayerPerceptron(int[] layerSizes, Activation hidden = Activation.Tanh, int seed = 1)
    {
        if (layerSizes.Length < 2)
        {
            throw new ArgumentException("Need at least an input and an output layer.", nameof(layerSizes));
        }

        _hidden = hidden;
        _rng = new Random(seed);
        _layers = new Layer[layerSizes.Length - 1];
        for (var l = 0; l < _layers.Length; l++)
        {
            // The last layer is the output layer (sigmoid); earlier layers use the hidden activation.
            var act = l == _layers.Length - 1 ? Activation.Sigmoid : hidden;
            _layers[l] = new Layer(layerSizes[l], layerSizes[l + 1], _rng, act);
        }
    }

    /// <summary>Run a single example forward and return the output-layer activations.</summary>
    public double[] Forward(double[] input)
    {
        var a = input;
        for (var l = 0; l < _layers.Length; l++)
        {
            var layer = _layers[l];
            var z = new double[layer.Outputs];
            var outA = new double[layer.Outputs];
            var isOutput = l == _layers.Length - 1;

            for (var o = 0; o < layer.Outputs; o++)
            {
                var sum = layer.Biases[o];
                var w = layer.Weights[o];
                for (var i = 0; i < layer.Inputs; i++)
                {
                    sum += w[i] * a[i];
                }
                z[o] = sum;
                outA[o] = Apply(isOutput ? Activation.Sigmoid : _hidden, sum);
            }

            layer.Z = z;
            layer.A = outA;
            a = outA;
        }
        return a;
    }

    /// <summary>
    /// Train for one full pass over the data using mini-batch stochastic gradient descent,
    /// and return the mean binary cross-entropy loss over the epoch (measured pre-update, per batch).
    /// </summary>
    public double TrainEpoch(double[][] x, double[][] y, double learningRate, int batchSize = 32)
    {
        var n = x.Length;
        var order = Enumerable.Range(0, n).ToArray();
        Shuffle(order);

        double totalLoss = 0;
        for (var start = 0; start < n; start += batchSize)
        {
            var end = Math.Min(start + batchSize, n);
            var count = end - start;

            // Accumulate gradients across the batch, then apply the averaged update once.
            var gradW = _layers.Select(l => l.Weights.Select(row => new double[row.Length]).ToArray()).ToArray();
            var gradB = _layers.Select(l => new double[l.Outputs]).ToArray();

            for (var k = start; k < end; k++)
            {
                var idx = order[k];
                totalLoss += Backprop(x[idx], y[idx], gradW, gradB);
            }

            var step = learningRate / count;
            for (var l = 0; l < _layers.Length; l++)
            {
                var layer = _layers[l];
                for (var o = 0; o < layer.Outputs; o++)
                {
                    for (var i = 0; i < layer.Inputs; i++)
                    {
                        layer.Weights[o][i] -= step * gradW[l][o][i];
                    }
                    layer.Biases[o] -= step * gradB[l][o];
                }
            }
        }

        return totalLoss / n;
    }

    /// <summary>
    /// Forward one example, then backpropagate the error, accumulating gradients into
    /// <paramref name="gradW"/>/<paramref name="gradB"/>. Returns this example's loss.
    /// </summary>
    private double Backprop(double[] input, double[] target, double[][][] gradW, double[][] gradB)
    {
        var output = Forward(input);

        // Output layer: with sigmoid + binary cross-entropy the delta simplifies to (a − y).
        var last = _layers.Length - 1;
        var delta = new double[_layers[last].Outputs];
        double loss = 0;
        for (var o = 0; o < delta.Length; o++)
        {
            var a = output[o];
            delta[o] = a - target[o];
            loss += BinaryCrossEntropy(a, target[o]);
        }

        for (var l = last; l >= 0; l--)
        {
            var layer = _layers[l];
            var prevA = l == 0 ? input : _layers[l - 1].A;

            for (var o = 0; o < layer.Outputs; o++)
            {
                var d = delta[o];
                gradB[l][o] += d;
                var gw = gradW[l][o];
                for (var i = 0; i < layer.Inputs; i++)
                {
                    gw[i] += d * prevA[i];
                }
            }

            if (l == 0)
            {
                break;
            }

            // Propagate delta to the previous (hidden) layer through its activation derivative.
            var prev = _layers[l - 1];
            var newDelta = new double[prev.Outputs];
            for (var i = 0; i < prev.Outputs; i++)
            {
                double sum = 0;
                for (var o = 0; o < layer.Outputs; o++)
                {
                    sum += layer.Weights[o][i] * delta[o];
                }
                newDelta[i] = sum * Derivative(_hidden, prev.Z[i], prev.A[i]);
            }
            delta = newDelta;
        }

        return loss; // already summed over the output neurons
    }

    /// <summary>Probability of class 1 for a single example (single-output networks).</summary>
    public double Predict(double[] input) => Forward(input)[0];

    /// <summary>Fraction of examples classified correctly at a 0.5 threshold (single-output).</summary>
    public double Accuracy(double[][] x, double[][] y, double threshold = 0.5)
    {
        var correct = 0;
        for (var i = 0; i < x.Length; i++)
        {
            var predicted = Forward(x[i])[0] >= threshold ? 1.0 : 0.0;
            if (Math.Abs(predicted - y[i][0]) < 0.5)
            {
                correct++;
            }
        }
        return x.Length == 0 ? 0 : (double)correct / x.Length;
    }

    /// <summary>Mean binary cross-entropy loss over a dataset (single-output).</summary>
    public double Loss(double[][] x, double[][] y)
    {
        double total = 0;
        for (var i = 0; i < x.Length; i++)
        {
            total += BinaryCrossEntropy(Forward(x[i])[0], y[i][0]);
        }
        return x.Length == 0 ? 0 : total / x.Length;
    }

    // ── Activation helpers ──────────────────────────────────────────────────────

    private static double Apply(Activation a, double z) => a switch
    {
        Activation.Tanh => Math.Tanh(z),
        Activation.ReLU => z > 0 ? z : 0,
        Activation.Sigmoid => Sigmoid(z),
        _ => z
    };

    private static double Derivative(Activation a, double z, double activated) => a switch
    {
        Activation.Tanh => 1 - activated * activated,
        Activation.ReLU => z > 0 ? 1 : 0,
        Activation.Sigmoid => activated * (1 - activated),
        _ => 1
    };

    private static double Sigmoid(double z) => 1.0 / (1.0 + Math.Exp(-z));

    private static double BinaryCrossEntropy(double a, double y)
    {
        const double eps = 1e-12;
        a = Math.Clamp(a, eps, 1 - eps);
        return -(y * Math.Log(a) + (1 - y) * Math.Log(1 - a));
    }

    private static double NextGaussian(Random rng)
    {
        // Box–Muller transform.
        var u1 = 1.0 - rng.NextDouble();
        var u2 = 1.0 - rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }

    private void Shuffle(int[] a)
    {
        for (var i = a.Length - 1; i > 0; i--)
        {
            var j = _rng.Next(i + 1);
            (a[i], a[j]) = (a[j], a[i]);
        }
    }
}
