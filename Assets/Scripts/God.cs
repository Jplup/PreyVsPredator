using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class God : MonoBehaviour{
    // Prey parameters
    [Header("Prey Parameters")]
    public float PreysStaminaConsuption=0.05f;
    public float PreysReproductionSpeed=0.2f;
    public float PreysMaxAngle=300.0f;
    public int PreysNumberOfRays=25;
    public float PreysVisionRange=6.0f;
    public float PreysMaxForcePerSecond=50;
    public float PreysLinearDrag=15;
    public List<int> PreysInicialHiddenLayersParameters;
    [Header("\n")]

    // Predator parameters
    [Header("Predator Parameters")]
    public float PredatorsStaminaConsuption=0.05f;
    public float PredatorsDigestionSpeed=0.8f;
    public float PredatorsEnergyPerPrey=0.5f;
    public float PredatorsReproductionPerPrey=0.5f;
    public float PredatorsMaxAngle=50.0f;
    public int PredatorsNumberOfRays=10;
    public float PredatorsVisionRange=15.0f;
    public float PredatorsMaxForcePerSecond=80;
    public float PredatorsLinearDrag=20;
    public List<int> PredatorsInicialHiddenLayersParameters;
    [Header("\n")]

    [Header("Global changes to functions")]
    public bool UseSmoothVelocityToReproduction;
    public bool UseSmoothRotation;
    public float LowerBoundOfSigmoidFunction;
    public bool UseLinearVelocityFromNeuron;
    public float StandarDeviationOfNOrmalFunction;
    public bool UseGeneticNN;
    [Header("\n")]

    [Header("Neural Network chances")]
    public float ChanceOfNewConnection;
    public float ChanceOfNewNeuron;
    public float ChanceOfMutation;
    [Header("\n")]

    public int numberOfPreys;
    public int numberOfPredators;
    public GameObject predatorBauplan;
    public GameObject preyBauplan;


    [Range(-.8f,1)] public float activationToSpeedLowerBound=1;
    public GameObject focused;
    public GameObject staminaBar;
    public GameObject reproductionBar;
    public GameObject digestionBar;
    public GameObject speedBar;
    public Text preycountText;
    public Text predatorCountText;
    

    Brain focusedsBrain;
    string oldName;

    public bool spawn;

    public int PredatorCount;
    public int PreyCount;

    public bool restart;

    public float zoomSpeed;
    public float panSpeed;

    Vector2 cameraMovment;

    public float mapSize;

    float timePassed;

    public List<int> testParameters;
    
    public List<float> ipnutForTest;
    public List<float> outputOfTest;
    public bool possibleToRestart;

    GeneticNeuralNetwork nnn;
    public bool calcOut;
    public bool evolveIt;
    public bool showNN;

    public InfinityBorders borders;
    
    // Start is called before the first frame update
    void Start(){
        nnn=new GeneticNeuralNetwork(testParameters);
        PredatorCount=GameObject.FindGameObjectsWithTag("Predator").Length;
        PreyCount=GameObject.FindGameObjectsWithTag("Prey").Length;
        if (spawn){
            if (focused!=null){
                focusedsBrain=focused.GetComponent<Brain>();
                oldName=focused.name;
            }
            for (int i = 0; i < numberOfPreys; i++){
                Instantiate(preyBauplan,new Vector3(Random.Range(-((mapSize/2)-10),((mapSize/2)-10)),Random.Range(-((mapSize/2)-10),((mapSize/2)-10)),0),Quaternion.identity);
                PreyCount++;
            }
            for (int i = 0; i < numberOfPredators; i++){
                Instantiate(predatorBauplan,new Vector3(Random.Range(-((mapSize/2)-10),((mapSize/2)-10)),Random.Range(-((mapSize/2)-10),((mapSize/2)-10)),0),Quaternion.identity);
                PredatorCount++;
            }
        }
        timePassed=0;
    }

    // Update is called once per frame
    void Update(){
        if (evolveIt) {nnn.Evolve();evolveIt=false;}
        if (calcOut) {outputOfTest=nnn.Output();calcOut=false;}
        if (showNN) {nnn.show();showNN=false;}

        timePassed+=Time.deltaTime;
        if (timePassed>30) {
            timePassed=0;
            PredatorCount=GameObject.FindGameObjectsWithTag("Predator").Length;
            PreyCount=GameObject.FindGameObjectsWithTag("Prey").Length;
            print("checou");
        }

        cameraMovment=new Vector2(0,0);
        Camera.main.orthographicSize-=Input.mouseScrollDelta.y*zoomSpeed;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {cameraMovment.y++;}
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {cameraMovment.x--;}
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {cameraMovment.y--;}
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {cameraMovment.x++;}
        
        transform.position=new Vector3(transform.position.x+(cameraMovment.x*panSpeed),transform.position.y+(cameraMovment.y*panSpeed),transform.position.z);

        if (focused!=null){
            if (oldName!=focused.name) {focusedsBrain=focused.GetComponent<Brain>();}
            staminaBar.transform.localScale=new Vector3(focusedsBrain.Stamina,1,1);
            reproductionBar.transform.localScale=new Vector3(focusedsBrain.Reproduction,1,1);
            digestionBar.transform.localScale=new Vector3(focusedsBrain.Digestion,1,1);
            speedBar.transform.localScale=new Vector3(focusedsBrain.body.velocity.magnitude/14,1,1);
        }
        predatorCountText.text=PredatorCount.ToString();
        preycountText.text=PreyCount.ToString();
        if ((PredatorCount<1 || PreyCount<1) && possibleToRestart) {Start();restart=false;}
        if (restart) {PreyCount=0;PredatorCount=0;print("resetu");}
    }

    // gera um número aleatório entre -1.5 e 1.5 com distribuição normal e soma esse numero a um original (entre -1 e 1)
    public float randomNormal(float originalNumber){
        // não sei como funfa, peguei da internet
        float v1,v2,v3;
        float numberOfIterationInThisThing=0; // quantas vezes o while rodou
        do {
            v1=2*Random.Range(0f,1f)-1;
            v2=2*Random.Range(0f,1f)-1;
            v3=v1*v1+v2*v2;
            numberOfIterationInThisThing+=1;
            // teoricamente esse negócio nuca passa de 2 ou 3 iterações, mas nuca é bom ter um while q poderia rodar pra sempre
            if (numberOfIterationInThisThing>100) {Debug.Log("brekou");return 0;}
        } while (v3>=1.0f||v3==0f);
        v3=Mathf.Sqrt((-2.0f*Mathf.Log(v3))/v3);
        return Mathf.Round(1000*(Mathf.Min(Mathf.Max(((v1*v3*StandarDeviationOfNOrmalFunction)+originalNumber),-1),1)))/1000;
    }
}
