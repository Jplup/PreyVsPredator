using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticNeuralNetwork{
    public List<Vector2> neurons;
    public List<List<Vector3>> connections;
    public List<float> activations;
    public List<float> biases;
    float s;
    float chanceOfConnection;
    float chanceOfNewNeuron;
    float chanceOfMutation;
    public List<float> input;
    public List<int> Parameters;
    God god;

    public GeneticNeuralNetwork(List<int> parameters){
        god=Camera.main.gameObject.GetComponent<God>();
        s=god.LowerBoundOfSigmoidFunction;
        chanceOfConnection=god.ChanceOfNewConnection;
        chanceOfNewNeuron=god.ChanceOfNewNeuron;
        chanceOfMutation=god.ChanceOfMutation;
        connections=new List<List<Vector3>>();
        neurons=new List<Vector2>();
        biases=new List<float>();
        activations=new List<float>();
        Parameters=parameters;
        for (int i = 0; i < parameters.Count; i++){
            for (int a = 0; a < parameters[i]; a++){
                neurons.Add(new Vector2(i,a));
                biases.Add(randomNumber());
                activations.Add(2);
            }
        }
        List<Vector2> possibleConnections=new List<Vector2>();
        List<Vector3> newConnections=new List<Vector3>();
        int indexOfNeuron;
        bool forceConnection=false;
        foreach (Vector2 neuron in neurons){
            if (neuron.x==0) {connections.Add(new List<Vector3>());}
            else{
                possibleConnections=new List<Vector2>();
                newConnections=new List<Vector3>();
                foreach (Vector2 otherNeuron in neurons){
                    if (otherNeuron.x<neuron.x) {possibleConnections.Add(otherNeuron);}
                    else {break;}
                }
                if (neuron.x==parameters.Count-1){
                    for (int i = 0; i < parameters[0]; i++) {possibleConnections.RemoveAt(0);}
                    foreach (Vector2 otherNeuron in neurons){
                        if (otherNeuron.x==0) {newConnections.Add(new Vector3(otherNeuron.x,otherNeuron.y,randomNumber()));}
                        else {break;}
                    }
                }else {forceConnection=true;}
                while ((chanceOfConnection>randomPercent() && possibleConnections.Count>0) || forceConnection){
                    forceConnection=false;
                    indexOfNeuron=Random.Range(0,possibleConnections.Count-1);
                    newConnections.Add(new Vector3(possibleConnections[indexOfNeuron].x,possibleConnections[indexOfNeuron].y,randomNumber()));
                    possibleConnections.RemoveAt(indexOfNeuron);
                }
                connections.Add(newConnections);
            }
        }
        input=god.ipnutForTest;
    }

    public void show(){
        Debug.Log("neurons: "+listToString2(neurons));
        Debug.Log("connections: ");
        for (int i = 0; i < connections.Count; i++){
            Debug.Log("         "+listToString(connections[i]));
        }
    }

    public void Evolve(){
        int layerOfNewNeuron;
        int indexOfNewNeuronInListOfPossibilitys;
        List<Vector2> possibleConnections;
        List<Vector3> newConnections;
        int numberOfNewNeurons=1;
        int numberOfNewConnections=1;
        while (randomPercent()<chanceFromNumberOfHappening(chanceOfNewNeuron,numberOfNewNeurons)){
            layerOfNewNeuron=Random.Range(1,Parameters.Count-1);
            neurons.Add(new Vector2(layerOfNewNeuron,getNumberOfNeuronsInLayer(layerOfNewNeuron)));
            activations.Add(2);
            biases.Add(randomNumber());
            possibleConnections=new List<Vector2>();
            foreach (Vector2 neuron in neurons){
                if (neuron.x<layerOfNewNeuron) {possibleConnections.Add(neuron);}
            }
            newConnections=new List<Vector3>();
            do {
                indexOfNewNeuronInListOfPossibilitys=Random.Range(0,possibleConnections.Count-1);
                newConnections.Add(new Vector3(
                    possibleConnections[indexOfNewNeuronInListOfPossibilitys].x,
                    possibleConnections[indexOfNewNeuronInListOfPossibilitys].y,
                    randomNumber()
                ));
                possibleConnections.RemoveAt(indexOfNewNeuronInListOfPossibilitys);
                numberOfNewConnections++;
            }while (randomPercent()<chanceFromNumberOfHappening(chanceOfConnection,numberOfNewConnections) && possibleConnections.Count>0);
            connections.Add(newConnections);
            numberOfNewNeurons++;
        }
        foreach (Vector2 neuron in neurons){
            if (neuron.x!=0) {
                numberOfNewConnections=1;
                possibleConnections=GetPossibleConnections(neuron);
                newConnections=connections[neurons.IndexOf(neuron)];
                int indexOfNeuron;
                while ((chanceFromNumberOfHappening(chanceOfConnection,numberOfNewConnections)>randomPercent() && possibleConnections.Count>0)){
                    indexOfNeuron=Random.Range(0,possibleConnections.Count-1);
                    newConnections.Add(new Vector3(possibleConnections[indexOfNeuron].x,possibleConnections[indexOfNeuron].y,randomNumber()));
                    possibleConnections.RemoveAt(indexOfNeuron);
                    numberOfNewConnections++;
                }
                connections[neurons.IndexOf(neuron)]=newConnections;
            }
        }
        for (int i = 0; i < connections.Count; i++){
            List<Vector3> newListOfConnections=connections[i];
            for (int a = 0; a < connections[i].Count; a++){
                if (randomPercent()<chanceOfMutation){
                    newListOfConnections[a]=new Vector3(
                        newListOfConnections[a].x,
                        newListOfConnections[a].y,
                        god.randomNormal(newListOfConnections[a].z)
                    );
                }
            }
            connections[i]=newListOfConnections;
        }
        for (int i = 0; i < biases.Count; i++){
            if (randomPercent()<chanceOfMutation){
                biases[i]=god.randomNormal(biases[i]);
            }
        }
    }

    public List<Vector2> GetPossibleConnections(Vector2 neuron){
        List<Vector2> possibleConnections=new List<Vector2>();
        List <Vector3> oldConnections=connections[neurons.IndexOf(neuron)];
        foreach (Vector2 otherNeuron in neurons){
            if (otherNeuron.x<neuron.x) {possibleConnections.Add(otherNeuron);}
        }
        foreach (Vector3 connection3 in oldConnections){
            Vector2 connection2=new Vector2(connection3.x,connection3.y);
            if (possibleConnections.Contains(connection2)) {possibleConnections.Remove(connection2);}
        }
        return possibleConnections;
    }

    int getNumberOfNeuronsInLayer(int layerIndex){
        List<Vector2> gg=new List<Vector2>();
        foreach (Vector2 neuron in neurons){
            if (neuron.x==layerIndex) {gg.Add(neuron);}
        }
        if (gg.Count==0) {return 0;}
        int theIndex=0;
        foreach (Vector2 neuron in gg){
            if (neuron.y>theIndex) {theIndex=(int)neuron.y;}
        }
        return theIndex+1;
    }

    float neuronOutput(Vector2 neuron){
        float result=0;
        int neuronIndex=neurons.IndexOf(neuron);
        int indexOfConnectedNeuron;
        if (neuron.x==0) {activations[neuronIndex]=input[(int)neuron.y];return activations[neuronIndex];}
        foreach (Vector3 connectedNeuron in connections[neuronIndex]){
            indexOfConnectedNeuron=neurons.IndexOf(new Vector2(connectedNeuron.x,connectedNeuron.y));
            if (activations[indexOfConnectedNeuron]==2) {result+=neuronOutput(connectedNeuron)*connectedNeuron.z;}
            else {result+=activations[indexOfConnectedNeuron]*connectedNeuron.z;}
        }
        activations[neuronIndex]=Sigmoid(result+biases[neuronIndex]);
        return activations[neuronIndex];
    }

    public List<float> Output(){
        List<float> output=new List<float>();
        foreach (Vector2 neuron in neurons){
            if (neuron.x==Parameters.Count-1){
                output.Add(neuronOutput(neuron));
            }
        }
        return output;
    }

    public GeneticNeuralNetwork Clone(){
        GeneticNeuralNetwork clone=new GeneticNeuralNetwork(Parameters);
        List<Vector2> neuronsCopy=new List<Vector2>();
        foreach (Vector2 neuron in neurons) {neuronsCopy.Add(new Vector2(neuron.x,neuron.y));}
        clone.neurons=neuronsCopy;
        List<float> biasesCopy=new List<float>();
        foreach (float bias in biases) {biasesCopy.Add(bias);}
        clone.biases=biasesCopy;
        List<float> activationsCopy=new List<float>();
        foreach (float activation in activations) {activationsCopy.Add(activation);}
        clone.activations=activationsCopy;
        List<List<Vector3>> connectionsCopy=new List<List<Vector3>>();
        List<Vector3> cs;
        for (int i = 0; i < connections.Count; i++){
            cs=new List<Vector3>();
            for (int a = 0; a < connections[i].Count; a++){
                cs.Add(new Vector3(connections[i][a].x,connections[i][a].y,connections[i][a].z));
            }
            connectionsCopy.Add(cs);
        }
        clone.connections=connectionsCopy;
        return clone;
    }

    float chanceFromNumberOfHappening(float chanceAt1, int happenings) {return chanceAt1/(Mathf.Pow(happenings,2));}
    float randomNumber() {return Mathf.Round(Random.Range(-1000,1000)*10000.0f)/10000000.0f;}
    float randomPercent() {return Random.Range(0,100000)/1000;}
    // sigmoid function (depende da variavel 's')
    float Sigmoid(float number) {return ((1-this.s)/(1+Mathf.Exp(-number)))+this.s;}
    public string listToString(List<Vector3> lista){
        string final="";
        for (int i = 0; i < lista.Count; i++){
            final=final+("("+lista[i].x+" / ")+(lista[i].y+" / ")+(lista[i].z+")")+" , ";
        }
        return final;
    }
    public string listToString2(List<Vector2> lista){
        string final="";
        for (int i = 0; i < lista.Count; i++){
            final=final+("("+lista[i].x+" / ")+(lista[i].y+")")+" , ";
        }
        return final;
    }
}
