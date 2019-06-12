using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    public class Target
    {
        private float fromTop;
        private float fromLeft;
        private GameObject target;

        public Target(GameObject target)
        {
            this.target = target;
        }

        public Target(GameObject target, bool asCoordinate)
        {
            this.fromTop = target.FromTop + (target.Height / 2);
            this.fromLeft = target.FromLeft + (target.Width / 2);
        }

        public Target(float fromLeft, float fromTop)
        {
            this.fromTop = fromTop;
            this.fromLeft = fromLeft;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public void SetTarget(GameObject target, bool asCoordinate)
        {
            this.fromTop = target.FromTop + (target.Height / 2);
            this.fromLeft = target.FromLeft + (target.Width / 2);
        }

        public void SetTarget(float fromLeft, float fromTop)
        {
            this.fromTop = fromTop;
            this.fromLeft = fromLeft;
        }

        public void SetFromLeft(float fromLeft)
        {
            this.fromLeft = fromLeft;
        }

        public void SetFromTop(float fromTop)
        {
            this.fromTop = fromTop;
        }

        public void AddFromLeft(float value)
        {
            this.fromLeft += value;
        }

        public void AddFromTop(float value)
        {
            this.fromTop += value;
        }

        public float FromTop()
        {
            if (target is null)
            {
                return fromTop;
            }
            return target.FromTop + (target.Height / 2);
        }

        public float FromLeft()
        {
            if (target is null)
            {
                return fromLeft;
            }
            return target.FromLeft + (target.Width / 2);
        }

        public GameObject GetGameObject()
        {
            if (!(target is null))
            {
                return target;
            }
            return null;
        }
    }
}
