using System;
using System.Collections.Generic;
namespace proves2
{
    class Program
    {
        //TOCAR
        const int DELAY = 0;
        const bool MOSTRA_TAULER = false;
        const int PROFUNDITAT = 6;
        const bool MOSTRA_PROFUNDITAT = false;
        const bool MOSTRA_DICCIONARI = false;
        //NO TOCAR
        const int GUANYEN = 3;
        const int PERDEN = 1;
        const int NEUTRAL = 2;
        //NO CERCATS SON ELS NOMBRES NEGATIUS
        struct ResultatPosició
        {
            public int resultat;
            public string moviment;
            public int evaluació;
        }
        struct Peça
        {
            public int cordX;
            public int cordY;
            public bool mort;
        }
        struct Bloqueig
        {
            public int cordX;
            public int cordY;
            public bool usat;
        }


        static Dictionary<long, ResultatPosició> visitats = new Dictionary<long, ResultatPosició>();
        
        static void Main(string[] args)
        {
            Peça[] pecesBlanques = new Peça[4];
            Peça[] pecesNegres = new Peça[4];
            Bloqueig[] bloquejosBlanques = new Bloqueig[3];
            Bloqueig[] bloquejosNegres = new Bloqueig[3];

            //ESCOLLIR POSICIÓ
            EstablirPosicióInicial(pecesBlanques, pecesNegres, bloquejosBlanques, bloquejosNegres);
            
            ResultatPosició resultat;
            Console.WriteLine("Hora inici: " + DateTime.Now.ToString("h:mm:ss tt"));
            MostrarTauler(RepresentarTauler(pecesBlanques, pecesNegres, bloquejosBlanques, bloquejosNegres),0);
            resultat = Jugar(pecesBlanques, pecesNegres, bloquejosBlanques, bloquejosNegres, true, 0);
            Console.WriteLine("Millor moviment: "+resultat.moviment);
            Console.WriteLine("Qui guanya(1 negres, 2 taules, 3 blanques i n<=0 s'ha de recórrer a l'heurística): " + resultat.resultat);
            Console.WriteLine("Avaluació heurística: " + resultat.evaluació);
            Console.WriteLine("Hora final: " + DateTime.Now.ToString("h:mm:ss tt"));
            Console.WriteLine("Posicions explorades fins ara: " + visitats.Count);
            Console.WriteLine("Codi posició: " + Codificar(RepresentarTauler(pecesBlanques, pecesNegres, bloquejosBlanques, bloquejosNegres), bloquejosBlanques));
        }

        //4.2.2.1 (Funció principal)
        static ResultatPosició Jugar(Peça[] pecesAliadesOriginals, Peça[] pecesEnemiguesOriginals, Bloqueig[] bloquejosAliatsOriginals, Bloqueig[] bloquejosEnemicsOriginals, bool repetir, int profunditat)
        {
            //INICIALITZAR LES PECES I ELS BLOQUEJOS
            Peça[] pecesAliades;
            Peça[] pecesEnemigues;
            Bloqueig[] bloquejosAliats;
            Bloqueig[] bloquejosEnemics;

            //SI REAPAREIX UNA PEÇA NO INCREMENTA EL COMPTADOR I LES PECES SON IDENTIQUES A LES ORIGINALS
            if (repetir)
            {
                pecesAliades = pecesAliadesOriginals;
                pecesEnemigues = pecesEnemiguesOriginals;
                bloquejosAliats = bloquejosAliatsOriginals;
                bloquejosEnemics = bloquejosEnemicsOriginals;
            }

            //SI NO REAPAREIX INCREMENTA EL COMPTADOR I ES GIREN LES PECES
            else
            {
                pecesAliades = GirarPeces(pecesEnemiguesOriginals);
                pecesEnemigues = GirarPeces(pecesAliadesOriginals);
                bloquejosAliats = GirarBloquejos(bloquejosEnemicsOriginals);
                bloquejosEnemics = GirarBloquejos(bloquejosAliatsOriginals);
                profunditat++;
                if (MOSTRA_PROFUNDITAT)
                {
                    Console.WriteLine(profunditat);
                }
            }

            //UN COP TENIM LES PECES LES COLOQUEM A UN TAULER QUE UTILITZAREM PER LES PROPERES FUNCIONS
            int[,] tauler = RepresentarTauler(pecesAliades, pecesEnemigues, bloquejosAliats, bloquejosEnemics);

            //CODIFIQUEM EL TAULER PER POSAR-LO AL DICCIONARI
            long codi = Codificar(tauler, bloquejosAliats);
            ResultatPosició resultatPosició;

            //MIRAR SI JA ESTA AL DICCIONARI PER NO HAVER DE TORNAR-HO A ANALITZAR

            if (visitats.ContainsKey(codi))
            {
                ResultatPosició valor = visitats[codi];
                switch (valor.resultat)
                {
                    case GUANYEN:
                        if (MOSTRA_DICCIONARI)
                        {
                            Console.WriteLine("GUANYEN");
                        }
                        return valor;
                    case PERDEN:
                        if (MOSTRA_DICCIONARI)
                        {
                            Console.WriteLine("PERDEN");
                        }
                        return valor;
                    case NEUTRAL:
                        if (MOSTRA_DICCIONARI)
                        {
                            Console.WriteLine("NEUTRAL");
                        }
                        return valor;
                    //MIRAR A QUINA PROFUNDITAT HA ARRIBAT
                    default:
                        if ((PROFUNDITAT - profunditat) <= -valor.resultat)
                        {
                            if (MOSTRA_DICCIONARI)
                            {
                                Console.WriteLine(4);
                            }
                            return valor;
                        }
                        else
                        {
                            if (MOSTRA_DICCIONARI)
                            {
                                Console.WriteLine("4 pero passar");
                            }
                            break;
                        }
                }
            }

            //COM QUE NO ESTA AL DICCIONARI SEL INICIALITZA AMB VALOR DE NEUTRAL
            else
            {
                resultatPosició.moviment = "";
                resultatPosició.resultat = 2;
                resultatPosició.evaluació = 0;
                //Console.WriteLine(visitatsB.Count);
                visitats.Add(codi, resultatPosició);
            }
            resultatPosició.moviment = "";
            resultatPosició.resultat = 1;
            resultatPosició.evaluació = 0;
            if (MOSTRA_TAULER)
            {
                MostrarTauler(tauler, profunditat);
            }
            //MIRAR SI HI HA PECES MORTES
            ResultatPosició resultatsReapareixer = Reapareixer(pecesAliades, pecesEnemigues, bloquejosAliats, bloquejosEnemics, tauler, profunditat);
            if (resultatsReapareixer.resultat != 4)
            {
                visitats[codi] = resultatsReapareixer;
                return resultatsReapareixer;
            }

            //MIRAR SI HA ARRIBAT AL MAXIM DE PROFUNDITAT
            if (profunditat >= PROFUNDITAT)
            {
                resultatPosició.resultat = 0;
                resultatPosició.evaluació = AnalitzarPosició(pecesAliades,pecesEnemigues,bloquejosAliats,bloquejosEnemics,tauler);
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }
            
            //MIRAR SI S'HA PERDUT PER OFEGAMENT
            if (!PotMoure(pecesAliades, bloquejosAliats, tauler))
            {
                resultatPosició.resultat = PERDEN;
                resultatPosició.moviment = "";
                resultatPosició.evaluació = 0;
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }

            //MIRAR SI POT GUANYAR EN AQUEST MOVIMENT (NOMES PECES)
            if (GuanyaEn1(pecesAliades, tauler))
            {
                resultatPosició.resultat = GUANYEN;
                resultatPosició.moviment = "";
                resultatPosició.evaluació = 0;
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }

            //DEFINIR RESULTAT

            //BLOQUEJOS
            resultatPosició = AnalitzarResultats(resultatPosició, Bloquejos(pecesAliades, pecesEnemigues, bloquejosAliats, bloquejosEnemics, tauler, profunditat));
            if (resultatPosició.resultat == GUANYEN)
            {
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }

            //SALTS
            for (int i = 0; i < 4; i++)
            {
                resultatPosició = AnalitzarResultats(resultatPosició, Salts(i, pecesAliades, pecesEnemigues, bloquejosAliats, bloquejosEnemics, tauler, profunditat));
                if (resultatPosició.resultat == GUANYEN)
                {
                    visitats[codi] = resultatPosició;
                    return resultatPosició;
                }
            }

            //MOVIMENTS BASICS
            resultatPosició = AnalitzarResultats(resultatPosició, MovimentsBasics(pecesAliades, pecesEnemigues, bloquejosAliats, bloquejosEnemics, tauler, profunditat));
            if (resultatPosició.resultat == GUANYEN)
            {
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }

            //RETORNAR RESULTAT
            if (resultatPosició.resultat <= 0)
            {
                resultatPosició.resultat -= 1;
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }
            else
            {
                visitats[codi] = resultatPosició;
                return resultatPosició;
            }
        }



        //-------------FUNCIONS DE SUPORT-----------
            //4.5.1
        static Peça[] GirarPeces(Peça[] peces)
        {
            Peça[] pecesGirades = new Peça[peces.Length];
            for (int i = 0; i < peces.Length; i++)
            {
                pecesGirades[i].cordX = 7 - peces[i].cordX;
                pecesGirades[i].cordY = 7 - peces[i].cordY;
                pecesGirades[i].mort = peces[i].mort;
            }
            return pecesGirades;
        }
        static Bloqueig[] GirarBloquejos(Bloqueig[] bloqueigs)
        {
            Bloqueig[] bloquejosGirats = new Bloqueig[bloqueigs.Length];
            for (int i = 0; i < bloqueigs.Length; i++)
            {
                bloquejosGirats[i].cordX = 7 - bloqueigs[i].cordX;
                bloquejosGirats[i].cordY = 7 - bloqueigs[i].cordY;
                bloquejosGirats[i].usat = bloqueigs[i].usat;
            }
            return bloquejosGirats;
        }
            //4.2.2.1
        static int[,] RepresentarTauler(Peça[] pecesAliades, Peça[] pecesEnemigues, Bloqueig[] bloquejosAliats, Bloqueig[] bloquejosEnemics)
        {
            int[,] tauler = new int[8, 8];
            foreach (Peça peça in pecesAliades)
            {
                if (!peça.mort)
                {
                    tauler[peça.cordX, peça.cordY] = 1;
                }
            }
            foreach (Peça peça in pecesEnemigues)
            {
                if (!peça.mort)
                {
                    tauler[peça.cordX, peça.cordY] = 2;
                }
            }
            foreach (Bloqueig bloqueig in bloquejosAliats)
            {
                if (bloqueig.usat)
                {
                    tauler[bloqueig.cordX, bloqueig.cordY] = 3;
                }
            }

            foreach (Bloqueig bloqueig in bloquejosEnemics)
            {
                if (bloqueig.usat)
                {
                    tauler[bloqueig.cordX, bloqueig.cordY] = 3;
                }
            }
            return tauler;
        }
            //4.3.3 i 4.5.2
        static long Codificar(int[,] tauler, Bloqueig[] bloquejosAliats)
        {
            int[] xBlanques = new int[4];
            int[] yBlanques = new int[4];
            int[] xNegres = new int[4];
            int[] yNegres = new int[4];
            int indexBlanques = 0;
            int indexNegres = 0;
            long resultat = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 != 0)
                    {
                        if (y < 6 && y >= 2)
                        {
                            resultat <<= 1;
                            if (tauler[x, y] == 3)
                            {
                                resultat++;
                            }
                        }
                        if (tauler[x, y] == 1)
                        {
                            xBlanques[indexBlanques] = x;
                            yBlanques[indexBlanques] = y;
                            indexBlanques++;
                        }
                        else if (tauler[x, y] == 2)
                        {
                            xNegres[indexNegres] = x;
                            yNegres[indexNegres] = y;
                            indexNegres++;
                        }
                    }
                }
            }
            for (int i = 0; i < indexBlanques; i++)
            {
                if (yBlanques[i] % 2 == 0)
                {
                    xBlanques[i] = (xBlanques[i] - 1) / 2;
                }
                else
                {
                    xBlanques[i] = xBlanques[i] / 2;
                }
                resultat <<= 3;
                resultat = resultat + yBlanques[i];
                resultat <<= 2;
                resultat = resultat + xBlanques[i];
            }
            resultat <<= (4 - indexBlanques) * 5;
            for (int i = 0; i < indexNegres; i++)
            {
                if (yNegres[i] % 2 == 0)
                {
                    xNegres[i] = (xNegres[i] - 1) / 2;
                }
                else
                {
                    xNegres[i] = xNegres[i] / 2;
                }
                resultat <<= 3;
                resultat = resultat + yNegres[i];
                resultat <<= 2;
                resultat = resultat + xNegres[i];
            }
            resultat <<= (4 - indexNegres) * 5;
            int contar = 0;

            foreach (Bloqueig bloqueig in bloquejosAliats)
            {
                if (bloqueig.usat)
                {
                    contar++;
                }
            }
            resultat <<= 2;
            resultat += contar;
            //Console.WriteLine(Convert.ToString(resultat,2));
            return resultat;
        }
            //4.3.2
        static void MostrarTauler(int[,] tauler, int profunditat)
        {
            for (int i = 7; i >= 0; i--)
            {
                if (profunditat % 2 == 0)
                {
                    Console.Write((i + 1) + ". ");
                }
                else
                {
                    Console.Write((7 - i + 1) + ". ");
                }
                for (int j = 0; j < 8; j++)
                {
                    if (tauler[j, i] == 1)
                    {
                        if (profunditat % 2 == 0)
                        {
                            Console.Write('b');
                        }
                        else
                        {
                            Console.Write('n');
                        }
                    }
                    else if (tauler[j, i] == 2)
                    {
                        if (profunditat % 2 == 0)
                        {
                            Console.Write('n');
                        }
                        else
                        {
                            Console.Write('b');
                        }
                    }
                    else if (tauler[j, i] == 3)
                    {
                        Console.Write('X');
                    }
                    else
                    {
                        Console.Write('_');
                    }
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            System.Threading.Thread.Sleep(DELAY);
        }
            //No se'n parla al treball escrit pero serveixen per copiar les structs.
        static Peça[] CopiarPeces(Peça[] peças)
        {
            Peça[] copia = new Peça[4];
            for (int i = 0; i < 4; i++)
            {
                copia[i].cordX = peças[i].cordX;
                copia[i].cordY = peças[i].cordY;
                copia[i].mort = peças[i].mort;
            }
            return copia;
        }
        static Bloqueig[] CopiaBloqueigs(Bloqueig[] original)
        {
            Bloqueig[] copia = new Bloqueig[3];
            for (int i = 0; i < 3; i++)
            {
                copia[i].cordX = original[i].cordX;
                copia[i].cordY = original[i].cordY;
                copia[i].usat = original[i].usat;
            }
            return copia;
        }
            //4.4.2.1
        static ResultatPosició AnalitzarResultats(ResultatPosició resultatOriginal, ResultatPosició resultatActual)
        {
            //SI L'ORIGINAL GUANYAVA NO TÉ SENTIT MIRAR ELS RESULTATS
            if (resultatOriginal.resultat == GUANYEN)
            {
                return resultatOriginal;
            }

            switch (resultatActual.resultat)
            {
                //SI GUANYEN RETORNA GUANYEN
                case GUANYEN:
                    return resultatActual;

                //SI ÉS NEUTRAL, NOMES RETORNARA NEUTRAL QUAN L'ORIGINAL PERDI
                case NEUTRAL:
                    if (resultatOriginal.resultat == PERDEN) return resultatActual;
                    break;

                //SI PERD RETORNARA EL RESULTAT ORIGINAL, DONCS PITJOR NO POT SER
                case PERDEN:
                    if (resultatOriginal.resultat == PERDEN)
                    {
                        return resultatActual;
                    }
                    break;

                //EN CAS QUE NO HI HAGI RESULTAT, RETORNARA NO RESULTAT. SI L'ORIGINAL TAMBÉ TORNAVA NO RESULTAT S'AGAFARA EL VALOR QUE MES S'APROXIMI A 0
                default:
                    if (resultatOriginal.resultat <= 0)
                    {
                        if (resultatOriginal.evaluació < resultatActual.evaluació)
                        {
                            return resultatActual;
                        }
                        else
                        {
                            return resultatOriginal;
                        }
                    }
                    return resultatActual;
            }
            return resultatOriginal;
        }
            //4.4.1.2.1
        static bool PotColocarseBloqueig(int[,] tauler, int x, int y)
        {
            bool[,] copiaTauler = new bool[8, 8];
            for (int x2 = 0; x2 < 8; x2++)
            {
                for (int y2 = 2; y2 < 6; y2++)
                {
                    copiaTauler[x2, y2] = tauler[x2, y2] == 3;
                }
            }
            copiaTauler[x, y] = true;
            bool resultat = false;
            resultat = resultat || PotColocarseRecusiu(copiaTauler, 1, 0);
            resultat = resultat || PotColocarseRecusiu(copiaTauler, 7, 0);
            return resultat;

        }
            //4.5.1
        static ResultatPosició GirarResultat(ResultatPosició resultatOriginal)
        {
            ResultatPosició resultat;
            resultat.moviment = resultatOriginal.moviment;
            resultat.evaluació = -resultatOriginal.evaluació;
            if (resultatOriginal.resultat == 3)
            {
                resultat.resultat = 1;
            }
            else if (resultatOriginal.resultat == 1)
            {
                resultat.resultat = 3;
            }
            else 
            {
                resultat.resultat = resultatOriginal.resultat;
            }
            return resultat;
        }



        //------------FUNCIONS GRANS------------
            //4.2.2.3
        static ResultatPosició Reapareixer(Peça[] pecesAliades, Peça[] pecesEnemigues, Bloqueig[] bloquejosAliats, Bloqueig[] bloquejosEnemics, int[,] tauler, int comptador)
        {
            ResultatPosició resultat;
            resultat.resultat = 4;
            resultat.moviment = "";
            resultat.evaluació = 0;
            for (int i = 0; i < 4 && resultat.resultat == 4; i++)
            {
                if (pecesAliades[i].mort)
                {
                    resultat.resultat = 1;

                    if (tauler[1, 0] == 0)
                    {
                        Peça[] copia = CopiarPeces(pecesAliades);
                        copia[i].cordX = 1;
                        copia[i].cordY = 0;
                        copia[i].mort = false;
                        ResultatPosició resultatA = Jugar(copia, pecesEnemigues, bloquejosAliats, bloquejosEnemics, true, comptador);
                        resultatA.moviment = resultatA.moviment + ", Reapareixer a (1,0)";
                        resultat = AnalitzarResultats(resultat, resultatA);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                    }

                    if (tauler[3, 0] == 0)
                    {
                        Peça[] copia = CopiarPeces(pecesAliades);
                        copia[i].cordX = 3;
                        copia[i].cordY = 0;
                        copia[i].mort = false;
                        ResultatPosició resultatA = Jugar(copia, pecesEnemigues, bloquejosAliats, bloquejosEnemics, true, comptador);
                        resultatA.moviment = resultatA.moviment + ", Reapareixer a (3,0)";
                        resultat = AnalitzarResultats(resultat, resultatA);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                    }

                    if (tauler[5, 0] == 0)
                    {
                        Peça[] copia = CopiarPeces(pecesAliades);
                        copia[i].cordX = 5;
                        copia[i].cordY = 0;
                        copia[i].mort = false;
                        ResultatPosició resultatA = Jugar(copia, pecesEnemigues, bloquejosAliats, bloquejosEnemics, true, comptador);
                        resultatA.moviment = resultatA.moviment + ", Reapareixer a (5,0)";
                        resultat = AnalitzarResultats(resultat, resultatA);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                    }

                    if (tauler[7, 0] == 0)
                    {
                        Peça[] copia = CopiarPeces(pecesAliades);
                        copia[i].cordX = 7;
                        copia[i].cordY = 0;
                        copia[i].mort = false;
                        ResultatPosició resultatA = Jugar(copia, pecesEnemigues, bloquejosAliats, bloquejosEnemics, true, comptador);
                        resultatA.moviment = resultatA.moviment + ", Reapareixer a (7,0)";
                        resultat = AnalitzarResultats(resultat, resultatA);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                    }
                }
            }
            return resultat;
        }
            //4.2.2.2 -> 4.5.3
        static bool PotMoure(Peça[] pecesAliades, Bloqueig[] bloquejosAliats, int[,] tauler)
        {
            //MIRAR SI POT COLOCAR BLOQUEJOS
            bool potMoure = !bloquejosAliats[0].usat || !bloquejosAliats[1].usat || !bloquejosAliats[2].usat;

            //MIRAR SI POT MOURE PECES
            if (!potMoure)
            {
                foreach (Peça peçaA in pecesAliades)
                {
                    if (peçaA.cordX != 7)
                    {
                        if (tauler[peçaA.cordX + 1, peçaA.cordY + 1] == 0)
                        {
                            potMoure = true;
                        }
                        else if (peçaA.cordY != 6 && peçaA.cordX != 6 && tauler[peçaA.cordX + 1, peçaA.cordY + 1] != 3 && tauler[peçaA.cordX + 2, peçaA.cordY + 2] == 0)
                        {
                            potMoure = true;
                        }
                    }
                    if (peçaA.cordX != 0)
                    {
                        if (tauler[peçaA.cordX - 1, peçaA.cordY + 1] == 0)
                        {
                            potMoure = true;
                        }
                        else if (peçaA.cordY != 6 && peçaA.cordX != 1 && tauler[peçaA.cordX - 1, peçaA.cordY + 1] != 3 && tauler[peçaA.cordX - 2, peçaA.cordY + 2] == 0)
                        {
                            potMoure = true;
                        }
                    }
                }
            }
            return potMoure;
        }
        static bool GuanyaEn1(Peça[] pecesAliades, int[,] tauler)
        {

            foreach (Peça peça in pecesAliades)
            {
                if (peça.cordX != 7) //pot anar a la dreta
                {
                    if (peça.cordY == 6) // està a la penúltima fila
                    {
                        if (tauler[peça.cordX + 1, peça.cordY + 1] == 0) //està lliure a la dreta
                        {
                            return true;
                        }
                    }
                    else if (peça.cordY % 2 != 0) //pot arribar saltant al final
                    {
                        if (GuanyaEn1Recursiu(tauler, peça, true))
                        {
                            return true;
                        }
                    }
                }


                if (peça.cordX != 0) //pot anar a l'esquerra
                {
                    if (peça.cordY == 6)
                    {
                        if (tauler[peça.cordX - 1, peça.cordY + 1] == 0)
                        {
                            return true;
                        }
                    }
                    else if (peça.cordY % 2 != 0)
                    {
                        if (GuanyaEn1Recursiu(tauler, peça, false))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
            //4.4.1.3
        static ResultatPosició Bloquejos(Peça[] pecesAliades, Peça[] pecesEnemigues, Bloqueig[] bloquejosAliats, Bloqueig[] bloquejosEnemics, int[,] tauler, int comptador)
        {
            ResultatPosició actual;
            ResultatPosició resultat;
            resultat.resultat = PERDEN;
            resultat.moviment = "";
            resultat.evaluació = 0;
            bool usat = false;
            for (int i = 0; i < 3 && !usat; i++)
            {
                if (!bloquejosAliats[i].usat)
                {
                    usat = true;
                    for (int y = 5; y >= 2; y--)
                    {
                        for (int x = 0; x < 8 && resultat.resultat != GUANYEN; x++)
                        {
                            if ((x + y) % 2 != 0 && tauler[x, y] == 0 && PotColocarseBloqueig(tauler, x, y))
                            {
                                Bloqueig[] copia = CopiaBloqueigs(bloquejosAliats);
                                copia[i].cordX = x;
                                copia[i].cordY = y;
                                copia[i].usat = true;
                                actual = Jugar(pecesAliades, pecesEnemigues, copia, bloquejosEnemics, false, comptador);
                                actual.moviment = "bloqueig a (" + copia[i].cordX + ", " + copia[i].cordY + ")";
                                resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                                if (resultat.resultat == GUANYEN)
                                {
                                    return resultat;
                                }

                                if (i < 2)
                                {

                                    int[,] copiaTauler = RepresentarTauler(pecesAliades, pecesEnemigues, copia, bloquejosEnemics);
                                    for (int y2 = y; y2 >= 2; y2--)
                                    {
                                        for (int x2 = 0; x2 < 8 && resultat.resultat != GUANYEN; x2++)
                                        {
                                            if ((x2 + y2) % 2 != 0 && copiaTauler[x2, y2] == 0 && PotColocarseBloqueig(copiaTauler, x2, y2))
                                            {

                                                copia[i + 1].cordX = x2;
                                                copia[i + 1].cordY = y2;
                                                copia[i + 1].usat = true;
                                                actual = Jugar(pecesAliades, pecesEnemigues, copia, bloquejosEnemics, false, comptador);
                                                actual.moviment = "bloqueig a (" + copia[i].cordX + ", " + copia[i].cordY + ") i a (" + copia[i + 1].cordX + ", " + copia[i + 1].cordY + ")"; ;
                                                resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                                                if (resultat.resultat == GUANYEN)
                                                {
                                                    return resultat;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return resultat;
        }
            //4.2.2.5
        static ResultatPosició Salts(int peça, Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres, int[,] tauler, int comptador)
        {
            ResultatPosició resultat;
            ResultatPosició actual;
            resultat.resultat = PERDEN;
            resultat.moviment = "";
            resultat.evaluació = 0;
            if (pecesBlanques[peça].cordX < 6 && pecesBlanques[peça].cordY < 6)
            {
                if (tauler[pecesBlanques[peça].cordX + 2, pecesBlanques[peça].cordY + 2] == 0)
                {
                    if (tauler[pecesBlanques[peça].cordX + 1, pecesBlanques[peça].cordY + 1] == 1)
                    {
                        Peça[] copia = CopiarPeces(pecesBlanques);
                        copia[peça].cordX = pecesBlanques[peça].cordX + 2;
                        copia[peça].cordY = pecesBlanques[peça].cordY + 2;
                        int[,] nouTauler = RepresentarTauler(copia, pecesNegres, bloquejosBlanques, bloquejosNegres);
                        actual = Salts(peça, copia, pecesNegres, bloquejosBlanques, bloquejosNegres, nouTauler, comptador);
                        actual.moviment += ", salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "no mata";
                        resultat = AnalitzarResultats(resultat, actual);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                        actual = Jugar(copia, pecesNegres, bloquejosBlanques, bloquejosNegres, false, comptador);
                        actual.moviment = "Salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "no mata";
                        resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                    }
                    else if (tauler[pecesBlanques[peça].cordX + 1, pecesBlanques[peça].cordY + 1] == 2)
                    {
                        Peça[] copia = CopiarPeces(pecesBlanques);
                        copia[peça].cordX = pecesBlanques[peça].cordX + 2;
                        copia[peça].cordY = pecesBlanques[peça].cordY + 2;
                        Peça[] copia2 = CopiarPeces(pecesNegres);
                        for (int i = 0; i < 4; i++)
                        {
                            if (copia2[i].cordX - 1 == pecesBlanques[peça].cordX && copia2[i].cordY - 1 == pecesBlanques[peça].cordY)
                            {
                                copia2[i].mort = true;
                            }
                        }

                        int[,] nouTauler = RepresentarTauler(copia, copia2, bloquejosBlanques, bloquejosNegres);
                        actual = Salts(peça, copia, copia2, bloquejosBlanques, bloquejosNegres, nouTauler, comptador);
                        actual.moviment += ", salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "mata";
                        resultat = AnalitzarResultats(resultat, actual);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                        actual = Jugar(copia, copia2, bloquejosBlanques, bloquejosNegres, false, comptador);
                        actual.moviment = "Salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "mata";
                        resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                    }
                }
            }
            if (pecesBlanques[peça].cordX > 1 && pecesBlanques[peça].cordY < 6)
            {
                if (tauler[pecesBlanques[peça].cordX - 2, pecesBlanques[peça].cordY + 2] == 0)
                {
                    if (tauler[pecesBlanques[peça].cordX - 1, pecesBlanques[peça].cordY + 1] == 1)
                    {
                        Peça[] copia = CopiarPeces(pecesBlanques);
                        copia[peça].cordX = pecesBlanques[peça].cordX - 2;
                        copia[peça].cordY = pecesBlanques[peça].cordY + 2;
                        int[,] nouTauler = RepresentarTauler(copia, pecesNegres, bloquejosBlanques, bloquejosNegres);
                        actual = Salts(peça, copia, pecesNegres, bloquejosBlanques, bloquejosNegres, nouTauler, comptador);
                        actual.moviment += ", salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "no mata";
                        resultat = AnalitzarResultats(resultat, actual);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                        actual = Jugar(copia, pecesNegres, bloquejosBlanques, bloquejosNegres, false, comptador);
                        actual.moviment = "Salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "no mata";
                        resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                    }
                    else if (tauler[pecesBlanques[peça].cordX - 1, pecesBlanques[peça].cordY + 1] == 2)
                    {
                        Peça[] copia = CopiarPeces(pecesBlanques);
                        copia[peça].cordX = pecesBlanques[peça].cordX - 2;
                        copia[peça].cordY = pecesBlanques[peça].cordY + 2;
                        Peça[] copia2 = CopiarPeces(pecesNegres);
                        for (int i = 0; i < 4; i++)
                        {
                            if (copia2[i].cordX + 1 == pecesBlanques[peça].cordX && copia2[i].cordY - 1 == pecesBlanques[peça].cordY)
                            {
                                copia2[i].mort = true;
                            }
                        }
                        int[,] nouTauler = RepresentarTauler(copia, copia2, bloquejosBlanques, bloquejosNegres);
                        actual = Salts(peça, copia, copia2, bloquejosBlanques, bloquejosNegres, nouTauler, comptador);
                        actual.moviment += ", salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "mata";
                        resultat = AnalitzarResultats(resultat, actual);
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                        actual = Jugar(copia, copia2, bloquejosBlanques, bloquejosNegres, false, comptador);
                        actual.moviment = "Salt de (" + pecesBlanques[peça].cordX + ", " + pecesBlanques[peça].cordY + ")" + " a " + "(" + copia[peça].cordX + ", " + copia[peça].cordY + ")" + "mata";
                        resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                    }
                }
            }
            return resultat;
        }
            //4.2.2.4
        static ResultatPosició MovimentsBasics(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres, int[,] tauler, int comptador)
        {
            ResultatPosició resultat;
            ResultatPosició actual;
            resultat.resultat = PERDEN;
            resultat.moviment = "";
            resultat.evaluació = 0;
            for (int i = 0; i < 4 && resultat.resultat != GUANYEN; i++)
            {
                if (!pecesBlanques[i].mort)
                {
                    if (pecesBlanques[i].cordX != 7)
                    {
                        if (tauler[pecesBlanques[i].cordX + 1, pecesBlanques[i].cordY + 1] == 0)
                        {
                            Peça[] copia = CopiarPeces(pecesBlanques);
                            copia[i].cordX++;
                            copia[i].cordY++;
                            actual = Jugar(copia, pecesNegres, bloquejosBlanques, bloquejosNegres, false, comptador);
                            actual.moviment = "Peça de (" + pecesBlanques[i].cordX + ", " + pecesBlanques[i].cordY + ") a (" + copia[i].cordX + ", " + copia[i].cordY + ")";
                            resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                            if (resultat.resultat == GUANYEN)
                            {
                                return resultat;
                            }
                        }
                    }
                    if (pecesBlanques[i].cordX != 0)
                    {
                        if (tauler[pecesBlanques[i].cordX - 1, pecesBlanques[i].cordY + 1] == 0)
                        {
                            Peça[] copia = CopiarPeces(pecesBlanques);
                            copia[i].cordX--;
                            copia[i].cordY++;
                            actual = Jugar(copia, pecesNegres, bloquejosBlanques, bloquejosNegres, false, comptador);
                            actual.moviment = "Peça de (" + pecesBlanques[i].cordX + ", " + pecesBlanques[i].cordY + ") a (" + copia[i].cordX + ", " + copia[i].cordY + ")";
                            resultat = AnalitzarResultats(resultat, GirarResultat(actual));
                        }
                        if (resultat.resultat == GUANYEN)
                        {
                            return resultat;
                        }
                    }
                }
            }
            return resultat;
        }
            //4.4.1.2.1
        static int AnalitzarPosició(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres, int[,] tauler) 
        {
            int resultat;
            //Primer comptar el nombre de peces bloquejades o en zona vermella.
            int pecesBlanquesLliures = 0;
            int espaiPecesBlanques = 0;
            foreach (Peça peça in pecesBlanques) 
            {
                int resultatActual=ComptarPecesBloquejades(peça, tauler, true);
                if (resultatActual == 1)
                {
                    pecesBlanquesLliures++;
                }
                else 
                {
                    espaiPecesBlanques = espaiPecesBlanques - resultatActual;
                }
            }
            int pecesNegresLliures = 0;
            int espaiPecesNegres = 0;
            foreach (Peça peça in pecesNegres) 
            {
                int resultatActual = ComptarPecesBloquejades(peça, tauler, false);
                if (resultatActual == 1)
                {
                    pecesNegresLliures++;
                }
                else
                {
                    espaiPecesNegres = espaiPecesNegres - resultatActual;
                }
            }
            int quantsBloquejosBlanques=0;
            int quantsBloquejosNegres = 0;
            for (int i = 0; i < 3; i++) 
            {
                if (!bloquejosBlanques[i].usat) 
                {
                    quantsBloquejosBlanques++;
                }
                if (!bloquejosNegres[i].usat)
                {
                    quantsBloquejosNegres++;
                }
            }
            resultat = (quantsBloquejosBlanques - quantsBloquejosNegres) * 6 + (pecesBlanquesLliures - pecesNegresLliures) * 10 + (espaiPecesBlanques - espaiPecesNegres);
            return resultat;
        }
            //4.6.1.1
        static int ComptarPecesBloquejades(Peça peça, int[,] tauler, bool color) 
        {
            int resultat = 0;
            int resultatActual = 0;
            if (color)
            {
                //condició de finalització
                if (peça.cordY >= 5)
                {
                    return 1;
                }
                if (peça.cordX < 7 && tauler[peça.cordX + 1, peça.cordY + 1] != 3) 
                {
                    Peça copia = new Peça();
                    copia.cordX = peça.cordX + 1;
                    copia.cordY = peça.cordY + 1;
                    copia.mort = false;
                    resultat = ComptarPecesBloquejades(copia, tauler, color);
                    if (resultat == 1)
                    {
                        return 1;
                    }
                }
                if (peça.cordX > 0 && tauler[peça.cordX - 1, peça.cordY + 1] != 3)
                {
                    Peça copia = new Peça();
                    copia.cordX = peça.cordX - 1;
                    copia.cordY = peça.cordY + 1;
                    copia.mort = false;
                    resultatActual = ComptarPecesBloquejades(copia, tauler, color);
                    if (resultatActual == 1)
                    {
                        return 1;
                    }
                    else 
                    {
                        if (resultatActual < resultat) 
                        {
                            resultat = resultatActual;
                        }
                    }
                }
                return resultat - 1;

            }
            else 
            {
                if (peça.cordY <= 2)
                {
                    return 1;
                }
                if (peça.cordX < 7 && tauler[peça.cordX + 1, peça.cordY - 1] != 3)
                {
                    Peça copia = new Peça();
                    copia.cordX = peça.cordX + 1;
                    copia.cordY = peça.cordY - 1;
                    copia.mort = false;
                    resultat = ComptarPecesBloquejades(copia, tauler, color);
                    if (resultat == 1)
                    {
                        return 1;
                    }
                }
                if (peça.cordX > 0 && tauler[peça.cordX - 1, peça.cordY - 1] != 3)
                {
                    Peça copia = new Peça();
                    copia.cordX = peça.cordX - 1;
                    copia.cordY = peça.cordY - 1;
                    copia.mort = false;
                    resultatActual = ComptarPecesBloquejades(copia, tauler, color);
                    if (resultatActual == 1)
                    {
                        return 1;
                    }
                    else
                    {
                        if (resultatActual < resultat)
                        {
                            resultat = resultatActual;
                        }
                    }
                }
                return resultat - 1;
            }
        }



        //---------FUNCIONS RECURSIVES COMPLEMENTS DE LES FUNCIONS GRANS---------
            //4.5.3
        static bool GuanyaEn1Recursiu(int[,] tauler, Peça peça, bool vaALaDreta)
        {
            if (vaALaDreta)
            {
                if ((tauler[peça.cordX + 1, peça.cordY + 1] == 2 || tauler[peça.cordX + 1, peça.cordY + 1] == 1) && peça.cordX + 2 < 8 && tauler[peça.cordX + 2, peça.cordY + 2] == 0) //pot saltar a la dreta
                {
                    if (peça.cordY == 5) //ha guanyat
                    {
                        return true;
                    }
                    else //tornar a saltar
                    {
                        Peça clon;
                        clon.cordX = peça.cordX + 2;
                        clon.cordY = peça.cordY + 2;
                        clon.mort = false;

                        if (clon.cordX + 2 < 8)
                        {
                            return (GuanyaEn1Recursiu(tauler, clon, true) || GuanyaEn1Recursiu(tauler, clon, false));
                        }
                        else
                        {
                            return GuanyaEn1Recursiu(tauler, clon, false);
                        }
                    }
                }
            }
            else
            {
                if ((tauler[peça.cordX - 1, peça.cordY + 1] == 2 || tauler[peça.cordX - 1, peça.cordY + 1] == 1) && peça.cordX - 2 >= 0 && tauler[peça.cordX - 2, peça.cordY + 2] == 0)
                {
                    if (peça.cordY == 5)
                    {
                        return true;
                    }
                    else
                    {
                        Peça clon;
                        clon.cordX = peça.cordX - 2;
                        clon.cordY = peça.cordY + 2;
                        clon.mort = false;

                        if (clon.cordX - 2 >= 0)
                        {
                            return (GuanyaEn1Recursiu(tauler, clon, true) || GuanyaEn1Recursiu(tauler, clon, false));
                        }
                        else
                        {
                            return GuanyaEn1Recursiu(tauler, clon, true);
                        }

                    }
                }
            }
            return false;
        }
            //4.4.1.2.2
        static bool PotColocarseRecusiu(bool[,] tauler, int x, int y)
        {
            if (y == 5)
            {
                return true;
            }
            bool resultat = false;
            if (x > 0)
            {
                if (!tauler[x - 1, y + 1])
                {
                    resultat = resultat || PotColocarseRecusiu(tauler, x - 1, y + 1);
                }
            }
            if (x < 7 && !resultat)
            {
                if (!tauler[x + 1, y + 1])
                {
                    resultat = resultat || PotColocarseRecusiu(tauler, x + 1, y + 1);
                }
            }
            return resultat;
        }



        //----------POSICIONS DE PROVA I TESTOS------------
            //Posició inicial
        static void EstablirPosicióInicial(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //POSICIO ORIGINAL
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 5;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 2;
            bloquejosBlanques[1].cordY = 5;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 6;
            bloquejosBlanques[2].cordY = 5;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 3;
            bloquejosNegres[0].cordY = 2;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 5;
            bloquejosNegres[1].cordY = 2;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 2;
            bloquejosNegres[2].cordY = 3;
            bloquejosNegres[2].usat = false;

        }
            
            //Obertures
        static void EstablirPosicióObertura1(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //b1 a a2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 7;
            pecesNegres[3].cordY = 6;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 3;
            bloquejosNegres[0].cordY = 2;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura2(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //b1 a c2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 5;
            pecesNegres[3].cordY = 6;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura3(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //d1 a c2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 5;
            pecesNegres[2].cordY = 6;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura4(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //d1 a e2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 3;
            pecesNegres[2].cordY = 6;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura5(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //f1 a e2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 3;
            pecesNegres[1].cordY = 6;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura6(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //f1 a g2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 1;
            pecesNegres[1].cordY = 6;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosicióObertura7(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //h1 a g2
            pecesBlanques[0].cordX = 1;
            pecesBlanques[0].cordY = 0;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 3;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 5;
            pecesBlanques[2].cordY = 0;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 1;
            pecesNegres[0].cordY = 6;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 2;
            pecesNegres[1].cordY = 7;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 4;
            pecesNegres[2].cordY = 7;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 6;
            pecesNegres[3].cordY = 7;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 0;
            bloquejosBlanques[0].cordY = 0;
            bloquejosBlanques[0].usat = false;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
            
            //Proves 4.4.2
        static void EstablirPosició2(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //GUNYEN BLANQUES PER OFEGAMENT
            pecesBlanques[0].cordX = 0;
            pecesBlanques[0].cordY = 1;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 1;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 2;
            pecesBlanques[2].cordY = 1;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 6;
            pecesBlanques[3].cordY = 5;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 5;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 1;
            pecesNegres[1].cordY = 4;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 0;
            pecesNegres[2].cordY = 3;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 2;
            pecesNegres[3].cordY = 3;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 1;
            bloquejosBlanques[0].cordY = 2;
            bloquejosBlanques[0].usat = true;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 3;
            bloquejosNegres[0].cordY = 2;
            bloquejosNegres[0].usat = true;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosició3(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //GUANYEN BLANQUES PER ARRIBADA
            pecesBlanques[0].cordX = 0;
            pecesBlanques[0].cordY = 1;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 1;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 2;
            pecesBlanques[2].cordY = 1;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 6;
            pecesBlanques[3].cordY = 5;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 4;
            pecesNegres[0].cordY = 5;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 1;
            pecesNegres[1].cordY = 4;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 0;
            pecesNegres[2].cordY = 3;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 2;
            pecesNegres[3].cordY = 3;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 1;
            bloquejosBlanques[0].cordY = 2;
            bloquejosBlanques[0].usat = true;

            bloquejosBlanques[1].cordX = 0;
            bloquejosBlanques[1].cordY = 0;
            bloquejosBlanques[1].usat = false;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 3;
            bloquejosNegres[0].cordY = 2;
            bloquejosNegres[0].usat = true;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosició4(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //GUANYEN NEGRES PER ARRIBADA
            pecesBlanques[0].cordX = 0;
            pecesBlanques[0].cordY = 1;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 1;
            pecesBlanques[1].cordY = 0;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 2;
            pecesBlanques[2].cordY = 1;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 6;
            pecesBlanques[3].cordY = 3;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 4;
            pecesNegres[0].cordY = 5;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 1;
            pecesNegres[1].cordY = 4;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 0;
            pecesNegres[2].cordY = 3;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 2;
            pecesNegres[3].cordY = 3;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 1;
            bloquejosBlanques[0].cordY = 2;
            bloquejosBlanques[0].usat = true;

            bloquejosBlanques[1].cordX = 3;
            bloquejosBlanques[1].cordY = 2;
            bloquejosBlanques[1].usat = true;

            bloquejosBlanques[2].cordX = 0;
            bloquejosBlanques[2].cordY = 0;
            bloquejosBlanques[2].usat = false;

            bloquejosNegres[0].cordX = 0;
            bloquejosNegres[0].cordY = 0;
            bloquejosNegres[0].usat = false;

            bloquejosNegres[1].cordX = 0;
            bloquejosNegres[1].cordY = 0;
            bloquejosNegres[1].usat = false;

            bloquejosNegres[2].cordX = 0;
            bloquejosNegres[2].cordY = 0;
            bloquejosNegres[2].usat = false;
        }
        static void EstablirPosició5(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //TAULES PER REPETICIÓ
            pecesBlanques[0].cordX = 0;
            pecesBlanques[0].cordY = 3;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 1;
            pecesBlanques[1].cordY = 2;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 2;
            pecesBlanques[2].cordY = 1;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 6;
            pecesBlanques[3].cordY = 1;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 5;
            pecesNegres[1].cordY = 6;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 6;
            pecesNegres[2].cordY = 5;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 7;
            pecesNegres[3].cordY = 4;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 4;
            bloquejosBlanques[0].cordY = 5;
            bloquejosBlanques[0].usat = true;

            bloquejosBlanques[1].cordX = 5;
            bloquejosBlanques[1].cordY = 4;
            bloquejosBlanques[1].usat = true;

            bloquejosBlanques[2].cordX = 6;
            bloquejosBlanques[2].cordY = 3;
            bloquejosBlanques[2].usat = true;

            bloquejosNegres[0].cordX = 1;
            bloquejosNegres[0].cordY = 4;
            bloquejosNegres[0].usat = true;

            bloquejosNegres[1].cordX = 2;
            bloquejosNegres[1].cordY = 3;
            bloquejosNegres[1].usat = true;

            bloquejosNegres[2].cordX = 3;
            bloquejosNegres[2].cordY = 2;
            bloquejosNegres[2].usat = true;
        }

        //Altres posicions interessants
        static void EstablirPosicióNoTaules(Peça[] pecesBlanques, Peça[] pecesNegres, Bloqueig[] bloquejosBlanques, Bloqueig[] bloquejosNegres)
        {
            //Posició del punt 4.2.1.3: Anti-intuitivament, guanyen blanques per arribada.
            pecesBlanques[0].cordX = 0;
            pecesBlanques[0].cordY = 3;
            pecesBlanques[0].mort = false;

            pecesBlanques[1].cordX = 1;
            pecesBlanques[1].cordY = 2;
            pecesBlanques[1].mort = false;

            pecesBlanques[2].cordX = 2;
            pecesBlanques[2].cordY = 1;
            pecesBlanques[2].mort = false;

            pecesBlanques[3].cordX = 7;
            pecesBlanques[3].cordY = 0;
            pecesBlanques[3].mort = false;

            pecesNegres[0].cordX = 0;
            pecesNegres[0].cordY = 7;
            pecesNegres[0].mort = false;

            pecesNegres[1].cordX = 5;
            pecesNegres[1].cordY = 6;
            pecesNegres[1].mort = false;

            pecesNegres[2].cordX = 6;
            pecesNegres[2].cordY = 5;
            pecesNegres[2].mort = false;

            pecesNegres[3].cordX = 7;
            pecesNegres[3].cordY = 4;
            pecesNegres[3].mort = false;

            bloquejosBlanques[0].cordX = 4;
            bloquejosBlanques[0].cordY = 5;
            bloquejosBlanques[0].usat = true;

            bloquejosBlanques[1].cordX = 5;
            bloquejosBlanques[1].cordY = 4;
            bloquejosBlanques[1].usat = true;

            bloquejosBlanques[2].cordX = 6;
            bloquejosBlanques[2].cordY = 3;
            bloquejosBlanques[2].usat = true;

            bloquejosNegres[0].cordX = 1;
            bloquejosNegres[0].cordY = 4;
            bloquejosNegres[0].usat = true;

            bloquejosNegres[1].cordX = 2;
            bloquejosNegres[1].cordY = 3;
            bloquejosNegres[1].usat = true;

            bloquejosNegres[2].cordX = 3;
            bloquejosNegres[2].cordY = 2;
            bloquejosNegres[2].usat = true;
        }
    }
}