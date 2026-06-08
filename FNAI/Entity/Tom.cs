using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FNAI.Entity
{
    public class Tom : Entity
    {
        public String Position { get => position; set => position = value; }

        private object _page;
        private object? page;
        private String position;
        public int IAporte { get; set; } = -1;

        public Tom(int AIscore) : base(AIscore)
        {
            {
                Random rng = new Random(50);
                if (rng.Next(1, 25) <= IAporte)
                {
                    Position = "PorteG";
                    _page = page;
                    IAporte = rng.Next(1, 25);
                }
                else
                {
                    Position = "PorteD";
                    _page = page;
                    IAporte = rng.Next(1, 25);
                }
            }
        }

        #region --- IA ---
        public override void Attack()
        {
            if (position == "Bureau")
            {
                jumpscare();
                Kill();
            }
        }

        public override void DoAnAction()
        {
            
        }

        public override void jumpscare()
        {            
            MessageBox.Show("JUMPSCARE !");
        }
        #region --- Deplacement ---
        public override void Move()
        {
            if (1<= IAporte && IAporte <= 25)
            {
                position = "PorteG";
            }
            else if (26<= IAporte && IAporte <= 50)
            {
                position = "PorteD";
            }

            if (position == "PorteG" || position == "PorteD")
            {
            }
        }
        #endregion

        #region --- Bruit ---
        public override void Noise()
        {
            
        }
        #endregion

        #endregion
    }

}
