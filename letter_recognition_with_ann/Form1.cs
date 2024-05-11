
namespace letter_recognition_with_ann
{
    public partial class Form1 : Form
    {
        private int inputSize = 7 * 5;
        private int outputSize = 5;
        private int hiddenSize = 10;
        private double rateOfLearning = 0.1;
        private int[][,] trainingData;
        private int epochs = 1000000;
        static readonly double defaultEpsilonThreshold = 0.1;
        private List<Button> buttons = new List<Button>();
        private ANeuralNetwork? aNeural;

        public Form1()
        {
            InitializeComponent();
            InitiliazeButtons();


            trainingData = new int[5][,] {
                // A
                new int[,] {
                    {0,0,1,0,0},
                    {0,1,0,1,0},
                    {1,0,0,0,1},
                    {1,0,0,0,1},
                    {1,1,1,1,1},
                    {1,0,0,0,1},
                    {1,0,0,0,1} },

                // B
                new int[,] {
                    {1,1,1,1,0},
                    {1,0,0,0,1},
                    {1,0,0,0,1},
                    {1,1,1,1,0},
                    {1,0,0,0,1},
                    {1,0,0,0,1},
                    {1,1,1,1,0} },


                // C
                new int[,] {
                    {0,0,1,1,1},
                    {0,1,0,0,0},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {0,1,0,0,0},
                    {0,0,1,1,1} },

                // D
                new int[,] {
                    {1,1,1,0,0},
                    {1,0,0,1,0},
                    {1,0,0,0,1},
                    {1,0,0,0,1},
                    {1,0,0,0,1},
                    {1,0,0,1,0},
                    {1,1,1,0,0} },

                // E
                new int[,] {
                    {1,1,1,1,1},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {1,1,1,1,1},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {1,1,1,1,1} }
            };


        }

        private void NetworkTrainer()
        {
            ANeuralNetwork ann = new ANeuralNetwork(inputSize, hiddenSize, outputSize);
            int k = 0;
            while (k < epochs)
            {

                double totalError = 0;
                for (int i = 0; i < outputSize; i++)
                {
                    int[] sequencedTrainingMatrix = SequenceMatrix(trainingData[i]);
                    double[] trainingDataList = sequencedTrainingMatrix.Select((e) => (double)e).ToArray();
                    double[] desiredOutputs = new double[outputSize];
                    desiredOutputs[i] = 1;

                    double[] outputValues = ann.Predict(trainingDataList);

                    double error = 0;
                    for (int j = 0; j < outputValues.Length; j++)
                    {
                        error += desiredOutputs[j] - outputValues[j];
                    }
                    totalError += error;
                    ann.TrainNetwork(trainingDataList, desiredOutputs, rateOfLearning);

                }

                if (totalError / 5 < GetEpsilonThreshold() && totalError / 5 > GetEpsilonThreshold() * -1)
                {
                    break;
                }
                k++;
            }
            MessageBox.Show($"Itearation value: {k.ToString()}");
            aNeural = ann;

        }

        private int[] SequenceMatrix(int[,] data)
        {
            int[] sequencedList = new int[data.Length];

            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sequencedList[count++] = data[i, j];
                }
            }

            return sequencedList;
        }

        private void InitiliazeButtons()
        {
            foreach (var control in this.panel1.Controls)
            {
                if (control is Button)
                {
                    Button button = (Button)control;
                    for (int i = 1; i < 36; i++)
                    {
                        if (button.Name == $"button{i}")
                        {
                            buttons.Add(button);
                        }
                    }

                }
            }
            buttons.Reverse();
        }


        private double[] GetInput()
        {
            double[] inputs = new double[inputSize];
            for (int i = 0; i < inputSize; i++)
            {
                if (buttons[i].BackColor == Color.Black)
                {
                    inputs[i] = 1;
                }
                else
                {
                    inputs[i] = 0;
                }
            }
            return inputs;
        }

        private double GetEpsilonThreshold()
        {
            double threshold = 0;
            if (textBox6.Text == "")
            {
                threshold = defaultEpsilonThreshold;
            }
            else
            {

                threshold = Convert.ToDouble(textBox6.Text);

            }

            return threshold;
        }

        private void PrintOutputValues(double[] outputValues)
        {
            for (int i = 0; i < outputSize; i++)
            {
                switch (i)
                {
                    case 0:
                        textBox1.Text = outputValues[i].ToString();
                        break;
                    case 1:
                        textBox2.Text = outputValues[i].ToString();
                        break;
                    case 2:
                        textBox3.Text = outputValues[i].ToString();
                        break;
                    case 3:
                        textBox4.Text = outputValues[i].ToString();
                        break;
                    case 4:
                        textBox5.Text = outputValues[i].ToString();
                        break;
                }
            }
        }


        private void train_Click(object sender, EventArgs e)
        {
            try
            {
                NetworkTrainer();
                Form2 completed = new Form2(aNeural!);
                completed.ShowDialog();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }


        }

        private void panel_button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (clickedButton.BackColor == Color.White)
            {
                clickedButton.BackColor = Color.Black;
            }
            else
            {
                clickedButton.BackColor = Color.White;
            }

        }

        private void predict_Click(object sender, EventArgs e)
        {
            if (aNeural != null)
            {
                double[] outputValues = aNeural.Predict(GetInput());
                PrintOutputValues(outputValues);
            }
            else
            {
                MessageBox.Show("ANN needs to be trained!!!");
            }

        }

        private void reset_Click(object sender, EventArgs e)
        {
            foreach (var b in buttons)
            {
                if (b.BackColor == Color.Black)
                {
                    b.BackColor = Color.White;
                }
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
           
           if(aNeural == null)
            {
                aNeural = new ANeuralNetwork(inputSize, hiddenSize, outputSize);
            }
            try
            {
                aNeural.ReadWeightsAndBiasesFromFile("values.txt");
                MessageBox.Show("Values are loaded!!!");
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + "Weights are started with initial values");
            }
           
        }
    }
}
