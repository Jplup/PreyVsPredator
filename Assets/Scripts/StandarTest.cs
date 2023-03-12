using System.Collections.Generic;
using UnityEngine;

// Bota esse script na camera principal pra testar uma função de RandomNormalDistribution
public class StandarTest : MonoBehaviour{
    // variaveis + prefab
    public GameObject Tester; // as bolas do grafico
    public int numberOfTesters; // quantas bolas vão ter no grafico
    public int numberOfTests; // quantos números aleatórios vão ser gerados para o teste
    public float standarDeviation; // tu sabe
    public float mean; // tu sabe tbm
    public float heightMultiplier; // multiplica a altura de todas as bolas no gráfico pra ficar mais fácil de ver
    public float rangeOfTesters; // o tamanho em x do gráfico

    // listas
    List<GameObject> testers; // uma lista com todas as bolas do grafico
    List<float> heights; // a lista de alturas das bolas do grafico

    // versãao antiga das variaveis pra rodar o script den ovo caso vc mude alguma variavel
    int oldt;
    int oldnot; 
    float oldsd;
    float oldm;
    float oldmul;

    void Start(){
        // inicializa a lista
        testers=new List<GameObject>();
    }

    void Update(){
        // checa se alguma variavel mudou, se s: updata as oldVariables e roda o teste
        if (numberOfTesters!=oldnot || standarDeviation!=oldsd || mean!=oldm || numberOfTests!=oldt || oldmul!=heightMultiplier){
            oldm=mean;
            oldsd=standarDeviation;
            oldt=numberOfTests;
            oldmul=heightMultiplier;
            StandarDeviationTest();
        }
    }

    // a função q faz o gráfico funfar
    public void StandarDeviationTest(){
        // muda a quantidade de bolas no grafico se precisar
        while (testers.Count<numberOfTesters) {testers.Add(Instantiate(Tester,transform.position,Quaternion.identity));} // add bolas
        while (testers.Count>numberOfTesters) {Destroy(testers[0]);} // remove bolas

        // updata oldNumberOfTesters
        oldnot=numberOfTesters;
        
        // clear and fill(com 0) a lista de alturas das bolas
        heights=new List<float>();
        for (int i = 0; i < numberOfTesters; i++) {heights.Add(0);}

        // inicializa e enche a lista de posições(só o valor em x) das bolinhas do gráfico
        List<float> xpos=new List<float>();
        for (int i = 0; i < numberOfTesters; i++) {xpos.Add(((rangeOfTesters/(numberOfTesters-1))*i)-(rangeOfTesters/2)+mean);}

        // calcula vários numeros usando a função teste e salva a quantidade de ocorrências de um certo número(or close enuf to it) na lista de alturas 
        int indexOfHeight;
        for (int i = 0; i < numberOfTests; i++){
            indexOfHeight=indexOfClosestValue(xpos,randomNormal());
            heights[indexOfHeight]=heights[indexOfHeight]+1;
        }

        // divide a quantidade de ocorrências pelo número de números gerados pra virar uma porcentagem e depois multiplica pelo heightMultiplier
        float max=heights[0];
        for (int i = 0; i < numberOfTesters; i++){
            heights[i]=(heights[i]/numberOfTests)*heightMultiplier;
        }

        // muda a posição das bolinhas
        for (int i = 0; i < numberOfTesters; i++){
            testers[i].gameObject.transform.position=new Vector3(xpos[i],heights[i],0);
        }
    }

    // a função a ser testada
    public float randomNormal(){
        // não sei como funfa, peguei da internet
        float v1,v2,s;
        float numberOfIterationInThisThing=0; // quantas vezes o while rodou
        do {
            v1=2*Random.Range(0f,1f)-1;
            v2=2*Random.Range(0f,1f)-1;
            s=v1*v1+v2*v2;
            numberOfIterationInThisThing+=1;
            // teoricamente esse negócio nuca passa de 2 ou 3 iterações, mas nuca é bom ter um while q poderia rodar pra sempre
            if (numberOfIterationInThisThing>100) {Debug.Log("brekou");return 0;}
        } while (s>=1.0f||s==0f);
        s=Mathf.Sqrt((-2.0f*Mathf.Log(s))/s);
        return (v1*s*standarDeviation)+mean;
    }

    // a função q vê qual o indice do float(de uma lista) q tá mais perto do float dado
    public int indexOfClosestValue(List<float> lista, float value){
        float[] listOfDistances=new float[lista.Count]; // lista das distâncias entre e numero a ser testado e cada numero da lista (eu testei mudar a lista original mas n funfa)
        // enche a lista de distâncias
        for (int i = 0; i < lista.Count; i++) {listOfDistances[i]=(Mathf.Abs(lista[i]-value));}
        // inicializa as variáveis a serem usadas
        float min=listOfDistances[0]; // menor distância
        int index=0; // index of menor distancia
        // ve qual é a menor distancia na lista
        for (int i = 0; i < lista.Count; i++){
            if (min>listOfDistances[i]) {min=listOfDistances[i];index=i;}
        }
        return index;
    }
}
