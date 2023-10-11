using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robot
{
    public class NodeRobot : GenericNode
    {
        public int[] Localisation;

        public NodeRobot(int[] newLocalisation) : base()
        {
            Localisation = newLocalisation;
        }

        public override bool IsEqual(GenericNode N2)
        {
            NodeRobot NT = (NodeRobot)(N2);
            return ((NT.Localisation[0] == Localisation[0]) && ((NT.Localisation[1] == Localisation[1]))); //Egalité des valeurs et non des objets
        }

        public override double GetArcCost(GenericNode N2)
        {
            NodeRobot NT = (NodeRobot)N2;
            return Form1.matrice[Localisation[0], Localisation[1], NT.Localisation[0], NT.Localisation[1]]; //On se réfère à la matrice d'ajacence
        }

        public override bool EndState()
        {
            if (Form1.listCells.Count == Form1.nbCases && (Localisation[0] == Form1.Cellule.row) && (Localisation[1] == Form1.Cellule.col))
            {
                return true;
            }

            return false;
        }

        public override List<GenericNode> GetListSucc()
        {
            //Noeuds accessibles depuis le noeud courant

            List<GenericNode> lsucc = new List<GenericNode>();

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (Form1.matrice[Localisation[0], Localisation[1], i, j] != -1) //On se réfère pour cela à la matrice d'adjacence
                    {
                        int[] local = new int[] { i, j };
                        NodeRobot newnode = new NodeRobot(local);
                        lsucc.Add(newnode);
                    }
                }
            }
            return lsucc;
        }

        public override double CalculeHCost()
        {
            //On minore la distance au point final
            double distance = 0.0;

            //Première heuristique : distance euclidienne à "vol d'oiseau"
            //distance = Math.Sqrt(Math.Pow(Localisation[0] - Form1.Cellule.row, 2) + Math.Pow(Localisation[1] - Form1.Cellule.col, 2));

            //Seconde heuristique : distance de Manhattan avec les diagonales, on prend en compte la contrainte des cases
            int dx = Math.Abs(Localisation[0] - Form1.Cellule.row);
            int dy = Math.Abs(Localisation[1] - Form1.Cellule.col);

            distance = Math.Min(dx, dy) * Math.Sqrt(2) + Math.Abs(dx - dy);

            return (distance);
        }

        public override string ToString()
        {
            return "Hor. : " + Convert.ToString(Localisation[1] + 1) + ", Ver. : " + Convert.ToString(Localisation[0] + 1);
        }
    }
}
