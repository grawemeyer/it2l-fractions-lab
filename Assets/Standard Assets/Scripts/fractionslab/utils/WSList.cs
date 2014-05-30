using System.Collections.Generic;

namespace fractionslab.utils
{
    public class WSList<T> : List<T>
    {
        public void Push(T item)
        {
            this.Insert(0, item);
        }

        public void Pull(T item)
        {
            int i = this.IndexOf(item);
            if (i >= 0)
            {
                T tmp = this[i];
                this.RemoveAt(i);
                Push(tmp);
            }
        }

        public void SendBack(T item)
        {
            int i = this.IndexOf(item);
            if (i >= 0)
            {
                T tmp = this[i];
                this.RemoveAt(i);
                Add(tmp);
            }
        }

        public override string ToString()
        {
            string tmp = string.Empty;
            for (int i = 0; i < this.Count; i++)
                tmp += this[i].ToString() + ", ";
            return tmp;
        }
    }
}
