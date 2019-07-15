namespace Magitek.Utilities.Collections
{
    public class DoubleLinkListIndexNode
    {

        public DoubleLinkListIndexNode Previous;
        public DoubleLinkListIndexNode Next;
        public int Index;

        public DoubleLinkListIndexNode(DoubleLinkListIndexNode previous, int index)
        {
            Previous = previous;
            Next = null;
            Index = index;
            if (Previous != null)
            {
                Previous.Next = this;
            }
        }

        public DoubleLinkListIndexNode(DoubleLinkListIndexNode previous, DoubleLinkListIndexNode next)
        {
            Previous = previous;
            Next = next;
            Index = next.Index;
            if (Previous != null)
            {
                Previous.Next = this;
            }

            if (Next == null)
                return;

            Next.Previous = this;
            IncrementForward();
        }

        public void Remove()
        {
            if (Previous != null)
            {
                Previous.Next = Next;
            }

            if (Next != null)
            {
                Next.Previous = Previous;
            }
            DecrementForward();
        }

        private void DecrementForward()
        {
            if (Next == null)
                return;

            Next.Index--;
            Next.DecrementForward();
        }

        private void IncrementForward()
        {
            if (Next == null)
                return;

            Next.Index++;
            Next.IncrementForward();
        }

    }
}
