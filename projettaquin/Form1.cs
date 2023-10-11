using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace robot
{
    public partial class Form1 : Form
    {
        private TableLayoutPanel cells = null;
        //Liste de tous les chemins possibles entre les noeuds sélectionnés par l'utilisateur
        private List<List<List<GenericNode>>> chemins;
        //Plateau de jeu
        static public double[,] grille;
        //Matrice d'adjacence de chaque case de la grille
        static public double[,,,] matrice;
        static public int taille = 20;
        //Noeuds sélectionnés par l'utilisateur
        static public List<Cell> listCells;
        static public Cell Cellule;
        //Matrice d'adjacence des noeuds sélectionnés par l'utilisateur
        static public double[,] matrixAdj = new double[400, 400];
        static public decimal nbCases;
        private List<int> solution;
        private List<GenericNode> noeudsFermes;

        public class Cell : PictureBox
        {
            public static readonly System.Drawing.Size CellSize = new System.Drawing.Size(30, 30);
            public int row, col;

            public Cell(int row, int col) : base()
            {
                this.row = row; this.col = col;
                this.Size = CellSize;
                this.Margin = new Padding(1);
            }

            public override string ToString() { return "Cell(" + row + "," + col + ")"; }

        }

        public Form1()
        {
            InitializeComponent();

            //Initialisation du modèle du plateau de jeu
            grille = new double[,]
            { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }};

            //Initialisation de la matrice d'adjacence de chaque case de la grille
            matrice = new double[20, 20, 20, 20];
            for (int x = 0; x < taille; x++)
                for (int y = 0; y < taille; y++)
                    for (int x2 = 0; x2 < taille; x2++)
                        for (int y2 = 0; y2 < taille; y2++)
                            matrice[x, y, x2, y2] = -1;

            for (int x = 0; x < taille; x++)
            {
                for (int y = 0; y < taille; y++)
                {
                    if (grille[x, y] == 1)//Si c'est une case où le robot peut se déplacer
                    {
                        for (int x2 = Math.Max(0, x - 1); x2 < Math.Min(20, x + 2); x2++)
                            for (int y2 = Math.Max(0, y - 1); y2 < Math.Min(20, y + 2); y2++)
                                if (grille[x2, y2] == 1)
                                {
                                    if ((Math.Abs(x - x2) + Math.Abs(y - y2)) == 1)
                                    {
                                        matrice[x, y, x2, y2] = 1;//voisine horizontale ou verticale
                                    }
                                    else if ((Math.Abs(x - x2) + Math.Abs(y - y2)) == 2)
                                    {
                                        matrice[x, y, x2, y2] = Math.Sqrt(2);//voisine diagonale
                                    }
                                }
                    }
                }
            }

            //Initialise la matrice d'adjacence des noeuds sélectionnés par l'utilisateur
            for (int i = 0; i < matrixAdj.GetLength(0); i++)
            {
                for (int j = 0; j < matrixAdj.GetLength(1); j++)
                {
                    matrixAdj[i, j] = 0;
                }
            }

            noeudsFermes = new List<GenericNode>();
            solution = new List<int>();
            listCells = new List<Cell>();
            chemins = new List<List<List<GenericNode>>>();
            cells = GetBoard();
            this.Controls.Add(cells);
        }

        //Crée le plateau de jeu tel qu'il doit être affiché
        private TableLayoutPanel GetBoard()
        {
            TableLayoutPanel b = new TableLayoutPanel();
            b.ColumnCount = taille;
            b.RowCount = taille;
            b.Padding = new Padding(0, 0, 0, 0);

            for (int i = 0; i < b.ColumnCount; i++) { b.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Cell.CellSize.Width)); }

            for (int i = 0; i < b.RowCount; i++) { b.RowStyles.Add(new RowStyle(SizeType.Absolute, Cell.CellSize.Height)); }

            for (int row = 0; row < b.RowCount; row++)
            {
                for (int col = 0; col < b.ColumnCount; col++)
                {
                    Cell cell = new Cell(row, col);
                    if (grille[row, col] == 0)
                    {
                        cell.BackColor = Color.Black;//Mur
                    }
                    else if (grille[row, col] == 1)
                    {
                        cell.BackColor = Color.White;//Sol
                    }

                    //Possibilité de cliquer dessus
                    cell.Click += new EventHandler(cell_Click);
                    b.Controls.Add(cell, col, row);
                }
            }

            b.Padding = new Padding(30);
            b.Size = new System.Drawing.Size((b.ColumnCount + 1) * Cell.CellSize.Width, (b.RowCount + 1) * Cell.CellSize.Height);
            return b;
        }

        private void cell_Click(object sender, EventArgs e)
        {
            Cell cell = (Cell)sender;

            if (cell.BackColor != Color.Black)//Pas un mur
            {
                //Clean le plateau de jeu
                foreach (Cell c in listCells)
                {
                    c.BackColor = Color.White;
                }

                //Echange la première case par la dernière case cliquée    
                if (listCells.Count == nbCasesNUD.Value)
                {
                    listCells.RemoveAt(0);
                }
                listCells.Add(cell);

                if (listCells.Count == nbCasesNUD.Value)
                {
                    //Clean la liste des précédents chemins
                    if (chemins.Count != 0)
                    {
                        for (int i = 0; i < listCells.Count; i++)
                        {
                            for (int j = 0; j < listCells.Count; j++)
                            {
                                foreach (GenericNode gn in chemins[i][j])
                                {
                                    NodeRobot n2 = (NodeRobot)gn;
                                    //Efface la grille
                                    cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                                }
                            }
                        }
                        chemins.Clear();
                    }

                    //Clean l'affichage des noeuds fermés
                    foreach (GenericNode gn in noeudsFermes)
                    {
                        NodeRobot n2 = (NodeRobot)gn;
                        cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                    }
                    noeudsFermes.Clear();

                    //Calcul des chemins possibles en entre toutes les cases cliquées
                    Graph g = new Graph();
                    int[] localisation = new int[2];
                    labelsolution.Text = "Une solution a été trouvée";
                    int NbNoeudsOuverts = 0;
                    int NbNoeudsFermés = 0;
                    nbCases = nbCasesNUD.Value;
                    for (int i = 0; i < listCells.Count; i++)
                    {
                        chemins.Add(new List<List<GenericNode>>());
                        for (int j = 0; j < listCells.Count; j++)
                        {
                            //Coordonnées de la case de départ 
                            localisation[0] = listCells[i].row;
                            localisation[1] = listCells[i].col;
                            Cellule = listCells[j];//Case d'arrivée
                            NodeRobot n1 = new NodeRobot(localisation);
                            List<GenericNode> chemin = g.RechercheSolutionAEtoile(n1);
                            chemins[i].Add(chemin);

                            //Remplissage de la matrice d'adjacence 
                            double coût = 0;
                            for (int k = 1; k < chemin.Count; k++)
                            {
                                NodeRobot nr1 = (NodeRobot)chemin[k - 1];
                                NodeRobot nr2 = (NodeRobot)chemin[k];
                                coût += matrice[nr1.Localisation[0], nr1.Localisation[1], nr2.Localisation[0], nr2.Localisation[1]];
                            }
                            matrixAdj[i, j] = coût;

                            if (chemins[i][j].Count == 0)
                            {
                                labelsolution.Text = "Pas de solution";
                                cheminOptimalLbl.Text = "";
                            }


                            noeudsFermes.AddRange(g.L_Fermes);
                            NbNoeudsOuverts += g.CountInOpenList();
                            NbNoeudsFermés += g.CountInClosedList();
                            g.GetSearchTree(treeView1);

                        }
                    }
                    labelcountopen.Text = "Nb noeuds des ouverts : " + NbNoeudsOuverts.ToString();
                    labelcountclosed.Text = "Nb noeuds des fermés : " + NbNoeudsFermés.ToString();
                    if (listCells.Count > 2)
                    {
                        label4.Text = "Arbre de recherche entre le dernier et avant-dernier point cliqué";
                        //Bruteforce du chemin le plus court
                        solution = bruteForce();
                    }
                    else
                    {
                        label4.Text = "Arbre de recherche";
                        solution.Clear();
                        solution.Add(0);
                        solution.Add(1);
                    }

                    //Affichage des noeuds fermés
                    foreach (GenericNode gn in noeudsFermes)
                    {
                        NodeRobot n2 = (NodeRobot)gn;
                        cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.LightGray;
                    }

                    //Affichage du chemin optimal entre les cases cliquées
                    string txt = "Ordre du chemin optimal choisi (identifiés par leur coordonnées (Horizontale,Verticale) :\n";
                    bool affichageChemin = true;
                    for (int i = 0; i < solution.Count - 1; i++)
                    {
                        if (chemins[solution[i]][solution[i + 1]].Count == 0)
                        {
                            affichageChemin = false;
                        }

                        foreach (GenericNode gn in chemins[solution[i]][solution[i + 1]])
                        {
                            NodeRobot n2 = (NodeRobot)gn;
                            cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.Orange;
                        }
                        txt += "(" + (listCells[solution[i]].col + 1) + "," + (listCells[solution[i]].row + 1) + ") ";
                    }
                    txt += "(" + (listCells[solution[solution.Count - 1]].col + 1) + "," + (listCells[solution[solution.Count - 1]].row + 1) + ")";
                    if (affichageChemin)
                    {
                        cheminOptimalLbl.Text = txt;
                    }


                    //HeldKarp
                    //Remplissage de la liste des points de passage
                    //List<int> passages = new List<int>();
                    //for (int i = 1; i < listCells.Count; i++)
                    //{
                    //    passages.Add(i);
                    //}
                    //System.Diagnostics.Debug.WriteLine("Chemin optimal : "+ heldKarp(0,passages));

                }


                //Affiche les cases cliquées sur le board
                listCells.First().BackColor = Color.Green;
                listCells.Last().BackColor = Color.Red;
                for (int i = 1; i < listCells.Count - 1; i++)
                {
                    listCells[i].BackColor = Color.Blue;
                }
            }
        }

        private void nbCasesNUD_ValueChanged(object sender, EventArgs e)
        {
            if (listCells.Count > 0)
            {
                if (labelcountopen.Text != "" && labelcountopen.Text != "Nb noeuds des ouverts : ")
                {
                    //Clean le board
                    //D'abord le chemin optimal
                    for (int i = 0; i < listCells.Count; i++)
                    {
                        for (int j = 0; j < listCells.Count; j++)
                        {
                            foreach (GenericNode gn in chemins[i][j])
                            {
                                NodeRobot n2 = (NodeRobot)gn;
                                cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                            }
                        }
                    }
                    chemins.Clear();
                }
                //Puis les noeuds sélectionnés par l'utilisateur
                foreach (Cell c in listCells)
                {
                    c.BackColor = Color.White;
                }

                //Clean l'affichage des noeuds fermés
                foreach (GenericNode gn in noeudsFermes)
                {
                    NodeRobot n2 = (NodeRobot)gn;
                    cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                }
                noeudsFermes.Clear();

                listCells.Clear();
                labelsolution.Text = "Solution ?";
                treeView1.Nodes.Clear();
                labelcountopen.Text = "Nb noeuds des ouverts : ";
                labelcountclosed.Text = "Nb noeuds des fermés : ";
                chemins.Clear();
                cheminOptimalLbl.Text = "";
            }
        }

        private void initBtn_Click(object sender, EventArgs e)
        {
            if (listCells.Count > 0)
            {
                if (listCells.Count == nbCasesNUD.Value)
                {
                    //Clean le board
                    //D'abord le chemin optimal
                    for (int i = 0; i < listCells.Count; i++)
                    {
                        for (int j = 0; j < listCells.Count; j++)
                        {
                            foreach (GenericNode gn in chemins[i][j])
                            {
                                NodeRobot n2 = (NodeRobot)gn;
                                cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                            }
                        }
                    }
                    chemins.Clear();
                }
                //Puis les noeuds sélectionnés par l'utilisateur
                foreach (Cell c in listCells)
                {
                    c.BackColor = Color.White;
                }

                //Clean l'affichage des noeuds ouverts
                foreach (GenericNode gn in noeudsFermes)
                {
                    NodeRobot n2 = (NodeRobot)gn;
                    cells.GetControlFromPosition(n2.Localisation[1], n2.Localisation[0]).BackColor = Color.White;
                }
                noeudsFermes.Clear();

                listCells.Clear();
                labelsolution.Text = "Solution ?";
                treeView1.Nodes.Clear();
                labelcountopen.Text = "Nb noeuds des ouverts : ";
                labelcountclosed.Text = "Nb noeuds des fermés : ";
                cheminOptimalLbl.Text = "";
            }
        }

        //Algorithme HeldKarp innachevé 
        private double heldKarp(int depart, List<int> passages)
        {
            if (passages.Count == 0)
            {
                return matrixAdj[0, depart];
            }
            else
            {
                int tempPassage = -1;
                double distMin = double.MaxValue;
                for (int i = 0; i < passages.Count; i++)
                {
                    int passage = passages[i];
                    passages.RemoveAt(i);

                    if (distMin > matrixAdj[passage, depart] + heldKarp(passage, passages))
                    {
                        distMin = matrixAdj[passage, depart] + heldKarp(passage, passages);
                        tempPassage = passage;
                    }
                    passages.Add(passage);
                }
                System.Diagnostics.Debug.WriteLine("dep : " + depart + " passage : " + tempPassage);
                System.Diagnostics.Debug.WriteLine("distance : " + distMin);
                return distMin;
            }

        }

        //Génère toutes les permutations possibles pour lister tous les chemins
        private static void PopulatePosition<T>(List<List<T>> finalList, List<T> list, List<T> temp, int position)
        {
            foreach (T element in list)
            {
                List<T> currentTemp = temp.ToList();
                if (!currentTemp.Contains(element))
                    currentTemp.Add(element);
                else
                    continue;

                if (position == list.Count)
                    finalList.Add(currentTemp);
                else
                    PopulatePosition(finalList, list, currentTemp, position + 1);
            }
        }

        public static List<List<int>> GetPermutations(List<int> list)
        {
            List<List<int>> results = new List<List<int>>();
            PopulatePosition(results, list, new List<int>(), 1);
            return results;
        }

        //Solution du chemin le plus court par bruteforce
        private List<int> bruteForce()
        {
            //Liste une suite d'entier dont la taille de la liste correspond au nombre de clics moins 1
            List<int> interieur = new List<int>();
            for (int j = 1; j < listCells.Count; j++)
            {
                interieur.Add(j);
            }

            //Liste tous les chemins possibles
            List<List<int>> results = GetPermutations(interieur);

            for (int j = 0; j < results.Count; j++)
            {
                results[j].Add(0);//Ajout du point de départ au début
                results[j].Insert(0, 0);//Ajout du point de départ à la fin de la liste
            }

            //Compare tous les couts des chemins pour trouver le chemin optimal
            List<int> cheminOpti = new List<int>();
            double coutOpti = double.MaxValue;
            foreach (List<int> results1 in results)
            {
                double coutTotal = matrixAdj[results1[0], results1[1]];

                for (int k = 1; k < results1.Count - 1; k++)
                {
                    coutTotal += matrixAdj[results1[k], results1[k + 1]];
                }

                if (coutOpti > coutTotal)
                {
                    coutOpti = coutTotal;
                    cheminOpti = results1;
                }
            }
            return cheminOpti;
        }
    }
}
