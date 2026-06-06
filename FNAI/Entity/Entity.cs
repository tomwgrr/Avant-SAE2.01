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
        private int _AISCore;
        private bool isAwake;
        #endregion

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
