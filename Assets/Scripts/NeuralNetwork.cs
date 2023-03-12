using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork{
    public List<List<float>> biases;
    public List<List<List<float>>> weights;
    public float s;
    public List<float> output;
    public List<int> parameters;
    float sigma;
    God god;

    public NeuralNetwork(List<int> parameters,bool isItStrict){
        // inicializa as variaveis
        god=Camera.main.GetComponent<God>();
        this.biases=new List<List<float>>(); // a lista com todos os vetores de biases
        this.weights=new List<List<List<float>>>(); // a lista com todas as matrizes de weights 
        List<float> bias; // um vetor de biases
        List<float> lineOfWeight; // uma linha de uma matrix de weights
        List<List<float>> weight; // uma matrix de weights
        this.s=god.LowerBoundOfSigmoidFunction;
        this.parameters=parameters;
        this.sigma=god.StandarDeviationOfNOrmalFunction;

        // pra cada layer da NN:
        for (int i = 1; i < parameters.Count; i++){
            // clear o current biases/weights
            bias=new List<float>();
            weight=new List<List<float>>();
            // pra cada neurônio desse layer:
            for (int a = 0; a < parameters[i]; a++){
                // menor ideia do q isso era pra ser
                // if (a==parameters[i]-1 && isPredator) {bias.Add(0.5f);}

                // começa os biases em 0 pq deu tudo errado quando eu começei com eles aleatórios
                bias.Add(randomNumber());
                // clear current line of weights
                lineOfWeight=new List<float>();
                // pra cada neurônio do layer anterior: add um numero aleatório na linha da matrix de weights
                for (int b = 0; b < parameters[i-1]; b++) {lineOfWeight.Add(randomNumber());}
                // add essa linha pra matrix
                weight.Add(lineOfWeight);
            }
            // add o vetor de biases e a mtriz de weights pra listas
            this.biases.Add(bias);
            this.weights.Add(weight);
        } 
    }

    // retorna um número aleatório entre -1 e 1
    float randomNumber() {return Mathf.Round(Random.Range(-1.0f,1.0f)*1000.0f)/1000.0f;}
    // sigmoid function (depende da variavel 's')
    float Sigmoid(float number) {return ((1-this.s)/(1+Mathf.Exp(-number)))+this.s;}

    // a função q pega um vetor de inputs e calcula o output da NN
    public void Output(List<float> Input){
        // começa com o current layer=input
        List<float> currentLayer=Input;
        // pra cada layer: currentLayer=(currentWeights * previousLayer) + currentBiases
        for (int i = 0; i < biases.Count; i++) {currentLayer=matrixMath(currentLayer,weights[i],biases[i]);}
        // retorna o currentLayer q vai ser o último q é o output
        this.output=currentLayer;
    }

    // a função q pega 2 matrizes e um vetor e faz M*M+V
    List<float> matrixMath(List<float> previousLayer, List<List<float>> wheighs, List<float> biases){
        // tem q inicializar uma nova variavel aqui pq se n quebra tudo, ou pelo menos quebrou quando eu testei
        List<float> nextLayer= new List<float>(); // o resultado final da conta
        // inicializa uma variável auxiliar q é o valor de um neurônio
        float result;
        // pra cada linha da matrix de weights:
        for (int line = 0; line < wheighs.Count; line++){
            result=0;
            // pra cada coluna da matrix de weights: vai adicionando o resultado da conta à variável
            for (int collum = 0; collum < wheighs[line].Count; collum++){
                result=result+wheighs[line][collum]*previousLayer[collum];
            }
            // add o resultado final(com sigmoid) ao vetor de valores do neuônio 
            nextLayer.Add(Sigmoid(result+biases[line]));
        }
        return nextLayer;
    }

    // pega uma lista e transforma numa string
    public string listToString(List<float> lista){
        string final="";
        for (int i = 0; i < lista.Count; i++) {final=final+lista[i].ToString()+"/";}
        return final;
    }

    // Debug.Loga as matrizes de weights e os vetores de biases da NN
    public void show(){
        for (int i = 0; i < this.biases.Count; i++){
            Debug.Log("Bias "+(i+1).ToString()+":");
            Debug.Log("           "+listToString(this.biases[i]));
        }
        
        for (int i = 0; i < this.weights.Count; i++){
            Debug.Log("Weight "+(i+1).ToString()+":");
            for (int a = 0; a < this.weights[i].Count; a++){
                Debug.Log("           "+listToString(this.weights[i][a]));
            }
        }
    }

    public void Evolve(){
        /*
        for (int i = 1; i < this.parameters.Count-1; i++){
            if (false){
                List<float> newLine=new List<float>();
                for (int a = 0; a < this.weights[i][0].Count; a++){
                    newLine.Add(randomNumber());
                }
                this.weights[i].Add(newLine);
                this.biases[i].Add(0);
            }
            
        }
        */
        for (int i = 0; i < this.biases.Count; i++){
            for (int a = 0; a < this.weights[i].Count; a++){
                this.biases[i][a]=god.randomNormal(this.biases[i][a]);
                for (int b = 0; b < this.weights[i][a].Count; b++){
                    this.weights[i][a][b]=god.randomNormal(this.weights[i][a][b]);
                }
            }
        }
    }

    
}
