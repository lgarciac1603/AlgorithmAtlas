using AlgorithmAtlas.Services.NeuralNetworks;

namespace AlgorithmAtlas.Tests;

public class MultiLayerPerceptronTests
{
    // XOR is the canonical non-linearly-separable problem: a network that solves it has a
    // working hidden layer and a correct backprop implementation.
    private static readonly double[][] XorX =
    [
        [0, 0], [0, 1], [1, 0], [1, 1]
    ];
    private static readonly double[][] XorY =
    [
        [0], [1], [1], [0]
    ];

    [Fact]
    public void Forward_ReturnsProbabilityInUnitInterval()
    {
        var net = new MultiLayerPerceptron([2, 4, 1], Activation.Tanh, seed: 1);
        var p = net.Predict([0.5, -0.3]);
        Assert.InRange(p, 0.0, 1.0);
    }

    [Fact]
    public void TrainEpoch_DecreasesLoss()
    {
        var net = new MultiLayerPerceptron([2, 6, 1], Activation.Tanh, seed: 3);
        var before = net.Loss(XorX, XorY);

        for (var i = 0; i < 50; i++)
        {
            net.TrainEpoch(XorX, XorY, learningRate: 0.5, batchSize: 4);
        }

        var after = net.Loss(XorX, XorY);
        Assert.True(after < before, $"loss should drop: before={before}, after={after}");
    }

    [Fact]
    public void Backprop_LearnsXor()
    {
        var net = new MultiLayerPerceptron([2, 6, 1], Activation.Tanh, seed: 3);
        for (var i = 0; i < 4000; i++)
        {
            net.TrainEpoch(XorX, XorY, learningRate: 0.5, batchSize: 4);
        }

        Assert.Equal(1.0, net.Accuracy(XorX, XorY), 3);
        Assert.True(net.Predict([0, 0]) < 0.5);
        Assert.True(net.Predict([1, 0]) > 0.5);
        Assert.True(net.Predict([0, 1]) > 0.5);
        Assert.True(net.Predict([1, 1]) < 0.5);
    }

    [Fact]
    public void SameSeed_IsDeterministic()
    {
        var a = new MultiLayerPerceptron([2, 4, 1], Activation.Tanh, seed: 9);
        var b = new MultiLayerPerceptron([2, 4, 1], Activation.Tanh, seed: 9);
        for (var i = 0; i < 20; i++)
        {
            a.TrainEpoch(XorX, XorY, 0.3, 4);
            b.TrainEpoch(XorX, XorY, 0.3, 4);
        }
        Assert.Equal(a.Predict([1, 0]), b.Predict([1, 0]), 10);
    }

    [Fact]
    public void Constructor_RejectsSingleLayer()
    {
        Assert.Throws<ArgumentException>(() => new MultiLayerPerceptron([3]));
    }

    [Fact]
    public void ReLuActivation_AlsoLearnsXor()
    {
        var net = new MultiLayerPerceptron([2, 8, 1], Activation.ReLU, seed: 5);
        for (var i = 0; i < 4000; i++)
        {
            net.TrainEpoch(XorX, XorY, learningRate: 0.1, batchSize: 4);
        }
        Assert.True(net.Accuracy(XorX, XorY) >= 0.75);
    }
}
