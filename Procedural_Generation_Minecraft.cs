using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generation : MonoBehaviour
{

    public int TailleMonde=30;
    public int TailleMondeHauteur=15;
    public int TailleMondeProfondeur=15;

    public GameObject cubePrefab;
    public Transform conteneurCubes;

    public struct CubeInfo{
        public int biome;
        public int material;
        public int shape;
        public bool reel;
        public GameObject gameObject;
    }

    public int[,] GrilleChunck;
    public CubeInfo[,,] GrilleTotale;
   
    void Start()
    {
        GrilleChunck = new int[TailleMonde, TailleMonde];
        GrilleTotale = new CubeInfo[TailleMonde, TailleMonde, TailleMondeHauteur+TailleMondeProfondeur];
        CreerChunck();
        CreerVolume();
        CreerGrotte();
    }

    void CreerChunck(){
        int DivisionChunk = 3; // Mettre la racine carree du nombre de division souhaite
        int TailleChunck = TailleMonde/DivisionChunk;
        int ValeurBiomeLocal;
        for(int DivisionX=0; DivisionX<DivisionChunk; DivisionX++){
            for(int DivisionZ=0; DivisionZ<DivisionChunk; DivisionZ++){
                ValeurBiomeLocal = Random.Range(2,6);
                if(DivisionX>0){
                    switch(GrilleChunck[(DivisionX-1)*TailleChunck,DivisionZ*TailleChunck]){
                        case 1 : ValeurBiomeLocal = ProbabilitePropagationchunck(DivisionX,DivisionZ,0.1f,ValeurBiomeLocal,TailleChunck);break;
                        case 2 : ValeurBiomeLocal = ProbabilitePropagationchunck(DivisionX,DivisionZ,0.3f,ValeurBiomeLocal,TailleChunck);break;
                        case 3 : ValeurBiomeLocal = ProbabilitePropagationchunck(DivisionX,DivisionZ,0.7f,ValeurBiomeLocal,TailleChunck);break;
                        case 4 : ValeurBiomeLocal = ProbabilitePropagationchunck(DivisionX,DivisionZ,0.2f,ValeurBiomeLocal,TailleChunck);break;
                        case 5 : ValeurBiomeLocal = ProbabilitePropagationchunck(DivisionX,DivisionZ,0.8f,ValeurBiomeLocal,TailleChunck);break;
                    }
                }
                for(int x=DivisionX*TailleChunck; x<(DivisionX+1)*TailleChunck; x++){
                    for(int z=DivisionZ*TailleChunck; z<(DivisionZ+1)*TailleChunck; z++){
                        GrilleChunck[x,z] = ValeurBiomeLocal;
                    }
                }
            }
        }
    }

    int ProbabilitePropagationchunck(int x, int z, float ProbabilitePropagationChunck, int ValeurBiomeLocal, int TailleChunck){
        if(x>0){
            if(Random.value < ProbabilitePropagationChunck){
                ValeurBiomeLocal = GrilleChunck[(x-1)*TailleChunck,z*TailleChunck];
            }
        }
        return ValeurBiomeLocal;
    }

    void CreerVolume(){
        PoserLesSommets();
        ElargirLesSommets();
        LisserLesSommets();
        ComblerLesFondations();
        ComblerGruyere();
    }

    void PoserLesSommets(){
        for(int x=0; x<TailleMonde; x++){
            for(int z=0; z<TailleMonde; z++){
                switch(GrilleChunck[x,z]){
                    case 1 : CreerLaHauteur(1, x, z);break;
                    case 2 : CreerLaHauteur(2, x, z);break;
                    case 3 : CreerLaHauteur(3, x, z);break;
                    case 4 : CreerLaHauteur(4, x, z);break;
                    case 5 : CreerLaHauteur(5, x, z);break;
                }
            }
        }
    }

    void CreerLaHauteur(int CoordonneeBiome, int x, int z){
        int i=TailleMondeHauteur;
        int HauteurTrouvee=1;
        float ProbabiliteApparitionHauteur=0;
        while(i>0){
            switch(CoordonneeBiome){
                case 1 : ProbabiliteApparitionHauteur = 1.0f/(i*i*i);break;
                case 2 : ProbabiliteApparitionHauteur = 1.0f/(i*i*i*i);break;
                case 3 : ProbabiliteApparitionHauteur = 1.0f/(i*i*i*i*i);break;
                case 4 : ProbabiliteApparitionHauteur = 1.0f/(i*i*i*i*i*i);break;
                case 5 : ProbabiliteApparitionHauteur = 0;break;
            }
            if(Random.value < ProbabiliteApparitionHauteur){
                HauteurTrouvee=i;
                i=0;
            }
            else{
                i--;
            }
        }
        CreerCube(x,z,HauteurTrouvee+TailleMondeProfondeur);
    }

    void CreerCube(int CoordonneeX, int CoordonneeZ, int CoordonneeY){
        if(GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].reel == true){
            return;
        }
        GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].reel = true;
        GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].biome = GrilleChunck[CoordonneeX,CoordonneeZ];

        GameObject cube = Instantiate(cubePrefab, new Vector3(CoordonneeX, CoordonneeY, CoordonneeZ), Quaternion.identity);
        GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].gameObject = cube;
        Color couleur = ObtenirCouleurBiome(GrilleChunck[CoordonneeX,CoordonneeZ]);
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material.color = couleur;
        cube.transform.parent = conteneurCubes;
    }
    Color ObtenirCouleurBiome(int biome){
        switch(biome){
            case 1: return new Color(0.5f, 0.5f, 0.5f); // Gris
            case 2: return new Color(0.8f, 0.3f, 0.1f); // Orange
            case 3: return new Color(0.2f, 0.6f, 0.2f); // Vert
            case 4: return new Color(0.9f, 0.8f, 0.5f); // Sable
            case 5: return new Color(0.2f, 0.4f, 0.8f); // Bleu
            default: return Color.white;
        }
    }
   

    void ElargirLesSommets(){
        for(int x=1; x<TailleMonde-1; x++){
            for(int z=1; z<TailleMonde-1; z++){
                for(int y=1; y<TailleMondeHauteur-1; y++){
                    if(GrilleTotale[x,z,y].reel == false){
                        if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x-1,z-1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x-1,z,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x-1,z+1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x,z+1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x+1,z+1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x+1,z,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x+1,z-1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeElargirLesSommets(x,z-1,y, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                    }
                }
            }
        }
    }

    bool VerifierLesAlentoursSousFonctionDeElargirLesSommets(int CoordonneeX, int CoordonneeZ, int CoordonneeY, int biome){
        bool CreerLeCube = false;
        float ProbabiliteContagion = 0;
        switch (biome){
            case 1 : ProbabiliteContagion = 0.35f;break;
            case 2 : ProbabiliteContagion = 0.35f;break;
            case 3 : ProbabiliteContagion = 0.4f;break;
            case 4 : ProbabiliteContagion = 0.5f;break;
            case 5 : ProbabiliteContagion = 0.35f;break;
        }
        if(GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].reel == true){
            if(Random.value<ProbabiliteContagion){
                CreerLeCube = true;
            }
        }
        return CreerLeCube;
    }

    void LisserLesSommets(){
        for(int x=1; x<TailleMonde-1; x++){
            for(int z=1; z<TailleMonde-1; z++){
                for(int y=TailleMondeHauteur-2; y>=0; y--){
                    if(GrilleTotale[x,z,y].reel == false){
                        if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x-1,z-1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x-1,z,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x-1,z+1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x,z+1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x+1,z+1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x+1,z,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x+1,z-1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                        else if(VerifierLesAlentoursSousFonctionDeLisserLesSommets(x,z-1,y+1, GrilleChunck[x,z]) == true){
                            CreerCube(x,z,y);
                        }
                    }
                }
            }
        }
    }

    bool VerifierLesAlentoursSousFonctionDeLisserLesSommets(int CoordonneeX, int CoordonneeZ, int CoordonneeY, int Biome){
        bool CreerLeCube = false;
        float ProbaDeLissage=0;
        switch(Biome){
            case 1 : ProbaDeLissage = 0.4f;break;
            case 2 : ProbaDeLissage = 0.3f;break;
            case 3 : ProbaDeLissage = 0.3f;break;
            case 4 : ProbaDeLissage = 0.3f;break;
            case 5 : ProbaDeLissage = 0.3f;break;
        }
        if(GrilleTotale[CoordonneeX, CoordonneeZ, CoordonneeY].reel == true){
            if(Random.value<ProbaDeLissage){
                CreerLeCube = true;
            }
        }
        return CreerLeCube;
    }

    void ComblerLesFondations(){
        for(int x=0; x<TailleMonde; x++){
            for(int z=0; z<TailleMonde; z++){
                for(int y=TailleMondeHauteur-1; y>=0; y--){
                    if(GrilleTotale[x,z,y+1].reel == true){
                        CreerCube(x,z,y);
                    }
                }
            }
        }
    }

    void ComblerGruyere(){
        for(int x=1; x<TailleMonde-1; x++){
            for(int z=1; z<TailleMonde-1; z++){
                for(int y=1; y<TailleMondeHauteur-1; y++){
                    if(CompterLeNombreDeVoisinsProches(x,z,y) >= 3){
                        CreerCube(x,z,y);
                    }
                }
            }
        }
    }

    int CompterLeNombreDeVoisinsProches(int x, int z, int y){
        int NombreDeVoisinsProches = 0;
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x-1,z,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x-1,z+1,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x,z+1,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x+1,z+1,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x+1,z,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x-1,z-1,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x,z-1,y);
        NombreDeVoisinsProches += VerifierLesAlentoursSousFonctionDeVoisinsProches(x+1,z-1,y);
        return NombreDeVoisinsProches;
    }

    int VerifierLesAlentoursSousFonctionDeVoisinsProches(int x,int z,int y){
        if(GrilleTotale[x,z,y].reel == true){
            return 1;
        }
        else{
            return 0;
        }
    }

    void CreerGrotte(){
        float ProbabiliteApparitionGrotte = 0.0003f;
        for(int x=0; x<TailleMonde; x++){
            for(int z=0; z<TailleMonde; z++){
                for(int y=0; y<TailleMondeProfondeur; y++){
                    if(Random.value <ProbabiliteApparitionGrotte){
                        EnleverCube(x,z,y);
                        ElargirGrotte(x,z,y);
                    }
                }
            }
        }
    }

    void EnleverCube(int x, int z, int y){
        Destroy(GrilleTotale[x,z,y].gameObject);
        GrilleTotale[x,z,y].reel = false;
    }

    void ElargirGrotte(int CoordonneeXdeLaGrotte, int CoordonneeZdeLaGrotte, int CoordonneeYdeLaGrotte){
        int TailleTrouGrotte = 0;
        for(int i=(TailleMondeProfondeur/2)-1; i>2; i--){
            if(Random.value < (1.0f/i)){
                TailleTrouGrotte = i;
                break;
            }
        }
        for(int x=CoordonneeXdeLaGrotte-TailleTrouGrotte; x<TailleTrouGrotte+CoordonneeXdeLaGrotte; x++){
            if(x>=0 && x<TailleMonde){
                for(int z=CoordonneeZdeLaGrotte-TailleTrouGrotte; z<TailleTrouGrotte+CoordonneeZdeLaGrotte; z++){
                    if(z>=0 && z<TailleMonde){
                        for(int y=CoordonneeYdeLaGrotte-TailleTrouGrotte; y<TailleTrouGrotte+CoordonneeYdeLaGrotte; y++){
                            if(y>=0 && y<TailleMondeProfondeur){
                                EnleverCube(x,z,y);
                            }
                        }
                    }
                }
            }
        }
        LisserLaGrotte(CoordonneeXdeLaGrotte,CoordonneeZdeLaGrotte,CoordonneeYdeLaGrotte,TailleTrouGrotte);
    }

    void LisserLaGrotte(int CoordonneeXdeLaGrotte, int CoordonneeZdeLaGrotte, int CoordonneeYdeLaGrotte, int TailleTrouGrotte){
        
    }
}