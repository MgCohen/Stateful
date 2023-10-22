namespace Scaffold.Stateful.Samples
{
    public class StageData
    {
        public StageData(int index, int stars, bool unlocked, bool completed)
        {
            Index = index;
            Stars = stars;
            Unlocked = unlocked;
            Completed = completed;
        }

        public int Index { get; private set; }
        public int Stars { get; private set; }
        public bool Unlocked { get; private set; }
        public bool Completed { get; private set; }

        public void Complete(int stars)
        {
            Completed = true;
            Stars = stars;
        }

        public void Unlock()
        {
            Unlocked = true;
        }
    }
}