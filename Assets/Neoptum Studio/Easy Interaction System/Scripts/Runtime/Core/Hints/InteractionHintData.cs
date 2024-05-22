namespace EIS.Runtime.Core.Hints
{
    public class InteractionHintData
    {
        public HintType spriteType;
        public string text;

        public InteractionHintData(HintType spriteType, string text)
        {
            this.spriteType = spriteType;
            this.text = text;
        }
    }
}