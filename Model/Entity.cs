using System;

namespace ConcursBaschet.domain
{
    [Serializable]
    public class Entity<ID>
    {
        private ID id;

        public Entity(ID id)
        {
            this.id = id;
        }

        /**
         * Method that returns the id of the entity
         * @return ID
         */
        public ID GetId()
        {
            return id;
        }

        public void SetId(ID id)
        {
            this.id = id;
        }

        /**
         * Determines the hash code of the user
         * @return int
         */
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /**
         * Method that verifies if the entity is equal to another object
         * @param obj Object
         * @return true if obj is equal with the instance and false otherwise
         */
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (!(obj is Entity<ID>))
            {
                return false;
            }

            Entity<ID> entity = (Entity<ID>)obj;
            return Equals(this.id, entity.id);
        }
    }
}