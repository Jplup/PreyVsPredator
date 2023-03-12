using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain : MonoBehaviour{
    [SerializeField] float StaminaConsuption=0;
    [SerializeField] float DigestionSpeed=0;
    [SerializeField] float ReproductionSpeed=0;
    public float Stamina=1;
    public float Digestion=0;
    public float Reproduction=0;
    [SerializeField] float EnergyPerPrey=0;
    [SerializeField] float ReproductionPerPrey=0;
    [SerializeField] float MaxAngle=0;
    [SerializeField] int NumberOfRays=0;
    [SerializeField] float VisionRange=0;
    public float MaxForcePerSecond=0;

    [SerializeField] float rayAngle=0;
    [SerializeField] float angle=0;
    [SerializeField] float activation=0;
    [SerializeField] List<float> NeuralOutput;
    [SerializeField] List<float> NeuralInput;
    public float maxSpeed;

    // as variaveis da rede
    

    public float[] rayHitDistance;
    public float[] rayHitType;


    public bool Controled=false;
    public float rotationSpeed;

    public Rigidbody2D body;

    [SerializeField] Vector2 AccelerationVector;

    public bool predator;

    public bool exhausted=false;

    public bool showNeuralNetwork; // o bool q se vc transformar em true: vira false e mostra a rede neural de um bixo

    God god;

    public float percentageOfRegenBasedOnSpeed;

    public float s=0;

    List<int> parameters;

    public bool isChild;

    public Vector2 velocity;


    NeuralNetwork NN;
    float primal;
    bool testing;

    GeneticNeuralNetwork GNN;


    void Start(){
        // inicialize/fill v3
        body=GetComponent<Rigidbody2D>(); // uma referencia ao rigidBody
        parameters=new List<int>(); // os parametros da rede neural
        god=Camera.main.gameObject.GetComponent<God>();
        s=god.LowerBoundOfSigmoidFunction;

        // vê qual o número de inputs e hidden layers q a rede neural deve ter baseado no tipo de criatura, e tbm pega os valores das variaveis from god
        if (gameObject.tag=="Predator"){
            DigestionSpeed=god.PredatorsDigestionSpeed;
            EnergyPerPrey=god.PredatorsEnergyPerPrey;
            ReproductionPerPrey=god.PredatorsReproductionPerPrey;
            StaminaConsuption=god.PredatorsStaminaConsuption;
            MaxAngle=god.PredatorsMaxAngle;
            NumberOfRays=god.PredatorsNumberOfRays;
            VisionRange=god.PredatorsVisionRange;
            MaxForcePerSecond=god.PredatorsMaxForcePerSecond;
            gameObject.GetComponent<Rigidbody2D>().drag=god.PredatorsLinearDrag;
            parameters.Add(NumberOfRays*2+1); // rayHitDistances + rayHitType + digestion + self.speed
            foreach (int parameter in god.PredatorsInicialHiddenLayersParameters) {parameters.Add(parameter);}
            Digestion=0;
        }
        else{
            StaminaConsuption=god.PreysStaminaConsuption;
            ReproductionSpeed=god.PreysReproductionSpeed;
            MaxAngle=god.PreysMaxAngle;
            NumberOfRays=god.PreysNumberOfRays;
            VisionRange=god.PreysVisionRange;
            MaxForcePerSecond=god.PreysMaxForcePerSecond;
            gameObject.GetComponent<Rigidbody2D>().drag=god.PreysLinearDrag;
            parameters.Add(NumberOfRays*2); // rayHitDistances + rayHitType + self.speed
            foreach (int parameter in god.PreysInicialHiddenLayersParameters) {parameters.Add(parameter);}
        }
        // add a quantidade de outputs na rede neural a lista de parametros
        parameters.Add(2);
        
        // inicializa a rede neural e as listas do raycast
        rayHitDistance=new float[NumberOfRays];
        rayHitType=new float[NumberOfRays];

        if (isChild) {
            if (god.UseGeneticNN) {GNN.Evolve();}
            else {NN.Evolve();}
        }
        else {
            if (god.UseGeneticNN) {GNN=new GeneticNeuralNetwork(parameters);}
            else {NN=new NeuralNetwork(parameters,true);}
        }
    }

    void Update(){
        // faz o bixo olhar pros sorroundings
        See();

        // base stamina Consuption
        Stamina=Stamina-(StaminaConsuption*0.05f*Time.deltaTime);

        // selfDestruct se a simulação acabou
        if (god.PredatorCount==0 || god.restart){
            Destroy(gameObject);
            god.PreyCount--;
        }

        // Ve se o movimento vai ser dado pela rede neural ou pelo usuário
        if (Controled){
            // move a criatura de acordo com os inputs do usuário
            if (Input.GetKey(KeyCode.A)){
                angle=angle+rotationSpeed;
                transform.eulerAngles=Vector3.forward*angle;
            }
            if (Input.GetKey(KeyCode.W)){
                body.AddForce(transform.up*MaxForcePerSecond);
            }
            if (Input.GetKey(KeyCode.D)){
                angle=angle-rotationSpeed;
                transform.eulerAngles=Vector3.forward*angle;
            }
            // garante q o |angulo| n vai explodir pra mais de 180 
            if (angle>360) {angle=angle-360;}
            if (angle<-360) {angle=angle+360;}

            // Isso é pra testar coisas de velocidade, pra funfar tem q descomentar umas variaveis
            
            if (Input.GetKeyUp(KeyCode.W)) {
                testing=true;
                primal=transform.position.y;
            }
            if (body.velocity.magnitude==0 && testing) {testing=false;print("foi "+(transform.position.y-primal)+" pra frente");}
            
        }else{
            // salva o input e output nas listas
            NeuralInput=GetInputForNeuralNetwork();
            if (god.UseGeneticNN){
                GNN.input=NeuralInput;
                NeuralOutput=GNN.Output();
            }else{
                NN.Output(NeuralInput);
                NeuralOutput=NN.output;
            }
            

            // checa se a criatura pode mover -> se ela ta exausta
            if (exhausted==false){
                // ve qual função de velocidade vai ser usada e calcula o quanto da velocidade máxima vai ser usada
                if (god.UseLinearVelocityFromNeuron) {activation=(NeuralOutput[0]-s)/(1-s);}
                else {activation=Mathf.Pow((NeuralOutput[0]-s),2)/(1-(3*s));}

                // salva as informações pro usuário ver no inspetor
                AccelerationVector=new Vector2(Mathf.Sin(angle*Mathf.Deg2Rad),Mathf.Cos(angle*Mathf.Deg2Rad))*(MaxForcePerSecond*activation);

                // add uma força na direção do movimento do bixo
                body.AddForce(transform.up*MaxForcePerSecond*activation);

                // abaixa a stamina de acordo com a stamina consumption e current speed
                Stamina=Stamina-(StaminaConsuption*((NeuralOutput[0]-s)/(1-s))*Time.deltaTime);
                // garante q stamina n fica negativa
                if (Stamina<0) {Stamina=0;}
                
                // ve qual tipo de função de rotação vai ser usada e roda o bixo de acordo com a função
                if (god.UseSmoothRotation){
                    angle=angle+rotationSpeed*((2/(1+(Mathf.Exp(-(27*s+30)*Mathf.Pow((NeuralOutput[1]-(s/2)-0.5f),3)))))-1);
                    transform.eulerAngles=Vector3.forward*angle;
                }else{
                    if (NeuralOutput[1]<(0.2+s)) {
                        angle=angle+rotationSpeed;
                        transform.eulerAngles=Vector3.forward*angle;
                    }
                    if (NeuralOutput[1]>0.8) {
                        angle=angle-rotationSpeed;
                        transform.eulerAngles=Vector3.forward*angle;
                    }
                }
            }
        }

        // checa se é pra mostras a rede neural desse bixo, se s: mostra
        if (showNeuralNetwork) {
            showNeuralNetwork=false;
            if (god.UseGeneticNN) {GNN.show();}
            else {NN.show();}
        }

        // isso é pro inspetor saber a velocidade
        velocity=body.velocity;

        // ve se é pra reproduzir ou n
        if (Reproduction>=1) {Reproduce();}

        // define oq o bixo vai fazer dependendo do tipo
        if (predator){
            // diminui a digestão de acordo com digestion speed
            Digestion=Digestion-DigestionSpeed*Time.deltaTime;
            // garante q digestão n vai ficar negativa
            if (Digestion<0){Digestion=0;}
            // se a energia acabar: morre
            if (Stamina==0){
                Destroy(gameObject);
                god.PredatorCount--;
                }
        }else{
            // ve se a criatura ficou exausta ou se ela se recuperou da exaustão
            if (Stamina==0) {exhausted=true;}
            if (Stamina>0.2 && exhausted) {exhausted=false;}

            
            // ve se é pra usar uma função continua ou n pra a quantidade de regeneração de estamina e reprodução
            if (god.UseSmoothVelocityToReproduction){
                // calcula o valor da função contínua
                percentageOfRegenBasedOnSpeed=Mathf.Max(0,(1/((100*((NeuralOutput[0]-s)/(1-s)))+1))-0.05f); // da um numero entre 0 e 1

                if (exhausted) {percentageOfRegenBasedOnSpeed=1;}

                // regenera stamina e reprodução
                Reproduction=Reproduction+(ReproductionSpeed*Time.deltaTime*percentageOfRegenBasedOnSpeed);
                Stamina=Stamina+StaminaConsuption*Time.deltaTime*percentageOfRegenBasedOnSpeed;
                if (Stamina>1){Stamina=1;}
            }else{
                // ve se é pra regenerar
                if (((NeuralOutput[0]-s)/(1-s))<0.1 || exhausted){
                    // regenera stamina e reprodução
                    Reproduction=Reproduction+ReproductionSpeed*Time.deltaTime;
                    Stamina=Stamina+StaminaConsuption*Time.deltaTime;
                    if (Stamina>1){Stamina=1;}
                } 
            }
        }
        if (transform.position.x>100 || transform.position.x<-100 || transform.position.y>100 || transform.position.y<-100) {
            Vector3 newPos=transform.position;
            if (transform.position.x>100)  {newPos.x=newPos.x-200;}
            if (transform.position.x<-100) {newPos.x=newPos.x+200;}
            if (transform.position.y>100)  {newPos.y=newPos.y-200;}
            if (transform.position.y<-100) {newPos.y=newPos.y+200;}
            transform.position=newPos;
        }
    }

    // essa função é chamada quando o triggerCollider bate em outro collider 
    void OnTriggerEnter2D(Collider2D other){
        // ve se a boca(triggerCollider) colidiu com uma presa ou n
        if (other.tag=="Prey") {
            // se o predador n estiver digerindo: come presa / começa a digerir
            if (Digestion==0){
                Destroy(other.gameObject);
                god.PreyCount--;
                Digestion=1;
                Reproduction=Reproduction+ReproductionPerPrey;
                Stamina=Stamina+EnergyPerPrey;
                if (Stamina>1) {Stamina=1;}
            }
        }
    }

    // a função q faz o bixo multiplicar
    public void Reproduce(){
        if (god.PreyCount>500 && gameObject.tag=="Prey") {return;}
        // cria um novo bixo e pega uma referência ao cérebro dela
        Brain child=Instantiate(gameObject,transform.position+new Vector3(0.1f,0,0),Quaternion.identity).GetComponent<Brain>();
        // seta reprodução em 0 se n ela reproduz instantâniamente (dont know y)
        child.Reproduction=0;
        child.isChild=true;
        if (gameObject.tag=="Predator") {god.PredatorCount++;}
        else {god.PreyCount++;}
        // resta reprodução
        Reproduction=0;

        if (god.UseGeneticNN) {
            child.GNN=GNN.Clone();
        }
        else {
            // inherit parents brain
            child.NN=new NeuralNetwork(NN.parameters,true);
            // por algum motivo maluco n da pra fazer:
            //    child.biases=new List<List<float>>(biases);    ou
            //    child.biases=biases;
            // então fiz uma função só pra copiar listas
            child.NN.biases=LLCopier(NN.biases);
            child.NN.weights=LLLCopier(NN.weights);
        }
    }

    public List<List<float>> LLCopier(List<List<float>> original){
        List<List<float>> copy=new List<List<float>>();
        List<float> innerCopy;
        for (int i = 0; i < original.Count; i++){
            innerCopy=new List<float>();
            for (int a = 0; a < original[i].Count; a++){
                innerCopy.Add(original[i][a]);
            }
            copy.Add(innerCopy);
        }
        return copy;
    }

    public List<List<List<float>>> LLLCopier(List<List<List<float>>> original){
        List<List<List<float>>> copy=new List<List<List<float>>>();
        List<List<float>> innerCopy;
        List<float> doublyInnerCopy;
        for (int i = 0; i < original.Count; i++){
            innerCopy=new List<List<float>>();
            for (int a = 0; a < original[i].Count; a++){
                doublyInnerCopy=new List<float>();
                for (int b = 0; b < original[i][a].Count; b++){
                    doublyInnerCopy.Add(original[i][a][b]);
                }
                innerCopy.Add(doublyInnerCopy);
            }
            copy.Add(innerCopy);
        }
        return copy;
    }

    // a função q transforma as informações sensoriais(visão+self speed+self digestion) e tranforma em uma lista de inputs pra NN 
    List<float> GetInputForNeuralNetwork(){
        // inicia a lista de inputs
        List<float> inputForNN=new List<float>();
        // add as informações de visão
        for (int i = 0; i < NumberOfRays; i++){
            inputForNN.Add(rayHitDistance[i]);
            inputForNN.Add(rayHitType[i]);
        }
        // add digestão se for predador
        if (gameObject.tag=="Predator") {inputForNN.Add(Digestion);}
        return inputForNN;
    }


    // a função q casta os rays e vê oq q da
    void See(){
        // pra cada raio:
        Vector3 rayDirection;
        for (int i = 0; i <= NumberOfRays-1; i++){
            // calcula o angulo do raio e a direção
            rayAngle=((-angle)-MaxAngle/2)+(i*MaxAngle/(NumberOfRays-1));
            rayDirection=new Vector3(Mathf.Sin(rayAngle*Mathf.Deg2Rad),Mathf.Cos(rayAngle*Mathf.Deg2Rad),0);
            // casta o raio
            RaycastHit2D hit=Physics2D.Raycast(transform.position+(rayDirection*0.51f),rayDirection,VisionRange);
            // mostras os raios 
            Debug.DrawRay(transform.position+(rayDirection*0.51f),rayDirection*VisionRange,Color.green);
            // se o raio bateu em alguma coisa: add as informações, se n: add 0
            if (hit.collider!=null){
                    // add a distancia
                    rayHitDistance[i]=VisionRange-hit.distance;
                    // ve qual tipo de objeto foi visto:
                    //    mesma espécie -> 0
                    //    parede -> 1
                    //    outra espécie -> -1
                    if (hit.collider.tag!=gameObject.tag){
                        rayHitType[i]=-1;
                        if (hit.collider.tag=="Obstacle") {rayHitType[i]=1;}
                    }else{rayHitType[i]=0;}
            }else{
                rayHitDistance[i]=0;
                rayHitType[i]=0;
            }
        }
    }
}
