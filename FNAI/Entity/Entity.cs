using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNAI.Entity
{
    public abstract class Entity
    {
        #region --- Attributs ---
        private int _AISCore = 10; // 1 → 20, plus c'est haut, plus l'IA est agressive
        private bool isAwake;
        #endregion
        protected int AIscore => _AISCore; 

        public Entity(int AIscore)
        {
           this._AISCore = AIscore;
        }
        public bool IsAwake { get; set; }
        public abstract  void DoAnAction();
        public abstract void Move();
        public abstract void Attack();
        public abstract void Noise();
        public abstract void jumpscare();
        public void Kill()
        { 
            jumpscare();
        }


    }
}
