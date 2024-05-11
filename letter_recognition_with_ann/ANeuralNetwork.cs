using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace letter_recognition_with_ann
{
    public class ANeuralNetwork
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;

        private double[,] inputHiddenWeights;
        private double[,] hiddenOutputWeights;

        private double[] hiddenBiases;
        private double[] outputBiases;

        public ANeuralNetwork(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            inputHiddenWeights = CalculateWeightValues(inputSize,hiddenSize);
            hiddenOutputWeights = CalculateWeightValues(inputSize,hiddenSize);
            hiddenBiases = CalculateBiases(hiddenSize);
            outputBiases = CalculateBiases(outputSize);
        }

        private double[,] CalculateWeightValues(int rows, int columns)
        {
            double[,] weight = new double[rows, columns];
            Random rnd = new Random();

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    weight[i, j] = rnd.NextDouble() - 0.5;
                }
            }

            return weight;
        }

        private double[] CalculateBiases(int size)
        {
            double[] biases = new double[size];
            Random random = new Random();
            
            for (int i = 0; i < size; i++)
            {
                biases[i] = random.NextDouble() - 0.5;
            }
            return biases;
        }


        public double[] Predict(double[] input)
        {
            double[] hiddenLayerOutputs = new double[hiddenSize];


            for(int i = 0; i < hiddenLayerOutputs.Length; i++)
            {
                double sum = 0;
                for(int j = 0; j < input.Length; j++)
                {
                    sum += input[j] * inputHiddenWeights[j, i];
                }
                sum += hiddenBiases[i];
                hiddenLayerOutputs[i] = Sigmoid(sum);

            }

            double[] outputLayerOutputs = new double[outputSize];

            for(int i = 0; i < outputLayerOutputs.Length; i++)
            {
                double sum = 0;
                for(int j = 0; j < hiddenLayerOutputs.Length; j++)
                {
                    sum += hiddenLayerOutputs[j] * hiddenOutputWeights[j, i];
                }
                sum += outputBiases[i];
                outputLayerOutputs[i] = Sigmoid(sum);
            }

            return outputLayerOutputs;

        }

        public void TrainNetwork(double[] inputs, double[] desiredOutputs, double rateOfLearning)
        {
            double[] hiddenLayerOutputs = new double[hiddenSize];


            for (int i = 0; i < hiddenLayerOutputs.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < inputs.Length; j++)
                {
                    sum += inputs[j] * inputHiddenWeights[j, i];
                }
                sum += hiddenBiases[i];
                hiddenLayerOutputs[i] = Sigmoid(sum);

            }

            double[] outputLayerOutputs = new double[outputSize];

            for (int i = 0; i < outputLayerOutputs.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < hiddenLayerOutputs.Length; j++)
                {
                    sum += hiddenLayerOutputs[j] * hiddenOutputWeights[j, i];
                }
                sum += outputBiases[i];
                outputLayerOutputs[i] = Sigmoid(sum);
            }


            double[] outputLayerErrors = new double[outputSize];

            for(int i = 0; i < outputLayerErrors.Length; i++)
            {
                double epsilon = desiredOutputs[i] - outputLayerOutputs[i];
                outputLayerErrors[i] = outputLayerOutputs[i] * (1 - outputLayerOutputs[i]) * epsilon;   
            }


            double[] hiddenLayerErrors = new double[hiddenSize];

            for(int i = 0; i < hiddenSize; i++)
            {
                double sum = 0;
                for(int j = 0; j < outputSize; j++)
                {
                    sum += hiddenOutputWeights[i, j] * outputLayerErrors[j]; 
                }

                hiddenLayerErrors[i] = hiddenLayerOutputs[i] * (1 - hiddenLayerOutputs[i]) * sum;

            }


            for(int i = 0; i < inputSize; i++)
            {
                for(int j =0; j < hiddenSize; j++)
                {
                    inputHiddenWeights[i, j] += rateOfLearning * inputs[i] * hiddenLayerErrors[j];
                }
                
            }


            for(int i = 0; i < hiddenSize; i++)
            {
                for(int j = 0; j < outputSize; j++)
                {
                    hiddenOutputWeights[i, j] += rateOfLearning * hiddenLayerOutputs[i] * outputLayerErrors[j];
                }
            }


            for(int i = 0; i < hiddenBiases.Length; i++)
            {
                hiddenBiases[i] += rateOfLearning * hiddenLayerErrors[i]; 
            }


            for (int i = 0; i < outputBiases.Length; i++)
            {
                outputBiases[i] = rateOfLearning * outputLayerErrors[i];
            }

        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }


        public void WriteWeightsAndBiasesToFile(string filePath)
        {
            
            string json = JsonConvert.SerializeObject(new
            {
                InputHiddenWeights = inputHiddenWeights,
                HiddenOutputWeights = hiddenOutputWeights,
                HiddenBiases = hiddenBiases,
                OutputBiases = outputBiases
            });

            
            File.WriteAllText(filePath, json);
        }

        public void ReadWeightsAndBiasesFromFile(string filePath)
        {
            
            string json = File.ReadAllText(filePath);

            
            dynamic data = JsonConvert.DeserializeObject(json);
            inputHiddenWeights = JsonConvert.DeserializeObject<double[,]>(data.InputHiddenWeights.ToString());
            hiddenOutputWeights = JsonConvert.DeserializeObject<double[,]>(data.HiddenOutputWeights.ToString());
            hiddenBiases = JsonConvert.DeserializeObject<double[]>(data.HiddenBiases.ToString());
            outputBiases = JsonConvert.DeserializeObject<double[]>(data.OutputBiases.ToString());
        }

    }
}
