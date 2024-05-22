namespace EIS.Runtime.Core.Hints
{
    public class HintIdentifier
    {

        public HintType hintType;
        public int instanceID;

        public HintIdentifier(HintType hintType, int instanceID)
        {
            this.hintType = hintType;
            this.instanceID = instanceID;
        }

        public override bool Equals(object obj)
        {
            return hintType == ((HintIdentifier)obj)!.hintType &&
                   instanceID == ((HintIdentifier)obj)!.instanceID;
        }

        public override int GetHashCode()
        {
            return instanceID + (int)hintType;
        }
    }
}